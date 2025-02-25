using MessageSystem;

public class TurnObjectGeneratedNotice : Notice
{
	public TurnObjectGeneratedNotice(ITurnObject target, float startGauge)
	{
		Target = target;
		StartGauge = startGauge;
	}

	public ITurnObject Target { get; }
	public float StartGauge { get; }
}