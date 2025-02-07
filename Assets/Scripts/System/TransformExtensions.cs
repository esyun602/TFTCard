using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
	public static List<GameObject> GetAllChildrenWithTag(this Transform parent, string tag)
	{
		List<GameObject> result = new List<GameObject>();

		foreach (Transform child in parent)
		{
			if (child.CompareTag(tag))
			{
				result.Add(child.gameObject);
			}
			result.AddRange(child.GetAllChildrenWithTag(tag));
		}

		return result;
	}
}