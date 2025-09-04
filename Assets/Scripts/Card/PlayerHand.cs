using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

/// <summary>
/// card가 선택되지 않은 상태일 경우 player hand가 set해준 targetPos로 따라감
/// </summary>
public class PlayerHand
{
	public List<BattleCardObjectInHand> CardList { get; } = new();
	private float StartOffset => (CardList.Count - 1) / 2f * CardDistance;
	private float startAngle => (CardList.Count - 1) / 2f * cardRotationAngle;

	private Vector3 handCenter =>
		(Camera.main.transform.position + (Constant.HandCenterZOffset - Camera.main.orthographicSize) * Camera.main.transform.up).GetX0z(Constant.HandCenterYPos);

	private float CardDistance => 3f - 0.15f * CardList.Count;
	private float cardRotationAngle = 5f;
	private int hoveredIdx = -1;

	private InputBlockFlag blockInput;

	//todo: fix
	public bool HasEnemyCard => CardList.Find( x => (x.Stat as UnitSkillCardBattleStat)?.Owner?.ObjectType == ObjectType.Enemy) != null;
	
	public void Initialize()
	{
		blockInput = InputBlockFlag.All;
		NoticeSystem.Instance.Subscribe<SkillHandCardHoverNotice>(OnHover);
		NoticeSystem.Instance.Subscribe<SkillHandCardRemoveHoverNotice>(OnRemoveHover);
		NoticeSystem.Instance.Subscribe<SkillHandCardSelectNotice>(OnSelect);
		NoticeSystem.Instance.Subscribe<SkillHandCardSelectCancelNotice>(OnSelectCancel);
		NoticeSystem.Instance.Subscribe<SkillHandCardStartUseNotice>(OnStartUse);
	}

	private void OnSelect(SkillHandCardSelectNotice m)
	{
		hoveredIdx = CardList.IndexOf(m.SelectedCard);
		AlignCards();
	}

	private void OnSelectCancel(SkillHandCardSelectCancelNotice m)
	{
		hoveredIdx = -1;
		AlignCards();
	}

	private void OnStartUse(SkillHandCardStartUseNotice m)
	{
		hoveredIdx = -1;
		AlignCards();
	}

	private void OnHover(SkillHandCardHoverNotice m)
	{
		hoveredIdx = CardList.IndexOf(m.SelectedCard);
		AlignCards();
	}

	private void OnRemoveHover(SkillHandCardRemoveHoverNotice m)
	{
		hoveredIdx = -1;
		AlignCards();
	}

	public void Dispose()
	{
		NoticeSystem.Instance.Unsubscribe<SkillHandCardHoverNotice>(OnHover);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardRemoveHoverNotice>(OnRemoveHover);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardSelectNotice>(OnSelect);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardSelectCancelNotice>(OnSelectCancel);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardStartUseNotice>(OnStartUse);
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
				StartOffset * Vector3.right + Vector3.right * i * CardDistance;
			if (hoveredIdx != -1 && hoveredIdx != i)
			{
				var dist = Mathf.Abs(i - hoveredIdx);
				lineTargetPos += (i < hoveredIdx ? Vector3.left : Vector3.right) * (Mathf.Exp((-dist + 1) / 3f) * 1.5f) ;
			}
			
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