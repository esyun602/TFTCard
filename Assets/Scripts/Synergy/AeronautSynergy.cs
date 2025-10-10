using System;
using System.Collections.Generic;

public class AeronautSynergy : IGlobalSynergy
{
    private HashSet<UnitCard> memberList;
    private SynergySpec spec;
    private bool IsActivated => Level >= 1;
    //todo: const로
    private string airShipName = "Airship";
    public UnitCard AirShip { get; private set; }

    public AeronautSynergy(SynergySpec spec)
    {
        this.spec = spec;
        memberList = new();
    }

    public int Level => spec.GetGrade(memberList.Count);

    public void Initialize()
    {
        memberList = new();
        AirShip = new UnitCard(GameDataSystem.Instance.GetGameData<CardData>().GetUnitCardSpecByName(airShipName));
    }

    public void Dispose()
    {
        memberList = null;
        AirShip = null;
    }

    public void AddMember(UnitCard obj)
    {
        var prev = IsActivated;
        memberList.Add(obj);
        if (!prev && IsActivated)
        {
            Game.Instance.GetPlayer().CurrentPlayInfo.DeployToSomewhere(AirShip, true);
        }
        
    }

    public void RemoveMember(UnitCard obj)
    {
        var prev = IsActivated;
        memberList.Remove(obj);
        if (prev && !IsActivated)
        {
            Game.Instance.GetPlayer().CurrentPlayInfo.UndeployCard(AirShip, false);
        }
    }
}