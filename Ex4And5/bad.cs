namespace Example1
{
    public class Circle {
        public double Radius { get; set; }
    }

    public static class ShapeCalculator {
        public static int CalculateCircumference(Circle circle) {
            return 2 * Math.PI * circle.Radius;
        }
    }   
}

namespace GoodExample
{
    public interface Shape {
        double CalculateCircumference();
    }

    public class Circle : Shape {
        public double Radius { get; set; }

        public double CalculateCircumference() {
            return 2 * Math.PI * this.Radius;
        }
    }

    public static class ShapeCalculator {
        public static double CalculateCircumference(Shape shape) {
            return shape.CalculateCircumference();
        }
    }   
}

