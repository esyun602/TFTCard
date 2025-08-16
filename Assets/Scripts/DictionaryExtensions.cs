using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public static class DictionaryExtensions
{
	public static int GetInt(this Dictionary<string, object> dict, string key)
	{
		return dict.TryGetValue(key, out var value) ? (value is int intValue ? intValue : default) : default;
	}
	
	public static float GetFloat(this Dictionary<string, object> dict, string key)
	{
		return dict.TryGetValue(key, out var value) ? (value is float floatValue ? floatValue : default) : default;
	}
	public static string GetString(this Dictionary<string, object> dict, string key)
	{
		return dict.TryGetValue(key, out var value) ? (value is string strValue ? strValue : default) : default;
	}
	public static bool GetBool(this Dictionary<string, object> dict, string key)
	{
		return dict.TryGetValue(key, out var value) ? (value is bool boolValue ? boolValue : default) : default;
	}
	
	public static Dictionary<string, object> GetObject(this Dictionary<string, object> dict, string key)
	{
		return dict.TryGetValue(key, out var value) ? (value is Dictionary<string, object> obj ? obj : default) : default;
	}
	
	public static int[] GetIntArray(this Dictionary<string, object> dict, string key)
	{
		if (!dict.TryGetValue(key, out var value) || value is not List<object> list)
		{
			if (value is int intv)
			{
				return new[] { intv };
			}
			
			return default;
		}
		List<int> ret = new();
		foreach (var obj in list)
		{
			if (obj is int integer)
			{
				ret.Add(integer);
			}
			else
			{
				ret.Add(default);
			}
		}

		return ret.ToArray();
	}
	
	public static string[] GetStringArray(this Dictionary<string, object> dict, string key)
	{
		if (!dict.TryGetValue(key, out var value) || value is not List<object> list)
		{
			if (value is string strv)
			{
				return new[] { strv };
			}
			
			return default;
		}
		List<string> ret = new();
		foreach (var obj in list)
		{
			if (obj is string str)
			{
				ret.Add(str);
			}
			else
			{
				ret.Add(default);
			}
		}

		return ret.ToArray();
	}
	
	public static Dictionary<string, object>[] GetObjectArray(this Dictionary<string, object> dict, string key)
	{
		if (!dict.TryGetValue(key, out var value) || value is not List<object> list)
		{
			if (value is Dictionary<string, object> obj)
			{
				return new[] { obj };
			}
			return default;
		}
		List<Dictionary<string, object>> ret = new();
		foreach (var obj in list)
		{
			if (obj is Dictionary<string, object> jObj)
			{
				ret.Add(jObj);
			}
			else
			{
				ret.Add(default);
			}
		}

		return ret.ToArray();
	}
}