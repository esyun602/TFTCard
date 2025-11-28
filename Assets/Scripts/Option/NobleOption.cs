using MessageSystem;
using System;

public class NobleOption : IOption
{
	private IBattleObject target;
	public int Level { get; set; }
	private bool isAdded;
	private int reinforce;
	private int divider;
    
	public void OnAdd(IBattleObject target)
	{
		this.target = target;
		NoticeSystem.Instance.Subscribe<GoldUpdateNotice>(OnGoldUpdate);
		divider = Level == 3 ? 10 : 20;
		reinforce = Game.Instance.GetPlayer().CurrentPlayInfo.Gold / divider;
		if (reinforce > 0)
		{
			isAdded = true;
			AddBuff();
		}
	}

	public void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<GoldUpdateNotice>(OnGoldUpdate);
		if (isAdded)
		{
			RemoveBuff();
		}
	}

	private void OnGoldUpdate(GoldUpdateNotice m)
	{
		if (reinforce == m.Value / divider || !Game.Instance.GetGameMode<BattleStageGameMode>().IsInGame) return;
		reinforce = m.Value / divider;
		RemoveBuff();
		AddBuff();
	}

	private void AddBuff()
	{
		target.UnitCardBattleStat.AddBuff(new ValueAddAttackBuff(reinforce), this);
	}

	private void RemoveBuff()
	{
		target.UnitCardBattleStat.RemoveBuff<ValueAddAttackBuff>(this);
	}
}