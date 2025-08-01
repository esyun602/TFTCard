using MessageSystem;

public class SynergyInfoUpdateNotice : Notice
{
	public SynergyInfoUpdateNotice(SynergyCategory targetSynergyCategory, int count)
	{
		TargetSynergyCategory = targetSynergyCategory;
		Count = count;
	}

	public SynergyCategory TargetSynergyCategory { get; }	
	public int Count { get; }
}