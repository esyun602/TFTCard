using MessageSystem;
using Unity.Mathematics;
using UnityEngine;

public class UnitCardDamagedBehaviour : IDamagedBehaviour
{
	private IBattleObject owner;
	public void AttachTo(IBattleObject obj)
	{
		owner = obj;
	}

	public void DetachFrom(IBattleObject obj)
	{
		owner = null;
	}

	public void Damage(DamageInfo info)
	{
		if (ProcessDodge(info.Sender))
		{
			return;
		}
		
		this.CalculateDamageFromStat(ref info);
		//dmg = 0 일 때 별도 연출 처리
		if (info.Dmg != 0)
		{
			ProcessShield(ref info);
		}

		if (info.Dmg != 0)
		{
			owner.UnitCardBattleStat.AddValueByValueType(UnitValueType.Hp, -info.Dmg);
			if (owner is IMessageReceiver mr)
			{
				NoticeSystem.Instance.Send(new DamageNotice(info, owner), mr);
			}
			NoticeSystem.Instance.Publish(new DamageNotice(info, owner));
			//todo: 죽음 및 데미지 처리 관련 다듬기 필요
			if (owner.IsDead())
			{
				//todo: 죽음으로 판정 필요?
				if (!Game.Instance.GetGameMode<BattleStageGameMode>().BattleFieldSystem.ReviveHandler.TryRevive(owner))
				{
					Die(info.Sender);
				}
			}
		}
	}

	public void Heal(HealInfo healInfo)
	{
		SfxManager.Instance.PlayAt("Potion and Alchemy 13", owner.Position);
		Game.Instance.GetGameMode<BattleStageGameMode>().BattleFxManager.RegisterFx(owner, UnityObjectPool.GetOrCreatePool("Fx", "HealFx", 5f));
		if (owner.UnitCardBattleStat.GetValueByValueType(UnitValueType.HealBan) == 0 && healInfo.HealAmount != 0)
		{
			owner.UnitCardBattleStat.AddValueByValueType(UnitValueType.Hp, healInfo.HealAmount);
			NoticeSystem.Instance.Publish(new HealNotice(healInfo, owner));
		}
	}
	
	public void Die(IBattleObject sender)
	{
		owner.AnimationController.RunDieAction();
		UpdatableRoutine.CurrentRoutine.AddInterrupt(() => RunDieRoutine(sender), 2f);
	}

	private void RunDieRoutine(IBattleObject sender)
	{
		owner.DestroyObject(sender);
	}

	//todo: 이후 기능이 많이 추가되면 데미지 관련 정책을 별도 interface로 개별적으로 분산시키는게 좋을듯
	private void CalculateDamageFromStat(ref DamageInfo info)
	{
		var catalyst = owner.UnitCardBattleStat.GetValueByValueType(UnitValueType.Catalyst);
		info.Dmg += catalyst;
	}

	private bool ProcessDodge(IBattleObject sender)
	{
		if (owner.UnitCardBattleStat.GetValueByValueType(UnitValueType.Dodge) > 0)
		{
			if (owner is IMessageReceiver mr)
			{
				NoticeSystem.Instance.Send(new DamageDodgeNotice(sender, owner), mr);
			}
			NoticeSystem.Instance.Publish(new DamageDodgeNotice(sender, owner));

			return true;
		}

		return false;
	}


	private void ProcessShield(ref DamageInfo info)
	{
		var shield = owner.UnitCardBattleStat.GetValueByValueType(UnitValueType.Shield);
		var damageAfter = Mathf.Max(0, info.Dmg - shield);
		var shieldDmg = info.Dmg - damageAfter;
		owner.UnitCardBattleStat.AddValueByValueType(UnitValueType.Shield, -shieldDmg);

		info.Dmg = damageAfter;
	}


}