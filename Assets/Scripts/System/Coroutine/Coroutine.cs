using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coroutine
{
    public class Coroutine
    {
        public GameObject parent { get; private set; }

        internal bool IsRunning { get; set; } = false;

        private List<Coroutine> subCoroutines = new List<Coroutine>();

        private IEnumerator enumerator;

        public Coroutine(GameObject parent, IEnumerator e)
        {
            enumerator = e;
            this.parent = parent;
        }
        
        public bool MoveNext()
        {
            if(parent == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Coroutine Terminated - Parent is Null : " + enumerator);
#endif
                IsRunning = false;
                return false;
            }

            if(subCoroutines.Count != 0)
            {
                for (int i = subCoroutines.Count - 1; i >= 0; i--)
                {
                    if (!subCoroutines[i].MoveNext())
                    {
                        subCoroutines.RemoveAt(i);
                    }
                }
                return true;
            }

            if (!enumerator.MoveNext())
            {
                IsRunning = false;
                return false;
            }

            if (enumerator.Current is Coroutine coroutine)
            {
                coroutine.MoveNext();
                subCoroutines.Add(coroutine);
            }
            else if(enumerator.Current is IEnumerator e)
            {
                var subRoutine = new Coroutine(parent, e);
                subRoutine.MoveNext();
                subCoroutines.Add(subRoutine);
            }

            return true;
        }
    }
}
