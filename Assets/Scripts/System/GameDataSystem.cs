//나중에 게임에 붙이기?

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameDataSystem : MonoBehaviour
{
	[SerializeField]
	private List<GameData> gameDataList;
	public static GameDataSystem Instance { get; private set; }
	
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
		foreach (var data in gameDataList)
		{
			data.Initialize();
		}
	}

	public T GetGameData<T>() where T : GameData
	{
		return (T)gameDataList.Find((data) => data is T);
	}

}