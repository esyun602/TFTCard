using System.Collections.Generic;
using MessageSystem;

public class StatSynergyAddNotice : Notice
{
	public StatSynergyAddNotice(List<SynergyCategory> addedSynergyList, IBattleObject target)
	{
		AddedSynergyList = addedSynergyList;
		Target = target;
	}

	public List<SynergyCategory> AddedSynergyList { get; }
	public IBattleObject Target { get; }
}