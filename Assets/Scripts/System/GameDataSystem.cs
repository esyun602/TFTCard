//나중에 게임에 붙이기?

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;
//todo: dispose
public class GameDataSystem : MonoBehaviour
{
	//todo: fix
	[SerializeField] private Constant constantData;
	[SerializeField] private List<TextAsset> gameDataParamList;
	public Dictionary<string, List<Dictionary<string, object>>> GameDataParams { get; private set; }
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
		
		gameDataList = new();
		
		gameDataList.Add(constantData);
		
		//todo: 일단 constant는 안되게 했는데 수정 필요
		var types = Assembly.GetExecutingAssembly()
			.GetTypes()
			.Where(t => typeof(GameData).IsAssignableFrom(t) && !t.IsAbstract && t != typeof(Constant));
		foreach (var type in types)
		{
			gameDataList.Add((GameData)Activator.CreateInstance(type));
		}

		var settings = new JsonSerializerSettings
		{
			Converters = { new DefaultJsonConverter() }
		};

		GameDataParams = new();
		foreach (var param in gameDataParamList)
		{
			GameDataParams[param.name + "Data"] = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(param.text, settings);
		}
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