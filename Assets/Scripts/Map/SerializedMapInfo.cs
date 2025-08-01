using System;
using System.Collections.Generic;
using DefaultNamespace;
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

		var colPosList = new List<float>();
		var rowPosList = new List<float>();

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
				var pos = tile.position;

				if (colPosList.Count == 0)
				{
					colPosList.Add(pos.x);	
				}
				else
				{
					for (var i = 0; i < colPosList.Count; i++)
					{
						if (colPosList[i].IsAlmostCloseTo(pos.x))
						{
							break;
						}
						else if(colPosList[i] > pos.x)
						{
							colPosList.Insert(i, pos.x);
							break;
						}
						else if (i == colPosList.Count - 1)
						{
							colPosList.Add(pos.x);
						}
					}
				}


				if (rowPosList.Count == 0)
				{
					rowPosList.Add(pos.z);
				}
				else
				{
					for (var i = 0; i < rowPosList.Count; i++)
					{
						if (rowPosList[i].IsAlmostCloseTo(pos.z))
						{
							break;
						}
						else if (rowPosList[i] > pos.z)
						{
							rowPosList.Insert(i, pos.z);
							break;
						}
						else if (i == rowPosList.Count - 1)
						{
							rowPosList.Add(pos.z);
						}
					}
				}
			}
		}

		foreach (var l in layer)
		{
			if (l.name == layerName)
			{
				foreach (var tile in l.tileInfos)
				{
					layerTiles.Add(tile.position.ToRowCol(rowPosList, colPosList));
				}
			}
		}

		return (rowPosList.Count, colPosList.Count);
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