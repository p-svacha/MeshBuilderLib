using MeshBuilderLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorGeneration
{
    public class BuildingGenerationSettings
    {
        public Polygon GroundPlan;
        public int NumFloors;
        public float FloorHeight;
        public float OuterWallWidth;

        public BuildingGenerationSettings(Polygon groundPlan, int numFloors, float floorHeight, float outerWallWidth)
        {
            GroundPlan = groundPlan;
            NumFloors = numFloors;
            FloorHeight = floorHeight;
            OuterWallWidth = outerWallWidth;
        }

        public static BuildingGenerationSettings GetRandomSettings()
        {
            Polygon GroundPlan = Polygon.GetRandomPolygon();
            int numFloors = Random.Range(1, 6);
            float floorHeight = Random.Range(2f, 3f);
            float outerWallWidth = Random.Range(0.1f, 0.5f);
            return new BuildingGenerationSettings(GroundPlan, numFloors, floorHeight, outerWallWidth);
        }
    }
}
