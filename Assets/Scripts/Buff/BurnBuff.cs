using MessageSystem;
using UnityEngine;

public class BurnBuff : BuffBase
{
	public BurnBuff(int burnLevel)
	{
		Level = burnLevel;
	}

	public override BuffType DefaultType => BuffType.Negative | BuffType.BlockOptionAdd;
	public override UnitValueType ControlUnitValueType => UnitValueType.Burn;

	protected override void OnAdd()
	{
		NoticeSystem.Instance.Subscribe<PlayerTurnEndNotice>(OnTurnEnd);
	}

	private void OnTurnEnd(PlayerTurnEndNotice m)
	{
		target.Damage(new DamageInfo()
		{
			Dmg = Level--,
		});

		if (Level == 0)
		{
			target.UnitCardBattleStat.RemoveBuff<BurnBuff>();
		}
	}

	protected override void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<PlayerTurnEndNotice>(OnTurnEnd);
	}

	protected override bool TryStackImpl(IBuff buff)
	{
		var canStack = buff is BurnBuff;
		if (canStack)
		{
			Level += buff.Level;
		}

		return canStack;
	}

	public override string Keyword => "Burn";
}