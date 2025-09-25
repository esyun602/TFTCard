using System;
using System.Collections.Generic;

public class BattleGlobalModifier
{
    public int RewardGoldAdd { get; set; }
    public int CardCostAdd { get; set; }
    public Queue<Action<BattleCardObjectInHand>> DropBlockAction { get; set; } = new();
}