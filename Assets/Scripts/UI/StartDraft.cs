using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MessageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StartDraftGenState
{
	public int CardPerDraft { get; set; }
	public int DraftCount { get; set; }
	public Action DoneAction { get; set; }
	public DraftAnimationType AnimationType { get; set; }
}

public enum DraftAnimationType
{
	None,
	Start,
	Pub,
}

public class StartDraft : UIInstance
{
	[SerializeField] private DraftSelectPanel selectPanel;
	[SerializeField] private int cardPerDraft;
	[SerializeField] private int draftCount;
	[SerializeField] private List<string> draftCandidatesStrings;
	[SerializeField] private GameObject endButton;
	[SerializeField] private Image titleText;
	[SerializeField] private Image titleLabel;
	private int currentDraftCount;
	private UnityObjectPool cardPool;
	private List<PooledUnityObject> currentCardList = new();
	[SerializeField]
	private GameObject[] placeHolder;
	private Vector3[] candidatePosList;
	private Action endAction;
	private DraftAnimationType animationType = DraftAnimationType.None;
	[SerializeField] private float cardScale = 1f;

	public override UIType UIType => UIType.SceneUI;

	protected override void Init(object param)
	{
		if (param is StartDraftGenState state)
		{
			draftCount = state.DraftCount;
			cardPerDraft = state.CardPerDraft;
			endAction = state.DoneAction;
			animationType = state.AnimationType;
		}
		var rectTransform = GetComponent<RectTransform>();
		currentDraftCount = 0;
		cardPool = UnityObjectPool.GetOrCreateUIPool("DraftCardPrefab");
		cardPool.transform.SetParent(transform);

		if (placeHolder.Length < cardPerDraft)
		{
			placeHolder = null;
			candidatePosList = rectTransform.GetHorizontalDivisions(cardPerDraft + 2, -10f)[1..];
		}
		else
		{
			candidatePosList = placeHolder.Select(x => x.transform.position).ToArray();
		}

		NoticeSystem.Instance.Subscribe<DraftUICardSelectedNotice>(OnSelected);

		RunAnimation();
	}

	private void RunAnimation()
	{
		var seq = DOTween.Sequence();
		switch (animationType)
		{
			case DraftAnimationType.Pub:
				titleLabel.transform.localScale = Vector3.zero;
				seq.AppendInterval(0.5f);
				seq.AppendCallback(() => titleLabel.transform.localScale = Vector3.one * 3f);
				seq.Append(titleLabel.transform.DOScale(1f, 0.2f));
				seq.AppendCallback(() => SfxManager.Instance.Play2D("Wood 15"));
				seq.AppendInterval(0.7f);

				seq.AppendCallback(ShowDraft);
				
				for (var i = 0; i < placeHolder.Length; i++)
				{
					placeHolder[i].transform.localScale = Vector3.zero;
					seq.AppendInterval(0.2f);
					var i1 = i;
					seq.AppendCallback(() =>
					{
						placeHolder[i1].transform.localScale = Vector3.one * 3f;
					});
					seq.Append(placeHolder[i1].transform.DOScale(1f, 0.2f));
					seq.AppendCallback(() => SfxManager.Instance.PlayAt("Wood 15", placeHolder[i1].transform.position));
				}
				
				seq.Play();
				break;
			case DraftAnimationType.Start:
				seq.AppendInterval(1f);
				seq.Append(titleText.DOFade(1f, 1f));
				seq.Join(titleLabel.DOFade(1f, 1f));
				seq.AppendCallback(ShowDraft);
				seq.Play();
				break;
		}
	}

	protected override void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<DraftUICardSelectedNotice>(OnSelected);
	}

	private void OnSelected(DraftUICardSelectedNotice m)
	{
		SfxManager.Instance.Play2D("cardclick");
		Game.Instance.GetPlayer().CurrentPlayInfo.AddCard(m.SelectedCard.TargetCard);
		for (var i = currentCardList.Count - 1; i >= 0; i--)
		{
			currentCardList[i].Dispose();
		}

		currentCardList.Clear();

		if (currentDraftCount >= draftCount)
		{
			if (animationType == DraftAnimationType.Start)
			{
				ShowEnd();
			}
			else
			{
				endAction?.Invoke();
			}
		}
		else
		{
			ShowDraft();
		}
	}

	private void ShowDraft()
	{
		currentDraftCount++;
		for (var i = 0; i < cardPerDraft; i++)
		{
			//todo: fix
			var randomCard = GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecByName(draftCandidatesStrings[Random.Range(0, draftCandidatesStrings.Count)]);
			while (Game.Instance.GetPlayer().CurrentPlayInfo.TotalUnitCards.Select(x => x.UnitCardStaticSpec).Any(x => x == randomCard))
			{
				randomCard = GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecByName(draftCandidatesStrings[Random.Range(0, draftCandidatesStrings.Count)]);
			}
			draftCandidatesStrings.Remove(randomCard.Name);
			var pos = candidatePosList[i];
			var instance = cardPool.Instantiate(pos, parent: placeHolder?[i]?.transform);
			currentCardList.Add(instance);
			var duuc = instance.GetComponent<DraftUIUnitCard>();
			duuc.Initialize(randomCard, animationType, cardScale);
			duuc.SetSelectPanel(selectPanel);
		}
	}

	private void ShowEnd()
	{
		endButton.SetActive(true);
	}

	public void OnEnd()
	{
		endAction?.Invoke();
	}
}