using System;
using UnityEngine;

public class PooledUnityObject : MonoBehaviour, IDisposable
{
	private Action<PooledUnityObject> disposeCallbacks;
	private Action lateUpdateCallbacks;
	public float? DisposeTime { get; set; }
	private float timePassed;

	//todo: 분리?
	//todo: follow flag 추가
	private Transform poolTransform;
	private Transform followTarget;
	private Vector3 localPos;
	private Vector3 localScale;
	private Quaternion localRotation;

	public void Initialize(Transform tr)
	{
		poolTransform = tr;
		gameObject.SetActive(true);
		timePassed = 0;
	}

	public void Dispose()
	{
		if (gameObject == null)
			return;

		SetParentToPool();
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

	//todo: set execution order
	private void LateUpdate()
	{
		lateUpdateCallbacks?.Invoke();
	}

	public void SetFollowTarget(Transform followTarget)
	{
		if (followTarget != null)
		{
			this.followTarget = followTarget;       
			localPos = followTarget.InverseTransformPoint(transform.position);
			localRotation = Quaternion.Inverse(followTarget.rotation) * transform.rotation;
			Vector3 parentLossy = followTarget.lossyScale;
			Vector3 myLossy     = transform.lossyScale;
			localScale = new Vector3(
				myLossy.x / parentLossy.x,
				myLossy.y / parentLossy.y,
				myLossy.z / parentLossy.z
			);

			lateUpdateCallbacks += FollowTarget;
			AddDisposeCallback(_ => lateUpdateCallbacks -= FollowTarget);
		}
	}

	public void FollowTarget()
	{
		if (followTarget == null) return;

		transform.position = followTarget.TransformPoint(localPos);
		transform.rotation = followTarget.rotation * localRotation;

		Vector3 parentLossy = followTarget.lossyScale;
		transform.localScale = new Vector3(
			localScale.x * parentLossy.x,
			localScale.y * parentLossy.y,
			localScale.z * parentLossy.z
		);
	}

	public void SetParentToPool()
	{
		transform.SetParent(poolTransform);
	}
}