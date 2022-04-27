using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorGeneration
{
    [CustomEditor(typeof(SpheresGenerator))]
    public class Editor_SpheresGenerator : Editor
    {
        public override void OnInspectorGUI()
        {
            SpheresGenerator Generator = (SpheresGenerator)target;

            // Gets called every time a value is changed
            if (DrawDefaultInspector())
            {

            }

            // Add a Generate button
            if (GUILayout.Button("Generate Spheres"))
            {
                Generator.GenerateSpheres(Generator.TargetObject);
            }

        }
    }
}
