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

	private MapInfo mapInfo;
	private List<MapNode> nodeList = new();
	
	public override UIType UIType => UIType.SceneUI;
	protected override void Init(object param)
	{
		GenerateNodeForTest();
		InstantiateNodes();
	}

#if UNITY_EDITOR
	private void GenerateNodeForTest()
	{
		if (Game.Instance.GetPlayer().CurrentPlayInfo.CurrentMapInfo != null)
		{
			mapInfo = Game.Instance.GetPlayer().CurrentPlayInfo.CurrentMapInfo;
			return;
		}
		
		Game.Instance.GetPlayer().CurrentPlayInfo.CurrentMapInfo = mapInfo = new();
		var root = new MapNodeInfo();
		root.OpenNode();
		//todo: fix..
		root.TargetStageSpec = GameDataSystem.Instance.GetGameData<StageData>().GetTestStageSpec();
		var tailNode = root;
		for (int i = 1; i <= 5; i++)
		{
			tailNode.AddChild(tailNode = new MapNodeInfo());
			//todo: fix
			if (i % 2 == 1)
			{
				tailNode.TargetStageSpec = GameDataSystem.Instance.GetGameData<StageData>().GetStageSpec("Scout");
			}
			else
			{
				tailNode.TargetStageSpec = GameDataSystem.Instance.GetGameData<StageData>().GetTestStageSpec();
			}
		}
		
		mapInfo.AddStartNode(root);
	}
#endif

	private void InstantiateNodes()
	{
		foreach (var nodeInfo in mapInfo.MapNodeInfos)
		{
			var node = Instantiate<MapNode>(mapNodePrefab, transform);
			node.targetInfo = nodeInfo;
			nodeList.Add(node);
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
			nodeList[i].GetComponent<RectTransform>().localPosition = Vector3.Lerp(TestNodeStart.localPosition,
				TestNodeEnd.localPosition, (float)i / (nodeList.Count - 1));
		}
	}
}