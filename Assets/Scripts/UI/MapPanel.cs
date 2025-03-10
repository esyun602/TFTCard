using System;
using System.Collections.Generic;
using UnityEngine;

public class MapPanel : UIInstance
{
	//todo:fix
	public RectTransform TestNodeStart;
	public RectTransform TestNodeEnd;
	
	[SerializeField]
	private MapNode mapNodePrefab;
	private List<MapNode> nodeList;
	public override UIType UIType => UIType.SceneUI;
	protected override void Init(object param)
	{
		GenerateNodeForTest();
	}

#if UNITY_EDITOR
	private void GenerateNodeForTest()
	{
		nodeList = new();
		var node = Instantiate<MapNode>(mapNodePrefab, transform);
		node.OpenNode();
		nodeList.Add(node);

		for (int i = 1; i <= 5; i++)
		{
			node = Instantiate<MapNode>(mapNodePrefab, transform);
			nodeList.Add(node);
			nodeList[i - 1].AddChild(nodeList[i]);
		}
	}
#endif

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
			nodeList[i].GetComponent<RectTransform>().localPosition = Vector3.Lerp(TestNodeStart.localPosition,
				TestNodeEnd.localPosition, (float)i / (nodeList.Count - 1));
		}
	}
}