using System.Collections;
using System.Collections.Generic;
using Coroutine;
using DG.Tweening;
using UnityEngine;

public class TestUnitCardAction : UnitCardActionBase
{
	private float timePassed = 0f;
	private float actionDuration;
	private GameObject fxPrefab;
	//private GridSelector gridInfo;

	//public override GridSelector AttackRangeInfo => gridInfo;

	public override object[] DescParams { get; }

	public override IEnumerable<ITile> Targets
	{
		get
		{
			yield return GetTarget();
		}
	}

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		routineDone = false;

		timePassed += dt;
		if (timePassed > 0.15f && timePassed - dt < 0.15f)
		{
			var targetTile = GetTarget();
			if (targetTile != null)
			{
				var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
				var target = map.GetBattleObjectOfTile(targetTile);
				if (fxPrefab != null)
				{
					Object.Instantiate(fxPrefab, targetTile.GetPosition() + Vector3.up, Quaternion.identity);
				}
				
				if (target?.ObjectType.IsHostile(owner.ObjectType) == true)
				{
					map.GetBattleObjectOfTile(targetTile).Damage(
						new DamageInfo()
						{
							Sender = owner,
							DamageType = DamageType.NormalAttack,
							Dmg = owner.UnitCardBattleStat.GetValueByValueType(UnitValueType.Attack)
						});
				}
			}
		}
		else if (timePassed > 0.5f)
		{
			routineDone = true;
		}
	}

	private ITile GetTarget()
	{
		var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;

		return map.GetAttackTargetTile(owner);
	}

	protected override void OnTrigger()
	{
		var rotSeq = DOTween.Sequence();
		rotSeq.Append(owner.FrameTransform.DOLocalRotate(
			Quaternion.AngleAxis((owner.ObjectType == ObjectType.Ally ? -1 : 1) * 20f, Vector3.forward).eulerAngles,
			0.15f).SetEase(Ease.InQuart));

		rotSeq.Append(owner.FrameTransform.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.OutQuart));

		var movSeq = DOTween.Sequence();
		movSeq.Append(owner.FrameTransform
			.DOLocalMove((owner.ObjectType == ObjectType.Ally ? 1f : -1f) * 3f * owner.Transform.right,
				0.15f).SetEase(Ease.InQuart));
		movSeq.Append(owner.FrameTransform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutQuart));

		movSeq.Play();
		rotSeq.Play();


		timePassed = 0f;
	}

	protected override void OnCancel()
	{
		throw new System.NotImplementedException();
	}

	public TestUnitCardAction(TestUnitCardActionSpec actionSpec)
	{
		actionDuration = actionSpec.actionDuration;
		fxPrefab = actionSpec.fxPrefab;
		//gridInfo = actionData.actionRange;
	}
}