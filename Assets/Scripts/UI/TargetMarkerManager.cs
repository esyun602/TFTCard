using System.Collections.Generic;
using UnityEngine;

public class TargetMarkerManager : UIInstance
{
	private Dictionary<object, List<PooledUnityObject>> markerRequests;

	public override UIType UIType => UIType.SceneCameraUI;
	protected override void Init(object param)
	{
		markerRequests = new();
		UnityObjectPool.GetOrCreateUIPool("TargetMarker").transform.SetParent(transform, false);
	}
	
	public void SetTargetMarkerTo(ITile target, object requester)
	{
		//todo: fix camera
		var obj = UnityObjectPool.GetOrCreateUIPool("TargetMarker").Instantiate(target.GetPosition().GetX0z(Constant.HighlightYPos + 1));
		
		if (markerRequests.TryGetValue(requester, out var list))
		{
			list.Add(obj);
		}

		markerRequests[requester] = new() { obj };
	}

	public void SetTargetMarkerTo(IEnumerable<ITile> targetList, object requester)
	{
		if (!markerRequests.TryGetValue(requester, out var list))
		{
			list = markerRequests[requester] = new();
		}

		foreach (var tile in targetList)
		{
			if(tile == null) continue;
			var obj = UnityObjectPool.GetOrCreateUIPool("TargetMarker").Instantiate(tile.GetPosition().GetX0z(Constant.HighlightYPos + 1));
			list.Add(obj);
		}
	}

	public void RemoveTargetMarker(object requester)
	{
		if (markerRequests.TryGetValue(requester, out var list))
		{
			foreach (var po in list)
			{
				po.Dispose();
			}

			markerRequests.Remove(requester);
		}
	}
}