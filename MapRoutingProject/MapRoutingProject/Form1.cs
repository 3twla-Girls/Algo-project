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
        }

        private void Form1_Load ( object sender , EventArgs e )
        {
            // project = new AlgoProject();
            //AlgoProject.loadMap("hhh");

            Input.Take_input ();
        }

        /*private void button1_Click ( object sender , EventArgs e )
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
           // Input.Take_input ();
        }*/



    }
}
