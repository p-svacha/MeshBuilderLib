using MeshBuilderLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiminalDungeonGeneration
{
    public static class ModuleGeneration
    {
        /// <summary>
        /// Returns a list of random exit points for a room
        /// </summary>
        public static List<ExitPoint> GetRandomExitPoints(MeshRoom room)
        {
            List<ExitPoint> exitPoints = new List<ExitPoint>();

            while (exitPoints.Count == 0) // Can't have a module without any exit point
            {
                foreach (MeshPlane wall in room.Walls)
                {
                    Vector2 point = new Vector2(wall.Vertex1.Position.x, wall.Vertex1.Position.z);
                    Vector2 nextPoint = new Vector2(wall.Vertex4.Position.x, wall.Vertex4.Position.z);
                    Vector2 wallVector = nextPoint - point;
                    float wallLength = wallVector.magnitude;

                    if (wallLength <= LiminalDungeonGenerator.MIN_WALL_LENGTH_FOR_EXIT_POINT) continue; // Can't have an exit pointon short walls
                    if (Random.Range(0f, 1f) > LiminalDungeonGenerator.EXIT_POINT_CHANCE_PER_WALL) continue; // Only randomly selected walls have an exit point

                    float splitRatio = Random.Range(0.35f, 0.65f); // 0.5 = exit point is exactly in the center of the wall
                    Vector2 exitPointPosition2d = point + splitRatio * wallVector;
                    Vector3 exitPointPosition = new Vector3(exitPointPosition2d.x, 0f, exitPointPosition2d.y);
                    float exitPointAngle = 90 + Vector2.SignedAngle(wallVector.normalized, Vector2.up);

                    ExitPoint exitPoint = new ExitPoint(exitPointPosition, exitPointAngle, wall, wallLength, splitRatio);
                    exitPoints.Add(exitPoint);
                }
            }

            return exitPoints;
        }

        public static Light AddLight(Vector3 position, Transform parent, Color color, float intensity = 1.5f, float range = 15f)
        {
            GameObject lightObject = new GameObject("light");
            lightObject.transform.position = position;
            Light light = lightObject.AddComponent<Light>();

            light.shadows = LightShadows.Soft;
            light.intensity = intensity;
            light.range = range;
            light.color = color;

            lightObject.transform.SetParent(parent);

            return light;
        }
    }
}
