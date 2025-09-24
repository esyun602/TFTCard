using System.Collections;
using System.Collections.Generic;
using MessageSystem;

public enum FlowNodeState
{
	Closed = 0,
	Opened = 1,
	Selected = 2,
	Cleared = 3,
}

public class FlowNodeInfo
{
	public FlowNodeState NodeState { get; private set; } = FlowNodeState.Closed;
	
	public StageSpec TargetStageSpec { get; set; }
	
	public int GenIdx { get; }

	public int ChildCount => childList.Count;
	
	public IEnumerable<FlowNodeInfo> Children
	{
		get
		{
			foreach (var child in childList)
			{
				yield return child;
			}
		}
	}

	/// <summary>
	/// 자기자신 포함, pre-order
	/// </summary>
	public IEnumerable<FlowNodeInfo> Descendants => GetDescendants(new HashSet<FlowNodeInfo>());
	
	private IEnumerable<FlowNodeInfo> GetDescendants(HashSet<FlowNodeInfo> visited)
	{
		if (visited.Add(this))
		{
			yield return this;
			foreach (var child in childList)
			{
				foreach (var descendant in child.GetDescendants(visited))
				{
					yield return descendant;
				}
			}
		}
	}

	private List<FlowNodeInfo> childList = new();

	public FlowNodeInfo(StageSpec targetStageSpec, int genIdx)
	{
		TargetStageSpec = targetStageSpec;
		GenIdx = genIdx;
	}

	public void AddChild(FlowNodeInfo target)
	{
		childList.Add(target);
	}

	public void OpenNode()
	{
		NodeState = FlowNodeState.Opened;
	}

	public void SelectNode()
	{
		if (NodeState != FlowNodeState.Opened)
		{
			return;
		}
		
		NodeState = FlowNodeState.Selected;
		NoticeSystem.Instance.Publish(new FlowNodeSelectNotice(this));
	}

	public void ClearNode()
	{
		NodeState = FlowNodeState.Cleared;
	}

	public void CloseNode()
	{
		NodeState = FlowNodeState.Closed;
	}
}