using EditorGeneration;
using MeshBuilderLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorGeneration
{
    public class SpheresGenerator : EditorMeshGenerator
    {
        public void GenerateSpheres(EditorMeshObject target)
        {
            InitGenerator(target);

            int sphereSubmeshIndex = target.MeshBuilder.AddNewSubmesh(MaterialHandler.Singleton.DefaultMaterial);

            int nSpheres = Random.Range(2, 13);
            for (int i = 0; i < nSpheres; i++)
            {
                SpheresGenerationSettings settings = SpheresGenerationSettings.GetRandomSettings();
                target.MeshBuilder.BuildSphere(sphereSubmeshIndex, settings.Position, settings.Width, settings.Height, settings.Rows, settings.Cols);
            }

            target.MeshBuilder.ApplyMesh(applyInEditor: true);
        }
    }
}
