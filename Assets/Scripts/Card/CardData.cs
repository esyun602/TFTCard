using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CardData : GameData
{
	[SerializeField]
	private List<CardSpec> CardSpecList;
	public override void Initialize()
	{
	}

	public override void Dispose()
	{
	}

	public CardSpec GetRandomSpec()
	{
		return CardSpecList[Random.Range(0, CardSpecList.Count)];
	}
}