using System;
using System.Collections.Generic;
using MessageSystem;
using TMPro;
using UnityEngine;

namespace UI
{
	public class TestTurnGaugeUI : MonoBehaviour
	{
		public GameObject testPrefab;
		private Dictionary<ITurnObject, GameObject> dict = new();
		private void Start()
		{
			NoticeSystem.Instance.Subscribe<TurnGaugeUpdateNotice>(OnGaugeUpdate);
			NoticeSystem.Instance.Subscribe<TurnObjectRegisterNotice>(OnRegister);
			NoticeSystem.Instance.Subscribe<TurnObjectUnregisterNotice>(OnUnregister);
		}

		private Vector3 GetPosWithGauge(float gauge, float maxGauge)
		{
			var rect = GetComponent<RectTransform>();
			float clamped = Mathf.Clamp(gauge, 0, maxGauge);
			float normalized = clamped / maxGauge;
        
			float leftEdge = rect.rect.xMin;
			float rightEdge = rect.rect.xMax;
        
			float mappedX = Mathf.Lerp(rightEdge, leftEdge, normalized);
        
			return new Vector3(mappedX, 0, 0f);
		}
		
		private void OnRegister(TurnObjectRegisterNotice m)
		{
			var rect = GetComponent<RectTransform>();
			dict[m.TurnObject] = Instantiate(testPrefab, transform);
			dict[m.TurnObject].GetComponent<RectTransform>().localPosition = new Vector3(rect.rect.xMax, 0, 0f);
		}

		private void OnUnregister(TurnObjectUnregisterNotice m)
		{
			if (dict.TryGetValue(m.TurnObject, out var obj))
			{
				Destroy(obj);
				dict.Remove(m.TurnObject);
			}
		}

		private void OnGaugeUpdate(TurnGaugeUpdateNotice m)
		{
			foreach (var each in m.GaugeInfoDictionary)
			{
				dict[each.Key].GetComponent<RectTransform>().localPosition = GetPosWithGauge(each.Value, m.MaxGauge);
			}
		}

		private void OnDestroy()
		{
			NoticeSystem.Instance.Unsubscribe<TurnGaugeUpdateNotice>(OnGaugeUpdate);
			NoticeSystem.Instance.Subscribe<TurnObjectRegisterNotice>(OnRegister);
			NoticeSystem.Instance.Subscribe<TurnObjectUnregisterNotice>(OnUnregister);
		}
	}
}