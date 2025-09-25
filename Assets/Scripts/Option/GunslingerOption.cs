using MessageSystem;

public class GunslingerOption : IOption
{
    private IBattleObject target;
    public int Level { get; set; }
    private bool isAdded;

    private bool IsMaxHp => target.UnitCardBattleStat.GetValuesByValueType(UnitValueType.Hp) ==
                            target.UnitCardBattleStat.GetValuesByValueType(UnitValueType.MaxHp);
    
    public void OnAdd(IBattleObject target)
    {
        this.target = target;
        if (IsMaxHp)
        {
            isAdded = true;
            AddBuff();
        }
        else
        {
            isAdded = false;
        }
        
        NoticeSystem.Instance.Subscribe<UnitBattleValueChangeNotice>(OnBattleValueChange);
    }

    public void OnRemove()
    {
        NoticeSystem.Instance.Unsubscribe<UnitBattleValueChangeNotice>(OnBattleValueChange);
        if (isAdded)
        {
            RemoveBuff();
        }
    }

    private void OnBattleValueChange(UnitBattleValueChangeNotice m)
    {
        if (m.Stat != target.UnitCardBattleStat || (m.Type != UnitValueType.Hp && m.Type != UnitValueType.MaxHp) ) return;
        if (isAdded && !IsMaxHp)
        {
            isAdded = false;
            RemoveBuff();
        }
        else if (!isAdded && IsMaxHp)
        {
            isAdded = true;
            AddBuff();
        }
    }


    private void AddBuff()
    {
        target.UnitCardBattleStat.AddBuff(new ValueAddAttackBuff(2), this);
    }

    private void RemoveBuff()
    {
        target.UnitCardBattleStat.RemoveBuff<ValueAddAttackBuff>(this);
    }
}