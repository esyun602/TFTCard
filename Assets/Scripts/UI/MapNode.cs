using System;
using System.Collections.Generic;
using MessageSystem;
using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
	//todo: fix
	public MapNodeInfo targetInfo { get; set; }

	
	public void OpenNode()
	{
		//todo:fix
		targetInfo.OpenNode();
		GetComponentInChildren<Image>().color = Color.red;
	}

	public void SelectNode()
	{
		//todo:fix
		//todo: 아예 클릭이 안되게
		targetInfo.SelectNode();
		GetComponentInChildren<Image>().color = Color.yellow;
		
	}

	public void ClearNode()
	{
		//todo:fix
		targetInfo.ClearNode();
		GetComponentInChildren<Image>().color = Color.blue;
		
	}
	
	//todo : fix
	public void UpdateNodeColor()
	{
		switch (targetInfo.NodeState)
		{
			case MapNodeState.Closed:
				GetComponentInChildren<Image>().color = Color.black;
				break;
			case MapNodeState.Opened:
				GetComponentInChildren<Image>().color = Color.red;
				break;
			case MapNodeState.Cleared:
				GetComponentInChildren<Image>().color = Color.blue;
				break;
			case MapNodeState.Selected:
				GetComponentInChildren<Image>().color = Color.yellow;
				break;
		}
	}

	private void Update()
	{
		UpdateNodeColor();
	}
}