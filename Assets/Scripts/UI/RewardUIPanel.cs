using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MessageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardUIPanelGenState
{
	public int GoldAmount { get; set; }
	public Action doneAction { get; set; }
}

public class RewardUIPanel : UIInstance
{
	public override UIType UIType => UIType.SceneUI;
	private Action cancelAction;

	private List<TacticsCardSpec> cardDataList;
	[SerializeField] private GameObject cardRewardPanel;
	[SerializeField] private GameObject selectPanel;
	
	[SerializeField] private List<DraftUISkillCard> cardist;

	protected override void Init(object param)
	{
		cancelAction = ((RewardUIPanelGenState)param).doneAction;

		cardRewardPanel.SetActive(false);
		selectPanel.SetActive(true);
		
		RenewCandidates();
		
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

	public void OnGoldClick()
	{
		//todo: 골드값 임시
		Game.Instance.GetPlayer().CurrentPlayInfo.GainGold(50);
		selectPanel.SetActive(false);
		OnEnd();
	}
	
	public void OnSelectCardClick()
	{
		selectPanel.SetActive(false);
		cardRewardPanel.SetActive(true);
	}

	public void OnCloseClick()
	{
		OnEnd();
	}

	public void OnCardClick(DraftUICardSelectedNotice notice)
	{
		//todo: fix
		Game.Instance.GetPlayer().CurrentPlayInfo.AddCard((TacticsCard)notice.SelectedCard.TargetCard);
		cardRewardPanel.SetActive(false);
		OnEnd();
	}

	private void OnEnd()
	{
		cancelAction?.Invoke();
	}
}