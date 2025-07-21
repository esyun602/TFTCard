using UnityEngine;

public static class Utils
{
	// p0 : 시작점
	// p1 : 컨트롤 포인트 (중간에서 위로 띄워 곡률 결정)
	// p2 : 끝점
	public static Vector2 Bezier(float t, Vector2 p0, Vector2 p1, Vector2 p2)
	{
		float u = 1f - t;
		return u * u * p0 + 2f * u * t * p1 + t * t * p2;
	}

	public static Vector2 BezierTangent(float t, Vector2 p0, Vector2 p1, Vector2 p2)
	{
		// 1차 도함수: 2*(1‑t)(p1‑p0) + 2*t(p2‑p1)
		return 2f * (1f - t) * (p1 - p0) + 2f * t * (p2 - p1);
	}

}