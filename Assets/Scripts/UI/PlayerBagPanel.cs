using System.Collections.Generic;
using System.Linq;
using MessageSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;

//todo: 시너지 표시
//todo: 최대 갯수
//todo: 정렬을 기준 수정 필요 
//todo: bag unit card랑 스킬 카드 포지션 및 배치 방식 관련 수정 필요
//todo: 유닛 개수가 최대 개수보다 적다면 자동 배치되도록 구현 필요
//todo: play info 건드리는 부분 분리 필요?
public class PlayerBagPanel : UIInstance
{
	private static PlayerBagPanel instance;
	public static PlayerBagPanel Instance => instance;
	public override UIType UIType => UIType.Popup;
	public BagUITile CurrentHoverBagUITile { get; private set; }

	[SerializeField] private RectTransform DeckCardArea;

	[SerializeField] private RectTransform DeployArea;

	[SerializeField] private List<BagUITile> bagUITileList;

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
			CalculateFieldUnitCardTile(deployInfos[i]).IsOccupied = true;
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
		var playInfo = Game.Instance.GetPlayer().CurrentPlayInfo;
		
		var locationInfos = playInfo.FieldDeployLocationInfo;

		
		/*
		 
		var pos = CalculateDeckCardPositionWithIndex(deckCards.Count - 1);
		var bagUICard = UnityObjectPool.GetOrCreateUIPool("BagSkillCard")
			.Instantiate(pos).GetComponent<BagSkillCard>();
		bagUICard.Initialize(unitSkillCard, pos);
		cardDictionary.Add(unitSkillCard, bagUICard);
		
		*/
		
		/*
		 
		var info = locationInfos.Find(info => info.TargetCard == m.TargetCard.TargetCard);
		BagUITile.GetTargetTile(info.Row, info.Col).IsOccupied = false;
		m.TargetTile.IsOccupied = true;
		
		*/


		playInfo.DeployCard(m.TargetTile.Row, m.TargetTile.Col, m.TargetCard.TargetUnitCard);
		
		SyncCardUIToPlayInfo();
		
		UpdateAndPropagateTargetPos();
	}


	private void OnCardUnDeploy(BagUICardUnPlaceNotice m)
	{
		//m.TargetTile.IsOccupied = false;
		
		var playInfo = Game.Instance.GetPlayer().CurrentPlayInfo;

		playInfo.UndeployCard(m.TargetCard.TargetUnitCard);
		//todo: pooledObj를 보유하고 있는게 나을수도
		/*cardDictionary[unitSkillCard].GetComponent<PooledUnityObject>().Dispose();
		cardDictionary.Remove(unitSkillCard);*/
		
		SyncCardUIToPlayInfo();
		
		UpdateAndPropagateTargetPos();
	}

	//성능에 문제가 있으면 reflect 말고 메세지 받아서 하는게 나을수도
	private void SyncCardUIToPlayInfo()
	{
		//데이터에 있는데 없는거 만들기
		var playInfo = Game.Instance.GetPlayer().CurrentPlayInfo;
		var locationInfos = playInfo.FieldDeployLocationInfo;
		var bagUnitCards = playInfo.BagUnitCardList;
		var deckCards = playInfo.DeckCardList;

		for (var i = 0; i < bagUnitCards.Count; i++)
		{
			if (!cardDictionary.ContainsKey(bagUnitCards[i]))
			{
				var pos = CalculateDeckCardPositionWithIndex(deckCards.Count - 1);
				var bagUICard = UnityObjectPool.GetOrCreateUIPool("BagUnitCard")
					.Instantiate(pos).GetComponent<BagUnitCard>();
				bagUICard.Initialize(bagUnitCards[i], pos);
				cardDictionary.Add(bagUnitCards[i], bagUICard);
			}
		}
		
		for (var i = 0; i < deckCards.Count; i++)
		{
			if (!cardDictionary.ContainsKey(deckCards[i]))
			{
				var pos = CalculateDeckCardPositionWithIndex(deckCards.Count - 1);
				var bagUICard = UnityObjectPool.GetOrCreateUIPool("BagSkillCard")
					.Instantiate(pos).GetComponent<BagSkillCard>();
				bagUICard.Initialize(deckCards[i], pos);
				cardDictionary.Add(deckCards[i], bagUICard);
			}
		}

		foreach (var info in locationInfos)
		{
			if (!cardDictionary.ContainsKey(info.TargetCard))
			{
				var tile = CalculateFieldUnitCardTile(info);
				var bagUICard = UnityObjectPool.GetOrCreateUIPool("BagUnitCard")
					.Instantiate(tile.GetPosition()).GetComponent<BagUnitCard>();
				bagUICard.Initialize(info.TargetCard, tile);
				cardDictionary.Add(info.TargetCard, bagUICard);
			}
		}
		
		
		//데이터에 없는데 있는거 없애기
		//todo: hashset 써서 효율 개선
		
		var toRemove = new List<ICard>();
		foreach (var key in cardDictionary.Keys)
		{
			if (key is UnitCard uc && !bagUnitCards.Contains(uc) && locationInfos.All(info => info.TargetCard != key))
			{
				toRemove.Add(key);
			}

			if (key is SkillCard sk && !deckCards.Contains(sk))
			{
				toRemove.Add(key);
			}
		}

		foreach (var card in toRemove)
		{
			var po = cardDictionary[card].GetComponent<PooledUnityObject>();
			po.Dispose();
			cardDictionary.Remove(card);
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

		foreach (var tile in bagUITileList)
		{
			tile.IsOccupied = false;
		}

		foreach (var info in locationInfos)
		{
			var bagUICard = cardDictionary[info.TargetCard];
			var tile = CalculateFieldUnitCardTile(info);
			NoticeSystem.Instance.Send(new BagCardPosUpdateNotice(tile), bagUICard);
			tile.IsOccupied = true;
		}
	}

	private Vector3 CalculateBagUnitCardPositionWithIndex(int idx)
	{
		return DeckCardArea.position + LeftTopOffset
		                             + Vector3.right * ((idx % cardCountPerRow) * horizontalSpace)
		                             + Vector3.down * ((idx / cardCountPerRow) * verticalSpace);
	}


	private BagUITile CalculateFieldUnitCardTile(DeployInfo info)
	{
		return bagUITileList[info.Row * 4 + info.Col];
	}


	private Vector3 CalculateDeckCardPositionWithIndex(int idx)
	{
		return DeckCardArea.position + LeftTopOffset
		                + Vector3.right * ((idx % cardCountPerRow) * horizontalSpace)
						+ Vector3.down * ((idx / cardCountPerRow + DeckCardStartRow) * verticalSpace);
	}
}