using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    static void Main(string[] args)
    {
        Supervisor _supervisor = new Supervisor();

        string[] inputs;
        int numSites = int.Parse(Console.ReadLine());
        for (int i = 0; i < numSites; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int siteId = int.Parse(inputs[0]);
            int x = int.Parse(inputs[1]);
            int y = int.Parse(inputs[2]);
            int radius = int.Parse(inputs[3]);
            _supervisor.Sites.Add(new Site(siteId, x, y, radius));
        }

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int gold = int.Parse(inputs[0]);
            int touchedSite = int.Parse(inputs[1]); // -1 if none
            _supervisor.Gold = gold;
            _supervisor.TouchedSite = touchedSite;
            for (int i = 0; i < numSites; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int siteId = int.Parse(inputs[0]);
                int ignore1 = int.Parse(inputs[1]); // used in future leagues
                int ignore2 = int.Parse(inputs[2]); // used in future leagues
                int structureType = int.Parse(inputs[3]); // -1 = No structure, 2 = Barracks
                int owner = int.Parse(inputs[4]); // -1 = No structure, 0 = Friendly, 1 = Enemy
                int param1 = int.Parse(inputs[5]);
                int param2 = int.Parse(inputs[6]);
                (_supervisor.Sites.Where(x => x.Id == siteId).ToList())[0].Update(structureType, owner, param1, param2);
            }
            int numUnits = int.Parse(Console.ReadLine());
            for (int i = 0; i < numUnits; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int x = int.Parse(inputs[0]);
                int y = int.Parse(inputs[1]);
                int owner = int.Parse(inputs[2]);
                int unitType = int.Parse(inputs[3]); // -1 = QUEEN, 0 = KNIGHT, 1 = ARCHER
                int health = int.Parse(inputs[4]);
                _supervisor.Units.Add(new Unit(unitType, owner, health, x, y));
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");


            // First line: A valid queen action
            // Second line: A set of training instructions
            Console.WriteLine(_supervisor.Action());
            Console.WriteLine(_supervisor.TrainAll(UnitType.Knight));
        }
    }
}


public class Supervisor
{

    public List<Site> Sites = new List<Site>();

    public List<Unit> Units = new List<Unit>();

    public Unit Queen { get { return GetQueen(); } }

    public int TouchedSite;

    public int Gold;

    public List<Site> SitesThatCanTrain { get { return GetSitesThatCanTrain(); } }

    private int Distance(int X, int Y, int xx, int yy)
    {
        var x = X - xx;
        var y = Y - yy;

        x *= x;
        y *= y;
        return (int)Math.Sqrt(x + y);
    }

    private List<Site> GetSitesThatCanTrain()
    {
        return Sites.Where(x => x.CanTrain == true && x.Owner == SiteOwner.Friendly).ToList();
    }

    public string Action()
    {
        /*string action = "WAIT";

        if (TouchedSite != -1 && GetClosestSite()?.Type == SiteType.Barrack)
        {
            return action;
        }*/
        return Build(SiteType.Barrack, UnitType.Knight);
    }

    public string TrainAll(UnitType type)
    {
        string command = "TRAIN";

        if (SitesThatCanTrain.Count == 0)
        {
            return command;
        }

        int knightCost = 80;
        int archerCost = 100;

        var numOfTrains = Gold / knightCost;
        numOfTrains = numOfTrains > SitesThatCanTrain.Count ? SitesThatCanTrain.Count : numOfTrains;

        for( int i = 0; i <= numOfTrains-1; i++)
        {
            command += " " + SitesThatCanTrain[i].Id;
        }

        return command;
    }

    public string Build(SiteType siteType, UnitType unitType)
    {
        string command = "WAIT";
        if (GetClosestEmptySite() == null)
        {
            return command;
        }

        command = "BUILD " + GetClosestEmptySite().Id;

        switch (siteType)
        {
            case SiteType.Barrack:
                command += " BARRACKS";
                break;
             case SiteType.Tower:
                command += " TOWER";
                break;
        }

        if(siteType == SiteType.Barrack)
        {
            switch (unitType)
            {
                case UnitType.Knight:
                    command += "-KNIGHT";
                    break;
                case UnitType.Archer:
                    command += "-ARCHER";
                    break;
                default:
                    break;
            }
        }
        
        return command;
    }

    public Site GetClosestEmptySite()
    {
        List<Site> distances = new List<Site>();
        foreach (var item in Sites)
        {
            if(item.Type == SiteType.None)
            {
                item.DistanceFromQueen = Distance(Queen.X, Queen.Y, item.X, item.Y);
                distances.Add(item);
            }
        }

        return distances?.OrderBy(x => x?.DistanceFromQueen)?.FirstOrDefault();
    }

    public Site GetClosestSite()
    {
        List<Site> distances = new List<Site>();
        foreach (var item in Sites)
        {
            if (item.Owner == SiteOwner.Friendly)
            {
                item.DistanceFromQueen = Distance(Queen.X, Queen.Y, item.X, item.Y);
                distances.Add(item);
            }
        }

        return distances?.OrderBy(x => x?.DistanceFromQueen)?.FirstOrDefault();
    }

    public Unit GetQueen()
    {
        return (Units.Where(x => x.Type == UnitType.Queen && x.Owner == UnitOwner.Friendly).ToList())[0];
    }
}

public class Site
{
    public int Id;
    public int X;
    public int Y;
    public int Radius;
    public SiteType Type;
    public SiteOwner Owner;
    public bool CanTrain;
    public CreepType CreepType;

    public int DistanceFromQueen;

    public Site(int id, int x, int y, int radius)
    {
        Id = id;
        X = x;
        Y = y;
        Radius = radius;
    }

    public void Update(int type, int owner, int canTrain, int creepType)
    {
        Owner = owner == -1 ? SiteOwner.None : owner == 0 ? SiteOwner.Friendly : SiteOwner.Enemy;
        CanTrain = canTrain == 0;

        switch (type)
        {
            case -1:
                Type = SiteType.None;
                break;
            case 1:
                Type = SiteType.Tower;
                break;
            case 2:
                Type = SiteType.Barrack;
                break;
        }

        switch (creepType)
        {
            case -1:
                CreepType = CreepType.None;
                break;
            case 0:
                CreepType = CreepType.Knight;
                break;
            case 1:
                CreepType = CreepType.Archer;
                break;
            case 2:
                CreepType = CreepType.Giant;
                break;
        }
    }

    public void UpdateDistance(int distance)
    {
        DistanceFromQueen = distance;
    }

}

public class Unit
{
    public UnitType Type;
    public UnitOwner Owner;
    public int X;
    public int Y;
    public int Health;

    public Unit(int type, int owner, int health, int x, int y)
    {
        X = x;
        Y = y;
        Health = health;
        Owner = owner == 0 ? UnitOwner.Friendly : UnitOwner.Enemy;

        switch(type)
        {
            case -1:
                Type = UnitType.Queen;
            break;
            case 0:
                Type = UnitType.Knight;
            break;
            case 1:
                Type = UnitType.Archer;
            break;
            case 2:
                Type = UnitType.Giant;
            break;
        }
    }
}

public enum SiteType { None, Barrack, Tower }

public enum SiteOwner { None, Friendly, Enemy }

public enum CreepType { None, Knight, Archer, Giant }

public enum UnitType { Queen, Knight, Archer, Giant }

public enum UnitOwner { Friendly, Enemy }