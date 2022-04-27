using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshBuilderLib
{
    /// <summary>
    /// A MeshElement is a basic data structure that contains vertex, triangle and submesh data of a part of a mesh.
    /// </summary>
    public class MeshElement
    {
        public int SubmeshIndex;
        public List<MeshVertex> Vertices;
        public List<MeshTriangle> Triangles;

        public MeshElement(int submeshIndex, List<MeshVertex> vertices, List<MeshTriangle> triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
        }
    }
}
