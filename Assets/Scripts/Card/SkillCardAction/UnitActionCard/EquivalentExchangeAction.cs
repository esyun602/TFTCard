using System;

public class EquivalentExchangeAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private IBattleObject target;

	public EquivalentExchangeAction(EquivalentExchangeActionSpec spec)
	{
	}

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		if (canceled)
		{
			routineDone = true;
			return;
		}

		routineDone = false;

		timePassed += dt;
		if (timePassed > 0f)
		{
			//todo: owner가 있으면 그냥 스탯에 owner 스탯을 합쳐버리는 방향으로 수정
			var handLimit = Constant.PlayerHandMax;
			var toDraw = handLimit - Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.PlayerHand.CardList.Count;

			for (var i = 0; i < toDraw; i++)
			{
				Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DrawCard();
			}

			stat.Owner.Damage(new DamageInfo()
			{
				Sender = stat.Owner,
				Dmg = toDraw
			});
			
			routineDone = true;
		}
	}

	protected override void OnTrigger(object triggerInfo)
	{
		timePassed = 0f;
		if (triggerInfo is not TargetingActionTriggerInfo ti)
		{
			throw new ArgumentException();
		}

		target = ti.Target;
	}

	protected override void OnCancel()
	{
		canceled = true;
	}
}