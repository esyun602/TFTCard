using System;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
	//subsystem?
	public static Game Instance { get; private set; }
	private IGameMode currentGameMode;
	[SerializeField]
	private TestStageSpec testStage;
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

	private void Start()
	{
		SceneHandler = new();
		UIManager = new();
		player = new();
		player.Initialize();
		ChangeGameMode(new TitleGameMode());
	}
	
	public void ChangeGameMode(IGameMode gameMode)
	{
		//todo: null check?
		//todo: transition을 넣도록 동작 수정 필요
		currentGameMode?.Dispose();
		currentGameMode = gameMode;
		currentGameMode.Initialize();
	}

	public T GetGameMode<T>() where T : class, IGameMode
	{
		return currentGameMode as T;
	}

	public Player GetPlayer()
	{
		return player;
	}

	private void Update()
	{
		(currentGameMode as IUpdatable)?.UpdateFrame(Time.deltaTime);
	}
}