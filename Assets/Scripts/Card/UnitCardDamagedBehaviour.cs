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
		
		var dmg = this.CalculateDamageFromStat(info.Dmg);
		//dmg = 0 일 때 별도 연출 처리
		if (dmg != 0)
		{
			dmg = ProcessShield(dmg);
		}

		if (dmg != 0)
		{
			owner.UnitCardBattleStat.AddValueByValueType(BattleValueType.Hp, -dmg);
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
		owner.Destroy(sender);
	}

	//todo: 이후 기능이 많이 추가되면 데미지 관련 정책을 별도 interface로 개별적으로 분산시키는게 좋을듯
	private int CalculateDamageFromStat(int dmg)
	{
		var catalyst = owner.UnitCardBattleStat.GetValueByValueType(BattleValueType.Catalyst);

		return dmg + catalyst;
	}

	private bool ProcessDodge(IBattleObject sender)
	{
		if (owner.UnitCardBattleStat.GetValueByValueType(BattleValueType.Dodge) > 0)
		{
			NoticeSystem.Instance.Publish(new DamageDodgeNotice(sender, owner));

			return true;
		}

		return false;
	}


	private int ProcessShield(int dmg)
	{
		var shield = owner.UnitCardBattleStat.GetValueByValueType(BattleValueType.Shield);
		var damageAfter = Mathf.Max(0, dmg - shield);
		owner.UnitCardBattleStat.AddValueByValueType(BattleValueType.Shield, -dmg);

		return damageAfter;
	}


}