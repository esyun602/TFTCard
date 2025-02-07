
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedLayerInfo
{
	public string name;
	public List<SerializedTileInfo> tileInfos = new();

	public GameObject DeSerialize()
	{
		var ret = new GameObject();
		ret.name = name;
		foreach (var tileInfo in tileInfos)
		{
			tileInfo.DeSerialize().transform.SetParent(ret.transform);
		}

		return ret;
	}
#if UNITY_EDITOR
	public GameObject DeSerializeForMapEditor()
	{
		var ret = new GameObject();
		ret.name = name;
		foreach (var tileInfo in tileInfos)
		{
			tileInfo.DeSerializeForMapEditor().transform.SetParent(ret.transform);
		}

		return ret;
	}
#endif	
}