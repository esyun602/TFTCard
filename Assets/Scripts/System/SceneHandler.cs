using System;
using System.Collections;
using Coroutine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler
{
	public void SetTransitionToNewScene(Action onTransitionDone)
	{
		CoroutineManager.Instance.StartCoroutine(SetTransitionToNewSceneRoutine(onTransitionDone));
	}

	private IEnumerator SetTransitionToNewSceneRoutine(Action onTransitionDone)
	{
		var newScene = SceneManager.CreateScene(DateTime.Now.ToString());
		var op = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
		SceneManager.SetActiveScene(newScene);
		
		while (op?.isDone == false)
		{
			yield return null;
		}
		
		onTransitionDone?.Invoke();
	}
}