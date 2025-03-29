using UnityEngine;

public abstract class SynergyActionSpec : ScriptableObject
{
	public abstract ISynergyInstance Create();
}