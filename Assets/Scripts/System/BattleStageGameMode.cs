
using System;
using System.Collections.Generic;
using MessageSystem;

public class BattleStageGameMode : StageGameMode, IUpdatable
{
	//sfx system
	//fx system
	//todo: input 관리해주는거 묶기
	//todo: 오브젝트 리스트 파편화해서 관리하는 것 하나로 통일 
	public DeckSystem DeckSystem { get; }
	public TurnSystem TurnSystem { get; }
	public WaveSystem WaveSystem { get; }
	public BattleFieldSystem BattleFieldSystem { get; }
	public SynergySystem SynergySystem { get; }

	private SimpleStateMachine battleStageStateMachine = new();

	//todo: map gamemode 넣는게 맞나?
	public BattleStageGameMode(List<WaveGrid> waveData, IStage targetStage) : base(targetStage)
	{
		DeckSystem = new();
		TurnSystem = new();
		BattleFieldSystem = new();
		WaveSystem = new(waveData);
		SynergySystem = new();
	}

	protected override void OnInitialize()
	{
		Game.Instance.UIManager.GenerateUI<BattleUI>(new BattleUIGenState()
		{
			InputHandler = this.DeckSystem.BlockInputHandler
		});
		
		NoticeSystem.Instance.Subscribe<BattleObjectGeneratedNotice>(OnBattleObjectGenerate);
		NoticeSystem.Instance.Subscribe<BattleObjectDestroyedNotice>(OnBattleObjectDestroy);
		NoticeSystem.Instance.Subscribe<BattleObjectTypeEliminateNotice>(OnBattleObjectEliminate);
		DeckSystem.Initialize();
		TurnSystem.Initialize();
		WaveSystem.Initialize();
		BattleFieldSystem.Initialize();
		SynergySystem.Initialize();
	}

	protected override void OnStageStart()
	{
		//todo: fix
		DeckSystem.SpawnAllyUnits();
		if (WaveSystem.TrySpawnNextWave(out var initialRoutine))
		{
			battleStageStateMachine.ChangeState(new BattleStageInitState(this, initialRoutine));
		}
		else
		{
			throw new ArgumentException();
		}
	}

	private void OnBattleObjectGenerate(BattleObjectGeneratedNotice m)
	{
		GetCurrentStage().Map.SetTile(m.TargetTile, m.TargetObject);
		BattleFieldSystem.Register(m.TargetObject);
		if (m.TargetObject.ObjectType == ObjectType.Ally)
		{
			SynergySystem.Register(m.TargetObject);
		}
	}
	
	private void OnBattleObjectDestroy(BattleObjectDestroyedNotice m)
	{
		GetCurrentStage().Map.RemoveFromTile(m.Target);
		BattleFieldSystem.UnRegister(m.Target, m.Context);
		if (m.Target.ObjectType == ObjectType.Ally)
		{
			SynergySystem.UnRegister(m.Target);
			if (m.Target is UnitCardInField bco)
			{
				DeckSystem.PlayerField.RemoveFromField(bco);
			}
		}
		
	}

	private void OnBattleObjectEliminate(BattleObjectTypeEliminateNotice m)
	{
		if (m.Type == ObjectType.Ally)
		{
			GameOver();
			battleStageStateMachine.ChangeState(new BattleStageGameOverState(this));
		}
		else if (m.Type == ObjectType.Enemy)
		{
			if (WaveSystem.TrySpawnNextWave(out var routine))
			{
				m.Context.AddChain(routine);
			}
			else
			{
				ClearStage();
				battleStageStateMachine.ChangeState(new BattleStageGameClearState(this));
			}
		}
	}

	protected override void OnDispose()
	{
		NoticeSystem.Instance.Unsubscribe<BattleObjectGeneratedNotice>(OnBattleObjectGenerate);
		NoticeSystem.Instance.Unsubscribe<BattleObjectDestroyedNotice>(OnBattleObjectDestroy);
		NoticeSystem.Instance.Unsubscribe<BattleObjectTypeEliminateNotice>(OnBattleObjectEliminate);
		DeckSystem.Dispose();
		TurnSystem.Dispose();
		WaveSystem.Dispose();
		BattleFieldSystem.Dispose();
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

	private class BattleStageGameClearState : IState, IUpdatable
	{
		private BattleStageGameMode owner;
		
		public BattleStageGameClearState(BattleStageGameMode owner)
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
	
	private class BattleStageGameOverState : IState, IUpdatable
	{
		private BattleStageGameMode owner;
		
		public BattleStageGameOverState(BattleStageGameMode owner)
		{
			this.owner = owner;
		}
		public void Enter(IState prevState)
		{
			Game.Instance.UIManager.GenerateUI<GameOverPanel>();
		}
		
		//todo: 고도화(캐시 사용)
		private void ReturnToTitleGameMode()
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