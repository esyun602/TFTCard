
using MessageSystem;

public class GuardOption  : IOption
{
    private IBattleObject target;
    public int Level { get; set; }
    private bool isAdded;
    
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
        if (m.Target != target || target.IsDead()) return;
        target.UnitCardBattleStat.AddValueByValueType(UnitValueType.Shield, 1);
    }
}