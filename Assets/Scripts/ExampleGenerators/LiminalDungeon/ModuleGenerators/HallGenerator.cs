using MeshBuilderLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiminalDungeonGeneration
{
    public static class HallGenerator
    {
        private const float MIN_HALL_SIZE = 20f;
        private const float MAX_HALL_SIZE = 40f;

        private const float MIN_HALL_HEIGHT = 4f;
        private const float MAX_HALL_HEIGHT = 8f;

        private const float LIGHT_INTERVAL = 3.5f; // in meters

        private const float LIGHT_CEILING_DISTANCE = 0.5f;

        public static DungeonModule GenerateRandomHall()
        {
            float hallLength = Random.Range(MIN_HALL_SIZE, MAX_HALL_SIZE);
            float hallWidth = Random.Range(MIN_HALL_SIZE, MAX_HALL_SIZE);
            float hallHeight = Random.Range(MIN_HALL_HEIGHT, MAX_HALL_HEIGHT);

            MeshBuilder meshBuilder = new MeshBuilder("hall", LiminalDungeonGenerator.COLLISION_LAYER);
            Polygon groundPlan = Polygon.GetRectangularPolygon(hallLength, hallWidth);

            MeshRoom room = meshBuilder.BuildRoom(groundPlan, hallHeight, LiminalDungeonGenerator.FLOOR_TEXTURE_SCALING, LiminalDungeonGenerator.WALL_TEXTURE_SCALING);
            List<ExitPoint> exitPoints = ModuleGeneration.GetRandomExitPoints(room);
            GameObject moduleObject = meshBuilder.ApplyMesh(addCollider: true);

            // Lights
            int numLightsX = (int)(hallLength / LIGHT_INTERVAL);
            float lightIntervalX = hallLength / (numLightsX + 1);
            int numLightsY = (int)(hallWidth / LIGHT_INTERVAL);
            float lightIntervalY = hallWidth / (numLightsY + 1);
            for(int y = 0; y < numLightsY; y++)
            {
                for(int x = 0; x < numLightsX; x++)
                {
                    float xPos = (x + 1) * lightIntervalX;
                    float yPos = (y + 1) * lightIntervalY;
                    Vector3 pos = new Vector3(xPos, hallHeight - LIGHT_CEILING_DISTANCE, yPos);
                    ModuleGeneration.AddLight(pos, moduleObject.transform, Color.white, 1.25f, hallHeight * 1.5f);
                }
            }
            
            DungeonModule module = moduleObject.AddComponent<DungeonModule>();
            module.Init(groundPlan, hallHeight, exitPoints, meshBuilder, room.WallSubmeshIndex);
            return module;
        }
    }
}

