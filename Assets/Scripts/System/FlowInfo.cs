using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlowInfo
{
	private HashSet<FlowNodeInfo> startNodeList = new();

	/// <summary>
	/// preorder traverse
	/// </summary>
	public IEnumerable<FlowNodeInfo> MapNodeInfos
	{
		get
		{
			var set = new HashSet<FlowNodeInfo>();
			foreach (var startNode in startNodeList)
			{
				foreach (var child in startNode.Descendants)
				{
					yield return child;
				}
			}
		}
	}

	public void AddStartNode(FlowNodeInfo nodeInfo)
	{
		startNodeList.Add(nodeInfo);
	}

	public void AddStartNodes(List<FlowNodeInfo> nodeInfos)
	{
		foreach (var node in nodeInfos)
		{
			startNodeList.Add(node);
		}
	}

	public List<FlowNodeInfo> GetHeads()
	{
		return startNodeList.ToList();
	}

	public List<FlowNodeInfo> GetTails()
	{
		var ret = new List<FlowNodeInfo>();
		foreach (var nodeInfo in MapNodeInfos)
		{
			if (nodeInfo.ChildCount == 0)
			{
				ret.Add(nodeInfo);
			}
		}

		return ret;
	}

	public List<FlowNodeInfo> GetCousins(FlowNodeInfo info)
	{
		var ret = new List<FlowNodeInfo>();
		foreach (var nodeInfo in MapNodeInfos)
		{
			if (nodeInfo.GenIdx == info.GenIdx)
			{
				ret.Add(nodeInfo);
			}
		}

		return ret;
	}
}