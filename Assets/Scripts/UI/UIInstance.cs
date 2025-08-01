using System;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
	testType = 1 << 2 | RenderMode.ScreenSpaceOverlay,
	DontDestroyUI = 2 << 2 | RenderMode.ScreenSpaceOverlay,
	SceneUI = 3 << 2 | RenderMode.ScreenSpaceOverlay,
	Popup = 4 << 2 | RenderMode.ScreenSpaceOverlay,
}

public abstract class UIInstance : MonoBehaviour
{
	private PooledUnityObject pooledObject;
	public PooledUnityObject PooledObject => pooledObject;
	private static int lastAllocatedId;
	private int id;
	public int Id => id;
	private Transform transformCache;
	public abstract UIType UIType { get; }

	public new Transform transform
	{
		get
		{
			if (transformCache == null && gameObject != null)
			{
				transformCache = gameObject.transform;
			}

			return transformCache;
		}
	}

	internal static T Instantiate<T>(object param, Vector3? position, Quaternion? rotation, Vector3? scale,
		Transform followTarget, System.Func<UIInstance, Transform> GetParent) where T : UIInstance
	{
		var pool = UnityObjectPool.GetOrCreateUIPool(typeof(T).Name);
		var pooledObject = pool.Instantiate(position, rotation, scale, followTarget);
		var instance = pooledObject.GetComponent<T>();
		instance.id = unchecked(++lastAllocatedId);
		instance.Init(param);
		instance.pooledObject = pooledObject;
		pool.transform.SetParent(GetParent(instance), false);
		return instance;
	}

	protected abstract void Init(object param);

	internal void Remove()
	{
		(pooledObject as IDisposable)?.Dispose();
		OnRemove();
	}

	private void OnDestroy()
	{
		OnRemove();
	}

	protected virtual void OnRemove()
	{
	}

	internal void Hide()
	{
		gameObject.SetActive(false);
		OnHide();
	}

	protected virtual void OnHide()
	{
		return;
	}

	internal void Activate()
	{
		gameObject.SetActive(true);
		OnActivated();
	}

	protected virtual void OnActivated()
	{
		return;
	}

	protected List<string> demandingUIList;
	protected List<UIInstance> childUIInstanceList;
}