using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

//todo: 배틀 관련된 것 제거
public abstract class StageBase : IStage
{
	//game -> subsystem -> stagemanager 
	public StageSpec StageSpec { get; protected set; }
	public StageType StageType => StageSpec.StageType;
	public GameObject StageGameObject => stageGo;
	private GameObject stageGo;
	protected StageCamera stageCamera;
	//라이팅 어떻게? -> 맵에 포함?

	protected StageBase(StageSpec stageSpec)
	{
		this.StageSpec = stageSpec;
	}
	
	public void Load()
	{
		stageGo = new GameObject(StageSpec.StageName);
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
		if (StageType == StageType.BattleStage || StageType == StageType.BossStage)
		{
			camera.transform.rotation = Quaternion.Euler(90, 0, 0);
			camera.transform.position = new Vector3(9f, Constant.StageCameraHeight, 5f);
		}

		Game.Instance.UIManager.CurrentUICamera = camComponent;
		
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