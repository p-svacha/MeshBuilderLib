using MeshBuilderLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorGeneration
{
    public class RoomGenerationSettings
    {
        public Polygon GroundPlan;
        public float Height;

        public RoomGenerationSettings(Polygon shape, float height)
        {
            GroundPlan = shape;
            Height = height;
        }

        public static RoomGenerationSettings GetRandomSettings()
        {
            float height = Random.Range(2.5f, 4f);
            return new RoomGenerationSettings(Polygon.GetRandomPolygon(), height);
        }
    }
}
