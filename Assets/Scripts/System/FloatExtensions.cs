using System;
using UnityEngine;

namespace DefaultNamespace
{
	public static class FloatExtensions
	{
		public static bool IsAlmostCloseTo(this float f, float target)
		{
			return (f - target).IsAlmostZero();
		}

		public static bool IsAlmostZero(this float f)
		{
			return Mathf.Abs(f) < Constant.Epsilon;
		}

		public static bool IsAlmostCloseAbs(this float f, float target)
		{
			return Mathf.Abs(f).IsAlmostCloseTo(Mathf.Abs(target));
		}
		
	}
}