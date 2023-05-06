using MeshBuilderLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorGeneration
{
    public class RoadGenerationSettings
    {
        public Path Path;
        public float Width;
        public Material Material;

        public RoadGenerationSettings(Path path, float width, Material material)
        {
            Path = path;
            Width = width;
            Material = material;
        }

        public void DebugPath(Vector3 offset)
        {
            for (int i = 1; i < Path.Points.Count; i++)
            {
                Debug.DrawLine(offset + Path.Points[i - 1].Position, offset + Path.Points[i].Position, Color.red, 20f, false);
            }
        }

        public static RoadGenerationSettings GetRandomSettings()
        {
            int numSegments = 1000;
            float width = 10;
            float segmentLength = 1f;
            float maxTurnAngle = 5f;
            float maxTurnAngleDelta = 0.5f;
            float maxSteepnessChangePerSegment = 0.01f;
            Material material = MaterialHandler.Singleton.Road;

            List<PathPoint> pathPoints = new List<PathPoint>();
            Vector3 lastPoint = Vector3.zero;
            float currentAngle = 0f;
            float currentTurnAngle = 0f;
            float currentSteepness = 0f;
            pathPoints.Add(new PathPoint(lastPoint, currentAngle));

            for(int i = 0; i < numSegments; i++)
            {
                float turnAngleDelta = Random.Range(-maxTurnAngleDelta, maxTurnAngleDelta);

                if (currentTurnAngle > 0)
                {
                    float turnAngleDeltaMin = -maxTurnAngleDelta;
                    float turnAngleDeltaMax = maxTurnAngleDelta * (1 - (currentTurnAngle / maxTurnAngle));
                    turnAngleDelta = Random.Range(turnAngleDeltaMin, turnAngleDeltaMax);
                    Debug.Log("current turn angle: " + currentTurnAngle + " => delta min: " + turnAngleDeltaMin + ", delta max: " + turnAngleDeltaMax);
                }
                else if(currentTurnAngle < 0)
                {
                    float turnAngleDeltaMin = -maxTurnAngleDelta * (1 - (currentTurnAngle / -maxTurnAngle));
                    float turnAngleDeltaMax = maxTurnAngleDelta;
                    turnAngleDelta = Random.Range(turnAngleDeltaMin, turnAngleDeltaMax);
                    Debug.Log("current turn angle: " + currentTurnAngle + " => delta min: " + turnAngleDeltaMin + ", delta max: " + turnAngleDeltaMax);
                }
                
                
                currentTurnAngle += turnAngleDelta;
                if (currentTurnAngle > maxTurnAngle) currentTurnAngle = maxTurnAngle;
                if (currentTurnAngle < -maxTurnAngle) currentTurnAngle = maxTurnAngle;
                currentAngle += currentTurnAngle;            
                float newX = lastPoint.x + (segmentLength * Mathf.Sin(Mathf.Deg2Rad * currentAngle));
                float newZ = lastPoint.z + (segmentLength * Mathf.Cos(Mathf.Deg2Rad * currentAngle));

                float steepnessChange = Random.Range(-maxSteepnessChangePerSegment, maxSteepnessChangePerSegment);
                currentSteepness += steepnessChange;
                float newY = lastPoint.y + currentSteepness;

                Vector3 nextPoint = new Vector3(newX, newY, newZ);
                pathPoints.Add(new PathPoint(nextPoint, currentAngle));

                lastPoint = nextPoint;
            }

            Path path = new Path(pathPoints);
            return new RoadGenerationSettings(path, width, material);
        }
    }
}
