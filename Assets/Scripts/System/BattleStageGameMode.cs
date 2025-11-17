
using System;
using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

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
	public BattleStage BattleStage { get; }
	public BattleFxManager BattleFxManager { get; }
	public BattleGlobalModifier BattleGlobalModifier { get; }
	private SimpleStateMachine battleStageStateMachine = new();
	//todo: map gamemode 넣는게 맞나?
	public BattleStageGameMode(IStage targetStage) : base(targetStage)
	{
		BattleStage =  (BattleStage)targetStage;
		DeckSystem = new();
		TurnSystem = new();
		BattleFieldSystem = new();
		WaveSystem = new( GameDataSystem.Instance.GetGameData<WaveData>().GetMultipleWaveSpec(((BattleStageSpec)BattleStage.StageSpec).WaveGridList));
		SynergySystem = new();
		BattleFxManager = new();
		BattleGlobalModifier = new();
	}

	protected override void OnInitialize()
	{
		Game.Instance.UIManager.GenerateUI<BattleUI>(new BattleUIGenState()
		{
			InputHandler = this.DeckSystem.BlockInputHandler,
			bgSprite = ((BattleStageSpec)BattleStage.StageSpec).BattleStageBg
		});
		
		NoticeSystem.Instance.Subscribe<BattleObjectGeneratedNotice>(OnBattleObjectGenerate);
		NoticeSystem.Instance.Subscribe<BattleObjectDestroyedNotice>(OnBattleObjectDestroy);
		NoticeSystem.Instance.Subscribe<BattleObjectTypeEliminateNotice>(OnBattleObjectEliminate);
		DeckSystem.Initialize();
		TurnSystem.Initialize();
		WaveSystem.Initialize();
		BattleFieldSystem.Initialize();
		SynergySystem.Initialize();
		BattleFxManager.Initialize();
	}

	protected override void OnStageStart()
	{
		//todo: fix
		SpawnAllyUnits();
		SynergySystem.ActivateSynergies();
		WaveSystem.SpawnInitialWave(out var initialRoutine);
		battleStageStateMachine.ChangeState(new BattleStageInitState(this, initialRoutine));
	}
	private void SpawnAllyUnits()
	{
		var playInfo = Game.Instance.GetPlayer().CurrentPlayInfo;
		var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
		var deployInfos = playInfo.FieldDeployLocationInfo;
		deployInfos.Sort((x, y) => x.Col == y.Col ? x.Row.CompareTo(y.Row) : y.Col.CompareTo(x.Col));
		
		foreach (var info in deployInfos)
		{
			var card = UnitCardInField.Instantiate(info.TargetCard, map.GetTileAt(info.Row, info.Col), ObjectType.Ally);
			
			card.UpdateBlockInput(DeckSystem.BlockInputHandler.BlockInput);
		}
	}

	private void OnBattleObjectGenerate(BattleObjectGeneratedNotice m)
	{
		BattleStage.Map.SetTile(m.TargetTile, m.TargetObject);
		BattleFieldSystem.Register(m.TargetObject);
		
		if (m.TargetObject.ObjectType == ObjectType.Ally)
		{
			SynergySystem.Register(m.TargetObject);
			if (m.TargetObject is UnitCardInField unitCardInField)
			{
				DeckSystem.OnAllyAdd(unitCardInField);
			}
		}
		//todo: fix
		else if(m.TargetObject.ObjectType == ObjectType.Enemy && m.TargetObject is UnitCardInField unitCard)
		{
			DeckSystem.OnEnemyAdd(unitCard);
		}
	}
	
	private void OnBattleObjectDestroy(BattleObjectDestroyedNotice m)
	{
		BattleStage.Map.RemoveFromTile(m.Target);
		BattleFieldSystem.UnRegister(m.Target, m.Context);
		if (m.Target.ObjectType == ObjectType.Ally)
		{
			//SynergySystem.UnRegister(m.Target);
			if (m.Target is UnitCardInField bco)
			{
				DeckSystem.OnAllyRemove(bco);
			}
		}
		//todo: fix
		else if(m.Target.ObjectType == ObjectType.Enemy && m.Target is UnitCardInField unitCard)
		{
			DeckSystem.OnEnemyRemove(unitCard);
			TurnSystem.OnEnemyRemove(unitCard);
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
			if(WaveSystem.IsInLastWave)
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
		SynergySystem.Dispose();
		BattleFxManager.Dispose();
		
		Game.Instance.UIManager.HideUI<BattleUI>();
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
			owner.BattleFxManager.UpdateFrame(dt);
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
			Game.Instance.UIManager.GenerateUI<VictoryPanel>(new VictoryPanelGenState(ReturnToMapGameMode));
		}
		
		//todo: 고도화(캐시 사용)
		private void ReturnToMapGameMode()
		{
			Game.Instance.ChangeGameMode(new FlowGameMode());
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
			Game.Instance.ChangeGameMode(new FlowGameMode());
		}

		public void Exit(IState nextState)
		{
			
		}

		public void UpdateFrame(float dt)
		{
			
		}
	}
}