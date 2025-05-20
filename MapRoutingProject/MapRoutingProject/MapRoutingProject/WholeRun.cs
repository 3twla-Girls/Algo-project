using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MapRoutingProject;

namespace MapRoutingProject
{
    internal class WholeRun
    {
        public static double timeWithoutIO;
        public static double timeWithIO;

        public static void Execute(string input, string query, string output)
        {
            try
            {
                Stopwatch swWithIO = Stopwatch.StartNew();

                LoadInput.loadMap(input);
                LoadInput.loadQuery(query);

                var resultsWithIO = GraphTheory.solve();

                swWithIO.Stop();
                timeWithIO = swWithIO.Elapsed.TotalMilliseconds;

                Stopwatch swWithoutIO = Stopwatch.StartNew();

                var resultsWithoutIO = GraphTheory.solve();

                swWithoutIO.Stop();
                timeWithoutIO = swWithoutIO.Elapsed.TotalMilliseconds;

                WriteOutput.WriteOutputsToFile("D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//results.txt", resultsWithIO);

                bool isCorrect = WriteOutput.CompareOutputFiles(
                    "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//results.txt",
                    output
                );

                Console.WriteLine($"Time WITH I/O: {timeWithIO:F2} ms");
                Console.WriteLine($"Time WITHOUT I/O: {timeWithoutIO:F2} ms");
                Console.WriteLine("Comparison result: " + (isCorrect ? "Passed" : "Failed"));

                MessageBox.Show(
                    $"Solve completed successfully.\n" +
                    $"Time WITH I/O: {timeWithIO:F2} ms\n" +
                    $"Time WITHOUT I/O: {timeWithoutIO:F2} ms\n" +
                    $"Comparison result: {(isCorrect ? "Passed" : "Failed")}"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unhandled exception during solve:\n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void Take_input()
        {
            while (true)
            {
                Console.WriteLine("choose [1] for sample cases [2] for medium cases [3] for large cases [4] for bonus cases");
                int choice = Convert.ToInt32(Console.ReadLine());
                if (choice == 1)
                {
                    Console.WriteLine("choose [1] for case 1 [2] for case 2 [3] for case 3 [4] for case 4 [5] for case 5");
                    int choice1 = Convert.ToInt32(Console.ReadLine());
                    if (choice1 >= 1 && choice1 <= 5)
                    {
                        Execute(
                            $"D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Input//map{choice1}.txt",
                            $"D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Input//queries{choice1}.txt",
                            $"D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Output//output{choice1}.txt"
                        );
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice");
                    }
                }
                else if (choice == 2)
                {
                    Execute(
                        "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[2] Medium Cases//Input//OLMap.txt",
                        "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[2] Medium Cases//Input//OLQueries.txt",
                        "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[2] Medium Cases//Output//OLOutput.txt"
                    );
                }
                else if (choice == 3)
                {
                    Execute(
                        "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[3] Large Cases//Input//SFMap.txt",
                        "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[3] Large Cases//Input//SFQueries.txt",
                        "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[3] Large Cases//Output//SFOutput.txt"
                    );
                }
                else if (choice == 4)
                {
                    Console.WriteLine("choose [1] for sample cases [2] for medium cases");
                    int choice1 = Convert.ToInt32(Console.ReadLine());
                    if (choice1 == 1)
                    {
                        Execute(
                            "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[4] BONUS Test Cases//[4] BONUS Test Cases//[1] Sample Cases//Input//map1B.txt",
                            "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[4] BONUS Test Cases//[4] BONUS Test Cases//[1] Sample Cases//Input//queries1.txt",
                            "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[4] BONUS Test Cases//[4] BONUS Test Cases//[1] Sample Cases//Output//output1.txt"
                        );
                    }
                    else if (choice1 == 2)
                    {
                        Execute(
                            "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[4] BONUS Test Cases//[4] BONUS Test Cases//[2] Medium Cases//Input//OLMapB.txt",
                            "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[4] BONUS Test Cases//[4] BONUS Test Cases//[2] Medium Cases//Input//OLQueries.txt",
                            "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[4] BONUS Test Cases//[4] BONUS Test Cases//[2] Medium Cases//Output//OLOutput.txt"
                        );
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice");
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
