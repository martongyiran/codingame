using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Save humans, destroy zombies!
 **/
class Player
{
    public static int Player_x;
    public static int Player_y;
    public static Human Target = new Human(0,0,0);

    static void Main(string[] args)
    {
        string[] inputs;
        GPS.InitGPS();
        Target.Area = "default";
        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int x = int.Parse(inputs[0]);
            Player_x = x;
            int y = int.Parse(inputs[1]);
            Player_y = y;
            int humanCount = int.Parse(Console.ReadLine());
            List<Human> humans = new List<Human>();
            List<Zombie> zombies = new List<Zombie>();

            for (int i = 0; i < humanCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int humanId = int.Parse(inputs[0]);
                int humanX = int.Parse(inputs[1]);
                int humanY = int.Parse(inputs[2]);
                humans.Add(new Human(humanId, humanX, humanY));
            }
            int zombieCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < zombieCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int zombieId = int.Parse(inputs[0]);
                int zombieX = int.Parse(inputs[1]);
                int zombieY = int.Parse(inputs[2]);
                int zombieXNext = int.Parse(inputs[3]);
                int zombieYNext = int.Parse(inputs[4]);
                zombies.Add(new Zombie(zombieId, zombieX, zombieY, zombieXNext, zombieYNext));
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");
           
            foreach (var human in humans)
            {
                human.DistanceOfClosestZombie = human.GetDistanceOfClosestZombie(zombies);
                human.Area = GPS.GetArea(human.X, human.Y);
            }

            humans = humans.OrderByDescending(i => i.DistanceOfClosestZombie).ToList();

            Human target = GetHumanInMostPopulatedArea(humans, zombies);
            if(target.Area == "none")
            {
                target = humans[0];
            }

            /*if(Target.Area == "default")
            {
                Target = target;
            }*/

            Human closest = GetClosestHumanToPlayer(humans);

            if (closest.InDanger(zombies))
            {
                target = closest;
            }


            Console.WriteLine(target.X + " " + target.Y); // Your destination coordinates
        }
    }

    static Human GetClosestHumanToPlayer(List<Human> humans)
    {
        Human closest = humans[0];

        for(int i = 0; i < humans.Count; i++)
        {
            if(humans[i].Distance(Player_x, Player_y) < closest.Distance(Player_x, Player_y))
            {
                closest = humans[i];
            }
        }

        return closest;
    }

    static Human GetHumanInMostPopulatedArea(List<Human> humans, List<Zombie> zombies)
    {
        Human result = new Human(0, 0, 0);
        result.Area = "none";

        foreach (var human in humans)
        {
            if(human.Area == GPS.GetMostPopulatedArea(zombies))
            {
                result = human;
            }
        }

        return result;
    }
}

public class Human
{
    public int Id;
    public int X;
    public int Y;
    public int DistanceOfClosestZombie;
    public string Area;

    public Human(int id, int x, int y)
    {
        Id = id;
        X = x;
        Y = y;
    }

    public int GetDistanceOfClosestZombie(List<Zombie> zombies)
    {
        List<int> distances = new List<int>();
        foreach (var item in zombies)
        {
            distances.Add(Distance(item.X, item.Y));
        }

        return distances.Min(x => x);
    }

    public int Distance(int xx, int yy)
    {
        var x = this.X - xx;
        var y = this.Y - yy;

        x *= x;
        y *= y;
        return (int)Math.Sqrt(x + y);
    }

    public bool InDanger(List<Zombie> zombies)
    {
        foreach (var zombie in zombies)
        {
            if( Distance(zombie.X, zombie.Y) < 2000 && Distance(zombie.Next_x, zombie.Next_y) > 500)
            {
                return true;
            }
        }
        return false;
    }
}

public class Zombie
{
    public int Id;
    public int X;
    public int Y;
    public int Next_x;
    public int Next_y;

    public Zombie(int id, int x, int y, int nx, int ny)
    {
        Id = id;
        X = x;
        Y = y;
        Next_x = nx;
        Next_y = ny;
    }
}

public static class GPS
{
    //In order: 00, 01, 10, 11
    public static List<Area> Areas = new List<Area>();

    public static void InitGPS()
    {
        Areas.Add(new Area(0, 0, "00"));
        Areas.Add(new Area(8000, 0, "01"));
        Areas.Add(new Area(0, 4500, "10"));
        Areas.Add(new Area(8000, 4500, "11"));
    }

    public static string GetMostPopulatedArea(List<Zombie> zombies)
    {
        CountArea(zombies);
        Areas = Areas.OrderByDescending(x => x.Population).ToList();

        return Areas[0].Id;
    }

    public static string GetArea(int x, int y)
    {
        for(int i = 0; i < 4; i++)
        {
            if (Areas[i].IsItIn(x, y))
            {
                return Areas[i].Id;
            }
        }
        return "";
    }
    
    static void CountArea(List<Zombie> zombies)
    {
        foreach (var zombie in zombies)
        {
            foreach (var area in Areas)
            {
                if(area.Id == GetArea(zombie.X, zombie.Y))
                {
                    area.Population++;
                }
            }
        }
    }

    public class Area
    {
        public int From_x;
        public int From_y;
        public string Id;
        public int Population = 0;

        public Area(int x, int y, string id)
        {
            From_x = x;
            From_y = y;
            Id = id;
        }

        public bool IsItIn(int x, int y)
        {
            if ((x > From_x && x < From_x + 8000) && (y > From_y && y < From_y + 4500))
            {
                return true;
            }
            return false;
        }
    }
}

