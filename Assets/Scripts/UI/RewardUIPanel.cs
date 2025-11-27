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

	[SerializeField] private GameObject goldButton;
	[SerializeField] private GameObject unitButton;

	[SerializeField] private GameObject additionalGoldButton;
	[SerializeField] private TextMeshProUGUI additionalGoldText;
	
	protected override void Init(object param)
	{
		cancelAction = ((RewardUIPanelGenState)param).doneAction;

		cardRewardPanel.SetActive(false);
		selectPanel.SetActive(true);
		goldButton.SetActive(true);
		unitButton.SetActive(true);
		
		RenewCandidates();

		if (Game.Instance.GetGameMode<BattleStageGameMode>().BattleGlobalModifier.RewardGoldAdd != 0)
		{
			additionalGoldButton.SetActive(true);
			additionalGoldText.text =
				$"추가 골드 (+{Game.Instance.GetGameMode<BattleStageGameMode>().BattleGlobalModifier.RewardGoldAdd}G)";
		}
		else
		{
			additionalGoldButton.SetActive(false);
		}
		
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

	public void OnAdditionalGoldClick()
	{
		//todo: 골드값 임시1
		SfxManager.Instance.Play2D("Coins 07");
		Game.Instance.GetPlayer().CurrentPlayInfo.GainGold(Game.Instance.GetGameMode<BattleStageGameMode>().BattleGlobalModifier.RewardGoldAdd);
		additionalGoldButton.SetActive(false);
	}
	
	public void OnGoldClick()
	{
		//todo: 골드값 임시1
		SfxManager.Instance.Play2D("Coins 07");
		Game.Instance.GetPlayer().CurrentPlayInfo.GainGold(50);
		goldButton.SetActive(false);
	}
	
	public void OnSelectCardClick()
	{
		SfxManager.Instance.Play2D("cardhover");
		selectPanel.SetActive(false);
		unitButton.SetActive(false);
		cardRewardPanel.SetActive(true);
	}

	public void OnCloseClick()
	{
		OnEnd();
	}

	public void OnCardClick(DraftUICardSelectedNotice notice)
	{
		//todo: fix
		SfxManager.Instance.Play2D("cardclick");
		Game.Instance.GetPlayer().CurrentPlayInfo.AddCard((TacticsCard)notice.SelectedCard.TargetCard);
		cardRewardPanel.SetActive(false);
		selectPanel.SetActive(true);
	}
	
	public void OnSelectPanelCloseClick()
	{
		selectPanel.SetActive(true);
		unitButton.SetActive(true);
		cardRewardPanel.SetActive(false);
	}

	private void OnEnd()
	{
		cancelAction?.Invoke();
	}
}