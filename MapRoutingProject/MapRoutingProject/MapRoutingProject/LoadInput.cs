using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapRoutingProject;

namespace MapRoutingProject
{
    #region Task1: Handel the Inputs and Stor them  
    internal class LoadInput
    {

        //handel the input
        public static Dictionary<long, DataStructure.Intersection> Intersections = new Dictionary<long, DataStructure.Intersection>();

        public static List<DataStructure.Query> Queries = new List<DataStructure.Query>();

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
                    DataStructure.Intersection intersection = new DataStructure.Intersection
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
                    DataStructure.Intersection intersection = Intersections[from_id];
                    DataStructure.Intersection intersection2 = Intersections[to_id];
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
                DataStructure.Query query = new DataStructure.Query();
                query.Source_X = double.Parse(parts[0]);
                query.Source_Y = double.Parse(parts[1]);
                query.Destination_X = double.Parse(parts[2]);
                query.Destination_Y = double.Parse(parts[3]);
                query.Available_meters = double.Parse(parts[4]);
                Queries.Add(query);
            }
        }
    }
    #endregion
}
