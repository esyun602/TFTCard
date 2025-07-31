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
		(Camera.main.transform.position + (Constant.HandCenterZOffset - Camera.main.orthographicSize) * Camera.main.transform.up).GetX0z(Constant.HandCenterYPos);

	private float cardDistance = 1.3f;
	private float cardRotationAngle = 5f;

	private InputBlockFlag blockInput;

	public void Initialize()
	{
		blockInput = InputBlockFlag.All;
	}

	public void Dispose()
	{
	}
	
	//todo: handler를 안넘겨줄 이유가 있나
	public void UpdateBlockFlags(InputBlockFlag flag)
	{
		blockInput = flag;
		
		foreach (var card in CardList)
		{
			card.UpdateBlockInput(blockInput);
		}
	}

	public void AddCard(BattleCardObjectInHand card)
	{
		if (CardList.Count >= Constant.PlayerHandMax)
		{
			return;
		}
		
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
			                i * -Camera.main.transform.forward * Constant.HandIndexYOffset;
			
			NoticeSystem.Instance.Send(
				new CardHandPosUpdatedNotice(
					targetPos,
					Quaternion.AngleAxis(-cardRotationAngle * i, Camera.main.transform.forward) *
					Quaternion.AngleAxis(startAngle, Camera.main.transform.forward) * Camera.main.transform
						.localRotation,
					(lineTargetPos + Camera.main.transform.up).GetX0z(Constant.HandHoverYPos)),
				CardList[i]
			);
		}
	}
}