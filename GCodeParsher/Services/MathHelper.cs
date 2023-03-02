using netDxf;

namespace oGCodeParshernsoleApp1.Services;

public static class GcodeMathHelper
{
        public static Vector2 ArcCenter(Vector3 P1, Vector3 P2, Double Radius, int Direction,out string ErrorDescription)
        {
            // returns arc center based on start and end points, radius and arc direction (2=CW, 3=CCW)
            // Radius can be negative (for arcs over 180 degrees)
            double Angle = 0, AdditionalAngle = 0, L1 = 0, L2 = 0, Diff=0;
            double AllowedError = 0.002;
            Vector2 Center = new Vector2(0, 0);
            ErrorDescription = "";
            Vector2 T1, T2;

            // Sort points depending of direction
            if (Direction == 2)
            {
                T1 = new Vector2(P2.X, P2.Y);
                T2 = new Vector2(P1.X, P1.Y);
            }
            else // 03
            {
                T1 = new Vector2(P1.X, P1.Y);
                T2 = new Vector2(P2.X, P2.Y);
            }

            // find angle arc covers
            Angle = CalculateAngle(T1, T2);

            L1 = PointDistance(T1, T2) / 2;
            Diff = L1 - Math.Abs(Radius);

            if (Math.Abs(Radius) < L1 && Diff > AllowedError)
            {
                ErrorDescription = "Error - wrong radius";
                return Center;
            }

            L2 = Math.Sqrt(Math.Abs(Math.Pow(Radius,2) - Math.Pow(L1,2)));

            if (L1 == 0)
                AdditionalAngle = Math.PI / 2;
            else
                AdditionalAngle = Math.Atan(L2 / L1);

            // Add or subtract from angle (depending of radius sign)
            if (Radius < 0)
                Angle -= AdditionalAngle;
            else
                Angle += AdditionalAngle;

            // calculate center (from T1)
            Center = new Vector2((float) (T1.X + Math.Abs(Radius) * Math.Cos(Angle)), (float) (T1.Y + Math.Abs(Radius) * Math.Sin(Angle)));
            return Center;
        }

        public static double CalculateAngle(Vector2 P1, Vector2 P2)
        {
            // returns Angle of line between 2 points and X axis (according to quadrants)
            double Angle = 0;

            if (P1 == P2) // same points
                return 0;
            else if (P1.X == P2.X) // 90 or 270
            {
                Angle = Math.PI / 2;
                if (P1.Y > P2.Y) Angle += Math.PI;
            }
            else if (P1.Y == P2.Y) // 0 or 180
            {
                Angle = 0;
                if (P1.X > P2.X) Angle += Math.PI;
            }
            else
            {
                Angle = Math.Atan(Math.Abs((P2.Y - P1.Y) / (P2.X - P1.X))); // 1. quadrant
                if (P1.X > P2.X && P1.Y < P2.Y) // 2. quadrant
                    Angle = Math.PI - Angle;
                else if (P1.X > P2.X && P1.Y > P2.Y) // 3. quadrant
                    Angle += Math.PI;
                else if (P1.X < P2.X && P1.Y > P2.Y) // 4. quadrant
                    Angle = 2 * Math.PI - Angle;
            }
            return Angle;
        }

        public static double PointDistance(Vector2 P1, Vector2 P2)
        {
            return Math.Sqrt(Math.Pow((P2.X - P1.X), 2) + Math.Pow((P2.Y - P1.Y), 2));
        }
}