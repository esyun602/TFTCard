using System.Collections;
using UnityEngine;

namespace Coroutine
{
	public class WaitForSeconds : IEnumerator
	{
		private float time;
		private float timePassed;

		public WaitForSeconds(float time)
		{
			this.time = time;
		}
		public bool MoveNext()
		{
			timePassed += Time.deltaTime;
			if (timePassed > time)
			{
				return false;
			}

			return true;
		}

		public void Reset()
		{
			timePassed = 0f;
		}

		public object Current { get; } = null;
	}
}