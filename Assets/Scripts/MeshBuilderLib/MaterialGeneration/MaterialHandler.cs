using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshBuilderLib
{
    public class MaterialHandler : MonoBehaviour
    {
        [Header("Debug")]
        public Material DefaultMaterial;

        [Header("Wood")]
        public Material WoodSiding;

        [Header("Plaster")]
        public Material Stucco;
        public Material PaintedPlaster;

        [Header("Brick")]
        public Material RoughBrick;

        [Header("Concrete")]
        public Material RoughConcrete;
        public Material BunkerConcrete6;

        [Header("Fabric")]
        public Material Carpet;
        public Material CarpetTiles;

        [Header("Ceiling")]
        public Material OfficeCeiling;

        private static MaterialHandler _Singleton;
        public static MaterialHandler Singleton => _Singleton;

        public void Start()
        {
            _Singleton = GameObject.Find("MaterialHandler").GetComponent<MaterialHandler>();
        }

        public Material GetRandomFloorMaterial()
        {
            List<Material> candidates = new List<Material>()
        {
            WoodSiding,
            Carpet,
            RoughConcrete,
            CarpetTiles
        };
            return candidates[Random.Range(0, candidates.Count)];
        }

        public Material GetRandomWallMaterial()
        {
            List<Material> candidates = new List<Material>()
        {
            Stucco,
            RoughBrick,
            BunkerConcrete6,
            RoughConcrete,
            PaintedPlaster
        };
            return candidates[Random.Range(0, candidates.Count)];
        }

        public Material GetRandomCeilingMaterial()
        {
            List<Material> candidates = new List<Material>()
        {
            Stucco,
            WoodSiding,
            RoughConcrete,
            OfficeCeiling
        };
            return candidates[Random.Range(0, candidates.Count)];
        }
    }
}
