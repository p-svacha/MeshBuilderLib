using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshBuilderLib
{
    public class Path
    {
        public List<PathLine> Points;

        public Path(List<PathLine> points)
        {
            Points = points;
        }

        public void DebugPath(Vector3 offset)
        {
            for (int i = 1; i < Points.Count; i++)
            {
                // center
                Debug.DrawLine(offset + Points[i - 1].GetCenterPoint(), offset + Points[i].GetCenterPoint(), Color.red, 20f, false);

                // bounds
                Debug.DrawLine(offset + Points[i - 1].GetLeftPoint(), offset + Points[i].GetLeftPoint(), Color.blue, 20f, false);
                Debug.DrawLine(offset + Points[i - 1].GetRightPoint(), offset + Points[i].GetRightPoint(), Color.blue, 20f, false);
            }
        }
    }
}
