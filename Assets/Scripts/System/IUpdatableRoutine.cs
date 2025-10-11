using System;

public interface IUpdatableRoutine
{
	public void Initialize();
	public void UpdateFrame(float dt, out bool routineDone);
	public void AddChain(IUpdatableRoutine routine);
	public void AddInterrupt(IUpdatableRoutine routine);
}

public static class IUpdatableRoutineExtensions
{
	public static void AddInterruptInterval(this IUpdatableRoutine routine, float timeAfter)
	{
		routine.AddInterrupt(GenerateRunAfterTime(timeAfter, null));
	}
	
	public static void AddInterrupt(this IUpdatableRoutine routine, Action action, float timeAfter)
	{
		routine.AddInterrupt(GenerateRunAfterTime(timeAfter, action));
	}
	
	public static void AddChain(this IUpdatableRoutine routine, Action action, float timeAfter)
	{
		routine.AddChain(GenerateRunAfterTime(timeAfter, action));
	}
	
	public static UpdatableRoutine GenerateRunAfterTime(float time, Action action = null)
	{
		var timePassed = 0f;
		return new UpdatableRoutine((float dt, out bool done) =>
		{
			timePassed += dt;
			if (timePassed > time && timePassed - dt < time)
			{
				action?.Invoke();
				done = true;
				return;
			}
			

			done = false;
		});
	}
}