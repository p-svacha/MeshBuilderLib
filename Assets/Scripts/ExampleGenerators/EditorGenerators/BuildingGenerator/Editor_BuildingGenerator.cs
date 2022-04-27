using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorGeneration
{
    [CustomEditor(typeof(BuildingGenerator))]
    public class Editor_BuildingGenerator : Editor
    {
        public override void OnInspectorGUI()
        {
            BuildingGenerator Generator = (BuildingGenerator)target;

            // Gets called every time a value is changed
            if (DrawDefaultInspector())
            {

            }

            // Add a Generate button
            if (GUILayout.Button("Generate Building"))
            {
                Generator.GenerateBuilding(Generator.TargetObject, BuildingGenerationSettings.GetRandomSettings());
            }

        }
    }
}
