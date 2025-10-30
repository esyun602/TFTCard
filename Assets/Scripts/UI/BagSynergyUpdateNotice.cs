using System.Collections.Generic;
using MessageSystem;

public class BagSynergyUpdateNotice : Notice
{
	public BagSynergyUpdateNotice(IEnumerable<KeyValuePair<SynergyCategory, int>> synergyInfo)
	{
		SynergyInfo = synergyInfo;
	}

	public IEnumerable<KeyValuePair<SynergyCategory, int>> SynergyInfo { get; private set; }
}