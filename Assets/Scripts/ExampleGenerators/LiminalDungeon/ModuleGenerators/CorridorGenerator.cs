using MeshBuilderLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiminalDungeonGeneration
{
    public static class CorridorGenerator
    {
        private const float MIN_CORRIDOR_LENGTH = 5f;
        private const float MAX_CORRIDOR_LENGTH = 20f;

        private const float MIN_CORRIDOR_WIDTH = 1.5f;
        private const float MAX_CORRIDOR_WIDTH = 3f;

        private const float MIN_CORRIDOR_HEIGHT = 2.5f;
        private const float MAX_CORRIDOR_HEIGHT = 3f;

        private const float ELEVATION_CHANGE_CHANCE = 0.5f;

        private const float MIN_ELEVATION_CHANGE_PER_METER = 0.1f;
        private const float MAX_ELEVATION_CHANGE_PER_METER = 0.5f;

        // Distance to where the slope starts after the exit points
        private const float MIN_ELEVATION_CHANGE_MARGIN = 0.5f;
        private const float MAX_ELEVATION_CHANGE_MARGIN = 2f; 
        

        public static DungeonModule GenerateRandomCorridor()
        {
            float corridorLength = Random.Range(MIN_CORRIDOR_LENGTH, MAX_CORRIDOR_LENGTH);
            float corridorWidth = Random.Range(MIN_CORRIDOR_WIDTH, MAX_CORRIDOR_WIDTH);
            float corridorHeight = Random.Range(MIN_CORRIDOR_HEIGHT, MAX_CORRIDOR_HEIGHT);
            bool hasElevationChange = Random.value < ELEVATION_CHANGE_CHANCE;
            float elevationChangePerMeter = Random.Range(MIN_ELEVATION_CHANGE_PER_METER, MAX_ELEVATION_CHANGE_PER_METER);
            float elevationChangeMargin = Random.Range(MIN_ELEVATION_CHANGE_MARGIN, MAX_ELEVATION_CHANGE_MARGIN);

            MeshBuilder meshBuilder = new MeshBuilder("corridor", LiminalDungeonGenerator.COLLISION_LAYER);
            Polygon groundPlan = Polygon.GetRectangularPolygon(corridorLength, corridorWidth);

            // Calculate module height
            float moduleHeight = corridorHeight;
            float elevationChange = 0f;
            if(hasElevationChange)
            {
                float elevationDistance = corridorLength - 2 * elevationChangeMargin;
                elevationChange = elevationChangePerMeter * elevationDistance;
                moduleHeight += elevationChange;
            }

            // Calculate all vertices for the 4 walls and ceiling
            Vector3 b1 = new Vector3(0f, 0f, 0f);
            Vector3 b2 = new Vector3(corridorLength, 0f, 0f);
            Vector3 b3 = new Vector3(corridorLength, 0f, corridorWidth);
            Vector3 b4 = new Vector3(0, 0f, corridorWidth);
            Vector3 t1 = new Vector3(0f, moduleHeight, 0f);
            Vector3 t2 = new Vector3(corridorLength, moduleHeight, 0f);
            Vector3 t3 = new Vector3(corridorLength, moduleHeight, corridorWidth);
            Vector3 t4 = new Vector3(0, moduleHeight, corridorWidth);

            // Add walls of corridor
            int wallSubmeshIndex = meshBuilder.AddNewSubmesh(MaterialHandler.Instance.GetRandomWallMaterial());
            MeshPlane leftWall = meshBuilder.BuildPlane(wallSubmeshIndex, b1, t1, t2, b2, Vector2.zero, new Vector2(corridorLength * LiminalDungeonGenerator.WALL_TEXTURE_SCALING, moduleHeight * LiminalDungeonGenerator.WALL_TEXTURE_SCALING));
            MeshPlane exitWall1 = meshBuilder.BuildPlane(wallSubmeshIndex, b2, t2, t3, b3, Vector2.zero, new Vector2(corridorWidth * LiminalDungeonGenerator.WALL_TEXTURE_SCALING, moduleHeight * LiminalDungeonGenerator.WALL_TEXTURE_SCALING));
            MeshPlane rightWall = meshBuilder.BuildPlane(wallSubmeshIndex, b3, t3, t4, b4, Vector2.zero, new Vector2(corridorLength * LiminalDungeonGenerator.WALL_TEXTURE_SCALING, moduleHeight * LiminalDungeonGenerator.WALL_TEXTURE_SCALING));
            MeshPlane exitWall2 = meshBuilder.BuildPlane(wallSubmeshIndex, b4, t4, t1, b1, Vector2.zero, new Vector2(corridorWidth * LiminalDungeonGenerator.WALL_TEXTURE_SCALING, moduleHeight * LiminalDungeonGenerator.WALL_TEXTURE_SCALING));

            // Add ceiling
            int ceilingSubmeshIndex = meshBuilder.AddNewSubmesh(MaterialHandler.Instance.GetRandomCeilingMaterial());
            meshBuilder.BuildPlane(ceilingSubmeshIndex, t1, t4, t3, t2, Vector2.zero, new Vector2(corridorLength * LiminalDungeonGenerator.FLOOR_TEXTURE_SCALING, corridorWidth * LiminalDungeonGenerator.FLOOR_TEXTURE_SCALING));

            // Add floor
            int floorSubmeshIndex = meshBuilder.AddNewSubmesh(MaterialHandler.Instance.GetRandomFloorMaterial());
            if(!hasElevationChange) meshBuilder.BuildPlane(floorSubmeshIndex, b1, b2, b3, b4, Vector2.zero, new Vector2(corridorWidth * LiminalDungeonGenerator.FLOOR_TEXTURE_SCALING, corridorLength * LiminalDungeonGenerator.FLOOR_TEXTURE_SCALING));
            else
            {
                Vector3 e1 = new Vector3(elevationChangeMargin, 0f, 0f);
                Vector3 e2 = new Vector3(corridorLength - elevationChangeMargin, elevationChange, 0f);
                Vector3 e3 = new Vector3(corridorLength - elevationChangeMargin, elevationChange, corridorWidth);
                Vector3 e4 = new Vector3(elevationChangeMargin, 0f, corridorWidth);
                Vector3 bt2 = new Vector3(corridorLength, elevationChange, 0f);
                Vector3 bt3 = new Vector3(corridorLength, elevationChange, corridorWidth);

                meshBuilder.BuildPlane(floorSubmeshIndex, b1, e1, e4, b4, Vector2.zero, new Vector2(corridorWidth, elevationChangeMargin));
                meshBuilder.BuildPlane(floorSubmeshIndex, e1, e2, e3, e4, new Vector2(0f, elevationChangeMargin), new Vector2(corridorWidth, corridorLength - elevationChangeMargin));
                meshBuilder.BuildPlane(floorSubmeshIndex, e2, bt2, bt3, e3, new Vector2(0f, corridorLength - elevationChangeMargin), new Vector2(corridorWidth, corridorLength));
            }

            // Add exit points
            Vector3 exitWall1Center = (b2 + b3) / 2;
            Vector3 exitPoint1Pos = exitWall1Center + new Vector3(0f, elevationChange, 0f);
            float exitWall1Length = (b3 - b2).magnitude;
            ExitPoint exitPoint1 = new ExitPoint(exitPoint1Pos, 90, exitWall1, exitWall1Length, 0.5f);

            Vector3 exitWall2Center = (b4 + b1) / 2;
            Vector3 exitPoint2Pos = exitWall2Center;
            float exitWall2Length = (b4 - b1).magnitude;
            ExitPoint exitPoint2 = new ExitPoint(exitPoint2Pos, 270, exitWall2, exitWall2Length, 0.5f);

            List<ExitPoint> exitPoints = new List<ExitPoint>() { exitPoint1, exitPoint2 };

            GameObject moduleObject = meshBuilder.ApplyMesh(addCollider: true);
            DungeonModule module = moduleObject.AddComponent<DungeonModule>();
            module.Init(groundPlan, moduleHeight, exitPoints, meshBuilder, wallSubmeshIndex);
            return module;
        }
    }
}
