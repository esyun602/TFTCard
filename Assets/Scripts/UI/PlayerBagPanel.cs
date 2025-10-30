using System.Collections.Generic;
using System.Linq;
using MessageSystem;
using NUnit.Framework;
using UI;
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

	[SerializeField] private GameObject skillCardDivider;

	[SerializeField] private float skillCardDividerHeight;

	[SerializeField] private float bottomOffsetHeight;

	[SerializeField] private float originHeight;

	[SerializeField] private Transform synergyArea;

	private List<PooledUnityObject> synergyLabelList;
	
	public Vector3 LeftTopOffset;
	public float horizontalSpace;
	public float verticalSpace;
	public int cardCountPerRow;

	public Dictionary<ICard, BagUICard> cardDictionary;
	private int BagUnitCardRowCount => 
		!Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList.Any() 
			? 0 
		: (Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList.Count() - 1) / cardCountPerRow + 1;
	private int BagSkillCardRowCount => 
		Game.Instance.GetPlayer().CurrentPlayInfo.TotalDeckCardsCount == 0 
			? 0
			: (Game.Instance.GetPlayer().CurrentPlayInfo.TotalDeckCardsCount - 1) / cardCountPerRow + 1;
	
	
	protected override void Init(object param)
	{
		instance = this;

		cardDictionary = new();
		NoticeSystem.Instance.Subscribe<BagUITileHoverNotice>(OnTileHover);
		NoticeSystem.Instance.Subscribe<BagUICardPlaceNotice>(OnCardDeploy);
		NoticeSystem.Instance.Subscribe<BagUICardUnPlaceNotice>(OnCardUnDeploy);
		NoticeSystem.Instance.Subscribe<UnitSkillCardUpdateNotice>(OnUnitSkillCardChange);
		NoticeSystem.Instance.Subscribe<BagSynergyUpdateNotice>(OnSynergyUpdate);
		InitializeBagUnitCards();
		InitializeDeckCards();
		InitializeField();
		
		skillCardDivider.transform.localPosition = GetSkillCardDividerPos();
		ExpandBagArea();
		//todo: fix

		synergyLabelList = new();
		UnityObjectPool.GetOrCreateUIPool("BagSynergyLabel").transform.SetParent(transform);
		UpdateSynergyArea(Game.Instance.GetPlayer().CurrentPlayInfo.SynergyNumDict);
	}

	private void OnSynergyUpdate(BagSynergyUpdateNotice m)
	{
		UpdateSynergyArea(m.SynergyInfo);
	}
	
	private void UpdateSynergyArea(IEnumerable<KeyValuePair<SynergyCategory, int>> dictionary)
	{
		DisposeAllSynergy();
		foreach (var kvp in dictionary)
		{
			var category = kvp.Key;
			var count = kvp.Value;

			var pool = UnityObjectPool.GetOrCreateUIPool("BagSynergyLabel").Instantiate(parent: synergyArea);
			synergyLabelList.Add(pool);
			pool.GetComponent<BagSynergyLabel>().Initialize(category, count);
		}
	}

	private void DisposeAllSynergy()
	{
		foreach (var po in synergyLabelList)
		{
			po.Dispose();	
		}
		
		synergyLabelList.Clear();
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
		NoticeSystem.Instance.Unsubscribe<UnitSkillCardUpdateNotice>(OnUnitSkillCardChange);
		NoticeSystem.Instance.Unsubscribe<BagSynergyUpdateNotice>(OnSynergyUpdate);
		DisposeAllSynergy();
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
			bagUICard.CanUnplaced = !deployInfos[i].IsFixed;
			cardDictionary.Add(deployInfos[i].TargetCard, bagUICard);
			CalculateFieldUnitCardTile(deployInfos[i]).IsOccupied = true;
		}
	}

	private void InitializeBagUnitCards()
	{
		var bagPool = UnityObjectPool.GetOrCreateUIPool("BagUnitCard");
		bagPool.transform.SetParent(transform);
		var cardList = Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList;
		var idx = 0;
		foreach (var card in cardList)
		{
			var pos = CalculateBagUnitCardPositionWithIndex(idx++);
			var bagUICard = bagPool.Instantiate(pos, parent: DeckCardArea, useLocalPos: true).GetComponent<BagUnitCard>();
			bagUICard.Initialize(card, pos);
			bagUICard.CanUnplaced = true;
			cardDictionary.Add(card, bagUICard);
			
		}
	}

	private void InitializeDeckCards()
	{
		var skillPool = UnityObjectPool.GetOrCreateUIPool("BagSkillCard");
		var unitActionPool = UnityObjectPool.GetOrCreateUIPool("BagUnitActionCard");
		skillPool.transform.SetParent(transform);
		unitActionPool.transform.SetParent(transform);
		//todo: fix
		var cardList = Game.Instance.GetPlayer().CurrentPlayInfo.TotalDeckCards.ToList();
		for (int i = 0; i < cardList.Count; i++)
		{
			var pos = CalculateDeckCardPositionWithIndex(i);
			BagSkillCard bagUICard;
			if (cardList[i].SkillCardStaticSpec is UnitSkillCardSpec)
			{
				bagUICard = unitActionPool.Instantiate(pos, parent: DeckCardArea, useLocalPos: true).GetComponent<BagSkillCard>();
			}
			else
			{
				bagUICard = skillPool.Instantiate(pos, parent: DeckCardArea, useLocalPos: true).GetComponent<BagSkillCard>();
			}

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

	private void OnUnitSkillCardChange(UnitSkillCardUpdateNotice _)
	{
		SyncCardUIToPlayInfo();

		UpdateAndPropagateTargetPos();
		SyncScroll();
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
		SyncScroll();
	}


	private void OnCardUnDeploy(BagUICardUnPlaceNotice m)
	{
		//m.TargetTile.IsOccupied = false;

		var playInfo = Game.Instance.GetPlayer().CurrentPlayInfo;
		if (playInfo.FieldDeployLocationInfo.Find((x) => x.TargetCard == m.TargetCard.TargetCard).IsFixed)
		{
			return;
		}

		playInfo.UndeployCard(m.TargetCard.TargetUnitCard);
		//todo: pooledObj를 보유하고 있는게 나을수도
		/*cardDictionary[unitSkillCard].GetComponent<PooledUnityObject>().Dispose();
		cardDictionary.Remove(unitSkillCard);*/

		SyncCardUIToPlayInfo();

		UpdateAndPropagateTargetPos();
		SyncScroll();
	}

	private void SyncScroll()
	{
	}

	//성능에 문제가 있으면 reflect 말고 메세지 받아서 하는게 나을수도
	private void SyncCardUIToPlayInfo()
	{
		//데이터에 있는데 없는거 만들기
		var playInfo = Game.Instance.GetPlayer().CurrentPlayInfo;
		var locationInfos = playInfo.FieldDeployLocationInfo;
		var bagUnitCards = playInfo.BagUnitCardList;
		//todo: fix
		var deckCards = playInfo.TotalDeckCards;

		foreach (var card in bagUnitCards)
		{
			if (!cardDictionary.ContainsKey(card))
			{
				var pos = CalculateDeckCardPositionWithIndex(deckCards.Count() - 1);
				var bagUICard = UnityObjectPool.GetOrCreateUIPool("BagUnitCard")
					.Instantiate(pos, parent: DeckCardArea, useLocalPos: true).GetComponent<BagUnitCard>();
				bagUICard.Initialize(card, pos);
				bagUICard.CanUnplaced = true;
				cardDictionary.Add(card, bagUICard);
			}
			else
			{
				cardDictionary[card].transform.SetParent(DeckCardArea);
			}
		}

		foreach (var card in deckCards)
		{
			if (!cardDictionary.ContainsKey(card))
			{
				var pos = CalculateDeckCardPositionWithIndex(deckCards.Count() - 1);
				var bagUICard = UnityObjectPool.GetOrCreateUIPool(card.SkillCardStaticSpec is UnitSkillCardSpec
						? "BagUnitActionCard"
						: "BagSkillCard")
					.Instantiate(pos, parent: DeckCardArea, useLocalPos: true).GetComponent<BagSkillCard>();
				bagUICard.Initialize(card, pos);
				cardDictionary.Add(card, bagUICard);
			}
			else
			{
				cardDictionary[card].transform.SetParent(DeckCardArea);
			}
		}

		foreach (var info in locationInfos)
		{
			if (!cardDictionary.ContainsKey(info.TargetCard))
			{
				var tile = CalculateFieldUnitCardTile(info);
				var bagUICard = UnityObjectPool.GetOrCreateUIPool("BagUnitCard")
					.Instantiate(tile.GetPosition()).GetComponent<BagUnitCard>();
				bagUICard.CanUnplaced = !info.IsFixed;
				bagUICard.Initialize(info.TargetCard, tile);
				cardDictionary.Add(info.TargetCard, bagUICard);
			}
			else
			{
				cardDictionary[info.TargetCard].transform.SetParent(UnityObjectPool.GetOrCreateUIPool("BagUnitCard").transform);
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

			if (key is SkillCardBase sk && !deckCards.Contains(sk))
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
		//todo: fix
		var deckCards = playInfo.TotalDeckCards.ToList();

		var idx = 0;
		foreach (var card in bagUnitCards)
		{
			var bagUICard = cardDictionary[card];
			var pos = CalculateBagUnitCardPositionWithIndex(idx++);
			NoticeSystem.Instance.Send(new BagCardPosUpdateNotice(pos), bagUICard);
		}

		skillCardDivider.transform.localPosition = GetSkillCardDividerPos();

		for (var i = 0; i < deckCards.Count; i++)
		{
			//todo : exception check?
			var bagUICard = cardDictionary[deckCards[i]];
			var pos = CalculateDeckCardPositionWithIndex(i);
			NoticeSystem.Instance.Send(new BagCardPosUpdateNotice(pos), bagUICard);
		}

		ExpandBagArea();

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
		return LeftTopOffset
		       + Vector3.right * ((idx % cardCountPerRow) * horizontalSpace)
		       + Vector3.down * ((idx / cardCountPerRow) * verticalSpace);
	}


	private BagUITile CalculateFieldUnitCardTile(DeployInfo info)
	{
		return bagUITileList[info.Row * 4 + info.Col];
	}


	private Vector3 CalculateDeckCardPositionWithIndex(int idx)
	{
		return LeftTopOffset
		       + Vector3.right * ((idx % cardCountPerRow) * horizontalSpace)
		       + Vector3.down * ((idx / cardCountPerRow + BagUnitCardRowCount) * verticalSpace + skillCardDividerHeight);
	}

	private Vector3 GetSkillCardDividerPos()
	{
		return skillCardDivider.transform.localPosition.GetX0z(
			-BagUnitCardRowCount * verticalSpace + LeftTopOffset.y);
	}

	private void ExpandBagArea()
	{
		//todo: fix
		DeckCardArea.offsetMin = new Vector2(960f, Mathf.Min(0,
			-(BagUnitCardRowCount + BagSkillCardRowCount) * verticalSpace - skillCardDividerHeight + LeftTopOffset.y + originHeight - bottomOffsetHeight));
	}
}