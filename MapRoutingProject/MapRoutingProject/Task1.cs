using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
public class FibonacciHeapNode<T>
{
    public T Data;
    public double Key;
    public FibonacciHeapNode<T> Parent;
    public FibonacciHeapNode<T> Child;
    public FibonacciHeapNode<T> Left;
    public FibonacciHeapNode<T> Right;
    public int Degree;
    public bool Mark;

    public FibonacciHeapNode ( T data , double key )
    {
        Data = data;
        Key = key;
        Left = Right = this;
    }
}

public class FibonacciHeap<T>
{
    private FibonacciHeapNode<T> minNode;
    private int nodeCount;
    private Dictionary<T , FibonacciHeapNode<T>> nodeLookup = new ();

    public bool IsEmpty => minNode == null;

    public void Insert ( T data , double key )
    {
        var node = new FibonacciHeapNode<T> (data , key);
        nodeLookup [data] = node;
        minNode = MergeLists (minNode , node);
        nodeCount++;
    }

    public void DecreaseKey ( T data , double newKey )
    {
        if ( !nodeLookup.TryGetValue (data , out var node) ) return;
        if ( newKey > node.Key ) throw new InvalidOperationException ("New key is greater");

        node.Key = newKey;
        var parent = node.Parent;

        if ( parent != null && node.Key < parent.Key )
        {
            Cut (node , parent);
            CascadingCut (parent);
        }

        if ( node.Key < minNode.Key )
            minNode = node;
    }

    public T ExtractMin ( )
    {
        var z = minNode;
        if ( z != null )
        {
            if ( z.Child != null )
            {
                var children = new List<FibonacciHeapNode<T>> ();
                var x = z.Child;
                do
                {
                    children.Add (x);
                    x = x.Right;
                } while ( x != z.Child );

                foreach ( var child in children )
                {
                    child.Parent = null;
                    minNode = MergeLists (minNode , child);
                }
            }

            z.Left.Right = z.Right;
            z.Right.Left = z.Left;

            if ( z == z.Right )
                minNode = null;
            else
            {
                minNode = z.Right;
                Consolidate ();
            }

            nodeCount--;
            nodeLookup.Remove (z.Data);
        }

        return z?.Data;
    }

    public bool Contains ( T data ) => nodeLookup.ContainsKey (data);
    public double GetKey ( T data ) => nodeLookup [data].Key;

    private void Cut ( FibonacciHeapNode<T> x , FibonacciHeapNode<T> y )
    {
        if ( x.Right == x )
            y.Child = null;
        else
        {
            x.Right.Left = x.Left;
            x.Left.Right = x.Right;
            if ( y.Child == x )
                y.Child = x.Right;
        }

        y.Degree--;
        minNode = MergeLists (minNode , x);
        x.Parent = null;
        x.Mark = false;
    }

    private void CascadingCut ( FibonacciHeapNode<T> y )
    {
        var z = y.Parent;
        if ( z != null )
        {
            if ( !y.Mark )
                y.Mark = true;
            else
            {
                Cut (y , z);
                CascadingCut (z);
            }
        }
    }

    private FibonacciHeapNode<T> MergeLists ( FibonacciHeapNode<T> a , FibonacciHeapNode<T> b )
    {
        if ( a == null ) return b;
        if ( b == null ) return a;

        var temp = a.Right;
        a.Right = b.Right;
        a.Right.Left = a;
        b.Right = temp;
        b.Right.Left = b;

        return a.Key < b.Key ? a : b;
    }

    private void Consolidate ( )
    {
        int maxDegree = (int) Math.Log (nodeCount) + 1;
        var A = new FibonacciHeapNode<T> [maxDegree];

        var nodes = new List<FibonacciHeapNode<T>> ();
        var x = minNode;
        if ( x != null )
        {
            do
            {
                nodes.Add (x);
                x = x.Right;
            } while ( x != minNode );
        }

        foreach ( var w in nodes )
        {
            var xNode = w;
            int d = xNode.Degree;
            while ( A [d] != null )
            {
                var y = A [d];
                if ( xNode.Key > y.Key )
                    (xNode, y) = (y, xNode);

                Link (y , xNode);
                A [d] = null;
                d++;
            }
            A [d] = xNode;
        }

        minNode = null;
        foreach ( var node in A )
            if ( node != null )
                minNode = MergeLists (minNode , node);
    }

    private void Link ( FibonacciHeapNode<T> y , FibonacciHeapNode<T> x )
    {
        y.Left.Right = y.Right;
        y.Right.Left = y.Left;

        y.Parent = x;
        if ( x.Child == null )
        {
            x.Child = y;
            y.Left = y.Right = y;
        }
        else
        {
            y.Left = x.Child;
            y.Right = x.Child.Right;
            x.Child.Right.Left = y;
            x.Child.Right = y;
        }

        x.Degree++;
        y.Mark = false;
    }
}


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
            List<Output> results = new List<Output> ();
            //Stopwatch sw = Stopwatch.StartNew ();
            foreach ( var q in Queries )
            {
                var temp_graph = DeepCopyIntersections (Intersections);
                CalculateGraph (q , temp_graph);
                results.Add (dijkstra (temp_graph , -1 , 1000000));
            }
            //sw.Stop ();
            return results;
        }


        private static Dictionary<long , Intersection> DeepCopyIntersections ( Dictionary<long , Intersection> original )
        {
            var copy = new Dictionary<long , Intersection> ();
            foreach ( var kvp in original )
            {
                copy [kvp.Key] = new Intersection
                {
                    x = kvp.Value.x ,
                    y = kvp.Value.y ,
                    neighbor_intersections = new List<(long, double, double)> (kvp.Value.neighbor_intersections)
                };
            }
            return copy;
        }

        public static Output dijkstra ( Dictionary<long , Intersection> graph , long start , long end )
        {
            var output = new Output ();
            var cost = new Dictionary<long , double> ();
            var previous = new Dictionary<long , long?> ();
            var distanceTo = new Dictionary<long , double> ();

            foreach ( var node in graph.Keys )
            {
                cost [node] = double.MaxValue;
                previous [node] = null;
                distanceTo [node] = 0.0;
            }

            cost [start] = 0.0;
            distanceTo [start] = 0.0;

            var pq = new PriorityQueue<long , double> ();
            pq.Enqueue (start , 0.0);

            while ( pq.Count > 0 )
            {
                pq.TryDequeue (out long currentNode , out double currentTime);

                if ( currentTime > cost [currentNode] ) continue;

                foreach ( var (neighborId, distance, speed) in graph [currentNode].neighbor_intersections )
                {
                    double time = distance / speed;
                    double newTime = cost [currentNode] + time;

                    if ( newTime < cost [neighborId] )
                    {
                        cost [neighborId] = newTime;
                        distanceTo [neighborId] = distanceTo [currentNode] + distance;
                        previous [neighborId] = currentNode;
                        pq.Enqueue (neighborId , newTime);
                    }
                }
            }

            var path = new List<long> ();
            for ( long? at = end ; at != null ; at = previous [at.Value] )
                path.Add (at.Value);
            path.Reverse ();

            output.Path = path;
            output.Total_walking_distance = 0.0;
            output.Vehicle_distance = 0.0;

            // Iterative calculation of distances
            for ( int i = 1 ; i < path.Count ; i++ )
            {
                long from = path [i - 1];
                long to = path [i];

                // Get distance between two connected nodes
                var edge = graph [from].neighbor_intersections
                    .FirstOrDefault (n => n.Item1 == to);

                if ( from == -1 || to == 1000000 )
                {
                    output.Total_walking_distance += edge.Item2;
                }
                else
                {
                    output.Vehicle_distance += edge.Item2;
                }
            }
            if ( path [0]==-1 )
            {
                path.RemoveAt (0);
            }
            if ( path [path.Count-1] == 1000000 )
            {
                path.RemoveAt (path.Count - 1);
            }

            output.Total_distance = Math.Round (output.Total_walking_distance + output.Vehicle_distance , 2);
            output.Total_walking_distance = Math.Round (output.Total_walking_distance , 2);
            output.Vehicle_distance = Math.Round (output.Vehicle_distance , 2);
            output.Shortest_time = Math.Round (cost [end] * 60 , 2);

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