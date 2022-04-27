using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorGeneration
{
    public class SpheresGenerationSettings
    {
        private const float MIN_WIDTH = 5f;
        private const float MAX_WIDTH = 10f;

        private const float MIN_HEIGHT = 5f;
        private const float MAX_HEIGHT = 10f;

        private const int MIN_ROWS = 15;
        private const int MAX_ROWS = 35;

        private const int MIN_COLS = 40;
        private const int MAX_COLS = 60;

        private const int MIN_SPHERES = 2;
        private const int MAX_SPHERES = 5;

        public float Width;
        public float Height;
        public int Rows;
        public int Cols;
        public Vector3 Position;

        public SpheresGenerationSettings(float width, float height, int rows, int cols, Vector3 position)
        {
            Width = width;
            Height = height;
            Rows = rows;
            Cols = cols;
            Position = position;
        }

        public static SpheresGenerationSettings GetRandomSettings()
        {
            float width = Random.Range(MIN_WIDTH, MAX_WIDTH);
            float height = Random.Range(MIN_HEIGHT, MAX_HEIGHT);
            int rows = Random.Range(MIN_ROWS, MAX_ROWS + 1);
            int cols = Random.Range(MIN_COLS, MAX_COLS + 1);
            Vector3 position = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            return new SpheresGenerationSettings(width, height, rows, cols, position);
        }
    }
}
