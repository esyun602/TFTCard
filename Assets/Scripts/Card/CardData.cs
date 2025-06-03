using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CardData : GameData
{
	[SerializeField]
	private List<UnitCardSpec> UnitCardSpecList;
	[SerializeField]
	private List<SkillCardSpec> SkillCardSpecList;
	
	public override void Initialize()
	{
	}

	public override void Dispose()
	{
	}

	public ICardSpec GetRandomUnitCardSpec()
	{
		return UnitCardSpecList[Random.Range(0, UnitCardSpecList.Count)];
	}
	
	//todo: fix?
	public ICardSpec GetSpecById(int id)
	{
		return UnitCardSpecList[id];
	}

	//todo: fix
	public UnitCardSpec GetUnitCardSpecById(int id)
	{
		return UnitCardSpecList[id];
		
	}
	
	public SkillCardSpec GetSkillCardSpecById(int id)
	{
		return SkillCardSpecList[id];
		
	}
}