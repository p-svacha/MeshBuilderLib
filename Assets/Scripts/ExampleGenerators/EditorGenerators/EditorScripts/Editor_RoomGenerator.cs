using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorGeneration
{
    [CustomEditor(typeof(RoomGenerator))]
    public class Editor_RoomGenerator : Editor
    {
        public override void OnInspectorGUI()
        {
            RoomGenerator Generator = (RoomGenerator)target;

            // Gets called every time a value is changed
            if (DrawDefaultInspector())
            {

            }

            // Add a Generate button
            if (GUILayout.Button("Generate Room"))
            {
                Generator.GenerateRoom(Generator.TestObject, RoomGenerationSettings.GetRandomSettings());
            }

        }
    }
}
