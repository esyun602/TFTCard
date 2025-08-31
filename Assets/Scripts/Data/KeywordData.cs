using System.Collections.Generic;

public class KeywordData : GameData
{
	private Dictionary<string, KeywordInfo> keywordDataDict;

	public KeywordInfo GetKeyword(string name)
	{
		return keywordDataDict[name];
	}
	public override void Initialize()
	{
		var deserializedObject = GameDataSystem.Instance.GameDataParams["KeywordData"];
		keywordDataDict = new();
		foreach (var specJson in deserializedObject)
		{
			var info = KeywordInfo.Create(specJson);
			keywordDataDict[info.Name] = info;
		}
		
	}

	public override void Dispose()
	{
		throw new System.NotImplementedException();
	}
}