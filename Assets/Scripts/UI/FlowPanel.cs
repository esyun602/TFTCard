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

	[SerializeField]
	private FlowEdgeDrawer edgeDrawer;
	private FlowInfo flowInfo;
	private List<List<FlowNode>> nodeList = new();
	
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
		pool.transform.SetParent(transform, false);
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
				var prog = (float) i / (Mathf.Max(nodeList.Count - 1, 1)) + flowInfo.GetProgOffset((i,j));
				rt.localPosition = new Vector3(
					Mathf.Lerp(TestNodeStart.localPosition.x,
						TestNodeEnd.localPosition.x, prog),
					flowInfo.FlowCurveList[j].Evaluate(prog) * Constant.MapEdgeCurveModifier
					,TestNodeStart.localPosition.z
					);
			}
		}

		GenerateEdges();
	}

	private void GenerateEdges()
	{
		edgeDrawer.transform.SetAsLastSibling();
		UnityObjectPool.GetOrCreateUIPool("FlowNode").transform.SetAsLastSibling();
		
		//todo: fix - 중간에서 갈라지는 경우는 없다고 우선 가정
		var count = GetChildrenOf(nodeList[0][0]).Count;
		for (var i = 0; i < count; i++)
		{
			edgeDrawer.SetPosition(nodeList[0][0].transform.position, nodeList[^1][0].transform.position, 
				flowInfo.FlowCurveList[i]);
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