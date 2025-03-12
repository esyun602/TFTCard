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
		//todo: 실제 빌드에선 이런 상황이 생기지 않도록
		var sceneName = DateTime.Now.ToString();
		if (SceneManager.GetActiveScene().name == sceneName)
		{
			sceneName += "_b";
		}
		var newScene = SceneManager.CreateScene(sceneName);
		var op = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
		SceneManager.SetActiveScene(newScene);
		
		while (op?.isDone == false)
		{
			yield return null;
		}
		
		onTransitionDone?.Invoke();
	}
}