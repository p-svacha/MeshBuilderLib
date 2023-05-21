using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorGeneration
{
    public class RoadGenerator : EditorMeshGenerator
    {
        public void GenerateRoad(EditorMeshObject target, RoadGenerationSettings settings)
        {
            InitGenerator(target);
            settings.Path.DebugPath(target.transform.position);
            target.MeshBuilder.BuildPath(settings.Path, settings.Material, uvScalingY: 0.1f);
            target.MeshBuilder.ApplyMesh(applyInEditor: true);
        }
    }
}
