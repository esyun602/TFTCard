using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

/// <summary>
/// card가 선택되지 않은 상태일 경우 player hand가 set해준 targetPos로 따라감
/// </summary>
public class PlayerHand
{
	public List<BattleCardObjectInHand> CardList { get; } = new();
	private float StartOffset => (CardList.Count - 1) / 2f * cardDistance;
	private float startAngle => (CardList.Count - 1) / 2f * cardRotationAngle;

	private Vector3 handCenter =>
		(Camera.main.transform.position + -Camera.main.orthographicSize * Camera.main.transform.up +
		 Camera.main.transform.up).GetX0z(1f);

	private float cardDistance = 1.3f;
	private float cardRotationAngle = 5f;

	private Dictionary<object, InputBlockFlag> blockRequestDict = new();
	private InputBlockFlag blockInput;

	public void Initialize()
	{
		NoticeSystem.Instance.Subscribe<HandCardSelectNotice>(OnCardSelected);
		NoticeSystem.Instance.Subscribe<HandCardSelectCancelNotice>(OnCardSelectCanceled);
		NoticeSystem.Instance.Subscribe<HandCardStartUseNotice>(OnCardStartUse);
		NoticeSystem.Instance.Subscribe<HandCardEndUseNotice>(OnCardEndUse);
		NoticeSystem.Instance.Subscribe<PlayerTurnStartNotice>(OnPlayerTurnStart);
		NoticeSystem.Instance.Subscribe<PlayerTurnEndNotice>(OnPlayerTurnEnd);
		//todo: fix
		blockInput = InputBlockFlag.Select;
	}

	public void Dispose()
	{
		NoticeSystem.Instance.Unsubscribe<HandCardSelectNotice>(OnCardSelected);
		NoticeSystem.Instance.Unsubscribe<HandCardSelectCancelNotice>(OnCardSelectCanceled);
		NoticeSystem.Instance.Unsubscribe<HandCardStartUseNotice>(OnCardStartUse);
		NoticeSystem.Instance.Unsubscribe<HandCardEndUseNotice>(OnCardEndUse);
		NoticeSystem.Instance.Unsubscribe<PlayerTurnStartNotice>(OnPlayerTurnStart);
		NoticeSystem.Instance.Unsubscribe<PlayerTurnEndNotice>(OnPlayerTurnEnd);
	}

	private void OnPlayerTurnStart(PlayerTurnStartNotice m)
	{
		//todo:fix
		if (!blockRequestDict.ContainsKey(m.PlayerTurnObject))
		{
			blockInput = InputBlockFlag.None;
			return;
		}
		RestoreInputs(InputBlockFlag.Select, m.PlayerTurnObject);
	}

	private void OnPlayerTurnEnd(PlayerTurnEndNotice m)
	{
		BlockInputs(InputBlockFlag.Select, m.PlayerTurnObject);
	}

	private void BlockInputs(InputBlockFlag flag, object requester)
	{
		if (!blockRequestDict.TryAdd(requester, flag))
		{
			blockRequestDict[requester] |= flag;
		}
		UpdateBlockFlags();
	}
	
	private void RestoreInputs(InputBlockFlag flag, object requester)
	{
		if (!blockRequestDict.ContainsKey(requester)) return;
		
		blockRequestDict[requester] &= ~flag;
		if (blockRequestDict[requester] == InputBlockFlag.None)
		{
			blockRequestDict.Remove(requester);
		}
		UpdateBlockFlags();
	}
	
	private void UpdateBlockFlags()
	{
		blockInput = InputBlockFlag.None;
		foreach (var eachFlag in blockRequestDict.Values)
		{
			blockInput |= eachFlag;
		}
		
		foreach (var card in CardList)
		{
			card.UpdateBlockInput(blockInput);
		}
	}
	

	private void OnCardSelected(HandCardSelectNotice m)
	{
		BlockInputs(InputBlockFlag.All, m.SelectedCard);
	}

	private void OnCardSelectCanceled(HandCardSelectCancelNotice m)
	{
		RestoreInputs(InputBlockFlag.All, m.SelectedCard);
	}

	private void OnCardStartUse(HandCardStartUseNotice m)
	{
		BlockInputs(InputBlockFlag.All, m.SelectedCard);
		RemoveCard(m.SelectedCard);
	}

	private void OnCardEndUse(HandCardEndUseNotice m)
	{
		RestoreInputs(InputBlockFlag.All, m.SelectedCard);
	}

	public void AddCard(BattleCardObjectInHand card)
	{
		CardList.Add(card);
		card.UpdateBlockInput(blockInput);

		AlignCards();
	}

	public void RemoveCard(BattleCardObjectInHand card)
	{
		CardList.Remove(card);
		AlignCards();
	}
	
	private void AlignCards()
	{
		for (var i = 0; i < CardList.Count; i++)
		{
			var lineTargetPos = handCenter -
				StartOffset * Vector3.right + Vector3.right * i * cardDistance;
			var targetPos = lineTargetPos + GameDataSystem.Instance.GetGameData<Constant>().HandCardVerticalOffsetCurve
				                .Evaluate(i -
				                          (CardList.Count - 1) / 2f) * Camera.main.transform.up +
			                i * -Camera.main.transform.forward * 0.001f;
			
			NoticeSystem.Instance.Send(
				new CardHandPosUpdatedNotice(
					targetPos,
					Quaternion.AngleAxis(-cardRotationAngle * i, Camera.main.transform.forward) *
					Quaternion.AngleAxis(startAngle, Camera.main.transform.forward) * Camera.main.transform
						.localRotation,
					lineTargetPos + Camera.main.transform.up * 1f +  Camera.main.transform.forward * -0.1f),
				CardList[i]
			);
		}
	}
}