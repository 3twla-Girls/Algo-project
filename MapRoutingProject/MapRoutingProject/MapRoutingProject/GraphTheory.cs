using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using MapRoutingProject;


namespace MapRoutingProject
{
    #region Task2: Constructe the Graph theory  
    internal class GraphTheory
    {

        public static List<dynamic> displayMap()
        {
            var table = new List<dynamic>();
            foreach (var item in LoadInput.Intersections)
            {
                long id = item.Key;
                DataStructure.Intersection intersection = item.Value;
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
            foreach (DataStructure.Query item in LoadInput.Queries)
            {

                table.Add(new { Source_X = item.Source_X, Source_Y = item.Source_Y, Destination_X = item.Destination_X, Destination_Y = item.Destination_Y, R = item.Available_meters });
            }
            return table;
        }

        public static Dictionary<long, DataStructure.Intersection> CalculateGraph(DataStructure.Query q, Dictionary<long, DataStructure.Intersection> graph)
        {
            double R = q.Available_meters / 1000.0;
            double R2 = R * R;

            // Add virtual nodes for the source (-1) and destination (1000000)
            if (!graph.ContainsKey(-1))
            {
                graph[-1] = new DataStructure.Intersection
                {
                    x = q.Source_X,
                    y = q.Source_Y,
                    neighbor_intersections = new List<(long, double, double)>()
                };
            }
            if (!graph.ContainsKey(1000000))
            {
                graph[1000000] = new DataStructure.Intersection
                {
                    x = q.Destination_X,
                    y = q.Destination_Y,
                    neighbor_intersections = new List<(long, double, double)>()
                };
            }

            // For lazy cloning: track which nodes’ neighbor lists have been cloned.
            HashSet<long> clonedNodes = new HashSet<long>();

            foreach (var kvp in LoadInput.Intersections)
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
                        graph[kvp.Key] = new DataStructure.Intersection
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
                        graph[kvp.Key] = new DataStructure.Intersection
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


        private static Dictionary<long, DataStructure.Intersection> CloneGraphForQuery()
        {
            var graph = new Dictionary<long, DataStructure.Intersection>(LoadInput.Intersections.Count);
            foreach (var kvp in LoadInput.Intersections)
            {

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



        public static List<DataStructure.Output> solve()
        {
            var results = new DataStructure.Output[LoadInput.Queries.Count]; // Use array for thread-safe index-based writing

            Parallel.ForEach(
                Partitioner.Create(0, LoadInput.Queries.Count),
                range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        var q = LoadInput.Queries[i];
                        var temp_graph = CloneGraphForQuery(); // Assume this creates an independent copy
                        CalculateGraph(q, temp_graph);
                        results[i] = dijkstra(temp_graph, -1, 1000000);
                    }
                });

            return results.ToList();
        }


        private static DataStructure.Output dijkstra(Dictionary<long, DataStructure.Intersection> graph, long start, long end)
        {
            var output = new DataStructure.Output();
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





    }


    #endregion

}