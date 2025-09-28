using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class UnityObjectPool : MonoBehaviour
{
	private GameObject prefab;
	private static Dictionary<string, UnityObjectPool> poolMap = new();
	private Queue<PooledUnityObject> AvailableQueue = new();
	private float? disposeTime;

	public static UnityObjectPool GetOrCreateUIPool(string assetName)
	{
		var pool = GetOrCreatePool("UI", assetName);
		if (!pool.TryGetComponent<RectTransform>(out var rect))
		{
			rect = pool.gameObject.AddComponent<RectTransform>();
			rect.offsetMin = Vector2.zero;
			rect.offsetMax = Vector2.zero;
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
		}

		return pool;
	}

	public static UnityObjectPool GetOrCreatePool(string pathName, string assetName, float? disposeTime = null)
	{
		var presetFullName = String.IsNullOrEmpty(pathName) ? assetName : pathName + "/" + assetName;
		if (String.IsNullOrEmpty(presetFullName)) return null;
		if (poolMap.TryGetValue(presetFullName, out var pool) && pool != null)
		{
			return pool;
		}

		var go = new GameObject(presetFullName + "Pool");
		pool = go.AddComponent<UnityObjectPool>();
		pool.prefab = Resources.Load<GameObject>(presetFullName);
		pool.disposeTime = disposeTime;
		return poolMap[presetFullName] = pool;
	}

	public PooledUnityObject Instantiate(Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null,
		Transform followTarget = null, Transform parent = null, bool useLocalPos = false)
	{
		PooledUnityObject ret;
		if (AvailableQueue.Count != 0)
		{
			ret = AvailableQueue.Dequeue();
		}
		else
		{
			var go = Object.Instantiate<GameObject>(prefab, transform, false);
			ret = go.AddComponent<PooledUnityObject>();
			ret.DisposeTime = disposeTime;
			ret.AddDisposeCallback(CollectDisposedObject);
		}

		if (parent != null)
		{
			ret.transform.SetParent(parent);
		}
		
		if (useLocalPos)
		{
			ret.transform.localPosition = position ?? (followTarget != null ? followTarget.position : ret.transform.position);
		}
		else
		{
			ret.transform.position = position ?? (followTarget != null ? followTarget.position : ret.transform.position);
		}
		ret.transform.rotation = rotation ?? (followTarget != null ? followTarget.rotation : ret.transform.rotation);
		ret.transform.localScale = scale ?? (followTarget != null ? followTarget.lossyScale : ret.transform.localScale);
		ret.SetFollowTarget(followTarget);
		
		ret.Initialize(transform);
		return ret;
	}

	private void CollectDisposedObject(PooledUnityObject disposedObject)
	{
		AvailableQueue.Enqueue(disposedObject);
	}
}