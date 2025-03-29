using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class SynergySpec : ScriptableObject
{
	public Synergy synergy; 
	public Sprite targetSprite;
	public string synergyName;
	public string commonDesc;
	public List<int> synergyCountList;
	public List<string> desc;

	public SynergyActionSpec actionSpec;

	public ISynergyInstance GenerateSynergyInstance()
	{
		return actionSpec.Create();
	}
}