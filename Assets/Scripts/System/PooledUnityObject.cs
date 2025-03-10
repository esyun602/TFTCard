using System;
using UnityEngine;

public class PooledUnityObject : MonoBehaviour, IDisposable
{
	private Action<PooledUnityObject> disposeCallbacks;
	public float? DisposeTime { get; set; }
	private float timePassed;

	public void Initialize()
	{
		gameObject.SetActive(true);
		timePassed = 0;
	}

	public void Dispose()
	{
		if (gameObject.activeSelf == false)
		{
			Debug.LogError("Object has already been disposed: " + gameObject.name);
			return;
		}

		gameObject.SetActive(false);
		disposeCallbacks?.Invoke(this);
	}

	public void AddDisposeCallback(Action<PooledUnityObject> callback)
	{
		disposeCallbacks += callback;
	}

	private void Update()
	{
		if (gameObject.activeSelf && DisposeTime != null)
		{
			timePassed += Time.deltaTime;
			if (timePassed > DisposeTime)
			{
				Dispose();
			}
		}
	}
}