using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MapRoutingProject
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ( )
        {
            button1 = new Button ();
            button2 = new Button ();
            dataGridView1 = new DataGridView ();
            dataGridView2 = new DataGridView ();
            button3 = new Button ();
            ( (System.ComponentModel.ISupportInitialize) dataGridView1 ).BeginInit ();
            ( (System.ComponentModel.ISupportInitialize) dataGridView2 ).BeginInit ();
            SuspendLayout ();
            // 
            // button1
            // 
            button1.Location = new Point (41 , 404);
            button1.Margin = new Padding (2);
            button1.Name = "button1";
            button1.Size = new Size (143 , 71);
            button1.TabIndex = 0;
            button1.Text = "Load Map File";
            button1.UseVisualStyleBackColor = true;
            button1.Click +=  button1_Click ;
            // 
            // button2
            // 
            button2.Location = new Point (565 , 404);
            button2.Margin = new Padding (2);
            button2.Name = "button2";
            button2.Size = new Size (155 , 71);
            button2.TabIndex = 1;
            button2.Text = "Load Query File";
            button2.UseVisualStyleBackColor = true;
            button2.Click +=  button2_Click ;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point (18 , 10);
            dataGridView1.Margin = new Padding (2);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.Size = new Size (411 , 390);
            dataGridView1.TabIndex = 2;
            dataGridView1.CellContentClick +=  dataGridView1_CellContentClick ;
            // 
            // dataGridView2
            // 
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Location = new Point (590 , 136);
            dataGridView2.Margin = new Padding (2);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.RowHeadersWidth = 62;
            dataGridView2.Size = new Size (288 , 180);
            dataGridView2.TabIndex = 3;
            dataGridView2.CellContentClick +=  dataGridView2_CellContentClick_1 ;
            // 
            // button3
            // 
            button3.Location = new Point (810 , 426);
            button3.Name = "button3";
            button3.Size = new Size (94 , 29);
            button3.TabIndex = 4;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = true;
            button3.Click +=  button3_Click ;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF (8F , 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size (918 , 519);
            Controls.Add (button3);
            Controls.Add (dataGridView2);
            Controls.Add (dataGridView1);
            Controls.Add (button2);
            Controls.Add (button1);
            Margin = new Padding (2);
            Name = "Form1";
            Text = "Form1";
            Load +=  Form1_Load ;
            ( (System.ComponentModel.ISupportInitialize) dataGridView1 ).EndInit ();
            ( (System.ComponentModel.ISupportInitialize) dataGridView2 ).EndInit ();
            ResumeLayout (false);
        }

        #endregion

        private Button button1;
        private Button button2;
        private DataGridView dataGridView1;
        private DataGridView dataGridView2;
        private Button button3;
    }
}
