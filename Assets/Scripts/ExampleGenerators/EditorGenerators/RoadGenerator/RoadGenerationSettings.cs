using MeshBuilderLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorGeneration
{
    public class RoadGenerationSettings
    {
        public Path Path;
        public Material Material;

        public RoadGenerationSettings(Path path, Material material)
        {
            Path = path;
            Material = material;
        }

        public static RoadGenerationSettings GetRandomSettings()
        {
            int numSegments = 1000;
            Material material = MaterialHandler.Singleton.Road;

            float currentWidth = 10f;
            float minWidth = 5;
            float maxWidth = 30;
            float currentWidthChange = 0f;
            float maxWidthChangeDelta = 0.04f;

            float segmentLength = 1f;
            float maxTurnAngle = 5f;
            float maxTurnAngleDelta = 0.5f;
            float maxSteepnessChangePerSegment = 0.01f;

            List<PathLine> pathPoints = new List<PathLine>();
            Vector3 lastPoint = Vector3.zero;
            float currentAngle = 0f;
            float currentTurnAngle = 0f;
            float currentSteepness = 0f;
            pathPoints.Add(new PathLine(lastPoint, currentAngle, currentWidth));

            for (int i = 0; i < numSegments; i++)
            {
                // width
                float widthChangeDelta = Random.Range(-maxWidthChangeDelta, maxWidthChangeDelta);
                currentWidthChange += widthChangeDelta;
                currentWidth += currentWidthChange;
                currentWidth = Mathf.Clamp(currentWidth, minWidth, maxWidth);

                // turn angle
                float turnAngleDelta = Random.Range(-maxTurnAngleDelta, maxTurnAngleDelta);

                if (currentTurnAngle > 0)
                {
                    float turnAngleDeltaMin = -maxTurnAngleDelta;
                    float turnAngleDeltaMax = maxTurnAngleDelta * (1 - (currentTurnAngle / maxTurnAngle));
                    turnAngleDelta = Random.Range(turnAngleDeltaMin, turnAngleDeltaMax);
                }
                else if (currentTurnAngle < 0)
                {
                    float turnAngleDeltaMin = -maxTurnAngleDelta * (1 - (currentTurnAngle / -maxTurnAngle));
                    float turnAngleDeltaMax = maxTurnAngleDelta;
                    turnAngleDelta = Random.Range(turnAngleDeltaMin, turnAngleDeltaMax);
                }

                currentTurnAngle += turnAngleDelta;
                if (currentTurnAngle > maxTurnAngle) currentTurnAngle = maxTurnAngle;
                if (currentTurnAngle < -maxTurnAngle) currentTurnAngle = maxTurnAngle;
                currentAngle += currentTurnAngle;
                float newX = lastPoint.x + (segmentLength * Mathf.Sin(Mathf.Deg2Rad * currentAngle));
                float newZ = lastPoint.z + (segmentLength * Mathf.Cos(Mathf.Deg2Rad * currentAngle));

                // steepness
                float steepnessChange = Random.Range(-maxSteepnessChangePerSegment, maxSteepnessChangePerSegment);
                currentSteepness += steepnessChange;
                float newY = lastPoint.y + currentSteepness;

                Vector3 nextPoint = new Vector3(newX, newY, newZ);
                pathPoints.Add(new PathLine(nextPoint, currentAngle, currentWidth));

                lastPoint = nextPoint;
            }

            Path path = new Path(pathPoints);

            return new RoadGenerationSettings(path, material);
        }
    }
}
