using System;
using System.Collections.Generic;
using MessageSystem;
using UnityEngine;
using UnityEngine.UI;

public class FlowNode : MonoBehaviour
{
	//todo: fix
	private FlowNodeInfo targetInfo;

	public FlowNodeInfo TargetInfo
	{
		get => targetInfo;
		set
		{
			targetInfo = value;
			image.sprite = targetInfo.TargetStageSpec.StageIcon;
		}
	}
	[SerializeField] private Image image;
	
	public void OpenNode()
	{
		//todo:fix
		TargetInfo.OpenNode();
		GetComponentInChildren<Image>().color = Color.red;
	}

	public void SelectNode()
	{
		//todo:fix
		//todo: 아예 클릭이 안되게
		TargetInfo.SelectNode();
		GetComponentInChildren<Image>().color = Color.yellow;
		
	}

	public void ClearNode()
	{
		//todo:fix
		TargetInfo.ClearNode();
		GetComponentInChildren<Image>().color = Color.blue;
		
	}
	
	//todo : fix
	public void UpdateNodeColor()
	{
		switch (TargetInfo.NodeState)
		{
			case FlowNodeState.Closed:
				GetComponentInChildren<Image>().color = Color.gray;
				break;
			case FlowNodeState.Opened:
				GetComponentInChildren<Image>().color = Color.white;
				break;
			case FlowNodeState.Cleared:
				GetComponentInChildren<Image>().color = Color.black;
				break;
			case FlowNodeState.Selected:
				GetComponentInChildren<Image>().color = Color.white;
				break;
		}
	}

	private void Update()
	{
		UpdateNodeColor();
	}
}