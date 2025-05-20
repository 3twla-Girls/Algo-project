using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapRoutingProject;
using static MapRoutingProject.DataStructure;

namespace MapRoutingProject
{
    #region Task4: Handel the Output and Stor them as same as the required formate 
    internal class WriteOutput
    {



        public static void WriteOutputsToFile(string outputFilePath, List<DataStructure.Output> results)
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
            sb.AppendLine($"{WholeRun.timeWithoutIO:F2} ms");
            sb.AppendLine();
            sb.AppendLine($"{WholeRun.timeWithIO:F2} ms");


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

            for (int i = 0; i < file1Lines.Length - 3; i++)
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
