using System.Collections.Generic;
using MessageSystem;

public class BagSynergyUpdateNotice : Notice
{
	public BagSynergyUpdateNotice(Dictionary<SynergyCategory, int> synergyInfo)
	{
		SynergyInfo = synergyInfo;
	}

	public Dictionary<SynergyCategory, int> SynergyInfo { get; private set; }
}