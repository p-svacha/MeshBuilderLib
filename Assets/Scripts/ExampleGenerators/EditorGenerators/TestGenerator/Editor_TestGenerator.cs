using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorGeneration
{
    [CustomEditor(typeof(TestGenerator))]
    public class Editor_TestGenerator : Editor
    {
        public override void OnInspectorGUI()
        {
            TestGenerator Generator = (TestGenerator)target;

            // Gets called every time a value is changed
            if (DrawDefaultInspector())
            {

            }

            // Add a Generate button
            if (GUILayout.Button("Generate"))
            {
                Generator.Generate(Generator.TargetObject);
            }

        }
    }
}
