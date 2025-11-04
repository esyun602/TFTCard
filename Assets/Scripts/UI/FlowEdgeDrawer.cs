using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//임시구현
public class FlowEdgeDrawer : MonoBehaviour, IDisposable
{
	private List<PooledUnityObject> poolList = new();
	private Image img;
	private void Awake()
	{
		img = GetComponent<Image>();
		UnityObjectPool.GetOrCreateUIPool("FlowLine").transform.SetParent(transform);
	}

	public void SetPosition(Vector3 start, Vector3 end, AnimationCurve curve, int divCount = 15)
	{
		for (int i = 0; i < divCount; i++)
		{
			var prog = (float)i / divCount;
			var x = Mathf.Lerp(start.x, end.x, prog);
			var y = start.y + curve.Evaluate(prog) * Constant.MapEdgeCurveModifier;
			var z = transform.position.z;

			var tangent = curve.GetTangentAt(prog) * (Constant.MapEdgeCurveModifier / (end.x - start.x));
			var angle = Mathf.Atan2(tangent, 1);
			UnityObjectPool.GetOrCreateUIPool("FlowLine")
				.Instantiate(new Vector3(x, y, z), Quaternion.Euler(0, 0, angle / Mathf.PI * 180));
		}
	}

	public void Dispose()
	{
		UnityObjectPool.GetOrCreateUIPool("FlowLine").transform.SetParent(null);
		foreach (var obj in poolList)
		{
			obj.Dispose();
		}

		poolList.Clear();
	}
}