using System;
using System.Collections.Concurrent;
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
    public class GraphAdapter
    {
        private readonly Dictionary<long, Intersection> baseGraph;
        private readonly Dictionary<long, List<(long, double, double)>> extraEdges = new();
        private readonly Dictionary<long, Intersection> virtualNodes = new();

        public GraphAdapter(Dictionary<long, Intersection> baseGraph)
        {
            this.baseGraph = baseGraph;
        }

        public void AddVirtualNode(long id, double x, double y)
        {
            if (!virtualNodes.ContainsKey(id))
            {
                virtualNodes[id] = new Intersection
                {
                    x = x,
                    y = y,
                    neighbor_intersections = new List<(long, double, double)>()
                };
            }
        }

        public void AddEdge(long from, long to, double length, double speed)
        {
            if (!extraEdges.ContainsKey(from))
            {
                extraEdges[from] = new List<(long, double, double)>();
            }
            extraEdges[from].Add((to, length, speed));
        }

        public Intersection GetNode(long id)
        {
            if (virtualNodes.ContainsKey(id))
            {
                var node = virtualNodes[id];
                node.neighbor_intersections = extraEdges.ContainsKey(id)
                    ? extraEdges[id]
                    : new List<(long, double, double)>();
                return node;
            }

            if (baseGraph.ContainsKey(id))
            {
                var baseNode = baseGraph[id];
                var neighbors = new List<(long, double, double)>(baseNode.neighbor_intersections);
                if (extraEdges.ContainsKey(id))
                {
                    neighbors.AddRange(extraEdges[id]);
                }
                return new Intersection
                {
                    x = baseNode.x,
                    y = baseNode.y,
                    neighbor_intersections = neighbors
                };
            }

            return new Intersection();
        }

        public IEnumerable<long> GetAllNodeIds()
        {
            return baseGraph.Keys
                .Concat(virtualNodes.Keys)
                .Concat(extraEdges.Keys)
                .Distinct();
        }
    }
    internal class Task1
    {


        //handel the input

        public static Dictionary<long, Intersection> Intersections = new Dictionary<long, Intersection>();

        public static List<Query> Queries = new List<Query>();

        public static void loadMap(string filePath)
        {
            Intersections.Clear();
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Read the number of intersections
                string line = reader.ReadLine();
                int n = int.Parse(line);

                // Read each intersection
                for (int i = 0; i < n; i++)
                {
                    line = reader.ReadLine();
                    var parts = line.Split(' ');
                    Intersection intersection = new Intersection
                    {
                        x = double.Parse(parts[1]),
                        y = double.Parse(parts[2]),
                        neighbor_intersections = new List<(long, double, double)>()
                    };
                    long id = long.Parse(parts[0]);
                    Intersections[id] = intersection;
                }

                // Read the number of roads
                line = reader.ReadLine();
                int m = int.Parse(line);

                // Read each road and update neighboring intersections
                for (int i = 0; i < m; i++)
                {
                    line = reader.ReadLine();
                    var parts = line.Split(' ');
                    long from_id = long.Parse(parts[0]);
                    long to_id = long.Parse(parts[1]);
                    double LengthKm = double.Parse(parts[2]);
                    double SpeedKmph = double.Parse(parts[3]);

                    // Add neighbor to both intersections (bidirectional)
                    Intersection intersection = Intersections[from_id];
                    Intersection intersection2 = Intersections[to_id];
                    intersection.neighbor_intersections.Add((to_id, LengthKm, SpeedKmph));
                    intersection2.neighbor_intersections.Add((from_id, LengthKm, SpeedKmph));
                }
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

        public static Dictionary<long, Intersection> CalculateGraph(Query q, Dictionary<long, Intersection> graph)
        {
            double R = q.Available_meters / 1000.0;
            double R2 = R * R;

            // Add virtual nodes for the source (-1) and destination (1000000)
            if (!graph.ContainsKey(-1))
            {
                graph[-1] = new Intersection
                {
                    x = q.Source_X,
                    y = q.Source_Y,
                    neighbor_intersections = new List<(long, double, double)>()
                };
            }
            if (!graph.ContainsKey(1000000))
            {
                graph[1000000] = new Intersection
                {
                    x = q.Destination_X,
                    y = q.Destination_Y,
                    neighbor_intersections = new List<(long, double, double)>()
                };
            }

            // For lazy cloning: track which nodes’ neighbor lists have been cloned.
            HashSet<long> clonedNodes = new HashSet<long>();

            foreach (var kvp in Task1.Intersections)
            {
                // Distance from the source:
                double dx = kvp.Value.x - q.Source_X;
                double dy = kvp.Value.y - q.Source_Y;
                double dist2 = dx * dx + dy * dy;
                if (dist2 <= R2)
                {
                    double distance = Math.Sqrt(dist2);
                    // Add connection from virtual source (-1) to actual node.
                    graph[-1].neighbor_intersections.Add((kvp.Key, distance, 5));

                    // Lazy clone node kvp.Key if not already cloned.
                    if (!clonedNodes.Contains(kvp.Key))
                    {
                        var orig = graph[kvp.Key].neighbor_intersections;
                        graph[kvp.Key] = new Intersection
                        {
                            x = graph[kvp.Key].x,
                            y = graph[kvp.Key].y,
                            neighbor_intersections = new List<(long, double, double)>(orig)
                        };
                        clonedNodes.Add(kvp.Key);
                    }
                    graph[kvp.Key].neighbor_intersections.Add((-1, distance, 5));
                }

                // Distance from the destination:
                double dx1 = kvp.Value.x - q.Destination_X;
                double dy1 = kvp.Value.y - q.Destination_Y;
                double dist2_1 = dx1 * dx1 + dy1 * dy1;
                if (dist2_1 <= R2)
                {
                    double distance1 = Math.Sqrt(dist2_1);
                    graph[1000000].neighbor_intersections.Add((kvp.Key, distance1, 5));

                    if (!clonedNodes.Contains(kvp.Key))
                    {
                        var orig = graph[kvp.Key].neighbor_intersections;
                        graph[kvp.Key] = new Intersection
                        {
                            x = graph[kvp.Key].x,
                            y = graph[kvp.Key].y,
                            neighbor_intersections = new List<(long, double, double)>(orig)
                        };
                        clonedNodes.Add(kvp.Key);
                    }
                    graph[kvp.Key].neighbor_intersections.Add((1000000, distance1, 5));
                }
            }
            return graph;
        }

        // Instead of a full DeepCopy, we perform a lazy shallow copy.
        // Because Intersection is a struct the basic fields get copied,
        // but we retain the same neighbor_intersections reference until modified.
        private static Dictionary<long, Intersection> CloneGraphForQuery()
        {
            var graph = new Dictionary<long, Intersection>(Task1.Intersections.Count);
            foreach (var kvp in Task1.Intersections)
            {
                // Shallow copy: note that neighbor_intersections (a reference type) is not duplicated.
                graph[kvp.Key] = kvp.Value;
            }
            return graph;
        }

        //public static List<Output> solve()
        //{
        //    // Using sequential processing here. If you have many queries,
        //    // you may try Parallel.For, but for some workloads the overhead hurts.
        //    var results = new List<Output>(Task1.Queries.Count);
        //    foreach (var q in Task1.Queries)
        //    {
        //        var temp_graph = CloneGraphForQuery();
        //        CalculateGraph(q, temp_graph);
        //        results.Add(dijkstra(temp_graph, -1, 1000000));
        //    }
        //    return results;
        //}



        public static List<Output> solve()
        {
            var results = new Output[Task1.Queries.Count]; // Use array for thread-safe index-based writing

            Parallel.ForEach(
                Partitioner.Create(0, Task1.Queries.Count),
                range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        var q = Task1.Queries[i];
                        var temp_graph = CloneGraphForQuery(); // Assume this creates an independent copy
                        CalculateGraph(q, temp_graph);
                        results[i] = dijkstra(temp_graph, -1, 1000000);
                    }
                });

            return results.ToList();
        }

        //public static List<Output> solve()
        //{
        //    List<Output> results = new List<Output>(new Output[Queries.Count]);

        //    Parallel.For(0, Queries.Count, i =>
        //    {
        //        var q = Queries[i];
        //        var temp_graph = DeepCopyIntersections(Intersections);
        //        CalculateGraph(q, temp_graph);
        //        results[i] = dijkstra(temp_graph, -1, 1000000);
        //    });

        //    return results;
        //}

        private static Output dijkstra(Dictionary<long, Intersection> graph, long start, long end)
        {
            var output = new Output();
            var count = graph.Count;

            // Pre-allocate dictionaries with exact capacity
            var cost = new Dictionary<long, double>(count);
            var previous = new Dictionary<long, long?>(count);
            var visited = new HashSet<long>();
            var pq = new PriorityQueue<long, double>(count);
            //**************************************
            // Initialize data structures
            foreach (var node in graph.Keys)
            {
                cost[node] = double.MaxValue;
                previous[node] = null;
            }

            cost[start] = 0;
            pq.Enqueue(start, 0);

            while (pq.Count > 0)
            {
                var currentNode = pq.Dequeue();

                if (visited.Contains(currentNode)) continue;
                if (currentNode == end) break;

                visited.Add(currentNode);
                var currentCost = cost[currentNode];
                var neighbors = graph[currentNode].neighbor_intersections;

                // Process neighbors with minimal allocations
                for (int i = 0; i < neighbors.Count; i++)
                {
                    var (neighborId, distance, speed) = neighbors[i];
                    if (visited.Contains(neighborId)) continue;

                    var newCost = currentCost + (distance / speed);
                    if (newCost < cost[neighborId])
                    {
                        cost[neighborId] = newCost;
                        previous[neighborId] = currentNode;
                        pq.Enqueue(neighborId, newCost);
                    }
                }
            }

            // Optimized path reconstruction
            var path = new List<long>();
            for (long? at = end; at != null; at = previous[at.Value])
            {
                path.Add(at.Value);
            }
            path.Reverse();

            // Calculate distances
            double walkingDist = 0, vehicleDist = 0;
            for (int i = 1; i < path.Count; i++)
            {
                var from = path[i - 1];
                var to = path[i];
                var edge = graph[from].neighbor_intersections.Find(n => n.Item1 == to);

                if (from == -1 || to == 1000000)
                    walkingDist += edge.Item2;
                else
                    vehicleDist += edge.Item2;
            }

            // Clean path endpoints
            if (path.Count > 0 && path[0] == -1) path.RemoveAt(0);
            if (path.Count > 0 && path[^1] == 1000000) path.RemoveAt(path.Count - 1);

            // Set output values
            output.Path = path;
            output.Shortest_time = Math.Round(cost[end] * 60, 2);
            output.Total_distance = Math.Round(walkingDist + vehicleDist, 2);
            output.Total_walking_distance = Math.Round(walkingDist, 2);
            output.Vehicle_distance = Math.Round(vehicleDist, 2);

            return output;
        }



        public static void WriteOutputsToFile(string outputFilePath, List<Output> results)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var output in results)
            {
                sb.AppendLine(string.Join(" ", output.Path));
                sb.AppendLine($"{output.Shortest_time:F2} mins");
                sb.AppendLine($"{output.Total_distance:F2} km");
                sb.AppendLine($"{output.Total_walking_distance:F2} km");
                sb.AppendLine($"{output.Vehicle_distance:F2} km");
                sb.AppendLine();
            }
            File.WriteAllText(outputFilePath, sb.ToString());

        }

        public static bool CompareOutputFiles(string filePath1, string filePath2)
        {
            var file1Lines = File.ReadAllLines(filePath1);
            var file2Lines = File.ReadAllLines(filePath2);

            /*if ( file1Lines.Length != file2Lines.Length )
            {
                Console.WriteLine ("Files have different number of lines.");
                return false;
            }*/

            for (int i = 0; i < file1Lines.Length; i++)
            {
                if (file1Lines[i].Trim() != file2Lines[i].Trim())
                {
                    Console.WriteLine($"Mismatch at line {i + 1}:");
                    Console.WriteLine($"File1: {file1Lines[i]}");
                    Console.WriteLine($"File2: {file2Lines[i]}");
                    return false;
                }
            }

            //Console.WriteLine ("Files match.");
            return true;
        }

    }


    #endregion

}