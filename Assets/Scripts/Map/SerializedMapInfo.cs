using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
	public (int, int) GetGridInfoOfLayer(string layerName, out HashSet<(int,int)> layerTiles)
	{
		layerTiles = new();
		var row = 0;
		var col = 0;

		var xSet = new HashSet<int>();
		var ySet = new HashSet<int>();

		var (xMin, yMin) = (int.MaxValue, int.MaxValue);
		
		layerName = !layerName.EndsWith("Layer")
			? layerName + "Layer"
			: layerName;

		foreach (var l in layer)
		{
			if (!l.name.Contains("Enemy") && !l.name.Contains("Ally"))
				continue;
			
			foreach (var tile in l.tileInfos)
			{
				var rpos = tile.position.ToRoundedVector2IntXZ();
				if (rpos.x < xMin)
				{
					xMin = rpos.x;
				}

				if (rpos.y < yMin)
				{
					yMin = rpos.y;
				}
			}
			
			foreach (var tile in l.tileInfos)
			{
				var rpos = tile.position.ToRoundedVector2IntXZ();
				if (xSet.Add(rpos.x))
				{
					col++;
				}
				
				if (ySet.Add(rpos.y))
				{
					row++;
				}

				if (l.name == layerName)
				{
					var pos = new Vector2Int((Mathf.RoundToInt(tile.position.x) - xMin)/ (int)tile.scale.x,
						(Mathf.RoundToInt(tile.position.z) - yMin)/ (int)tile.scale.z);
					layerTiles.Add((pos.y, pos.x));
				}
			}
		}

		return (row, col);
	}

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