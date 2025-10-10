using System;
using System.Collections.Generic;
using System.Linq.Expressions;

public class AddOptionBattleSynergySpec : SynergySpec
{
    private Func<IOption> optionCtor;

    public override bool TryGenerateGlobalSynergyInstance(out IGlobalSynergy globalSynergy)
    {
        globalSynergy = null;
        return false;
    }

    public override bool TryGenerateBattleSynergyInstance(out IBattleSynergy battleSynergy)
    {
        battleSynergy = new AddOptionBattleSynergy(this);
        return true;
    }

    protected override void Initialize(Dictionary<string, object> param)
    {
        var optionName = param.GetString("OptionName");
        var type = Type.GetType(optionName);
        var ctor = type.GetConstructor(Type.EmptyTypes);
        Expression newExpr;
        if (ctor == null)
        {
            ctor = type.GetConstructor(new Type[] { typeof(int) });
            newExpr = Expression.New(ctor, Expression.Constant(0));
        }
        else
        {
            newExpr = Expression.New(ctor);
        }
        
        var lambda = Expression.Lambda<Func<IOption>>(newExpr);
        optionCtor = lambda.Compile();
    }

    public IOption CreateOption()
    {
        return optionCtor.Invoke();
    }
}