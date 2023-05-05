using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MeshBuilderLib
{
    /// <summary>
    /// Used for outlines, ground plans, parcels, etc. 2D representation of an area in the world defined by its edges.
    /// </summary>
    public class Polygon
    {
        public int NumPoints;
        public List<Vector2> Points;

        public float MinX;
        public float MaxX;
        public float MinY;
        public float MaxY;

        public Vector2 Dimensions;


        public Polygon(List<Vector2> points)
        {
            Points = points;
            NumPoints = points.Count;
            MinX = Points.Min(x => x.x);
            MaxX = Points.Max(x => x.x);
            MinY = Points.Min(x => x.y);
            MaxY = Points.Max(x => x.y);
            Dimensions = new Vector2(MaxX - MinX, MaxY - MinY);
        }

        public List<Vector2> GetUVs(float scaleFactor = 1f, bool flipFaceDirection = false)
        {
            List<Vector2> UVs = new List<Vector2>();
            foreach (Vector2 v in Points)
            {
                float uvX, uvY;
                if (flipFaceDirection)
                {
                    uvX = (MaxX - v.x) * scaleFactor;
                    uvY = (v.y - MinY) * scaleFactor;
                }
                else
                {
                    uvX = (v.x - MinX) * scaleFactor;
                    uvY = (v.y - MinY) * scaleFactor;
                }
                UVs.Add(new Vector2(uvX, uvY));
            }
            return UVs;
        }

        /// <summary>
        /// Returns a vertex of the polygon transformed by a given transform
        /// </summary>
        public Vector2 GetTransformedPoint(int index, Transform transform)
        {
            Vector3 transformedWorldPoint = transform.TransformPoint(Points[index].x, 0f, Points[index].y);
            return new Vector2(transformedWorldPoint.x, transformedWorldPoint.z);
        }


        #region static functions
        public static Polygon GetRectangularPolygon(float length, float width)
        {
            List<Vector2> points = new List<Vector2>()
            {
                new Vector2(0f, 0f),
                new Vector2(length, 0f),
                new Vector2(length, width),
                new Vector2(0f, width)
            };
            return new Polygon(points);
        }

        public static Polygon GetRandomPolygon()
        {
            float length = Random.Range(5f, 15f);
            float width = Random.Range(5f, 15f);
            List<Vector2> groundPlanPoints = new List<Vector2>()
        {
            new Vector2(0f, 0f),
            new Vector2(length, 0f),
            new Vector2(length, width),
            new Vector2(0, width)
        };

            int numPreNudges = Random.Range(0, 2);
            for (int i = 0; i < numPreNudges; i++) NudgeGroundPlan(groundPlanPoints);

            List<Vector2> forbiddenExtrudePoints = new List<Vector2>();
            int numExtrudes = Random.Range(0, 4);
            for (int i = 0; i < numExtrudes; i++) ExpandGroundPlan(groundPlanPoints, forbiddenExtrudePoints);

            int numNudges = Random.Range(0, 2);
            for (int i = 0; i < numNudges; i++) NudgeGroundPlan(groundPlanPoints);

            return new Polygon(groundPlanPoints);
        }
        private static void ExpandGroundPlan(List<Vector2> plan, List<Vector2> forbiddenExtrudePoints)
        {
            // Take a random startpoint and adjacent wall of the plan
            int index = Random.Range(0, plan.Count);
            while (forbiddenExtrudePoints.Contains(plan[index])) index = Random.Range(0, plan.Count);

            int nextIndex = index == plan.Count - 1 ? 0 : index + 1;
            Vector2 startPoint = plan[index];
            Vector2 nextPoint = plan[nextIndex];
            Vector2 wall = nextPoint - startPoint;

            // Take a random split point on thal wall and extrude the side [startPoint - splitPoint]
            float splitRatio = Random.Range(0.35f, 0.65f);
            Vector2 splitPoint = startPoint + splitRatio * wall;

            float extrudeLength = Random.Range(5f, 10f);
            Vector2 wall90 = HelperFunctions.RotateVector(wall, -90);
            wall90 = wall90.normalized * extrudeLength;
            Vector2 newPoint1 = startPoint + wall90;
            Vector2 newPoint2 = splitPoint + wall90;
            Vector2 newPoint3 = splitPoint;

            plan.Insert(index + 1, newPoint1);
            plan.Insert(index + 2, newPoint2);
            plan.Insert(index + 3, newPoint3);
            forbiddenExtrudePoints.Add(newPoint3);
        }
        private static void NudgeGroundPlan(List<Vector2> plan)
        {
            float maxNudge = 2.5f;

            // Take a random point and move it around a little
            int index = Random.Range(0, plan.Count);
            Vector2 point = plan[index];

            // Decide if nudge in which or both directions
            float nudgeDirection = Random.value;
            if (nudgeDirection < 0.33f) point.x += Random.Range(-maxNudge, maxNudge);
            else if (nudgeDirection < 0.66d) point.y += Random.Range(-maxNudge, maxNudge);
            else
            {
                point.x += Random.Range(-maxNudge, maxNudge);
                point.y += Random.Range(-maxNudge, maxNudge);
            }

            plan[index] = point;
        }

        #endregion
    }
}
