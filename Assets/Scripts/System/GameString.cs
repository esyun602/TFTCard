using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

	private string GetStringImpl(string key)
	{
		return korStringKeyDict.GetValueOrDefault(key, key);
	}

	private string ProcessKeyword(string str)
	{
		return Regex.Replace(str, "%particle%", m =>
		{
			int pos = m.Index - 1;
			if (pos < 0) return "이";

			char lastChar = str[pos];

			if (lastChar >= 0xAC00 && lastChar <= 0xD7A3)
			{
				int code = lastChar - 0xAC00;
				int jong = code % 28;

				return (jong == 0) ? "가" : "이";
			}

			return "가";
		});
	}

	//todo: language 대응 추가 필요
	public string GetString(string key)
	{
		var str = GetStringImpl(key);
	
		return ProcessKeyword(str);
	}
	
	public string Format(string key, params object[] parameters)
	{
		if (parameters == null) return GetString(key);
		var targetString = GetStringImpl(key); 
		targetString = String.IsNullOrEmpty(targetString) ? "" : String.Format(targetString, parameters);
		targetString = ProcessKeyword(targetString);
		return targetString;
	}
}