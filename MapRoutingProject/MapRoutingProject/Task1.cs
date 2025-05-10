using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
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
        public static Dictionary<long , Intersection> Intersections = new Dictionary<long , Intersection> ();
        //public static Dictionary<(long, long), Road> totalRoads = new Dictionary<(long, long), Road>();
        public static List<Query> Queries = new List<Query> ();

        public static void loadMap ( string filePath )
        {
            Intersections.Clear ();
            var lines = File.ReadAllLines (filePath);
            int index = 0;

            // read the number of intersections and conver it to int
            int n = int.Parse (lines [index++]);
            // index=1
            for ( int i = 0 ; i < n ; i++ )
            {
                var parts = lines [index++].Split (' ');
                Intersection intersection = new Intersection ();
                long id = long.Parse (parts [0]);
                intersection.x = double.Parse (parts [1]);
                intersection.y = double.Parse (parts [2]);
                intersection.neighbor_intersections = new List<(long, double, double)> ();
                Intersections [id] = intersection;
            }
            // read the number of roads and conver it to int
            int m = int.Parse (lines [index++]);
            for ( int i = 0 ; i < m ; i++ )
            {
                var parts = lines [index++].Split (' ');

                long from_id = long.Parse (parts [0]);
                long to_id = long.Parse (parts [1]);
                double LengthKm = double.Parse (parts [2]);
                double SpeedKmph = double.Parse (parts [3]);
                Intersection intersection = Intersections [from_id];
                Intersection intersection2 = Intersections [to_id];
                intersection.neighbor_intersections.Add ((to_id, LengthKm, SpeedKmph));
                intersection2.neighbor_intersections.Add ((from_id, LengthKm, SpeedKmph));
                //road.From = long.Parse(parts[0]);
                //road.To = long.Parse(parts[1]);

                //totalRoads[(from_id, to_id)] = road;
            }
        }

        public static void loadQuery ( string filePath )
        {
            Queries.Clear ();
            var lines = File.ReadAllLines (filePath);
            int index = 0;

            // read the number of Queries and conver it to int
            int n = int.Parse (lines [index++]);
            // index=1
            for ( int i = 0 ; i < n ; i++ )
            {
                var parts = lines [index++].Split (' ');
                Query query = new Query ();
                query.Source_X = double.Parse (parts [0]);
                query.Source_Y = double.Parse (parts [1]);
                query.Destination_X = double.Parse (parts [2]);
                query.Destination_Y = double.Parse (parts [3]);
                query.Available_meters = double.Parse (parts [4]);
                Queries.Add (query);
            }
        }

        public static List<dynamic> displayMap ( )
        {
            var table = new List<dynamic> ();
            foreach ( var item in Intersections )
            {
                long id = item.Key;
                Intersection intersection = item.Value;
                foreach ( var item2 in intersection.neighbor_intersections )
                {
                    table.Add (new
                    {
                        Main_Node = id ,
                        X = intersection.x ,
                        Y = intersection.y ,
                        Neighbor_ = item2.Item1 ,
                        With_Length = item2.Item2 ,
                        With_Speed = item2.Item3


                    });
                }


            }
            return table;

        }
        public static List<dynamic> displayQuery ( )
        {
            var table = new List<dynamic> ();
            foreach ( Query item in Queries )
            {

                table.Add (new { Source_X = item.Source_X , Source_Y = item.Source_Y , Destination_X = item.Destination_X , Destination_Y = item.Destination_Y , R = item.Available_meters });
            }
            return table;
        }

        public static Dictionary<long , Intersection> CalculateGraph ( Query q , Dictionary<long , Intersection> graph )
        {

            foreach ( var kvp in Intersections )
            {
                double dx = kvp.Value.x - q.Source_X;
                double dy = kvp.Value.y - q.Source_Y;
                double dx1 = kvp.Value.x - q.Destination_X;
                double dy1 = kvp.Value.y - q.Destination_Y;
                double distance = Math.Sqrt (dx * dx + dy * dy);
                double distance1 = Math.Sqrt (dx1 * dx1 + dy1 * dy1);
                double R = q.Available_meters / 1000.0;
                if ( distance <= R )
                {
                    if ( !graph.ContainsKey (-1) )
                    {
                        graph [-1] = new Intersection
                        {
                            x = q.Source_X ,
                            y = q.Source_Y ,
                            neighbor_intersections = new List<(long, double, double)> ()
                        };
                    }

                    graph [-1].neighbor_intersections.Add ((kvp.Key, distance, 5));
                    graph [kvp.Key].neighbor_intersections.Add ((-1, distance, 5));
                }
                if ( distance1 <= R )
                {

                    if ( !graph.ContainsKey (1000000) )
                    {
                        graph [1000000] = new Intersection
                        {
                            x = q.Destination_X ,
                            y = q.Destination_Y ,
                            neighbor_intersections = new List<(long, double, double)> ()
                        };
                    }
                    graph [1000000].neighbor_intersections.Add ((kvp.Key, distance1, 5));
                    graph [kvp.Key].neighbor_intersections.Add ((1000000, distance1, 5));
                }

            }
            return graph;
        }


        public static List<Output> solve ( )
        {
            // Pre-allocate results list with exact capacity
            var results = new Output [Queries.Count];
            var totalTime = 0.0;

            // Determine optimal degree of parallelism
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            Stopwatch sw = Stopwatch.StartNew();
            // Process queries in parallel
            double t = 0.0;
            Parallel.For (0 , Queries.Count , options , i =>
            {
                sw.Restart ();
                var q = Queries [i];
               

                // Create thread-local graph copy
                var tempGraph = new Dictionary<long , Intersection> (Intersections.Count);
                foreach ( var kvp in Intersections )
                {
                    tempGraph [kvp.Key] = new Intersection
                    {
                        x = kvp.Value.x ,
                        y = kvp.Value.y ,
                        neighbor_intersections = new List<(long, double, double)> (kvp.Value.neighbor_intersections)
                    };
                }

                // Calculate graph modifications
                CalculateGraph (q , tempGraph);
                // Run Dijkstra and store result
                results [i] =dijkstra (tempGraph , -1 , 1000000);
                sw.Stop ();
                t += sw.ElapsedMilliseconds;
            });
            Console.WriteLine ("timee:" + t);

            return results.ToList ();
        }

        private static Output dijkstra ( Dictionary<long , Intersection> graph , long start , long end )
        {
            var output = new Output ();
            var count = graph.Count;

            // Pre-allocate dictionaries with exact capacity
            var cost = new Dictionary<long , double> (count);
            var previous = new Dictionary<long , long?> (count);
            var visited = new HashSet<long> ();
            var pq = new PriorityQueue<long , double> (count);
            //**************************************
            // Initialize data structures
            foreach ( var node in graph.Keys )
            {
                cost [node] = double.MaxValue;
                previous [node] = null;
            }

            cost [start] = 0;
            pq.Enqueue (start , 0);

            while ( pq.Count > 0 )
            {
                var currentNode = pq.Dequeue ();

                if ( visited.Contains (currentNode) ) continue;
                if ( currentNode == end ) break;

                visited.Add (currentNode);
                var currentCost = cost [currentNode];
                var neighbors = graph [currentNode].neighbor_intersections;

                // Process neighbors with minimal allocations
                for ( int i = 0 ; i < neighbors.Count ; i++ )
                {
                    var (neighborId, distance, speed) = neighbors [i];
                    if ( visited.Contains (neighborId) ) continue;

                    var newCost = currentCost + ( distance / speed );
                    if ( newCost < cost [neighborId] )
                    {
                        cost [neighborId] = newCost;
                        previous [neighborId] = currentNode;
                        pq.Enqueue (neighborId , newCost);
                    }
                }
            }

            // Optimized path reconstruction
            var path = new List<long> ();
            for ( long? at = end ; at != null ; at = previous [at.Value] )
            {
                path.Add (at.Value);
            }
            path.Reverse ();

            // Calculate distances
            double walkingDist = 0, vehicleDist = 0;
            for ( int i = 1 ; i < path.Count ; i++ )
            {
                var from = path [i - 1];
                var to = path [i];
                var edge = graph [from].neighbor_intersections.Find (n => n.Item1 == to);

                if ( from == -1 || to == 1000000 )
                    walkingDist += edge.Item2;
                else
                    vehicleDist += edge.Item2;
            }

            // Clean path endpoints
            if ( path.Count > 0 && path [0] == -1 ) path.RemoveAt (0);
            if ( path.Count > 0 && path [^1] == 1000000 ) path.RemoveAt (path.Count - 1);

            // Set output values
            output.Path = path;
            output.Shortest_time = Math.Round (cost [end] * 60 , 2);
            output.Total_distance = Math.Round (walkingDist + vehicleDist , 2);
            output.Total_walking_distance = Math.Round (walkingDist , 2);
            output.Vehicle_distance = Math.Round (vehicleDist , 2);

            return output;
        }



        public static void WriteOutputsToFile ( string outputFilePath , List<Output> results )
        {
            using ( StreamWriter writer = new StreamWriter (outputFilePath) )
            {
                foreach ( var output in results )
                {
                    writer.WriteLine (string.Join (" " , output.Path));
                    writer.WriteLine ($"{output.Shortest_time:F2} mins");
                    writer.WriteLine ($"{output.Total_distance:F2} km");
                    writer.WriteLine ($"{output.Total_walking_distance:F2} km");
                    writer.WriteLine ($"{output.Vehicle_distance:F2} km");
                    writer.WriteLine (); // blank line between entries
                }
            }
        }

        public static bool CompareOutputFiles ( string filePath1 , string filePath2 )
        {
            var file1Lines = File.ReadAllLines (filePath1);
            var file2Lines = File.ReadAllLines (filePath2);

            /*if ( file1Lines.Length != file2Lines.Length )
            {
                Console.WriteLine ("Files have different number of lines.");
                return false;
            }*/

            for ( int i = 0 ; i < file1Lines.Length ; i++ )
            {
                if ( file1Lines [i].Trim () != file2Lines [i].Trim () )
                {
                    Console.WriteLine ($"Mismatch at line {i + 1}:");
                    Console.WriteLine ($"File1: {file1Lines [i]}");
                    Console.WriteLine ($"File2: {file2Lines [i]}");
                    return false;
                }
            }

            //Console.WriteLine ("Files match.");
            return true;
        }

    }


    #endregion

}