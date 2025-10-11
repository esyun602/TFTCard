using MessageSystem;

public class CurrentUsedCostChangeNotice : Notice
{
    public CurrentUsedCostChangeNotice(int currentUsedCost)
    {
        CurrentUsedCost = currentUsedCost;
    }

    public int CurrentUsedCost { get; private set; }
}