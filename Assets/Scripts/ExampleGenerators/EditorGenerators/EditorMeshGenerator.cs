using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorGeneration
{
    public class EditorMeshGenerator : MonoBehaviour
    {
        public EditorMeshObject TargetObject;

        /// <summary>
        /// Initializes the MeshBuilder of the target object within the editor.
        /// </summary>
        protected void InitGenerator(EditorMeshObject target)
        {
            target.ClearInEditor();
            target.Init();
        }
    }
}
