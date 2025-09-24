using System.Collections.Generic;
using UnityEngine;

//테스트를 위한 임시 어댑터
public class UnitActionAdapterSpec : SkillCardActionSpec
{
	private string targetUnitCardAction;
	public override SkillCardActionBase CreateCardAction()
	{
		return new UnitActionAdapter(this, GameDataSystem.Instance.GetGameData<ActionData>().GetUnitActionByName(targetUnitCardAction).CreateCardAction());
	}

	protected override void OnInitialize(Dictionary<string, object> param)
	{
		targetUnitCardAction = param.GetString("TargetUnitCardAction");
	}
}