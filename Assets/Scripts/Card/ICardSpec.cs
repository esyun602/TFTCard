using UnityEngine;

public enum CardType
{
	Unit = 0,
	Skill = 1,
}

public interface ICardSpec
{
	public Sprite CardResource { get; }
	public string Name { get; }
	public string Desc { get; }
	//todo: desc?
}