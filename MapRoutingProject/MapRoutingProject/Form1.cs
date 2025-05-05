using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//using Task1;

public class ConsoleHelper
{
    [DllImport ("kernel32.dll")]
    public static extern bool AllocConsole ( );
}

namespace MapRoutingProject
{
    public partial class Form1 : Form
    {
        //AlgoProject project;
        public Form1 ( )
        {
            InitializeComponent ();
            // Open the console window
            ConsoleHelper.AllocConsole ();

            // Now you can use Console.WriteLine to write to the console window
            Console.WriteLine ("This is a test message in the console window!");
        }

        private void Form1_Load ( object sender , EventArgs e )
        {
            // project = new AlgoProject();
            //AlgoProject.loadMap("hhh");


        }

        private void button1_Click ( object sender , EventArgs e )
        {
            OpenFileDialog openFileDialog = new OpenFileDialog ();
            openFileDialog.Title = "Select Map File";
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";


            if ( openFileDialog.ShowDialog () == DialogResult.OK )
            {
                try
                {
                    Task1.loadMap (openFileDialog.FileName);
                    MessageBox.Show ("Map file loaded successfully.");
                    dataGridView1.DataSource = Task1.displayMap ();
                }
                catch ( Exception ex )
                {
                    MessageBox.Show ("Error loading map file:\n" + ex.Message);
                }
            }
        }

        private void button2_Click ( object sender , EventArgs e )
        {
            OpenFileDialog openFileDialog = new OpenFileDialog ();
            openFileDialog.Title = "Select Query File";
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if ( openFileDialog.ShowDialog () == DialogResult.OK )
            {
                try
                {
                    Task1.loadQuery (openFileDialog.FileName);
                    // dataGridView2.DataSource = AlgoProject.displayQuery();
                    MessageBox.Show ("Query file loaded successfully.");
                    dataGridView2.DataSource = Task1.displayQuery ();
                    //dataGridView2.DataSource = AlgoProject.Queries;

                }
                catch ( Exception ex )
                {
                    MessageBox.Show ("Error loading query file:\n" + ex.Message);
                }
            }
        }

        private void dataGridView1_CellContentClick ( object sender , DataGridViewCellEventArgs e )
        {
            //dataGridView1.DataSource = AlgoProject.displayMap();
        }

        private void dataGridView2_CellContentClick ( object sender , DataGridViewCellEventArgs e )
        {
            // dataGridView2.DataSource = AlgoProject.Queries;
        }

        private void dataGridView2_CellContentClick_1 ( object sender , DataGridViewCellEventArgs e )
        {

        }

        private void button3_Click ( object sender , EventArgs e )
        {
            try
            {
                // Time WITH I/O
                Stopwatch swWithIO = Stopwatch.StartNew ();

                Task1.loadMap ("D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Input//map1.txt"); // Replace with stored path if needed
                Task1.loadQuery ("D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Input//queries1.txt"); // Replace with stored path if needed

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
                    "D://MapRoutingProject//MapRoutingProject//MapRoutingProject//TEST CASES//[1] Sample Cases//Output//output1.txt"
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



    }
}
