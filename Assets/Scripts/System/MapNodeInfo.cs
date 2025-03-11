using System.Collections;
using System.Collections.Generic;
using MessageSystem;

public enum MapNodeState
{
	Closed = 0,
	Opened = 1,
	Selected = 2,
	Cleared = 3,
}

public class MapNodeInfo
{
	public MapNodeState NodeState { get; private set; } = MapNodeState.Closed;
	
	public StageSpec TargetStageSpec { get; set; }

	public IEnumerable<MapNodeInfo> Children
	{
		get
		{
			foreach (var child in childList)
			{
				yield return child;
			}
		}
	}

	public IEnumerable<MapNodeInfo> GetDescendants(HashSet<MapNodeInfo> visited)
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

	private List<MapNodeInfo> childList = new();
	
	public void AddChild(MapNodeInfo target)
	{
		childList.Add(target);
	}

	public void OpenNode()
	{
		NodeState = MapNodeState.Opened;
	}

	public void SelectNode()
	{
		if (NodeState != MapNodeState.Opened)
		{
			return;
		}
		
		NodeState = MapNodeState.Selected;
		NoticeSystem.Instance.Publish(new MapNodeSelectNotice(this));
	}

	public void ClearNode()
	{
		NodeState = MapNodeState.Cleared;
	}
}