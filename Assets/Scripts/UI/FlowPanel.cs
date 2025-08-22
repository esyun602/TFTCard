using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Serialization;

public class FlowPanelGenState
{
	public FlowInfo FlowInfo { get; set; }
}

public class FlowPanel : UIInstance
{
	//todo:fix
	public RectTransform TestNodeStart;
	public RectTransform TestNodeEnd;
	public RectTransform TestNodeTop;
	public RectTransform TestNodeBottom;
	
	private FlowInfo flowInfo;
	private List<List<FlowNode>> nodeList = new();
	
	private List<PooledUnityObject> nodePoList = new();
	private List<PooledUnityObject> edgePoList;
	
	public override UIType UIType => UIType.SceneUI;
	protected override void Init(object param)
	{
		if (param is FlowPanelGenState state)
		{
			flowInfo = state.FlowInfo;
		}
		InstantiateNodes();
		UnityObjectPool.GetOrCreateUIPool("FlowEdge").transform.SetParent(transform);
		edgePoList = new();
	}

	protected override void OnRemove()
	{
		DisposeEdges();
		DisposeNodes();
		UnityObjectPool.GetOrCreateUIPool("FlowEdge").transform.SetParent(null);
		
	}

	private void InstantiateNodes()
	{
		var pool = UnityObjectPool.GetOrCreateUIPool("FlowNode");
		pool.transform.SetParent(transform);
		foreach (var nodeInfo in flowInfo.MapNodeInfos)
		{
			if (nodeList.Count <= nodeInfo.GenIdx)
			{
				nodeList.Add(new List<FlowNode>());
			}
			var nodePo = pool.Instantiate();
			nodePoList.Add(nodePo);

			var node = nodePo.GetComponent<FlowNode>();
			node.TargetInfo = nodeInfo;
			nodeList[nodeInfo.GenIdx].Add(node);
		}
	}

	/// <summary>
	/// frame issue로 Start에서 Align
	/// </summary>
	private void Start()
	{
		AlignNodes();
	}

	private void AlignNodes()
	{
		for (int i = 0; i < nodeList.Count; i++)
		{
			for (int j = 0; j < nodeList[i].Count; j++)
			{
				var targetNode = nodeList[i][j];
				var rt = targetNode.GetComponent<RectTransform>();
				rt.localPosition = new Vector3(
					Mathf.Lerp(TestNodeStart.localPosition.x,
						TestNodeEnd.localPosition.x, (float)i / (nodeList.Count - 1)),
					Mathf.Lerp(TestNodeBottom.localPosition.y,
						TestNodeTop.localPosition.y, (float)(j+1) / (nodeList[i].Count + 1)),
				TestNodeStart.localPosition.z
					);
			}
		}

		GenerateEdges();
	}

	private void GenerateEdges()
	{
		var pool = UnityObjectPool.GetOrCreateUIPool("FlowEdge");
		pool.transform.SetAsLastSibling();
		UnityObjectPool.GetOrCreateUIPool("FlowNode").transform.SetAsLastSibling();
		
		
		for (int i = 0; i < nodeList.Count; i++)
		{
			for (int j = 0; j < nodeList[i].Count; j++)
			{
				var targetNode = nodeList[i][j];
				var children = GetChildrenOf(targetNode);
				if (children == null) return;
				
				foreach (var child in children)
				{
					var obj = pool.Instantiate();
					edgePoList.Add(obj);
					obj.GetComponent<FlowEdgeDrawer>().SetPosition(targetNode.transform.position, child.transform.position);
				}
			}
		}
	}

	private void DisposeEdges()
	{
		foreach (var edge in edgePoList)
		{
			edge.Dispose();
		}
		edgePoList.Clear();
	}

	private void DisposeNodes()
	{
		foreach (var node in nodePoList)
		{
			node.Dispose();
		}
		nodePoList.Clear();
		nodeList.Clear();
	}

	private List<FlowNode> GetChildrenOf(FlowNode node)
	{
		var targetGen = node.TargetInfo.GenIdx + 1;
		if (targetGen >= nodeList.Count) return null;
		return nodeList[targetGen].FindAll(x => node.TargetInfo.Children.Contains(x.TargetInfo));
	}
}