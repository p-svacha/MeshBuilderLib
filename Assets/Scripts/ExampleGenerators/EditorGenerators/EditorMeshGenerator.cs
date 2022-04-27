using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorGeneration
{
    public class EditorMeshGenerator : MonoBehaviour
    {
        public EditorMeshObject TargetObject;

        protected void InitGenerator(EditorMeshObject target)
        {
            target.ClearInEditor();
            target.Init();
        }
    }
}
