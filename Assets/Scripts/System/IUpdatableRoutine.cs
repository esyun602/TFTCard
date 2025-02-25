public interface IUpdatableRoutine
{
	public void Initialize();
	public void UpdateFrame(float dt, out bool routineDone);
	public void AddChain(IUpdatableRoutine routine);
	public void AddInterrupt(IUpdatableRoutine routine);
}