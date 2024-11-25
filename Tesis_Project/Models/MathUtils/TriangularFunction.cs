using System;

namespace Tesis_Project.Models.MathUtils
{
    public class TriangularFunction
    {

        public string TerminoName { get; set; }

        // Points defining the triangular fuzzy set
        public Point LeftPoint { get; set; }
        public Point PeakPoint { get; set; }
        public Point RightPoint { get; set; }

        // Constructor to initialize the triangular function with the three points
        public TriangularFunction(string terminoName , Point leftPoint, Point peakPoint, Point rightPoint )
        {
            TerminoName = terminoName;
            LeftPoint = leftPoint;
            PeakPoint = peakPoint;
            RightPoint = rightPoint;
        }

        // Method to calculate the membership value of a given X (fuzzification)
        public double CalculateMembership(double x)
        {
            double a = LeftPoint.X;
            double b = PeakPoint.X;
            double c = RightPoint.X;

            if (x < a || x > c)
            {
                return 0; // Fuera del rango
            }
            else if (x >= a && x < b)
            {
                // Pendiente ascendente (lado izquierdo del triángulo)
                return (x - a) / (b - a);
            }
            else if (x >= b && x <= b)
            {
                // Pico del triángulo
                return 1;
            }
            else if (x > b && x <= c)
            {
                // Pendiente descendente (lado derecho del triángulo)
                return (c - x) / (c - b);
            }

            return 0; // Seguridad adicional (nunca debería alcanzarse)
        }

        // Method to get a string representation of the triangular function
        public override string ToString()
        {
            return $"Left Point: ({LeftPoint.X}, {LeftPoint.Y})\n" +
                   $"Peak Point: ({PeakPoint.X}, {PeakPoint.Y})\n" +
                   $"Right Point: ({RightPoint.X}, {RightPoint.Y})";
        }
    }
}
