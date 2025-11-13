using System;
using System.Collections;
using Coroutine;
using UnityEngine;

public class Game : MonoBehaviour
{
	//subsystem?
	public static Game Instance { get; private set; }
	private IGameMode currentGameMode;
	[SerializeField]
	private BattleStageSpec battleStage;
	private Player player;
	public UIManager UIManager { get; private set; }
	public SceneHandler SceneHandler { get; private set; }
	
	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	protected void Initialize()
	{
		//todo:fix
		Screen.SetResolution(1920, 1080, true);

		SceneHandler = new();
		UIManager = new();
		RenewPlayer();
		UIManager.GenerateUI<DefaultSceneTransitionUI>();
	}

	private void Start()
	{
		Initialize();
		ChangeGameMode(new TitleGameMode());
	}
	
	public void ChangeGameMode(IGameMode gameMode)
	{
		CoroutineManager.Instance.StartCoroutine(ChangeGameModeRoutine(gameMode));
	}

	private IEnumerator ChangeGameModeRoutine(IGameMode gameMode)
	{
		DefaultSceneTransitionUI.Instance.Set();
		yield return new Coroutine.WaitForSeconds(0.5f);
		currentGameMode?.Dispose();
		currentGameMode = gameMode;
		currentGameMode.Initialize();
		while (!currentGameMode.LoadComplete) yield return null;
		yield return new Coroutine.WaitForSeconds(0.5f);
		DefaultSceneTransitionUI.Instance.Unset();
	}

	public void ResetProgressInfo()
	{
		//todo: 수정 필요
		RenewPlayer();
		ChangeGameMode(new TitleGameMode());
	}

	public T GetGameMode<T>() where T : class, IGameMode
	{
		return currentGameMode as T;
	}

	public Player GetPlayer()
	{
		return player;
	}

	private void RenewPlayer()
	{
		player?.Dispose();
		player = new();
		player.Initialize();
	}

	private void Update()
	{
		(currentGameMode as IUpdatable)?.UpdateFrame(Time.deltaTime);
	}
}