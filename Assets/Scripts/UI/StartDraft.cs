using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

public class StartDraft : UIInstance
{
	[SerializeField] private int cardPerDraft;
	[SerializeField] private int draftCount;
	[SerializeField] private string[] draftCandidatesStrings;
	[SerializeField] private GameObject endButton;
	private int currentDraftCount;
	private UnityObjectPool cardPool;
	private List<PooledUnityObject> currentCardList = new();
	private Vector3[] candidatePosList;

	public override UIType UIType => UIType.SceneUI;

	protected override void Init(object param)
	{
		var rectTransform = GetComponent<RectTransform>();
		currentDraftCount = 0;
		cardPool = UnityObjectPool.GetOrCreateUIPool("DraftCardPrefab");
		cardPool.transform.SetParent(transform);

		candidatePosList = rectTransform.GetHorizontalDivisions(cardPerDraft + 2);

		NoticeSystem.Instance.Subscribe<DraftUICardSelectedNotice>(OnSelected);

		ShowDraft();
	}

	protected override void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<DraftUICardSelectedNotice>(OnSelected);
	}

	private void OnSelected(DraftUICardSelectedNotice m)
	{
		Game.Instance.GetPlayer().CurrentPlayInfo.BagUnitCardList.Add(m.SelectedCard.TargetCard);
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
			var randomCard = draftCandidatesStrings[Random.Range(0, draftCandidatesStrings.Length)];
			var pos = candidatePosList[i];
			var instance = cardPool.Instantiate(pos);
			currentCardList.Add(instance);
			instance.GetComponent<DraftUICard>().Initialize(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecByName(randomCard));
		}
	}

	private void ShowEnd()
	{
		endButton.SetActive(true);
	}

	public void OnEnd()
	{
		Game.Instance.ChangeGameMode(new MapGameMode());
	}
}