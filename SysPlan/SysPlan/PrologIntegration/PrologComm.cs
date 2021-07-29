using SbsSW.SwiPlCs;
using System;
using System.Linq;

namespace SysPlan
{
    public static class PrologComm
    {
        public class Coords
        {
            public int x;
            public int y;
            public Coords(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
            public override string ToString()
            {
                return "(" + x + "," + y + ")";
            }
            public bool Equals(Coords c)
            {
                return c.x == x && c.y == y;
            }
        }

        private static Coords[] StringToCoords(string s)
        {
            string newS = s.Trim('[', ']');
            string[] s2 = newS.Split(',');
            Coords[] c = new Coords[s2.Length];
            for (int i = 0; i < s2.Length; i++)
            {
                string[] coords = s2[i].Split('/');
                c[i] = new Coords(Int32.Parse(coords[0]), Int32.Parse(coords[1]));
            }
            return c;
        }

        public static void Begin()
        {
            if (!PlEngine.IsInitialized)
            {
                string[] param = { "-q" };
                PlEngine.Initialize(param);
                _ = PlQuery.PlCall("ensure_loaded(['./Prolog/Program.pl'])");
            }
        }

        public static void End()
        {
            if (PlEngine.IsInitialized)
            {
                PlEngine.PlCleanup();
            }
        }
        public static int[] GetXYmax()
        {
            if (PlEngine.IsInitialized)
            {
                int[] ret = new int[2];
                using (PlQuery q1 = new PlQuery("xMaxCoord(S)"))
                {                                       
                    PlQueryVariables v = q1.SolutionVariables.First();
                    string s = v["S"].ToString();                   
                    ret[0] = int.Parse(s);                             
                }
                using (PlQuery q2 = new PlQuery("yMaxCoord(S)"))
                {
                    PlQueryVariables v = q2.SolutionVariables.First();
                    string s = v["S"].ToString();
                    ret[1] = int.Parse(s);
                }
                return ret;
            }
            return null;
        }
        public static Coords[] GetWallInfo()
        {
            if (PlEngine.IsInitialized)
            {
                using (PlQuery q = new PlQuery("walls(S)"))
                {
                    PlQueryVariables v = q.SolutionVariables.First();
                    string s = v["S"].ToString();
                    return StringToCoords(s);
                }
            }
            return null;
        }
        public static void SetGoal(Coords goal)
        {
            if (PlEngine.IsInitialized)
            {
                _ = PlQuery.PlCall("retract(goal(_))");
                _ = PlQuery.PlCall($"asserta(goal({goal.x}/{goal.y}))");
            }
        }
        public static void AddObstacle(Coords obstacle)
        {
            if (PlEngine.IsInitialized)
            {
                _ = PlQuery.PlCall("asserta(obstacle(" + obstacle.x + "/" + obstacle.y + "))");
            }
        }
        public static void RemoveObstacle(Coords obstacle)
        {
            if (PlEngine.IsInitialized)
            {
                _ = PlQuery.PlCall("retract(obstacle(" + obstacle.x + "/" + obstacle.y + "))");
            }
        }
        public static Coords[] GetPath(Coords from)
        {
            if (PlEngine.IsInitialized)
            {
                using (PlQuery q = new PlQuery($"bestfirst({from.x}/{from.y}, S)"))
                {
                    try
                    {
                        PlQueryVariables v = q.SolutionVariables.First();
                        string s = v["S"].ToString();
                        return StringToCoords(s);
                    }
                    catch(InvalidOperationException _)
                    {
                        return null;
                    }                   
                }
            }
            return null;
        }
    }  
}
