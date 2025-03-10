using System.Collections.Generic;
using MessageSystem;
using UnityEngine;
using UnityEngine.UI;

public enum NodeState
{
	Closed = 0,
	Opened = 1,
	Selected = 2,
	Cleared = 3,
}

public class MapNode : MonoBehaviour
{
	public StageSpec targetStageSpec;

	private List<MapNode> mapNodeList = new();

	public NodeState NodeState { get; private set; } = NodeState.Closed;
	
	public void AddChild(MapNode target)
	{
		mapNodeList.Add(target);
	}

	public void OpenNode()
	{
		//todo:fix
		NodeState = NodeState.Opened;
		GetComponentInChildren<Image>().color = Color.red;
	}

	public void SelectNode()
	{
		//todo:fix
		//todo: 아예 클릭이 안되게
		if (NodeState != NodeState.Opened)
		{
			return;
		}
		
		NodeState = NodeState.Selected;
		GetComponentInChildren<Image>().color = Color.yellow;
		
		NoticeSystem.Instance.Publish(new MapNodeSelectNotice(targetStageSpec));
	}

	public void ClearNode()
	{
		//todo:fix
		NodeState = NodeState.Cleared;
		GetComponentInChildren<Image>().color = Color.blue;
		
	}
}