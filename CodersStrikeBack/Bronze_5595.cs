using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{

    static bool boostAvailable = true;
    static int minBoostDistance = 7000;
    static int break1 = 1200;
    static int break2 = 900;
    static int break3 = 500;

    static int getSpeed(int angle, int dist)
    {
        var output = 100;
        angle = Math.Abs(angle);

        if (angle > 90)
        {
            output = 0;
        }
        else if (angle > 25)
        {
            output = 55;
        }
        else if (dist <= break3)
        {
            output = 10;
        }
        else if (dist <= break2)
        {
            output = 35;
        }
        else if (dist <= break1)
        {
            output = 60;
        }

        return output;
    }

    static bool getBoost(int angle, int dist)
    {
        if (boostAvailable && dist > minBoostDistance && angle < 4)
        {
            boostAvailable = false;
            return true;
        }
        return false;
    }

    static void Main(string[] args)
    {
        string[] inputs;
        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int x = int.Parse(inputs[0]);
            int y = int.Parse(inputs[1]);
            int nextCheckpointX = int.Parse(inputs[2]); // x position of the next check point
            int nextCheckpointY = int.Parse(inputs[3]); // y position of the next check point
            int nextCheckpointDist = int.Parse(inputs[4]); // distance to the next checkpoint
            int nextCheckpointAngle = int.Parse(inputs[5]); // angle between your pod orientation and the direction of the next checkpoint
            inputs = Console.ReadLine().Split(' ');
            int opponentX = int.Parse(inputs[0]);
            int opponentY = int.Parse(inputs[1]);

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            // You have to output the target position
            // followed by the power (0 <= thrust <= 100)
            // i.e.: "x y thrust"

            int speed = getSpeed(nextCheckpointAngle, nextCheckpointDist);
            if (getBoost(nextCheckpointAngle, nextCheckpointDist))
            {
                Console.WriteLine(nextCheckpointX + " " + nextCheckpointY + " BOOST");
                Console.Error.WriteLine("BOOSTER USED");
            }
            else
            {
                Console.WriteLine(nextCheckpointX + " " + nextCheckpointY + " " + speed);
            }

        }

    }
}