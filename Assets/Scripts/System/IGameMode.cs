using System.Collections;

public interface IGameMode
{
	public bool LoadComplete { get; }
	public void Initialize();
	public void Dispose();
}