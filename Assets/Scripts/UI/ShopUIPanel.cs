using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MessageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIPanelGenState
{
	public Action doneAction { get; set; }
}

public class ShopUIPanel : UIInstance
{
	public override UIType UIType => UIType.SceneUI;
	private int rollCount;
	private Action cancelAction;

	private List<TacticsCardSpec> cardDataList;
	[SerializeField] private List<DraftUISkillCard> cardist;
	[SerializeField] private GameObject shopMainPanel;
	
	protected override void Init(object param)
	{
		cancelAction = ((ShopUIPanelGenState)param).doneAction;

		RenewCandidates();
		
		shopMainPanel.SetActive(false);
		
		NoticeSystem.Instance.Subscribe<DraftUICardSelectedNotice>(OnCardClick);
	}

	protected override void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<DraftUICardSelectedNotice>(OnCardClick);
	}

	private void RenewCandidates()
	{
		cardDataList = new();
		//todo: constant
		
		cardDataList = GameDataSystem.Instance.GetGameData<CardData>().GetRandomTacticsCardSpecs(3);

		for (int i = 0; i < 3; i++)
		{
			cardist[i].gameObject.SetActive(true);
			cardist[i].Initialize(cardDataList[i]);
		}
	}

	public void OnCloseClick()
	{
		OnEnd();
	}

	public void OnShopEnter()
	{
		shopMainPanel.SetActive(true);
	}

	public void OnShopExit()
	{
		ExitShopPanel();
	}

	public void ExitShopPanel()
	{
		shopMainPanel.SetActive(false);
	}
	
	//todo:fix gold
	public void OnCardClick(DraftUICardSelectedNotice notice)
	{
		//todo: fix
		if (Game.Instance.GetPlayer().CurrentPlayInfo.TryUseGold(20))
		{
			Game.Instance.GetPlayer().CurrentPlayInfo.AddCard((TacticsCard)notice.SelectedCard.TargetCard);
		}
	}

	private void OnEnd()
	{
		cancelAction?.Invoke();
	}
}