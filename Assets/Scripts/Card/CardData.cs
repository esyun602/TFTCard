using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CardData : GameData
{
	[SerializeField]
	private List<UnitCardSpec> CardSpecList;
	public override void Initialize()
	{
	}

	public override void Dispose()
	{
	}

	public UnitCardSpec GetRandomSpec()
	{
		return CardSpecList[Random.Range(0, CardSpecList.Count)];
	}
	
	//todo: fix?
	public UnitCardSpec GetSpecById(int id)
	{
		return CardSpecList[id];
	}
}