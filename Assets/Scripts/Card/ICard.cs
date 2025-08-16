public interface ICard
{
	ICardSpec CardStaticSpec { get; }
	public string Name { get; }
	public string Desc { get; }
}