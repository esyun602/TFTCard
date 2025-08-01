using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public abstract class StageBase : IStage
{
	//game -> subsystem -> stagemanager 
	protected StageSpec StageSpec;
	protected MapData mapData;
	protected IMap map;
	public GameObject StageGameObject => stageGo;
	private GameObject stageGo;
	public IMap Map => map;
	protected StageCamera stageCamera;
	//라이팅 어떻게? -> 맵에 포함?

	protected StageBase(StageSpec stageSpec)
	{
		this.StageSpec = stageSpec;
		mapData = stageSpec.MapData;
	}
	
	public void Load()
	{
		stageGo = new GameObject(StageSpec.StageName);
		map = mapData.InstantiateMap();
		map.Load();
		stageCamera = SpawnStageCamera();
		//stage default UI load
		OnLoad();
	}

	public void Start()
	{
		OnStart();
	}

	protected virtual void OnStart()
	{
	}

	public void End()
	{
		OnEnd();
	}

	protected virtual void OnEnd()
	{
	}

	protected virtual StageCamera SpawnStageCamera()
	{
		var camera = new GameObject("StageCamera");
		var camComponent = camera.AddComponent<Camera>();
		camComponent.orthographic = true;
		camComponent.orthographicSize = StageSpec.CamSize;
		camera.AddComponent<AudioListener>();
		camera.AddComponent<UniversalAdditionalCameraData>();
		camera.AddComponent<PhysicsRaycaster>();
		if (EventSystem.current == null)
		{
			var es = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
			es.transform.SetParent(StageGameObject.transform);
		}
		camera.transform.SetParent(StageGameObject.transform);
		camera.tag = "MainCamera";
		camera.transform.rotation = Quaternion.Euler(90, 0, 0);
		camera.transform.position = new Vector3(9f, 100f, 5f);
		return camera.AddComponent<StageCamera>();
	}

	protected virtual void OnLoad()
	{
		return;
	}

	public void UnLoad()
	{
		GameObject.Destroy(StageGameObject);
		
		OnUnLoad();
	}
	
	protected virtual void OnUnLoad()
	{
		return;
	}
}