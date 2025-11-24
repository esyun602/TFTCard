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
	[SerializeField] private GameObject check; 
	
	public void SelectNode()
	{
		//todo:fix
		//todo: 아예 클릭이 안되게
		TargetInfo.SelectNode();
		
	}
	
	//todo : fix
	public void UpdateNodeColor()
	{
		switch (TargetInfo.NodeState)
		{
			case FlowNodeState.Closed:
				image.color = Color.gray;
				break;
			case FlowNodeState.Opened:
				image.color = Color.white;
				break;
			case FlowNodeState.Cleared:
				image.color = Color.gray;
				check.SetActive(true);
				break;
			case FlowNodeState.Selected:
				image.color = Color.gray;
				break;
		}
	}

	private void Update()
	{
		UpdateNodeColor();
	}
}