using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedMapInfo
{
	public string name;
	public List<SerializedLayerInfo> layer = new();
	public List<SerializedTileInfo> tileInfos = new();

	public GameObject DeSerialize()
	{
		var ret = new GameObject();
		ret.name = name;
		foreach (var l in layer)
		{
			l.DeSerialize().transform.SetParent(ret.transform);
		}

		foreach (var tile in tileInfos)
		{
			tile.DeSerialize().transform.SetParent(ret.transform);
		}

		return ret;
	}

#if UNITY_EDITOR

	public GameObject DeSerializeForMapEditor()
	{
		var ret = new GameObject();
		ret.name = name;
		foreach (var l in layer)
		{
			l.DeSerializeForMapEditor().transform.SetParent(ret.transform);
		}

		foreach (var tile in tileInfos)
		{
			tile.DeSerializeForMapEditor().transform.SetParent(ret.transform);
		}

		return ret;
	}
#endif
}