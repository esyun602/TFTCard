using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MessageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIPanelGenState
{
	public int rollCount { get; set; }
	public Action doneAction { get; set; }
}

public class ShopUIPanel : UIInstance
{
	public override UIType UIType => UIType.SceneUI;
	private int rollCount;
	private Action cancelAction;

	private List<SkillCardSpec> cardDataList;
	[SerializeField] private List<DraftUISkillCard> cardist;
	[SerializeField] private TextMeshProUGUI rollCountUI;

	protected override void Init(object param)
	{
		rollCount = ((ShopUIPanelGenState)param).rollCount;
		cancelAction = ((ShopUIPanelGenState)param).doneAction;

		UpdateRollCountText();
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
		
		cardDataList = GameDataSystem.Instance.GetGameData<CardData>().GetRandomSkillCardSpecs(3);

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

	public void OnRollClick()
	{
		RenewCandidates();
		rollCount--;
		UpdateRollCountText();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void UpdateRollCountText()
	{
		rollCountUI.text = $"Roll\nCount\n{rollCount}";
	}

	public void OnCardClick(DraftUICardSelectedNotice notice)
	{
		//todo: 어케하지
		Game.Instance.GetPlayer().CurrentPlayInfo.DeckCardList.Add((SkillCard)notice.SelectedCard.TargetCard);
		OnEnd();
	}

	private void OnEnd()
	{
		cancelAction?.Invoke();
		Game.Instance.UIManager.RemoveUI(Id);
	}
}