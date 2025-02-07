
public class BattleStageGameMode : StageGameMode, IUpdatable
{
	//sfx system
	//fx system
	//turn system
	//deck system
	//battle system
	public DeckSystem DeckSystem { get; }
	public TurnSystem TurnSystem { get; }
	public WaveSystem WaveSystem { get; }

	public BattleStageGameMode(IStage targetStage) : base(targetStage)
	{
		DeckSystem = new();
		TurnSystem = new();
	}

	protected override void OnInitialize()
	{
		DeckSystem.Initialize();
		TurnSystem.Initialize();
	}

	protected override void OnDispose()
	{
		DeckSystem.Dispose();
		TurnSystem.Dispose();
	}

	public void UpdateFrame(float dt)
	{
		TurnSystem.UpdateTurn(dt);
	}
}