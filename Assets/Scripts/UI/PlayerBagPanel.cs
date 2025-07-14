using System.Collections.Generic;
using MessageSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;

//todo: 시너지 표시
//todo: 최대 갯수
//todo: 정렬을 기준 수정 필요 
//todo: bag unit card랑 스킬 카드 포지션 및 배치 방식 관련 수정 필요
//todo: 유닛 개수가 최대 개수보다 적다면 자동 배치되도록 구현 필요
public class PlayerBagPanel : UIInstance
{
	private static PlayerBagPanel instance;
	public static PlayerBagPanel Instance => instance;
	public override UIType UIType => UIType.Popup;
	public BagUITile CurrentHoverBagUITile { get; private set; }

	[SerializeField] private RectTransform DeckCardArea;

	[SerializeField] private RectTransform DeployArea;

	public Vector3 LeftTopOffset;
	public float horizontalSpace;
	public float verticalSpace;
	public int cardCountPerRow;

	private int DeckCardStartRow =>
		(Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList.Count - 1) / cardCountPerRow + 1;

	public Dictionary<ICard, BagUICard> cardDictionary;
	
	protected override void Init(object param)
	{
		instance = this;
		
		cardDictionary = new();
		NoticeSystem.Instance.Subscribe<BagUITileHoverNotice>(OnTileHover);
		NoticeSystem.Instance.Subscribe<BagUICardPlaceNotice>(OnCardDeploy);
		NoticeSystem.Instance.Subscribe<BagUICardUnPlaceNotice>(OnCardUnDeploy);
		InitializeBagUnitCards();
		InitializeDeckCards();
		InitializeField();
	}

	public void OnClose()
	{
		Game.Instance.UIManager.RemoveUI(Id);
	}

	protected override void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<BagUITileHoverNotice>(OnTileHover);
		NoticeSystem.Instance.Unsubscribe<BagUICardPlaceNotice>(OnCardDeploy);
		NoticeSystem.Instance.Unsubscribe<BagUICardUnPlaceNotice>(OnCardUnDeploy);
		foreach (var kvp in cardDictionary)
		{
			var card = kvp.Value;
			if (card != null && card.gameObject.activeSelf)
			{
				card.GetComponent<PooledUnityObject>()?.Dispose();
			}
		}
	}

	private void InitializeField()
	{
		var bagPool = UnityObjectPool.GetOrCreateUIPool("BagUnitCard");
		bagPool.transform.SetParent(transform);
		var deployInfos = Game.Instance.GetPlayer().CurrentPlayInfo.FieldDeployLocationInfo;

		for (int i = 0; i < deployInfos.Count; i++)
		{
			var tile = CalculateFieldUnitCardTile(deployInfos[i]);
			var bagUICard = bagPool.Instantiate(tile.GetPosition()).GetComponent<BagUnitCard>();
			bagUICard.Initialize(deployInfos[i].TargetCard, tile);
			cardDictionary.Add(deployInfos[i].TargetCard, bagUICard);
		}
	}

	private void InitializeBagUnitCards()
	{
		var bagPool = UnityObjectPool.GetOrCreateUIPool("BagUnitCard");
		bagPool.transform.SetParent(transform);
		var cardList = Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList;
		for (int i = 0; i < cardList.Count; i++)
		{
			var pos = CalculateBagUnitCardPositionWithIndex(i);
			var bagUICard = bagPool.Instantiate(pos).GetComponent<BagUnitCard>();
			bagUICard.Initialize(cardList[i], pos);
			cardDictionary.Add(cardList[i], bagUICard);
		}
	}

	private void InitializeDeckCards()
	{
		var bagPool = UnityObjectPool.GetOrCreateUIPool("BagSkillCard");
		bagPool.transform.SetParent(transform);
		var cardList = Game.Instance.GetPlayer().CurrentPlayInfo.DeckCardList;
		for (int i = 0; i < cardList.Count; i++)
		{
			var pos = CalculateDeckCardPositionWithIndex(i);
			var bagUICard = bagPool.Instantiate(pos).GetComponent<BagSkillCard>();
			bagUICard.Initialize(cardList[i], pos);
			cardDictionary.Add(cardList[i], bagUICard);
		}
	}

	private void OnTileHover(BagUITileHoverNotice m)
	{
		if (m.HoverType == HoverType.Enter)
		{
			CurrentHoverBagUITile = m.TargetTile;
		}
		else if (m.HoverType == HoverType.Exit && m.TargetTile == CurrentHoverBagUITile)
		{
			CurrentHoverBagUITile = null;
		}
	}
	
	private void OnCardDeploy(BagUICardPlaceNotice m)
	{
		//플레이어 덱 및 배치 정보 업데이트
		//그거 기반으로 포지션들 다시 계산해서 뿌리기
		//유닛 스킬카드 생성 해줘야함
		
		var locationInfos = Game.Instance.GetPlayer().CurrentPlayInfo.FieldDeployLocationInfo;
		var bagUnitCards = Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList;

		bagUnitCards.Remove(m.TargetCard.TargetUnitCard);
		
		var info = locationInfos.Find(info => info.TargetCard == m.TargetCard.TargetCard);
		BagUITile.GetTargetTile(info.Row, info.Col).IsOccupied = false;
		m.TargetTile.IsOccupied = true;
		locationInfos.Remove(info);
		
		locationInfos.Add(new DeployInfo(m.TargetTile.Row, m.TargetTile.Col, m.TargetCard.TargetUnitCard));

		NormalizeLocationInfos();
		
		UpdateAndPropagateTargetPos();

	}


	private void OnCardUnDeploy(BagUICardUnPlaceNotice m)
	{
		//플레이어 덱 및 배치 정보 업데이트
		//그거 기반으로 포지션들 다시 계산해서 뿌리기
		//유닛 스킬카드 제거 해줘야함
		m.TargetTile.IsOccupied = false;
		
		var locationInfos = Game.Instance.GetPlayer().CurrentPlayInfo.FieldDeployLocationInfo;
		var bagUnitCards = Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList;
		
		locationInfos.RemoveAll(info => info.TargetCard == m.TargetCard.TargetCard);
		bagUnitCards.Add(m.TargetCard.TargetUnitCard);
		
		NormalizeLocationInfos();
		
		UpdateAndPropagateTargetPos();
	}

	private void NormalizeLocationInfos()
	{
		var locationInfos = Game.Instance.GetPlayer().CurrentPlayInfo.FieldDeployLocationInfo;
		var tmp = new List<(int, int)>();
		
		for (int row = 0; row < 3; row++)
		{
			if (!BagUITile.GetTargetTile(row, 3).IsOccupied)
			{
				for (int col = 2; col >= 0; col--)
				{
					if (BagUITile.GetTargetTile(row, col).IsOccupied)
					{
						tmp.Add((row, col));
						break;
					}	
				}
			}
		}

		foreach (var target in tmp)
		{
			var (row, col) = target;
			for (var i = locationInfos.Count - 1; i >= 0; i--)
			{
				var info = locationInfos[i];
				if (info.Row == row && info.Col == col)
				{
					locationInfos.RemoveAt(i);
					BagUITile.GetTargetTile(row, col).IsOccupied = false;
					locationInfos.Add(new DeployInfo(row, 3, info.TargetCard));
					BagUITile.GetTargetTile(row, 3).IsOccupied = true;
				}
			}
			
		}
	}

	private void UpdateAndPropagateTargetPos()
	{
		var playInfo = Game.Instance.GetPlayer().CurrentPlayInfo;
		var locationInfos = playInfo.FieldDeployLocationInfo;
		var bagUnitCards = playInfo.BagUnitCardList;
		var deckCards = playInfo.DeckCardList;

		for (var i = 0; i < bagUnitCards.Count; i++)
		{
			//todo : exception check?
			var bagUICard = cardDictionary[bagUnitCards[i]];
			var pos = CalculateBagUnitCardPositionWithIndex(i);
			NoticeSystem.Instance.Send(new BagCardPosUpdateNotice(pos), bagUICard);
		}
		
		for (var i = 0; i < deckCards.Count; i++)
		{
			//todo : exception check?
			var bagUICard = cardDictionary[deckCards[i]];
			var pos = CalculateDeckCardPositionWithIndex(i);
			NoticeSystem.Instance.Send(new BagCardPosUpdateNotice(pos), bagUICard);
		}

		foreach (var info in locationInfos)
		{
			var bagUICard = cardDictionary[info.TargetCard];
			var tile = CalculateFieldUnitCardTile(info);
			NoticeSystem.Instance.Send(new BagCardPosUpdateNotice(tile), bagUICard);
		}
	}

	private Vector3 CalculateBagUnitCardPositionWithIndex(int idx)
	{
		return DeckCardArea.position + LeftTopOffset
		                             + Vector3.right * (idx % cardCountPerRow) * horizontalSpace
		                             + Vector3.down * (idx / cardCountPerRow) * verticalSpace;
	}


	private BagUITile CalculateFieldUnitCardTile(DeployInfo info)
	{
		return BagUITile.GetTargetTile(info.Row, info.Col);
	}


	private Vector3 CalculateDeckCardPositionWithIndex(int idx)
	{
		return DeckCardArea.position + LeftTopOffset
		                + Vector3.right * (idx % cardCountPerRow) * horizontalSpace
						+ Vector3.down * (idx / cardCountPerRow + DeckCardStartRow) * verticalSpace;
	}
}