using System.Collections.Generic;
using MessageSystem;

public class FieldCardFxHandler : IFieldCardFxHandler
{
	public FieldCardFxHandler(UnitCardInField owner)
	{
		Owner = owner;
	}

	public UnitCardInField Owner { get; }
	public bool ActivateFx { get; private set; }

	public void Initialize()
	{
		NoticeSystem.Instance.Subscribe<SkillHandCardSelectNotice>(OnSelect);
		NoticeSystem.Instance.Subscribe<SkillHandCardSelectCancelNotice>(OnSelectCancel);
		NoticeSystem.Instance.Subscribe<SkillHandCardHoverNotice>(OnHover);
		NoticeSystem.Instance.Subscribe<SkillHandCardRemoveHoverNotice>(OnRemoveHover);
		NoticeSystem.Instance.Subscribe<SkillHandCardStartUseNotice>(OnStartUse);
		ActivateFx = false;
	}

	private void OnStartUse(SkillHandCardStartUseNotice m)
	{
		if(m.SelectedCard.TargetCard is UnitSkillCard card 
		   && Owner.TargetUnitCard.UnitSkillCard.Contains(card))
			ActivateFx = false;
	}

	private void OnHover(SkillHandCardHoverNotice m)
	{
		if (m.SelectedCard.TargetCard is UnitSkillCard card 
		    && Owner.TargetUnitCard.UnitSkillCard.Contains(card))
			ActivateFx = true;
		else
			ActivateFx = false;
	}

	private void OnRemoveHover(SkillHandCardRemoveHoverNotice m)
	{
		if(m.SelectedCard.TargetCard is UnitSkillCard card 
		   && Owner.TargetUnitCard.UnitSkillCard.Contains(card))
			ActivateFx = false;
	}

	private void OnSelectCancel(SkillHandCardSelectCancelNotice m)
	{
		if(m.SelectedCard.TargetCard is UnitSkillCard card 
		   && Owner.TargetUnitCard.UnitSkillCard.Contains(card))
			ActivateFx = false;
	}

	private void OnSelect(SkillHandCardSelectNotice m)
	{
		if(m.SelectedCard.TargetCard is UnitSkillCard card 
		   && Owner.TargetUnitCard.UnitSkillCard.Contains(card))
			ActivateFx = true;
		else
			ActivateFx = false;
	}

	public void Dispose()
	{
		NoticeSystem.Instance.Unsubscribe<SkillHandCardSelectNotice>(OnSelect);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardSelectCancelNotice>(OnSelectCancel);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardHoverNotice>(OnHover);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardRemoveHoverNotice>(OnRemoveHover);
		NoticeSystem.Instance.Unsubscribe<SkillHandCardStartUseNotice>(OnStartUse);
		
	}
}