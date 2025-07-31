using System;
using System.Linq;

public class ChainReactionAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private IBattleObject target;

	public ChainReactionAction(ChainReactionActionSpec spec)
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
			var enumerator = Game.Instance.GetGameMode<BattleStageGameMode>().BattleFieldSystem
				.GetAllObjectOfType(ObjectType.Enemy).Where(x =>
					x.UnitCardBattleStat.GetValueByValueType(BattleValueType.Catalyst) > 0);

			foreach (var bo in enumerator)
			{
				bo.Damage(new DamageInfo()
				{
					Sender = stat.Owner,
					Dmg = 1,
				});
			}
			
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