using DefaultNamespace;
using UnityEngine;

public static class VectorExtensions
{
	public static bool IsAlmostCloseTo(this Vector3 v, Vector3 target)
	{
		return (v - target).magnitude.IsAlmostZero();
	}

	public static bool IsAlmostCloseToXZ(this Vector3 v, Vector3 target)
	{
		return (GetX0z(v)).IsAlmostCloseTo(GetX0z(target));
	}

	public static Vector3 GetX0z(this Vector3 v, float y = 0)
	{
		return new Vector3(v.x, y, v.z);
	}

	public static bool IsParallelTo(this Vector3 v, Vector3 target)
	{
		return Vector3.Dot(v.normalized, target.normalized).IsAlmostCloseAbs(1);
	}

	public static bool IsCounterDirection(this Vector3 v, Vector3 target)
	{
		return Vector3.Dot(v.normalized, target.normalized) < 0 && v.IsParallelTo(target);
	}

	public static bool IsSameDirection(this Vector3 v, Vector3 target)
	{
		return Vector3.Dot(v.normalized, target.normalized) > 0 && v.IsParallelTo(target);
	}

	public static bool IsOrthogonal(this Vector3 v, Vector3 target)
	{
		return Vector3.Dot(v.normalized, target.normalized).IsAlmostZero();
	}
	
	public static Vector3 To4WayUnitVector(this Vector3 v)
	{
		var ret = new Vector3[] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };

		Vector3 retV = ret[0];
		var max = -1f;

		foreach (var target in ret)
		{
			if (Vector3.Dot(target, v) > max)
			{
				max = Vector3.Dot(target, v);
				retV = target;
			}
		}

		return retV;
	}

	public static Vector2 ToVector2XZ(this Vector3 vector)
	{
		return new Vector2(vector.x, vector.z);
	}
	
	public static Vector3 ToVector3XZ(this Vector2 vector, float y = 0f)
	{
		return new Vector3(vector.x, y, vector.y);
	}
	
	public static Vector3 ToVector3XZ(this Vector2Int vector, float y = 0f)
	{
		return ToVector3XZ((Vector2)vector, y);
	}
	
	public static Vector2Int ToRoundedVector2IntXZ(this Vector3 vector)
	{
		return Vector2Int.RoundToInt(ToVector2XZ(vector));
	}
}

public static class VectorUtil
{
	public static bool IsPassPoint(Vector3 startPos, Vector3 move, Vector3 inspectPos)
	{
		return (inspectPos - startPos).IsSameDirection(move) && (inspectPos - startPos).magnitude < move.magnitude;
	}
}