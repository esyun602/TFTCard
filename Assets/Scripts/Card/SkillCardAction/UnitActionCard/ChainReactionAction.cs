using System;
using System.Linq;

public class ChainReactionAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;

	public ChainReactionAction(ChainReactionActionSpec spec)
	{
	}

	public override object[] DescParams { get; }

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
					x.UnitCardBattleStat.GetValueByValueType(UnitValueType.Catalyst) > 0);

			foreach (var bo in enumerator)
			{
				bo.Damage(new DamageInfo()
				{
					Sender = BattleStat.Owner,
					Dmg = BattleStat.GetValueByValueType(SkillValueType.Damage),
				});
			}
			
			routineDone = true;
		}
	}

	protected override void OnTrigger(object triggerInfo)
	{
		timePassed = 0f;
	}

	protected override void OnCancel()
	{
		canceled = true;
	}
}