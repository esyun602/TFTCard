using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coroutine
{
    public class CoroutineManager : MonoBehaviour
    {
        public static CoroutineManager Instance { get; private set; }

        private List<Coroutine> runningCoroutines = new List<Coroutine>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public new void StartCoroutine(IEnumerator e)
        {
            var coroutine = new Coroutine(gameObject, e);
            StartCoroutine(coroutine);
        }

        public void StartCoroutine(GameObject parent, IEnumerator e)
        {
            var coroutine = new Coroutine(parent, e);
            StartCoroutine(coroutine);
        }

        public void StartCoroutine(Coroutine coroutine)
        {
            if(coroutine.parent == null)
            {
                Debug.LogError("Cannot Start Coroutine - parent is null");
                return;
            }
            coroutine.MoveNext();

            runningCoroutines.Add(coroutine);
        }

        private void Update()
        {
            for(int i = runningCoroutines.Count-1; i >= 0; i--)
            {
                if (!runningCoroutines[i].MoveNext())
                { 
                    runningCoroutines.RemoveAt(i);
                }
            }
        }
    }
}
