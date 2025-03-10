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
		if (poolMap.TryGetValue(presetFullName, out var pool))
		{
			return pool;
		}

		var go = new GameObject(presetFullName + "Pool");
		pool = go.AddComponent<UnityObjectPool>();
		pool.prefab = Resources.Load<GameObject>(presetFullName);
		pool.disposeTime = disposeTime;
		return poolMap[presetFullName] = pool;
	}

	public PooledUnityObject Instantiate(Vector3? position = null)
	{
		PooledUnityObject ret;
		if (AvailableQueue.Count != 0)
		{
			ret = AvailableQueue.Dequeue();
			ret.transform.position = position ?? ret.transform.position;
			ret.Initialize();
			return ret;
		}

		var go = Object.Instantiate<GameObject>(prefab, transform, false);
		var pooledGo = go.AddComponent<PooledUnityObject>();
		pooledGo.DisposeTime = disposeTime;
		pooledGo.AddDisposeCallback(CollectDisposedObject);
		pooledGo.transform.position = position ?? pooledGo.transform.position;
		pooledGo.Initialize();
		return pooledGo;
	}

	private void CollectDisposedObject(PooledUnityObject disposedObject)
	{
		AvailableQueue.Enqueue(disposedObject);
	}
}