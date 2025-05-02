using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace MapRoutingProject
{
    #region Task1: Handel the Inputs and Stor them  

    public struct Intersection
    {
        //public long ID;
        public double x;
        public double y;
        //Id,length,speed
        public List<(long, double, double)> neighbor_intersections;

    }

    //public class Road
    //{
    //public long From;
    //public long To;
    // public double LengthKm;
    // public double SpeedKmph;
    //}
    public class Query
    {
        public double Source_X;
        public double Source_Y;
        public double Destination_X;
        public double Destination_Y;
        public double Available_meters;

    }

    public class Output
    {
        public List<long> Path;
        public double Shortest_time;
        public double Total_distance;
        public double Total_walking_distance;
        public double Vehicle_distance;

    }
    internal class Task1
    {


        //handel the input
        //public static Dictionary<long, Tuple<Tuple<long, long>, List<long>>> GraphMap = new Dictionary<long, Tuple<Tuple<long, long>, List<long>>>();
        public static Dictionary<long, Intersection> Intersections = new Dictionary<long, Intersection>();
        //public static Dictionary<(long, long), Road> totalRoads = new Dictionary<(long, long), Road>();
        public static List<Query> Queries = new List<Query>();

        public static void loadMap(string filePath)
        {
            Intersections.Clear();
            var lines = File.ReadAllLines(filePath);
            int index = 0;

            // read the number of intersections and conver it to int
            int n = int.Parse(lines[index++]);
            // index=1
            for (int i = 0; i < n; i++)
            {
                var parts = lines[index++].Split(' ');
                Intersection intersection = new Intersection();
                long id = long.Parse(parts[0]);
                intersection.x = double.Parse(parts[1]);
                intersection.y = double.Parse(parts[2]);
                intersection.neighbor_intersections = new List<(long, double, double)>();
                Intersections[id] = intersection;
            }
            // read the number of roads and conver it to int
            int m = int.Parse(lines[index++]);
            for (int i = 0; i < m; i++)
            {
                var parts = lines[index++].Split(' ');

                long from_id = long.Parse(parts[0]);
                long to_id = long.Parse(parts[1]);
                double LengthKm = double.Parse(parts[2]);
                double SpeedKmph = double.Parse(parts[3]);
                Intersection intersection = Intersections[from_id];
                Intersection intersection2 = Intersections[to_id];
                intersection.neighbor_intersections.Add((to_id, LengthKm, SpeedKmph));
                intersection2.neighbor_intersections.Add((from_id, LengthKm, SpeedKmph));
                //road.From = long.Parse(parts[0]);
                //road.To = long.Parse(parts[1]);

                //totalRoads[(from_id, to_id)] = road;
            }
        }

        public static void loadQuery(string filePath)
        {
            Queries.Clear();
            var lines = File.ReadAllLines(filePath);
            int index = 0;

            // read the number of Queries and conver it to int
            int n = int.Parse(lines[index++]);
            // index=1
            for (int i = 0; i < n; i++)
            {
                var parts = lines[index++].Split(' ');
                Query query = new Query();
                query.Source_X = double.Parse(parts[0]);
                query.Source_Y = double.Parse(parts[1]);
                query.Destination_X = double.Parse(parts[2]);
                query.Destination_Y = double.Parse(parts[3]);
                query.Available_meters = double.Parse(parts[4]);
                Queries.Add(query);
            }
        }

        public static List<dynamic> displayMap()
        {
            var table = new List<dynamic>();
            foreach (var item in Intersections)
            {
                long id = item.Key;
                Intersection intersection = item.Value;
                foreach (var item2 in intersection.neighbor_intersections)
                {
                    table.Add(new
                    {
                        Main_Node = id,
                        X = intersection.x,
                        Y = intersection.y,
                        Neighbor_ = item2.Item1,
                        With_Length = item2.Item2,
                        With_Speed = item2.Item3


                    });
                }


            }
            return table;

        }
        public static List<dynamic> displayQuery()
        {
            var table = new List<dynamic>();
            foreach (Query item in Queries)
            {

                table.Add(new { Source_X = item.Source_X, Source_Y = item.Source_Y, Destination_X = item.Destination_X, Destination_Y = item.Destination_Y, R = item.Available_meters });
            }
            return table;
        }

        public static Dictionary<long , Intersection> CalculateGraph (Query q ,double x ,double y ,long node ,Dictionary<long , Intersection> graph )
        {
            double x2 = x;
            double y2 = y;

            foreach ( var i in Intersections )
            {
                long intersectionId = i.Key;
                Intersection intersection = i.Value;
                double x1 = intersection.x;
                double y1 = intersection.y;
                if(x2==x1&& y2 == y1 )
                {
                    return graph;
                }
            }

            foreach ( var kvp in Intersections )
            {
                long intersectionId = kvp.Key;
                Intersection intersection = kvp.Value;
                double x1 = intersection.x;
                double y1 = intersection.y;

                double distance = Math.Sqrt (Math.Pow (x1 - x2 , 2) + Math.Pow (y1 - y2 , 2));
                double R = q.Available_meters / 1000.0;

                if ( distance <= R)
                {
                    if ( !graph.ContainsKey (node) )
                    {
                        graph [node] = new Intersection
                        {
                            x = x2 ,
                            y = y2 ,
                            neighbor_intersections = new List<(long, double, double)> ()
                        };
                    }
                    Console.WriteLine (distance);
                    graph [node].neighbor_intersections.Add ((intersectionId, distance, 5));
                }
            }

            return graph;
        }


        public static List<Output> solve ( )
        {
            List<Output> output_q = new List<Output> ();
            foreach (var q in Queries )
            {
                Dictionary<long , Intersection> graph = new Dictionary<long , Intersection> (Intersections);
                CalculateGraph (q , q.Source_X , q.Source_Y , -1 , graph);
                CalculateGraph (q , q.Destination_X , q.Destination_Y , 1000000 , graph);
                output_q.Add(dijkstra (graph,-1));
            }
            return output_q;
        }

        public static Output dijkstra(Dictionary<long , Intersection> graph,long start )
        {
            var output = new Output ();
            //write your code here

            return output;
        }


    }


    #endregion

}
