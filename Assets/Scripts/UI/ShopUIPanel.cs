using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
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
	[SerializeField] private GameObject title;
	[SerializeField] private TextMeshProUGUI dialogue;
	
	private float titleTargetPos;
	private string dialogueText;
	
	protected override void Init(object param)
	{
		InitializeAnimation();
		
		cancelAction = ((ShopUIPanelGenState)param).doneAction;

		RenewCandidates();
		
		shopMainPanel.SetActive(false);
		
		NoticeSystem.Instance.Subscribe<DraftUICardSelectedNotice>(OnCardClick);
		NoticeSystem.Instance.Subscribe<TransitionMotionDoneNotice>(RunAnimation);
	}

	private void InitializeAnimation()
	{
		titleTargetPos = title.transform.position.y;
		title.transform.position += Vector3.up * 500f;

		dialogueText = dialogue.text;
		dialogue.text = "";
		
		dialogue.transform.parent.localScale = Vector3.zero;
	}

	private void RunAnimation(TransitionMotionDoneNotice _)
	{
		NoticeSystem.Instance.Unsubscribe<TransitionMotionDoneNotice>(RunAnimation);
		
		var seq = DOTween.Sequence();
		seq.Append(title.transform.DOMoveY(titleTargetPos, 1f));
		var parent = dialogue.transform.parent;
		seq.Append(parent.DOScale(Vector3.one, 0.5f));

		seq.Append(dialogue.DOText(dialogueText, 1f));
		seq.SetTarget(dialogue.transform);
		seq.Play();
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
		if (Game.Instance.GetPlayer().CurrentPlayInfo.TryUseGold(5))
		{
			SfxManager.Instance.Play2D("Coins 07");
			notice.SelectedCard.gameObject.SetActive(false);
			Game.Instance.GetPlayer().CurrentPlayInfo.AddCard((TacticsCard)notice.SelectedCard.TargetCard);
		}
	}

	private void OnEnd()
	{
		cancelAction?.Invoke();
	}
}