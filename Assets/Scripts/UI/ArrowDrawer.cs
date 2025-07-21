using System;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDrawer : MonoBehaviour
{
	private List<float> bezierPoints = new();
	[SerializeField] private GameObject arrowHead;
	private List<PooledUnityObject> arrowDotList = new();
	private Vector2 startPoint;

	public void Activate(Vector2 startPoint, int dotCount)
	{
		if (gameObject.activeSelf)
		{
			return;
		}

		this.startPoint = startPoint;
		bezierPoints = new();
		arrowDotList = new();
		var pool = UnityObjectPool.GetOrCreateUIPool("ArrowDot");
		pool.transform.SetParent(transform);
		var samplingRate = 1f / dotCount;
		for (var i = 0; i < dotCount; i++)
		{
			bezierPoints.Add(samplingRate * i);
			arrowDotList.Add(pool.Instantiate());
		}

		gameObject.SetActive(true);
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
		foreach (var po in arrowDotList)
		{
			po.Dispose();
		}
	}

	public void SetArrowTarget(Vector2 controlPoint, Vector2 targetPoint)
	{
		for (var i = 0; i < bezierPoints.Count; i++)
		{
			var targetPos = Utils.Bezier(bezierPoints[i], startPoint, controlPoint, targetPoint);
			arrowDotList[i].transform.position = targetPos;
		}

		var tangent = Utils.BezierTangent(1, startPoint, controlPoint, targetPoint);
		arrowHead.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg - 90f);
		arrowHead.transform.position = targetPoint;
	}
}