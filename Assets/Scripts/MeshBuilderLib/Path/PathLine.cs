using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshBuilderLib
{
    /// <summary>
    /// A PathLine represents a crosssection of a path at a certain position that stretches across the path from the left end to the right end.
    /// The elevation is the same across all of a path line.
    /// </summary>
    public class PathLine
    {
        private Vector3 Position;
        public float Angle { get; private set; }
        public float Width { get; private set; }

        public PathLine(Vector3 position, float angle, float width)
        {
            Position = position;
            Angle = angle;
            Width = width;
        }

        /// <summary>
        /// Returns a position on the PathLine, whereas relativePos is the factor (0-1) that defines the position 
        /// <br/> from the furthest left point (0) to furthest right point (1) regarding the width. 
        /// </summary>
        public Vector3 GetPosition(float relativePos)
        {
            float relativeWidth = -(Width / 2) + (Width * relativePos);
            return new Vector3(
                Position.x + (relativeWidth * Mathf.Sin(Mathf.Deg2Rad * (Angle + 90))),
                Position.y,
                Position.z + (relativeWidth * Mathf.Cos(Mathf.Deg2Rad * (Angle + 90)))
                );
        }

        /// <summary>
        /// Returns the position of the left end of the PathLine.
        /// </summary>
        public Vector3 GetLeftPoint()
        {
            return GetPosition(0f);
        }

        /// <summary>
        /// Returns the position of the left end of the PathLine.
        /// </summary>
        public Vector3 GetRightPoint()
        {
            return GetPosition(1f);
        }

        /// <summary>
        /// Returns the position in the center of the PathLine.
        /// </summary>
        public Vector3 GetCenterPoint()
        {
            return Position;
        }
    }
}
