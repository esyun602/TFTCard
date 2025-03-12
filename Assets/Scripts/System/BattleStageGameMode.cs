
using System;
using System.Collections.Generic;
using MessageSystem;

public class BattleStageGameMode : StageGameMode, IUpdatable
{
	//sfx system
	//fx system
	public DeckSystem DeckSystem { get; }
	public TurnSystem TurnSystem { get; }
	public WaveSystem WaveSystem { get; }
	public BattleSystem BattleSystem { get; }

	private SimpleStateMachine battleStageStateMachine = new();

	//todo: map gamemode 넣는게 맞나?
	public BattleStageGameMode(List<WaveGrid> waveData, IStage targetStage) : base(targetStage)
	{
		DeckSystem = new();
		TurnSystem = new();
		BattleSystem = new();
		WaveSystem = new(waveData);
	}

	protected override void OnInitialize()
	{
		Game.Instance.UIManager.GenerateUI<BattleUI>(new BattleUIGenState()
		{
			InputHandler = this.DeckSystem.BlockInputHandler
		});
		
		NoticeSystem.Instance.Subscribe<TurnObjectGeneratedNotice>(OnTurnObjectGenerate);
		NoticeSystem.Instance.Subscribe<BattleObjectGeneratedNotice>(OnBattleObjectGenerate);
		NoticeSystem.Instance.Subscribe<BattleObjectDestroyedNotice>(OnBattleObjectDestroy);
		NoticeSystem.Instance.Subscribe<TurnObjectDestroyNotice>(OnTurnObjectDestroy);
		NoticeSystem.Instance.Subscribe<BattleObjectTypeEliminateNotice>(OnBattleObjectEliminate);
		DeckSystem.Initialize();
		TurnSystem.Initialize();
		WaveSystem.Initialize();
		BattleSystem.Initialize();
	}

	protected override void OnStageStart()
	{
		//todo: fix
		if (WaveSystem.TrySpawnNextWave(out var initialRoutine))
		{
			battleStageStateMachine.ChangeState(new BattleStageInitState(this, initialRoutine));
		}
		else
		{
			throw new ArgumentException();
		}
	}

	private void OnTurnObjectGenerate(TurnObjectGeneratedNotice m)
	{
		TurnSystem.RegisterNewObject(m.Target, m.StartGauge);
	}

	private void OnBattleObjectGenerate(BattleObjectGeneratedNotice m)
	{
		GetCurrentStage().Map.SetTile(m.TargetTile, m.TargetObject);
		BattleSystem.Register(m.TargetObject);
	}
	
	private void OnTurnObjectDestroy(TurnObjectDestroyNotice m)
	{
		TurnSystem.UnregisterObject(m.Target);
	}

	private void OnBattleObjectDestroy(BattleObjectDestroyedNotice m)
	{
		GetCurrentStage().Map.RemoveFromTile(m.Target);
		BattleSystem.UnRegister(m.Target, m.Context);
	}

	private void OnBattleObjectEliminate(BattleObjectTypeEliminateNotice m)
	{
		if (m.Type != ObjectType.Enemy) return;
		if (WaveSystem.TrySpawnNextWave(out var routine))
		{
			m.Context.AddChain(routine);
		}
		else
		{
			ClearStage();
			battleStageStateMachine.ChangeState(new BattleStageGameEndState(this));
		}
	}

	protected override void OnDispose()
	{
		NoticeSystem.Instance.Unsubscribe<TurnObjectGeneratedNotice>(OnTurnObjectGenerate);
		NoticeSystem.Instance.Unsubscribe<BattleObjectGeneratedNotice>(OnBattleObjectGenerate);
		NoticeSystem.Instance.Unsubscribe<BattleObjectDestroyedNotice>(OnBattleObjectDestroy);
		NoticeSystem.Instance.Unsubscribe<TurnObjectDestroyNotice>(OnTurnObjectDestroy);
		NoticeSystem.Instance.Unsubscribe<BattleObjectTypeEliminateNotice>(OnBattleObjectEliminate);
		DeckSystem.Dispose();
		TurnSystem.Dispose();
		WaveSystem.Dispose();
		BattleSystem.Dispose();
	}

	public void UpdateFrame(float dt)
	{
		(battleStageStateMachine.CurrentState as IUpdatable)?.UpdateFrame(dt);
	}

	private class BattleStageInitState : IState, IUpdatable
	{
		private BattleStageGameMode owner;
		private IUpdatableRoutine routine;
		
		public BattleStageInitState(BattleStageGameMode owner, IUpdatableRoutine routine)
		{
			this.owner = owner;
			this.routine = routine;
		}

		public void Enter(IState prevState)
		{
			//todo: check
			routine.Initialize();
		}

		public void Exit(IState nextState)
		{
			NoticeSystem.Instance.Publish(new BattleStageInitRoutineDoneNotice());
		}

		public void UpdateFrame(float dt)
		{
			routine.UpdateFrame(dt, out var done);
			if (done)
			{
				owner.battleStageStateMachine.ChangeState(new BattleStageInGameState(owner));
			}
		}
	}
	private class BattleStageInGameState : IState, IUpdatable
	{
		private BattleStageGameMode owner;
		
		public BattleStageInGameState(BattleStageGameMode owner)
		{
			this.owner = owner;
		}
		public void Enter(IState prevState)
		{
			
		}

		public void Exit(IState nextState)
		{
		}
		
		public void UpdateFrame(float dt)
		{
			owner.TurnSystem.UpdateTurn(dt);
		}
	}

	private class BattleStageGameEndState : IState, IUpdatable
	{
		private BattleStageGameMode owner;
		
		public BattleStageGameEndState(BattleStageGameMode owner)
		{
			this.owner = owner;
		}
		public void Enter(IState prevState)
		{
			Game.Instance.UIManager.GenerateUI<ShopUIPanel>(new ShopUIPanelGenState()
			{
				rollCount = 5,
				doneAction = ReturnToMapGameMode
			});
		}
		
		//todo: 고도화(캐시 사용)
		private void ReturnToMapGameMode()
		{
			Game.Instance.ChangeGameMode(new MapGameMode());
		}

		public void Exit(IState nextState)
		{
			
		}

		public void UpdateFrame(float dt)
		{
			
		}
	}
}