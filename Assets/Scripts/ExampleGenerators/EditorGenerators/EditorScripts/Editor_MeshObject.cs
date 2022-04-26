using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorGeneration
{
    [CustomEditor(typeof(EditorMeshObject))]
    public class Editor_MeshObject : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorMeshObject meshObj = (EditorMeshObject)target;

            // Gets called every time a value is changed
            if (DrawDefaultInspector())
            {

            }


            // Add a Reset button
            if (GUILayout.Button("Reset"))
            {
                meshObj.ClearInEditor();
            }
        }
    }
}
