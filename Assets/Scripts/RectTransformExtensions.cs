using System;
using System.Collections.Generic;
using UnityEngine;

public static class RectTransformExtensions
{
	public static Vector3[] GetHorizontalDivisions(this RectTransform panel, int divisions)
	{
		if (panel == null) throw new ArgumentNullException(nameof(panel));
		if (divisions < 2) throw new ArgumentOutOfRangeException(nameof(divisions), "divisions must be ≥ 2");

		var result = new Vector3[divisions];

		float halfWidth = panel.rect.width * 0.5f;
		float step      = panel.rect.width / (divisions - 1);

		for (int i = 0; i < divisions; ++i)
		{
			Vector3 localPos = new Vector3(-halfWidth + step * i, 0f, 0f);
			result[i] = panel.TransformPoint(localPos);
		}

		return result;
	}
}