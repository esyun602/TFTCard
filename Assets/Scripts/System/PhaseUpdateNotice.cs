using MessageSystem;

public class PhaseUpdateNotice : Notice
{
	public PhaseUpdateNotice(int prevPhase, int currentPhase)
	{
		PrevPhase = prevPhase;
		CurrentPhase = currentPhase;
	}

	public int PrevPhase { get; }
	public int CurrentPhase { get; }
	
	
}