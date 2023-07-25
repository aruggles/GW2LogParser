﻿using System;

namespace GW2EIEvtcParser.EIData
{
    public class Point3D
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        private static float Mix(float a, float b, float c)
        {
            return (1.0f - c) * a + c * b;
        }

        public static Point3D operator +(Point3D a, Point3D b)
        {
            var newPt = new Point3D(a);
            newPt.Add(b);
            return newPt;
        }

        public static Point3D operator -(Point3D a, Point3D b)
        {
            var newPt = new Point3D(a);
            newPt.Substract(b);
            return newPt;
        }
        public static Point3D operator *(Point3D a, Point3D b)
        {
            var newPt = new Point3D(a);
            newPt.Multiply(b);
            return newPt;
        }
        public static Point3D operator *(float a, Point3D b)
        {
            var newPt = new Point3D(b);
            newPt.MultiplyScalar(a);
            return newPt;
        }
        public static Point3D operator *(Point3D a, float b)
        {
            var newPt = new Point3D(a);
            newPt.MultiplyScalar(b);
            return newPt;
        }
        public static Point3D operator -(Point3D a)
        {
            var newPt = new Point3D(a);
            newPt.MultiplyScalar(-1);
            return newPt;
        }

        public void Add(Point3D a)
        {
            X += a.X;
            Y += a.Y;
            Z += a.Z;
        }
        public void Substract(Point3D a)
        {
            X -= a.X;
            Y -= a.Y;
            Z -= a.Z;
        }
        public void Multiply(Point3D a)
        {
            X *= a.X;
            Y *= a.Y;
            Z *= a.Z;
        }
        public void MultiplyScalar(float a)
        {
            X *= a;
            Y *= a;
            Z *= a;
        }

        public float DistanceToPoint(Point3D endPoint)
        {
            float distance = (float)Math.Sqrt((endPoint.X - X) * (endPoint.X - X) + (endPoint.Y - Y) * (endPoint.Y - Y) + (endPoint.Z - Z) * (endPoint.Z - Z));
            return distance;
        }
        public float Distance2DToPoint(Point3D endPoint)
        {
            float distance = (float)Math.Sqrt((endPoint.X - X) * (endPoint.X - X) + (endPoint.Y - Y) * (endPoint.Y - Y));
            return distance;
        }

        public float Length()
        {
            float length = (float)Math.Sqrt(X * X + Y * Y + Z * Z);
            return length;
        }

        public Point3D(float x, float y)
        {
            X = x;
            Y = y;
            Z = 0;
        }

        public Point3D(float x, float y, float z) : this(x,y)
        {
            Z = z;
        }

        public Point3D(Point3D a) : this(a.X, a.Y, a.Z)
        {
        }


        public Point3D(Point3D a, Point3D b, float ratio)
        {
            X = Mix(a.X, b.X, ratio);
            Y = Mix(a.Y, b.Y, ratio);
            Z = Mix(a.Z, b.Z, ratio);
        }

        public static float GetRotationFromFacing(Point3D facing)
        {
            return (float)Math.Round(ParserHelper.RadianToDegree(Math.Atan2(facing.Y, facing.X)), ParserHelper.CombatReplayDataDigit);
        }
    }
}
