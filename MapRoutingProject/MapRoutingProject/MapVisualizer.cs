using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MapRoutingProject
{
    internal class MapVisualizer : Control
    {
        private Dictionary<long , Intersection> graph;
        private readonly Color backgroundColor = Color.White;
        private readonly Color axisColor = Color.Black;
        private readonly Color gridColor = Color.FromArgb (240 , 240 , 240);
        private readonly Color nodeColor = Color.Azure;
        private readonly Color edgeColor = Color.LightSkyBlue;

        public void SetGraph ( Dictionary<long , Intersection> graph )
        {
            this.graph = graph;
            this.Invalidate (); 
        }

        protected override void OnPaint ( PaintEventArgs e )
        {
            base.OnPaint (e);
            if ( graph == null || graph.Count == 0 ) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            // Fill background
            using ( var backgroundBrush = new SolidBrush (backgroundColor) )
            {
                g.FillRectangle (backgroundBrush , ClientRectangle);
            }

            // Determine bounds for scaling
            double minX = double.MaxValue, maxX = double.MinValue;
            double minY = double.MaxValue, maxY = double.MinValue;

            foreach ( var intersection in graph.Values )
            {
                if ( intersection.x < minX ) minX = intersection.x;
                if ( intersection.x > maxX ) maxX = intersection.x;
                if ( intersection.y < minY ) minY = intersection.y;
                if ( intersection.y > maxY ) maxY = intersection.y;
            }

            // Add 10% padding around the data
            double xRange = maxX - minX;
            double yRange = maxY - minY;
            minX -= xRange * 0.1;
            maxX += xRange * 0.1;
            minY -= yRange * 0.1;
            maxY += yRange * 0.1;

            float padding = 60;
            double scaleX = ( Width - 2 * padding ) / ( maxX - minX );
            double scaleY = ( Height - 2 * padding ) / ( maxY - minY );

            PointF Transform ( double x , double y )
            {
                float tx = (float) ( padding + ( x - minX ) * scaleX );
                float ty = (float) ( Height - ( padding + ( y - minY ) * scaleY ) ); // Flip Y
                return new PointF (tx , ty);
            }

            // Draw grid lines
            using ( var gridPen = new Pen (gridColor , 1f) )
            using ( var axisPen = new Pen (axisColor , 2f) )
            using ( var axisFont = new Font ("Arial" , 8 , FontStyle.Regular) )
            using ( var axisBrush = new SolidBrush (axisColor) )
            using ( var nodeBrush = new SolidBrush (nodeColor) )
            using ( var edgePen = new Pen (edgeColor , 1f) )
            using ( var nodeFont = new Font ("Arial" , 7) )
            using ( var textBrush = new SolidBrush (Color.Black) )
            {
                // Calculate nice intervals for grid lines
                double xInterval = CalculateNiceInterval (maxX - minX);
                double yInterval = CalculateNiceInterval (maxY - minY);

                // Draw vertical grid lines and x-axis labels
                for ( double x = Math.Ceiling (minX / xInterval) * xInterval ; x <= maxX ; x += xInterval )
                {
                    float xPos = Transform (x , minY).X;
                    g.DrawLine (gridPen , xPos , padding , xPos , Height - padding);

                    // Draw x-axis label
                    string label = x.ToString ("0.##");
                    SizeF labelSize = g.MeasureString (label , axisFont);
                    g.DrawString (label , axisFont , axisBrush ,
                        xPos - labelSize.Width / 2 ,
                        Height - padding + 5);
                }

                // Draw horizontal grid lines and y-axis labels
                for ( double y = Math.Ceiling (minY / yInterval) * yInterval ; y <= maxY ; y += yInterval )
                {
                    float yPos = Transform (minX , y).Y;
                    g.DrawLine (gridPen , padding , yPos , Width - padding , yPos);

                    // Draw y-axis label
                    string label = y.ToString ("0.##");
                    SizeF labelSize = g.MeasureString (label , axisFont);
                    g.DrawString (label , axisFont , axisBrush ,
                        padding - labelSize.Width - 5 ,
                        yPos - labelSize.Height / 2);
                }

                // Draw main axes
                PointF origin = Transform (0 , 0);
                if ( minX <= 0 && maxX >= 0 )
                {
                    g.DrawLine (axisPen , origin.X , padding , origin.X , Height - padding);
                }
                if ( minY <= 0 && maxY >= 0 )
                {
                    g.DrawLine (axisPen , padding , origin.Y , Width - padding , origin.Y);
                }

                // Draw axis titles
                string xTitle = "Longitude";
                string yTitle = "Latitude";
                SizeF xTitleSize = g.MeasureString (xTitle , axisFont);
                SizeF yTitleSize = g.MeasureString (yTitle , axisFont);

                g.DrawString (xTitle , axisFont , axisBrush ,
                    Width - padding - xTitleSize.Width ,
                    Height - padding + 20);

                // Rotate for y-axis title
                var saveState = g.Save ();
                g.TranslateTransform (10 , Height / 2);
                g.RotateTransform (-90);
                g.DrawString (yTitle , axisFont , axisBrush , 0 , 0);
                g.Restore (saveState);

                // Draw edges
                foreach ( var kvp in graph )
                {
                    var intersection = kvp.Value;
                    var start = Transform (intersection.x , intersection.y);

                    foreach ( var (neighborId, _, _) in intersection.neighbor_intersections )
                    {
                        if ( graph.TryGetValue (neighborId , out var neighbor) )
                        {
                            var end = Transform (neighbor.x , neighbor.y);
                            g.DrawLine (edgePen , start , end);
                        }
                    }
                }

                /*// Draw nodes
                float nodeRadius = 4f;
                foreach ( var kvp in graph )
                {
                    var intersection = kvp.Value;
                    var pos = Transform (intersection.x , intersection.y);

                    // Draw node
                    g.FillEllipse (nodeBrush , pos.X - nodeRadius , pos.Y - nodeRadius ,
                        nodeRadius * 2 , nodeRadius * 2);

                    // Draw node ID on hover or selection (optional)
                    // Could be implemented with mouse move events
                }*/
            }
        }

        private double CalculateNiceInterval ( double range )
        {
            // Calculate a "nice" interval for grid lines
            double exponent = Math.Floor (Math.Log10 (range));
            double fraction = range / Math.Pow (10 , exponent);

            double niceFraction;
            if ( fraction <= 1.5 ) niceFraction = 1;
            else if ( fraction <= 3 ) niceFraction = 2;
            else if ( fraction <= 7 ) niceFraction = 5;
            else niceFraction = 10;

            return niceFraction * Math.Pow (10 , exponent - 1);
        }
    }
}