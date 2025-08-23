using System;
using UnityEngine;
using UnityEngine.UI;

//임시구현
public class FlowEdgeDrawer : MonoBehaviour
{
	private Image img;
	private void Awake()
	{
		img = GetComponent<Image>();
	}

	public void SetPosition(Vector3 start, Vector3 end)
	{
		img.rectTransform.position = (start + end) / 2;

		// 길이와 각도 조정
		float distance = Vector3.Distance(start, end);
		img.rectTransform.sizeDelta = new Vector2(distance, img.rectTransform.sizeDelta.y);

		Vector3 dir = (end - start).normalized;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		img.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
	}
}