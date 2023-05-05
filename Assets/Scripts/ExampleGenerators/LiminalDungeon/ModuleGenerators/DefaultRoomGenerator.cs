using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshBuilderLib;

namespace LiminalDungeonGeneration
{
    public static class DefaultRoomGenerator
    {
        private const float MIN_HEIGHT = 2.5f;
        private const float MAX_HEIGHT = 4f;

        private const float MIN_LIGHT_INTENSITY = 0.8f;
        private const float MAX_LIGHT_INTENSITY = 3f;

        private const float MIN_LIGHT_RANGE = 8f;
        private const float MAX_LIGHT_RANGE = 30f;

        private const float MAX_LIGHT_DARKNESS = 0.5f;

        private const float LIGHT_CEILING_DISTANCE = 0.3f;

        public static DungeonModule GenerateRandomRoom()
        {
            MeshBuilder meshBuilder = new MeshBuilder("room", LiminalDungeonGenerator.COLLISION_LAYER);
            Polygon groundPlan = Polygon.GetRandomPolygon();
            float height = Random.Range(MIN_HEIGHT, MAX_HEIGHT);

            MeshRoom room = meshBuilder.BuildRoom(groundPlan, height, LiminalDungeonGenerator.FLOOR_TEXTURE_SCALING, LiminalDungeonGenerator.WALL_TEXTURE_SCALING);
            List<ExitPoint> exitPoints = ModuleGeneration.GetRandomExitPoints(room);

            GameObject moduleObject = meshBuilder.ApplyMesh(addCollider: true);

            // Lights (place a light at the poi of the groundplan
            Vector2 poi = PolygonCenterFinder.GetPolyLabel(PolygonCenterFinder.ConvertPolygonToFloatArray(groundPlan));
            Vector3 lightPos = new Vector3(poi.x, height - LIGHT_CEILING_DISTANCE, poi.y);
            float lightIntensity = Random.Range(MIN_LIGHT_INTENSITY, MAX_LIGHT_INTENSITY);
            float lightRange = Random.Range(MIN_LIGHT_RANGE, MAX_LIGHT_RANGE);
            Color lightColor = new Color(MAX_LIGHT_DARKNESS + Random.value * MAX_LIGHT_DARKNESS, MAX_LIGHT_DARKNESS + Random.value * MAX_LIGHT_DARKNESS, MAX_LIGHT_DARKNESS + Random.value * MAX_LIGHT_DARKNESS);
            ModuleGeneration.AddLight(lightPos, moduleObject.transform, lightColor, lightIntensity, lightRange);

            DungeonModule module = moduleObject.AddComponent<DungeonModule>();
            module.Init(groundPlan, height, exitPoints, meshBuilder, room.WallSubmeshIndex);
            return module;
        }
    }
}
