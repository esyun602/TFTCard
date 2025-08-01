using System.Collections.Generic;
using MessageSystem;

public class StatSynergyRemoveNotice : Notice
{
	public StatSynergyRemoveNotice(List<SynergyCategory> removedSynergyList, IBattleObject target)
	{
		RemovedSynergyList = removedSynergyList;
		Target = target;
	}

	public List<SynergyCategory> RemovedSynergyList { get; }
	public IBattleObject Target { get; }
}