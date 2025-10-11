using System.Collections;
using System.Collections.Generic;
using MessageSystem;

public class EnemyCardRegisteredNotice : Notice
{
    public EnemyCardRegisteredNotice(int totalCost, List<int> cumCost, List<EnemySkillCardObject> cardList)
    {
        TotalCost = totalCost;
        CumCost = cumCost;
        CardList = cardList;
    }

    public int TotalCost { get; private set; }
    public List<int> CumCost { get; private set; }
    public List<EnemySkillCardObject> CardList { get; private set; }
}