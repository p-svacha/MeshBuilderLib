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
            target.MeshBuilder.BuildRoom(settings.GroundPlan, settings.Height, MaterialHandler.Singleton.DebugMaterial, MaterialHandler.Singleton.DebugMaterial, MaterialHandler.Singleton.DebugMaterial, LiminalDungeonGenerator.FLOOR_TEXTURE_SCALING, LiminalDungeonGenerator.WALL_TEXTURE_SCALING);
            target.MeshBuilder.ApplyMesh(applyInEditor: true);
        }
    }
}
