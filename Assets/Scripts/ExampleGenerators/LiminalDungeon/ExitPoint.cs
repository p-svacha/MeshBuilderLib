using MeshBuilderLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiminalDungeonGeneration
{
    /// <summary>
    /// An exit point is a part of a dungeon module that marks points where modules can be connected to each other.
    /// ExitPoints are always straight on the y axis, meaning they never go up or down.
    /// </summary>
    public class ExitPoint
    {
        public DungeonModule Module;
        private Vector3 LocalPosition; // The local position of the exitpoint within the module
        private float LocalDirection; // The local direction of the exitpoint facing outside the module

        public float RelativeWallPosition; // [0-1] What ration the exit point is placed on the wall,  0.5 = center
        public float WallLength;

        public MeshPlane Wall;

        public ExitPoint(Vector3 position, float direction, MeshPlane wall, float wallLength, float relativeWallPosition)
        {
            LocalPosition = position;
            LocalDirection = direction;
            Wall = wall;
            WallLength = wallLength;
            RelativeWallPosition = relativeWallPosition;
        }

        public float GetLocalHeight()
        {
            return LocalPosition.y;
        }

        public Vector3 GetWorldPosition()
        {
            return Module.transform.TransformPoint(LocalPosition);
        }

        public float GetWorldDirection()
        {
            return Module.transform.rotation.eulerAngles.y + LocalDirection;
        }

        /// <summary>
        /// Returns the position of a point in the direction of the exit point in a given distance.
        /// </summary>
        public Vector3 GetForwardPosition(float distance)
        {
            Vector3 endOffset = new Vector3(distance * Mathf.Sin(Mathf.Deg2Rad * GetWorldDirection()), 0f, distance * Mathf.Cos(Mathf.Deg2Rad * GetWorldDirection()));
            return GetWorldPosition() + endOffset;
        }

        /// <summary>
        /// Returns a point along the wall of the exit point in a given distance and at a given height. 
        /// </summary>
        public Vector3 GetOffsetPosition(float distance, float height)
        {
            float offsetAngle = GetWorldDirection() + 90;
            Vector3 endOffset = new Vector3(distance * Mathf.Sin(Mathf.Deg2Rad * offsetAngle), height, distance * Mathf.Cos(Mathf.Deg2Rad * offsetAngle));
            return GetWorldPosition() + endOffset;
        }
    }
}
