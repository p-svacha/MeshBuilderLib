using MeshBuilderLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiminalDungeonGeneration
{
    /// <summary>
    /// A dungeon module is a room, corridor or junction. It is represented by a polygon with exit points that can connect to other dungeon modules.
    /// </summary>
    public class DungeonModule : MonoBehaviour
    {
        public Polygon Bounds;
        public float Height;
        public List<ExitPoint> ExitPoints = new List<ExitPoint>();

        // Mesh
        private MeshBuilder MeshBuilder;
        private int WallSubmeshIndex; // The index of the wall submesh

        public void Init(Polygon bounds, float height, List<ExitPoint> exitPoints, MeshBuilder meshBuilder, int wallSubmeshIndex)
        {
            Bounds = bounds;
            Height = height;
            ExitPoints = exitPoints;
            foreach (ExitPoint exitPoint in ExitPoints) exitPoint.Module = this;

            MeshBuilder = meshBuilder;
            WallSubmeshIndex = wallSubmeshIndex;
        }

        /// <summary>
        /// Changes the mesh of the module so that an exitpoints opens.
        /// </summary>
        public void OpenExitPoint(ExitPoint exitPoint)
        {
            if (!ExitPoints.Contains(exitPoint)) throw new System.Exception("Can't open an exit point that is part of another module.");
            MeshBuilder.CarveHoleInPlane(WallSubmeshIndex, 
                exitPoint.Wall, 
                new Vector2(exitPoint.RelativeWallPosition * exitPoint.WallLength, exitPoint.GetLocalHeight() + LiminalDungeonGenerator.CONNECTION_HEIGHT / 2), 
                new Vector2(LiminalDungeonGenerator.CONNECTION_WIDTH, LiminalDungeonGenerator.CONNECTION_HEIGHT)
                );
        }
    }
}
