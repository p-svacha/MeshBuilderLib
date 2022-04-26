using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshBuilderLib
{
    /// <summary>
    /// A MeshPolygon represents a 2-dimensional irregular plane, defined by a Polygon. MeshPolygons are always flat on the y-axis.
    /// </summary>
    public class MeshPolygon
    {
        public List<MeshVertex> Vertices;
        public List<MeshTriangle> Triangles;

        public MeshPolygon(List<MeshVertex> vertices, List<MeshTriangle> triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
        }
    }
}
