using MeshBuilderLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorGeneration
{
    public class RoomGenerator : EditorMeshGenerator
    {
        public void GenerateRoom(EditorMeshObject target, RoomGenerationSettings settings)
        {
            InitGenerator(target);

            // Floor
            int floorSubmeshIndex = target.MeshBuilder.AddNewSubmesh(MaterialHandler.Singleton.DefaultMaterial);
            List<MeshVertex> floorVertices = new List<MeshVertex>();

            List<Vector2> uvs = settings.GroundPlan.GetUVs(LiminalDungeonGenerator.FLOOR_TEXTURE_SCALING);
            for (int i = 0; i < settings.GroundPlan.NumPoints; i++)
            {
                floorVertices.Add(target.MeshBuilder.AddVertex(new Vector3(settings.GroundPlan.Points[i].x, 0, settings.GroundPlan.Points[i].y), uvs[i]));
            }
            int[] floorTriangles = PolygonTriangulator.Triangulate(settings.GroundPlan);
            for (int i = 0; i < floorTriangles.Length; i += 3) target.MeshBuilder.AddTriangle(floorSubmeshIndex, floorVertices[floorTriangles[i]], floorVertices[floorTriangles[i + 1]], floorVertices[floorTriangles[i + 2]]);

            // Ceiling
            int ceilingSubmeshIndex = target.MeshBuilder.AddNewSubmesh(MaterialHandler.Singleton.DefaultMaterial);
            List<MeshVertex> ceilingVertices = new List<MeshVertex>();

            for (int i = 0; i < settings.GroundPlan.NumPoints; i++)
            {
                ceilingVertices.Add(target.MeshBuilder.AddVertex(new Vector3(settings.GroundPlan.Points[i].x, settings.Height, settings.GroundPlan.Points[i].y), uvs[i]));
            }
            int[] ceilingTriangles = PolygonTriangulator.Triangulate(settings.GroundPlan, flipFaceDirection: true);
            for (int i = 0; i < ceilingTriangles.Length; i += 3) target.MeshBuilder.AddTriangle(ceilingSubmeshIndex, ceilingVertices[ceilingTriangles[i]], ceilingVertices[ceilingTriangles[i + 1]], ceilingVertices[ceilingTriangles[i + 2]]);

            // Walls
            int wallSubmeshIndex = target.MeshBuilder.AddNewSubmesh(MaterialHandler.Singleton.DefaultMaterial);
            List<MeshVertex> wallVertices = new List<MeshVertex>();

            float uvStart = 0f;
            float uvEnd = 0f;
            for (int i = 0; i < settings.GroundPlan.Points.Count; i++)
            {
                Vector2 point = settings.GroundPlan.Points[i];
                Vector2 nextPoint = i < settings.GroundPlan.Points.Count - 1 ? settings.GroundPlan.Points[i + 1] : settings.GroundPlan.Points[0];

                uvEnd += Vector2.Distance(point, nextPoint);
                float scaleFactor = 0.2f;

                int startIndex = wallVertices.Count;

                wallVertices.Add(target.MeshBuilder.AddVertex(new Vector3(point.x, 0, point.y), new Vector2(uvStart * scaleFactor, 0)));
                wallVertices.Add(target.MeshBuilder.AddVertex(new Vector3(point.x, settings.Height, point.y), new Vector2(uvStart * scaleFactor, settings.Height * scaleFactor)));
                wallVertices.Add(target.MeshBuilder.AddVertex(new Vector3(nextPoint.x, 0, nextPoint.y), new Vector2(uvEnd * scaleFactor, 0)));
                wallVertices.Add(target.MeshBuilder.AddVertex(new Vector3(nextPoint.x, settings.Height, nextPoint.y), new Vector2(uvEnd * scaleFactor, settings.Height * scaleFactor)));

                target.MeshBuilder.AddTriangle(wallSubmeshIndex, wallVertices[startIndex], wallVertices[startIndex + 3], wallVertices[startIndex + 1]);
                target.MeshBuilder.AddTriangle(wallSubmeshIndex, wallVertices[startIndex], wallVertices[startIndex + 2], wallVertices[startIndex + 3]);

                uvStart = uvEnd;
            }

            target.MeshBuilder.ApplyMesh(applyInEditor: true);
        }
    }
}
