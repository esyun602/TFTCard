using System;
using System.Collections.Generic;
using System.Linq;
using MessageSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class StartDraftGenState
{
	public int CardPerDraft { get; set; }
	public int DraftCount { get; set; }
	public Action DoneAction { get; set; }
}

public class StartDraft : UIInstance
{
	[SerializeField] private int cardPerDraft;
	[SerializeField] private int draftCount;
	[SerializeField] private List<string> draftCandidatesStrings;
	[SerializeField] private GameObject endButton;
	private int currentDraftCount;
	private UnityObjectPool cardPool;
	private List<PooledUnityObject> currentCardList = new();
	private Vector3[] candidatePosList;
	private Action endAction;

	public override UIType UIType => UIType.SceneUI;

	protected override void Init(object param)
	{
		if (param is StartDraftGenState state)
		{
			draftCount = state.DraftCount;
			cardPerDraft = state.CardPerDraft;
			endAction = state.DoneAction;
		}
		var rectTransform = GetComponent<RectTransform>();
		currentDraftCount = 0;
		cardPool = UnityObjectPool.GetOrCreateUIPool("DraftCardPrefab");
		cardPool.transform.SetParent(transform);

		candidatePosList = rectTransform.GetHorizontalDivisions(cardPerDraft + 2, -80f);

		NoticeSystem.Instance.Subscribe<DraftUICardSelectedNotice>(OnSelected);

		ShowDraft();
	}

	protected override void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<DraftUICardSelectedNotice>(OnSelected);
	}

	private void OnSelected(DraftUICardSelectedNotice m)
	{
		Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList.Add((UnitCard)m.SelectedCard.TargetCard);
		for (var i = currentCardList.Count - 1; i >= 0; i--)
		{
			currentCardList[i].Dispose();
		}

		currentCardList.Clear();

		if (currentDraftCount >= draftCount)
		{
			ShowEnd();
		}
		else
		{
			ShowDraft();
		}
	}

	private void ShowDraft()
	{
		currentDraftCount++;
		for (var i = 1; i <= cardPerDraft; i++)
		{
			//todo: fix
			var randomCard = GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecByName(draftCandidatesStrings[Random.Range(0, draftCandidatesStrings.Count)]);
			while (Game.Instance.GetPlayer().CurrentPlayInfo.TotalUnitCards.Select(x => x.UnitCardStaticSpec).Any(x => x == randomCard))
			{
				randomCard = GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecByName(draftCandidatesStrings[Random.Range(0, draftCandidatesStrings.Count)]);
			}
			draftCandidatesStrings.Remove(randomCard.Name);
			var pos = candidatePosList[i];
			var instance = cardPool.Instantiate(pos);
			currentCardList.Add(instance);
			instance.GetComponent<DraftUIUnitCard>().Initialize(randomCard);
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