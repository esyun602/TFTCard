using System;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
	//subsystem?
	public static Game Instance { get; private set; }
	private IGameMode currentGameMode;
	[SerializeField]
	private TestStageData testStage;
	private Player player;
	
	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		
		Instance = this;
	}

	private void Start()
	{
		player = new Player();
		var stage = testStage.InstantiateStage();
		ChangeGameMode(new BattleStageGameMode(testStage.WaveData, stage));
	}

	public void ChangeGameMode(IGameMode gameMode)
	{
		//todo: null check?
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