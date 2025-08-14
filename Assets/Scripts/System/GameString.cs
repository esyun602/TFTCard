using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class GameString : GameData
{
	private Dictionary<string, string> korStringKeyDict;
	public override void Initialize()
	{
		korStringKeyDict = new();
		var deserializedObject = GameDataSystem.Instance.GameDataParams["GameStringData"];
		foreach (var dict in deserializedObject)
		{
			korStringKeyDict[dict.GetString("StringKey")] = dict.GetString("KorString");
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
		var targetString = GetString(key);
		if (parameters == null)
		{
			return targetString;
		}
        
		return String.IsNullOrEmpty(targetString) ? "" : String.Format(targetString, parameters);
	}
}