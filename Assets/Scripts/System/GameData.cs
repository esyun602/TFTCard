
using UnityEngine;

public abstract class GameData : ScriptableObject
{
	public abstract void Initialize();
	public abstract void Dispose();
}