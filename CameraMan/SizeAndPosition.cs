using UnityEngine;

namespace CameraMan
{
    public class SizeAndPosition
    {
        public SizeAndPosition()
        {
            Width = 0;
            Height = 0;
            X = 0;
            Y = 0;
        }

        internal SizeAndPosition(string value)
        {
            var values = value.Split(',');
            if (values.Length != 4)
            {
                Jotunn.Logger.LogWarning($"Invalid value for SizeAndPosition: {value}");

                Width = 0;
                Height = 0;
                X = 0;
                Y = 0;
                return;
            }

            Width = float.Parse(values[0]);
            Height = float.Parse(values[1]);
            X = float.Parse(values[2]);
            Y = float.Parse(values[3]);
        }

        public override string ToString()
        {
            return $"{Width},{Height},{X},{Y}";
        }

        public float Y { get; set; }

        public float X { get; set; }

        public float Height { get; set; }

        public float Width { get; set; }

        public Vector2 Position => new Vector2(X, Y);

        public Vector2 Size => new Vector2(Width, Height);
    }
}