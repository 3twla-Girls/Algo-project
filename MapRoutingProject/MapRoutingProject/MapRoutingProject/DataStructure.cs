using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapRoutingProject
{
    #region Task1: Data Structure for the Graph  
    internal class DataStructure
    {
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
    }
    #endregion
}
