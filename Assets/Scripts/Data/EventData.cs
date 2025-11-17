using System.Collections.Generic;

public class EventData: GameData
{
    private Dictionary<string, EventSpec> eventDataDict;

    public EventSpec GetEvent(string name)
    {
        return eventDataDict[name];
    }
    public override void Initialize()
    {
        var deserializedObject = GameDataSystem.Instance.GameDataParams["EventData"];
        eventDataDict = new();
        foreach (var specJson in deserializedObject)
        {
            var info = EventSpec.Create(specJson);
            eventDataDict[info.Name] = info;
        }
		
    }

    public override void Dispose()
    {
    }
}