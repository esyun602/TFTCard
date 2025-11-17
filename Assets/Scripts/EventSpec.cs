using System;
using System.Collections.Generic;
using UnityEngine;

public class EventSpec
{
    public class ContinueEventInfo
    {
        public ContinueEventInfo(string desc, string nextEvent)
        {
            Desc = desc;
            NextEvent = nextEvent;
        }

        public string Desc { get; }
        public string NextEvent { get; }
    }

    public class EndInfo
    {
        public EndInfo(string desc)
        {
            Desc = desc;
        }

        public string Desc { get; }
    }

    public string Name { get; private set; }
    public string CategoryKey { get; private set; }
    public Sprite TargetSprite { get; private set; }
    public string Desc { get; private set; }
    public EndInfo EndInfoInstance { get; private set; }
    public List<ContinueEventInfo> ContinueEventInfos { get; private set; }

    public static EventSpec Create(Dictionary<string, object> param)
    {
        EventSpec spec = new();
        spec.Name = param.GetString(nameof(Name));
        spec.TargetSprite = Resources.Load<Sprite>("Sprites/" + param.GetString(nameof(TargetSprite)));
        spec.Desc = param.GetString(nameof(Desc));
        spec.CategoryKey = param.GetString(nameof(CategoryKey));
        var endString = param.GetString("EndDesc");
        if (!string.IsNullOrEmpty(endString))
        {
            spec.EndInfoInstance = new EndInfo(endString);
        }

        spec.ContinueEventInfos = new();
        
        var jsonList = param.GetObjectArray(nameof(ContinueEventInfo));
        foreach (var obj in jsonList)
        {
            ContinueEventInfo info = new ContinueEventInfo(obj.GetString("Desc"), obj.GetString("NextEventName"));
            spec.ContinueEventInfos.Add(info);
        }

        return spec;
    }
}