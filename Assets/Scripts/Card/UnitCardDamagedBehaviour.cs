using MessageSystem;
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
			owner.UnitCardBattleStat.AddValueByValueType(BattleValueType.Hp, -info.Dmg);
			if (owner is IMessageReceiver mr)
			{
				NoticeSystem.Instance.Send(new DamageNotice(info, owner), mr);
			}
			NoticeSystem.Instance.Publish(new DamageNotice(info, owner));
			//todo: 죽음 및 데미지 처리 관련 다듬기 필요
			if (owner.IsDead())
			{
				Die(info.Sender);
			}
		}
	}

	public void Heal(HealInfo healInfo)
	{
		if (owner.UnitCardBattleStat.GetValueByValueType(BattleValueType.HealBan) != 0)
		{
			owner.UnitCardBattleStat.AddValueByValueType(BattleValueType.Hp, healInfo.HealAmount);
		}
	}

	public void Die(IBattleObject sender)
	{
		owner.DestroyObject(sender);
	}

	//todo: 이후 기능이 많이 추가되면 데미지 관련 정책을 별도 interface로 개별적으로 분산시키는게 좋을듯
	private void CalculateDamageFromStat(ref DamageInfo info)
	{
		var catalyst = owner.UnitCardBattleStat.GetValueByValueType(BattleValueType.Catalyst);
		info.Dmg += catalyst;
	}

	private bool ProcessDodge(IBattleObject sender)
	{
		if (owner.UnitCardBattleStat.GetValueByValueType(BattleValueType.Dodge) > 0)
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
		var shield = owner.UnitCardBattleStat.GetValueByValueType(BattleValueType.Shield);
		var damageAfter = Mathf.Max(0, info.Dmg - shield);
		var shieldDmg = info.Dmg - damageAfter;
		owner.UnitCardBattleStat.AddValueByValueType(BattleValueType.Shield, -shieldDmg);

		info.Dmg = damageAfter;
	}


}