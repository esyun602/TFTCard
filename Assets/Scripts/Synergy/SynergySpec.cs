using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public abstract class SynergySpec : ScriptableObject
{
	public SynergyCategory synergyCategory; 
	public Sprite targetSprite;
	public string synergyName;
	public string commonDesc;
	public List<int> synergyCountList;
	public List<string> desc;

	public abstract ISynergy GenerateSynergyInstance();
}