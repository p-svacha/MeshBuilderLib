using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshBuilderLib
{
    public class PathPoint
    {
        public Vector3 Position;
        public float Angle;

        public PathPoint(Vector3 position, float angle)
        {
            Position = position;
            Angle = angle;
        }
    }
}
