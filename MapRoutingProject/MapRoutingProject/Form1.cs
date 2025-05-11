using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//using Task1;

namespace MapRoutingProject
{
    public partial class Form1 : Form
    {
        public Form1 ( )
        {
            InitializeComponent ();
        }

        private void Form1_Load ( object sender , EventArgs e )
        {
            Input.Take_input (); 

            if ( Task1.Intersections == null || Task1.Intersections.Count == 0 )
            {
                MessageBox.Show ("No intersections found to visualize.");
                return;
            }

            var visualizer = new MapVisualizer
            {
                Dock = DockStyle.Fill
            };

            Controls.Add (visualizer);

            var graph = Task1.Intersections; 
            visualizer.SetGraph (graph);
        }
        

    }
}
