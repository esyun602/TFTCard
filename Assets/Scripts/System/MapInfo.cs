using System.Collections;
using System.Collections.Generic;

public class MapInfo
{
	private HashSet<MapNodeInfo> startNodeList = new();

	public IEnumerable<MapNodeInfo> MapNodeInfos
	{
		get
		{
			var set = new HashSet<MapNodeInfo>();
			foreach (var startNode in startNodeList)
			{
				foreach (var child in startNode.GetDescendants(set))
				{
					yield return child;
				}
			}
		}
	}

	public void AddStartNode(MapNodeInfo nodeInfo)
	{
		startNodeList.Add(nodeInfo);
	}

}