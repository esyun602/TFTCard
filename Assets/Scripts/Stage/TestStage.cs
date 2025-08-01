
using System.Collections;
using UnityEngine;
using Coroutine;
using WaitForSeconds = Coroutine.WaitForSeconds;

public class TestStage : BattleStage
{
	private UnitCardSpec unitCardSpec;
	public TestStage(UnitCardSpec unitCardSpec, StageSpec stageSpec) : base(stageSpec)
	{
		this.unitCardSpec = unitCardSpec;
	}
	
	protected override void OnLoad()
	{
		base.OnLoad();
	}

	protected override void OnStart()
	{
	}
	
	protected override void OnUnLoad()
	{
		base.OnUnLoad();
	}

}