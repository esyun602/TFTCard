using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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

	[SerializeField] private float defaultHeight = -300;
	[SerializeField] private Transform scrollContent;
	
	[SerializeField]
	private FlowEdgeDrawer edgeDrawer;
	private FlowInfo flowInfo;
	private List<List<FlowNode>> nodeList = new();
	private List<int> midPoints = new();
	
	private List<PooledUnityObject> nodePoList = new();
	public override UIType UIType => UIType.SceneUI;
	protected override void Init(object param)
	{
		if (param is FlowPanelGenState state)
		{
			flowInfo = state.FlowInfo;
		}
		InstantiateNodes();
	}

	protected override void OnRemove()
	{
		DisposeEdges();
		DisposeNodes();
	}

	private void InstantiateNodes()
	{
		var pool = UnityObjectPool.GetOrCreateUIPool("FlowNode");
		pool.transform.SetParent(scrollContent, false);
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
		
		for (int i = 0; i < nodeList.Count; i++)
		{
			if (nodeList[i].Count == 1)
			{
				midPoints.Add(i);
			}
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
		for (var p = 0; p < midPoints.Count - 1; p++)
		{
			var cur = midPoints[p];
			var next = midPoints[p + 1];
			
			var curProg = GetProg(cur);
			var nextProg = GetProg(next);
			
			for (int i = cur; i <= next; i++)
			{
				for (int j = 0; j < nodeList[i].Count; j++)
				{
					var targetNode = nodeList[i][j];
					var rt = targetNode.GetComponent<RectTransform>();
					var progOffset = flowInfo.GetProgOffset((i, j));
					var prog = GetProg(i) + (i == cur || i == next ? 0: progOffset);
					var localProg = (prog - curProg) / (nextProg - curProg);
					rt.position = new Vector3(
						Mathf.Lerp(TestNodeStart.position.x,
							TestNodeEnd.position.x, prog),
						defaultHeight + flowInfo.FlowCurveList[j].Evaluate(localProg) * Constant.MapEdgeCurveModifier
						,TestNodeStart.position.z
					);
				}
			}
		}

		GenerateEdges();
	}

	private float GetProg(int idx)
	{
		return (float) idx/ (Mathf.Max(nodeList.Count - 1, 1));
	}
	
	
	private void GenerateEdges()
	{
		edgeDrawer.transform.SetAsLastSibling();
		UnityObjectPool.GetOrCreateUIPool("FlowNode").transform.SetAsLastSibling();

		for (var i = 0; i < midPoints.Count-1; i++)
		{
			var cur = midPoints[i];
			var next = midPoints[i + 1];
			var count = GetChildrenOf(nodeList[cur][0]).Count;
			for (var j = 0; j < count; j++)
			{
				edgeDrawer.SetPosition(nodeList[cur][0].transform.position, nodeList[next][0].transform.position, 
					flowInfo.FlowCurveList[j], (next-cur) * 5);
			}
		}
		
	}

	private void DisposeEdges()
	{
		edgeDrawer.Dispose();
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