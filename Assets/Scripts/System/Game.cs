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
		player = new();
		player.Initialize();
	}

	private void Start()
	{
		Initialize();
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

	public void ResetProgressInfo()
	{
		//todo: 수정 필요
		player = new();
		player.Initialize();
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

	private void Update()
	{
		(currentGameMode as IUpdatable)?.UpdateFrame(Time.deltaTime);
	}
}