using MeshBuilderLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorGeneration
{
    /// <summary>
    /// This object is only used for testing generators in the editor!
    /// </summary>
    public class EditorMeshObject : MonoBehaviour
    {
        public MeshBuilder MeshBuilder;

        /// <summary>
        /// Removes the mesh from this object in the editor.
        /// </summary>
        public void ClearInEditor()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh.Clear();
        }

        /// <summary>
        /// Must be called by EditorMeshGenerators before using the MeshBuilder.
        /// </summary>
        public void Init()
        {
            MeshBuilder = new MeshBuilder(gameObject);
        }
    }
}
