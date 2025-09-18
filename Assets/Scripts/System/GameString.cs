using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

public class GameString : GameData
{
	private Dictionary<string, string> korStringKeyDict;
	public override void Initialize()
	{
		korStringKeyDict = new();
		var deserializedObject = GameDataSystem.Instance.GameDataParams["GameStringData"];
		foreach (var dict in deserializedObject)
		{
			var key = dict.GetString("StringKey");
			var value = dict.GetString("KorString");
			korStringKeyDict[key] = value;
			var matches = Regex.Matches(value, @"\{([^}]*)\}");
			if (matches.Count != 0)
			{
				AEEvaluator.RegisterExpression(dict.GetString("StringKey"), matches.Select(m => m.Groups[1].Value));
			}
		}
	}

	public override void Dispose()
	{
	}

	private string GetStringImpl(string key)
	{
		return korStringKeyDict.GetValueOrDefault(key, key);
	}

	private string ProcessKeyword(string str, IStat stat = null)
	{
		
		var targetName = "???";
		if (stat is UnitSkillCardStat unitSkillCardStat)
		{
			targetName = unitSkillCardStat.Owner.Name;
		}
		else if (stat is UnitSkillCardBattleStat unitSkillCardBattleStat)
		{
			targetName = unitSkillCardBattleStat.Owner.Name;
		}

		var nameProcessed = Regex.Replace(str, "\\$name", targetName);

		
		var particleProcessed = Regex.Replace(nameProcessed, "%particle%", m =>
		{
			int pos = m.Index - 1;
			if (pos < 0) return "이";

			char lastChar = nameProcessed[pos];

			if (lastChar >= 0xAC00 && lastChar <= 0xD7A3)
			{
				int code = lastChar - 0xAC00;
				int jong = code % 28;

				return (jong == 0) ? "가" : "이";
			}

			return "가";
		});

		return particleProcessed;
	}

	public string GetStringWithStat(string key, IStat stat)
	{
		var vals = AEEvaluator.GetExprValue(key, stat);

		if (vals == null)
		{
			var str = GetStringImpl(key);

			return ProcessKeyword(str, stat);
		}
		
		var targetString = GetStringImpl(key);
		var idx = 0;
		targetString = Regex.Replace(targetString, @"\{([^}]*)\}", _ => vals[idx++].ToString());
		
		targetString = ProcessKeyword(targetString, stat);
		return targetString;
	}

	//todo: language 대응 추가 필요
	public string GetString(string key)
	{
		var str = GetStringImpl(key);
	
		return ProcessKeyword(str);
	}
}