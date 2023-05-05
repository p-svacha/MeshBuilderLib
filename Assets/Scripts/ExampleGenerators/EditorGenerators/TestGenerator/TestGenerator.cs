using MeshBuilderLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorGeneration
{
    /// <summary>
    /// Used for whatever I'm currently working on in the MeshBuilder and want to test.
    /// </summary>
    public class TestGenerator : EditorMeshGenerator
    {
        public void Generate(EditorMeshObject target)
        {
            InitGenerator(target);

            int submeshIndex = target.MeshBuilder.AddNewSubmesh(MaterialHandler.Singleton.DefaultMaterial);
            int nSegments = Random.Range(50, 100);
            List<float> radii = new List<float>();
            List<float> heights = new List<float>();
            float currentRadius = 10f;
            float radiusChange = 0f;
            for(int i = 0; i < nSegments; i++)
            {
                radiusChange += Random.Range(-0.1f, 0.1f);
                currentRadius += radiusChange;
                if (currentRadius < 0.1f) currentRadius = 0.1f;
                if (currentRadius > 20f) currentRadius = 20f;
                radii.Add(currentRadius);
                if(i > 0) heights.Add(Random.Range(0.3f, 0.8f));
            }
            target.MeshBuilder.BuildSegmentedCylinder(submeshIndex, Vector3.zero, radii, heights, 12);

            target.MeshBuilder.ApplyMesh(applyInEditor: true);
        }
    }
}
