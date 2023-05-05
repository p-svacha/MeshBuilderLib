using MeshBuilderLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorGeneration
{
    public class BuildingGenerator : EditorMeshGenerator
    {
        public void GenerateBuilding(EditorMeshObject target, BuildingGenerationSettings settings)
        {
            InitGenerator(target);

            int groundPlanCorners = settings.GroundPlan.Points.Count;

            target.MeshBuilder.AddNewSubmesh(MaterialHandler.Singleton.DebugMaterial);
            int submeshIndex = 0;

            // Add outer vertices that follow the ground plan
            List<MeshVertex> vertices = new List<MeshVertex>();
            for (int floor = 0; floor < settings.NumFloors + 1; floor++)
            {
                float currentHeight = floor * settings.FloorHeight;
                for (int i = 0; i < groundPlanCorners; i++)
                {
                    Vector2 point = settings.GroundPlan.Points[i];
                    vertices.Add(target.MeshBuilder.AddVertex(new Vector3(point.x, currentHeight, point.y), Vector2.zero));
                }
            }
            // Add triangles for outer vertices
            for (int floor = 0; floor < settings.NumFloors; floor++)
            {
                int startIndex = floor * groundPlanCorners;
                for (int i = 0; i < groundPlanCorners; i++)
                {
                    if (i < groundPlanCorners - 1)
                    {
                        target.MeshBuilder.AddTriangle(submeshIndex, vertices[startIndex + i], vertices[startIndex + i + 1], vertices[startIndex + groundPlanCorners + i]);
                        target.MeshBuilder.AddTriangle(submeshIndex, vertices[startIndex + i + 1], vertices[startIndex + groundPlanCorners + i + 1], vertices[startIndex + groundPlanCorners + i]);
                    }
                    else
                    {
                        target.MeshBuilder.AddTriangle(submeshIndex, vertices[startIndex + i], vertices[startIndex], vertices[startIndex + groundPlanCorners + i]);
                        target.MeshBuilder.AddTriangle(submeshIndex, vertices[startIndex], vertices[startIndex + groundPlanCorners], vertices[startIndex + groundPlanCorners + i]);
                    }
                }
            }

            int numOuterVertices = vertices.Count;

            // Add inner vertices that follow the ground plan
            for (int floor = 0; floor < settings.NumFloors + 1; floor++)
            {
                float currentHeight = floor * settings.FloorHeight;
                for (int i = 0; i < groundPlanCorners; i++)
                {
                    Vector2 point = settings.GroundPlan.Points[i];
                    Vector2 prevPoint = settings.GroundPlan.Points[i == 0 ? groundPlanCorners - 1 : i - 1];
                    Vector2 nextPoint = settings.GroundPlan.Points[i == groundPlanCorners - 1 ? 0 : i + 1];
                    Vector2 intersectPoint = HelperFunctions.GetOffsetIntersection(prevPoint, point, nextPoint, settings.OuterWallWidth, settings.OuterWallWidth, false);

                    vertices.Add(target.MeshBuilder.AddVertex(new Vector3(intersectPoint.x, currentHeight, intersectPoint.y), Vector2.zero));
                }
            }
            // Add triangles for inner vertices
            for (int floor = 0; floor < settings.NumFloors; floor++)
            {
                int startIndex = numOuterVertices + (floor * groundPlanCorners);
                for (int i = 0; i < groundPlanCorners; i++)
                {
                    if (i < groundPlanCorners - 1)
                    {
                        target.MeshBuilder.AddTriangle(submeshIndex, vertices[startIndex + i], vertices[startIndex + groundPlanCorners + i], vertices[startIndex + i + 1]);
                        target.MeshBuilder.AddTriangle(submeshIndex, vertices[startIndex + i + 1], vertices[startIndex + groundPlanCorners + i], vertices[startIndex + groundPlanCorners + i + 1]);
                    }
                    else
                    {
                        target.MeshBuilder.AddTriangle(submeshIndex, vertices[startIndex + i], vertices[startIndex + groundPlanCorners + i], vertices[startIndex]);
                        target.MeshBuilder.AddTriangle(submeshIndex, vertices[startIndex], vertices[startIndex + groundPlanCorners + i], vertices[startIndex + groundPlanCorners]);
                    }
                }
            }

            int numInnerVertices = vertices.Count;

            // Add triangles for outer wall tops
            for (int i = 0; i < groundPlanCorners; i++)
            {
                if (i < groundPlanCorners - 1)
                {
                    target.MeshBuilder.AddTriangle(submeshIndex, vertices[numOuterVertices - groundPlanCorners + i], vertices[numOuterVertices - groundPlanCorners + i + 1], vertices[numInnerVertices - groundPlanCorners + i]);
                    target.MeshBuilder.AddTriangle(submeshIndex, vertices[numOuterVertices - groundPlanCorners + i + 1], vertices[numInnerVertices - groundPlanCorners + i + 1], vertices[numInnerVertices - groundPlanCorners + i]);
                }
                else
                {
                    target.MeshBuilder.AddTriangle(submeshIndex, vertices[numOuterVertices - groundPlanCorners + i], vertices[numOuterVertices - groundPlanCorners], vertices[numInnerVertices - groundPlanCorners + i]);
                    target.MeshBuilder.AddTriangle(submeshIndex, vertices[numOuterVertices - groundPlanCorners], vertices[numInnerVertices - groundPlanCorners], vertices[numInnerVertices - groundPlanCorners + i]);
                }
            }
            // Add triangles for outer wall bottoms
            for (int i = 0; i < groundPlanCorners; i++)
            {
                if (i < groundPlanCorners - 1)
                {
                    target.MeshBuilder.AddTriangle(submeshIndex, vertices[i], vertices[numOuterVertices + i], vertices[i + 1]);
                    target.MeshBuilder.AddTriangle(submeshIndex, vertices[i + 1], vertices[numOuterVertices + i], vertices[numOuterVertices + i + 1]);
                }
                else
                {
                    target.MeshBuilder.AddTriangle(submeshIndex, vertices[i], vertices[numOuterVertices + i], vertices[0]);
                    target.MeshBuilder.AddTriangle(submeshIndex, vertices[0], vertices[numOuterVertices + i], vertices[numOuterVertices]);
                }
            }


            target.MeshBuilder.ApplyMesh(applyInEditor: true);
        }
    }
}
