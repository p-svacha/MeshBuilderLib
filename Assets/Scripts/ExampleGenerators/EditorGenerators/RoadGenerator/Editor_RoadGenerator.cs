using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorGeneration
{
    [CustomEditor(typeof(RoadGenerator))]
    public class Editor_RoadGenerator : Editor
    {
        public override void OnInspectorGUI()
        {
            RoadGenerator Generator = (RoadGenerator)target;

            // Gets called every time a value is changed
            if (DrawDefaultInspector())
            {

            }

            // Add a Generate button
            if (GUILayout.Button("Generate Road"))
            {
                RoadGenerationSettings settings = RoadGenerationSettings.GetRandomSettings();
                settings.DebugPath(Generator.TargetObject.transform.position);
                Generator.GenerateRoad(Generator.TargetObject, settings);
            }

        }
    }
}