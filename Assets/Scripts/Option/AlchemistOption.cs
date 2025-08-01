using MessageSystem;

//todo: 공격을 할 때 추가 값으로서 계산할건지 이벤트 받아서 부여할건지 결정
public class AlchemistOption : IOption
{
	private IBattleObject target;
	public int Level { get; set; }
	
	public AlchemistOption(int level)
	{
		Level = level;
	}

	public void OnAdd(IBattleObject target)
	{
		this.target = target;
		NoticeSystem.Instance.Subscribe<DamageNotice>(OnDamage);
	}

	public void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<DamageNotice>(OnDamage);
	}
	
	private void OnDamage(DamageNotice m)
	{
		if (m.DamageInfo.Sender == target && (m.DamageInfo.DamageType & DamageType.NormalAttack) != 0)
		{
			//todo: 스펙값으로
			m.Target.UnitCardBattleStat.AddBuff(new CatalystBuff(Level));
		}
	}
}