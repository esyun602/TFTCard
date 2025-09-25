using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagnetMonsterAttractionAction : UnitSkillCardActionBase
{
	private float timePassed = 0f;

	public MagnetMonsterAttractionAction(MagnetMonsterAttractionActionSpec spec) : base(spec)
	{
	}

	public override IEnumerable<ITile> Targets => new ITile[] { };

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		routineDone = false;

		timePassed += dt;
		if (timePassed > 0.15f && timePassed - dt < 0.15f)
		{
			
		}
		else if (timePassed > 1.5f)
		{
			routineDone = true;
		}
	}

	protected override void OnTrigger()
	{
		BattleStat.Owner.RunAttackMotion();
		timePassed = 0f;
	}

	protected override void OnCancel()
	{
	}
}