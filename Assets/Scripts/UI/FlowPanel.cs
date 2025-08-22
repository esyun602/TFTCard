using System;
using System.Collections.Generic;
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
	
	[FormerlySerializedAs("mapNodePrefab")] [SerializeField]
	private FlowNode flowNodePrefab;

	private FlowInfo flowInfo;
	private List<List<FlowNode>> nodeList = new();
	
	public override UIType UIType => UIType.SceneUI;
	protected override void Init(object param)
	{
		if (param is FlowPanelGenState state)
		{
			flowInfo = state.FlowInfo;
		}
		InstantiateNodes();
	}

	private void InstantiateNodes()
	{
		foreach (var nodeInfo in flowInfo.MapNodeInfos)
		{
			if (nodeList.Count <= nodeInfo.GenIdx)
			{
				nodeList.Add(new List<FlowNode>());
			}
			var node = Instantiate<FlowNode>(flowNodePrefab, transform);
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
	}
}