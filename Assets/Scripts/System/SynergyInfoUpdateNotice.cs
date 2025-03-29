using MessageSystem;

public class SynergyInfoUpdateNotice : Notice
{
	public SynergyInfoUpdateNotice(Synergy targetSynergy, int count)
	{
		TargetSynergy = targetSynergy;
		Count = count;
	}

	public Synergy TargetSynergy { get; }	
	public int Count { get; }
}