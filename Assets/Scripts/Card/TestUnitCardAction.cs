using System.Collections;
using Coroutine;
using DG.Tweening;
using UnityEngine;

public class TestUnitCardAction : UnitCardActionBase
{
	private float timePassed = 0f;
	private float actionDuration;
	private GameObject fxPrefab;
	private GridSelector gridInfo;

	public override GridSelector AttackRangeInfo => gridInfo;

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		routineDone = false;
		
		timePassed += dt;
		if (timePassed > 0.15f && timePassed - dt < 0.15f)
		{
			var map = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map;
			
			var targetTile = map.GetAttackTargetTile(owner);
			if (targetTile != null)
			{
				var target = map.GetBattleObjectOfTile(targetTile);
				Object.Instantiate(fxPrefab, targetTile.GetPosition(), Quaternion.identity);
				if (target?.ObjectType.IsHostile(owner.ObjectType) == true)
				{
					map.GetBattleObjectOfTile(targetTile).Damage(owner, owner.UnitCardBattleStat.Attack);
				}
			}
		}
		else if (timePassed > 0.5f)
		{
			routineDone = true;
		}
	}

	protected override void OnTrigger(object triggerInfo = null)
	{
		var rotSeq = DOTween.Sequence();
		rotSeq.Append(owner.Transform.DORotate(
			(Quaternion.AngleAxis((owner.ObjectType == ObjectType.Ally ? -1 : 1) * 20f, owner.Transform.forward) *
			 owner.Transform.localRotation).eulerAngles,
			0.15f).SetEase(Ease.InQuart));
		
		rotSeq.Append(owner.Transform.DORotate(
			(owner.Transform.localRotation).eulerAngles,
			0.5f).SetEase(Ease.OutQuart));
		
		var movSeq = DOTween.Sequence();
		movSeq.Append(owner.Transform
			.DOMove((owner.ObjectType == ObjectType.Ally ? 1f : -1f) * 3f * owner.Transform.right + owner.Position,
				0.15f).SetEase(Ease.InQuart));
		movSeq.Append(owner.Transform.DOMove(owner.Position, 0.5f).SetEase(Ease.OutQuart));

		movSeq.Play();
		rotSeq.Play();
		

		timePassed = 0f;
	}

	protected override void OnCancel()
	{
		throw new System.NotImplementedException();
	}

	public TestUnitCardAction(TestUnitCardActionData actionData)
	{
		actionDuration = actionData.actionDuration;
		fxPrefab = actionData.fxPrefab;
		gridInfo = actionData.actionRange;
	}
}