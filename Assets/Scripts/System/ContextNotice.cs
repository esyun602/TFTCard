using MessageSystem;
using Unity.VisualScripting;

public abstract class ContextNotice : Notice
{
	/// <summary>
	/// routine 컨텍스트에서 생성되지 않았을 경우 강제로 빈 루틴 하나를 실행
	/// todo: 검증 필요
	/// </summary>
	public IUpdatableRoutine Context { get; } = UpdatableRoutine.CurrentRoutine ?? EmptyRoutine.Register();
	
	private class EmptyRoutine : UpdatableRoutine
	{
		private static EmptyRoutine Instance { get; } = new();

		public static EmptyRoutine Register()
		{
			Instance.Initialize();
			CurrentRoutine = Instance;
			Game.Instance.GetGameMode<BattleStageGameMode>().TurnSystem.RegisterPriorityRoutine(Instance);
			return Instance;
		}

		private EmptyRoutine() : base((float _, out bool done) => { done = true; })
		{
		}
	}
}
