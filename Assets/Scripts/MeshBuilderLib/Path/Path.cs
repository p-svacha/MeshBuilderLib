using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshBuilderLib
{
    public class Path
    {
        public List<PathPoint> Points;

        public Path(List<PathPoint> points)
        {
            Points = points;
        }
    }
}
