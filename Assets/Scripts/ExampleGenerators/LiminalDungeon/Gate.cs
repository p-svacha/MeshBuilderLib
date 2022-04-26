using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiminalDungeonGeneration
{
    /// <summary>
    /// A gate is the very small section between two modules.
    /// </summary>
    public class Gate : MonoBehaviour
    {
        public ExitPoint ExitPoint1;
        public ExitPoint ExitPoint2;

        public void Init(ExitPoint exitPoint1, ExitPoint exitPoint2)
        {
            ExitPoint1 = exitPoint1;
            ExitPoint2 = exitPoint2;
        }
    }
}
