using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlowInfo
{
	public FlowInfo(List<AnimationCurve> idxList)
	{
		FlowCurveList = new(idxList);
	}
	private HashSet<FlowNodeInfo> startNodeList = new();
	public List<AnimationCurve> FlowCurveList { get; }
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

	private Dictionary<(int, int), float> progOffset = new();
	public float GetProgOffset((int,int) idxPair)
	{
		if (progOffset.TryGetValue(idxPair, out var v))
		{
			return v;
		}

		return progOffset[idxPair] = (Random.value - 0.5f) * 0.03f;
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