using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu]
public class GameString : GameData
{
	[SerializeField]
	private TextAsset korJson;

	private Dictionary<string, string> korStringKeyDict;
	public override void Initialize()
	{
		korStringKeyDict = new();
		var deserializedObject = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(korJson.text);
		foreach (var dict in deserializedObject)
		{
			korStringKeyDict[dict["StringKey"]] = dict["KorString"];
		}
	}

	public override void Dispose()
	{
	}

	//todo: language 대응 추가 필요
	public string GetString(string key)
	{
		return korStringKeyDict.GetValueOrDefault(key, key);
	}
	
	public string Format(string key, params object[] parameters)
	{
		return String.Format(GetString(key), parameters);
	}
}