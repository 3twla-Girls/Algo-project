using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapRoutingProject
{
    internal class Input
    {
        public static void Execute ( string input , string query , string output )
        {
            try
            {
                // Time WITH I/O
                Stopwatch swWithIO = Stopwatch.StartNew ();

                Task1.loadMap (input); // Replace with stored path if needed
                Task1.loadQuery (query); // Replace with stored path if needed

                var resultsWithIO = Task1.solve ();
                Task1.WriteOutputsToFile ("D://MapRoutingProject//MapRoutingProject//MapRoutingProject//results.txt" , resultsWithIO);

                swWithIO.Stop ();
                double timeWithIO = swWithIO.Elapsed.TotalMilliseconds;

                // Time WITHOUT I/O
                Stopwatch swWithoutIO = Stopwatch.StartNew ();

                var resultsWithoutIO = Task1.solve ();

                swWithoutIO.Stop ();
                double timeWithoutIO = swWithoutIO.Elapsed.TotalMilliseconds;


                // Now compare files
                bool isCorrect = Task1.CompareOutputFiles (
                    "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//results.txt" ,
                    output
                );

                // Console + MessageBox feedback
                Console.WriteLine ($"Time WITH I/O: {timeWithIO:F2} ms");
                Console.WriteLine ($"Time WITHOUT I/O: {timeWithoutIO:F2} ms");
                Console.WriteLine ("Comparison result: " + ( isCorrect ? "Passed" : "Failed" ));

                MessageBox.Show (
                    $"Solve completed successfully.\n" +
                    $"Time WITH I/O: {timeWithIO:F2} ms\n" +
                    $"Time WITHOUT I/O: {timeWithoutIO:F2} ms\n" +
                    $"Comparison result: {( isCorrect ? "Passed" : "Failed" )}"
                );
            }
            catch ( Exception ex )
            {
                MessageBox.Show ("Unhandled exception during solve:\n" + ex.Message + "\n" + ex.StackTrace);
            }
        }
        public static void Take_input ( )
        {
            while (true) {
                Console.WriteLine ("choose [1] for sample cases [2] for medium cases [3] for large cases [4] for bonus cases");
                int choice = Convert.ToInt32 (Console.ReadLine ());
                if ( choice == 1 )
                {
                    Console.WriteLine ("choose [1] for case 1 [2] for case 2 [3] for case 3 [4] for case 4 [5] for case 5");
                    int choice1 = Convert.ToInt32 (Console.ReadLine ());
                    if ( choice1 == 1 )
                    {
                        Execute ("D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Input//map1.txt" ,
                           "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Input//queries1.txt" ,
                           "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Output//output1.txt");
                    }
                    else if ( choice1 == 2 )
                    {
                        Execute ("D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Input//map2.txt" ,
                           "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Input//queries2.txt" ,
                           "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Output//output2.txt");
                    }
                    else if ( choice1 == 3 )
                    {
                        Execute ("D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Input//map3.txt" ,
                           "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Input//queries3.txt" ,
                           "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Output//output3.txt");
                    }
                    else if ( choice1 == 4 )
                    {
                        Execute ("D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Input//map4.txt" ,
                           "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Input//queries4.txt" ,
                           "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Output//output4.txt");
                    }
                    else if ( choice1 == 5 )
                    {
                        Execute ("D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Input//map5.txt" ,
                           "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Input//queries5.txt" ,
                           "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Output//output5.txt");
                    }
                    else
                    {
                        Console.WriteLine ("Invalid choice");
                    }

                }
                else if ( choice == 2 )
                {
                    Execute ("D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[2] Medium Cases//Input//OLMap.txt" ,
                          "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[2] Medium Cases//Input//OLQueries.txt" ,
                          "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[2] Medium Cases//Output//OLOutput.txt");
                }
                else if ( choice == 3 )
                {
                    Execute ("D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[3] Large Cases//Input//SFMap.txt" ,
                          "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[3] Large Cases//Input//SFQueries.txt" ,
                          "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[3] Large Cases//Output//SFOutput.txt");
                }
                else if ( choice == 4 )
                {
                    Console.WriteLine ("choose [1] for sample cases [2] for medium cases");
                    int choice1 = Convert.ToInt32 (Console.ReadLine ());
                    if ( choice1 == 1 )
                    {
                        Execute ("D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[4] BONUS Test Cases//[4] BONUS Test Cases//[1] Sample Cases//Input//map1B.txt" ,
                          "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[4] BONUS Test Cases//[4] BONUS Test Cases//[1] Sample Cases//Input//queries1.txt" ,
                          "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[4] BONUS Test Cases//[4] BONUS Test Cases//[1] Sample Cases//Output//output1.txt");
                    }
                    else if ( choice1 == 2 )
                    {
                        Execute ("D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[4] BONUS Test Cases//[4] BONUS Test Cases//[2] Medium Cases//Input//OLMapB.txt" ,
                          "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[4] BONUS Test Cases//[4] BONUS Test Cases//[2] Medium Cases//Input//OLQueries.txt" ,
                          "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[4] BONUS Test Cases//[4] BONUS Test Cases//[2] Medium Cases//Output//OLOutput.txt");
                    }
                    else
                    {
                        Console.WriteLine ("Invalid choice");
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
