using System.Collections.Generic;
using System.Linq;

public class WaveData : GameData
{
	private Dictionary<string, WaveSpec> waveSpecDict;
	public override void Initialize()
	{
		waveSpecDict = new();
		var deserializedObject = GameDataSystem.Instance.GameDataParams["WaveData"];
		foreach (var specJson in deserializedObject)
		{
			var spec = WaveSpec.Create(specJson);
			waveSpecDict[spec.Name] = spec;
		}
	}

	public WaveSpec GetWaveSpec(string name)
	{
		return waveSpecDict[name];
	}

	public List<WaveSpec> GetMultipleWaveSpec(List<string> names)
	{
		return names.Select(name => waveSpecDict[name]).ToList();
	}

	public override void Dispose()
	{
	}
}