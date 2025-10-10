using MessageSystem;

public class BuffStackFailNotice : Notice
{
    public BuffStackFailNotice(IBuff resultBuff, IBuff stackedBuff, IBattleObject target)
    {
        ResultBuff = resultBuff;
        StackedBuff = stackedBuff;
        Target = target;
    }

    public IBuff ResultBuff { get; }
    public IBuff StackedBuff { get; }
    public IBattleObject Target { get; }
}