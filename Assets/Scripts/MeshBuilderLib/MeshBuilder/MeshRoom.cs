using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshBuilderLib
{
    /// <summary>
    /// A mesh room represents a simple rectangular room, consisting of walls, a foor and a ceiling
    /// </summary>
    public class MeshRoom
    {
        public MeshElement Floor;
        public MeshElement Ceiling;
        public List<MeshPlane> Walls;

        public int FloorSubmeshIndex;
        public int CeilingSubmeshIndex;
        public int WallSubmeshIndex;

        public MeshRoom(MeshElement floor, MeshElement ceiling, List<MeshPlane> walls, int floorSubmeshIndex, int ceilingSubmeshIndex, int wallSubmeshIndex)
        {
            Floor = floor;
            Ceiling = ceiling;
            Walls = walls;
            FloorSubmeshIndex = floorSubmeshIndex;
            CeilingSubmeshIndex = ceilingSubmeshIndex;
            WallSubmeshIndex = wallSubmeshIndex;
        }
    }
}
