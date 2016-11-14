using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MillingMachineSimulator
{
    public class SquarePrimitive : GeometricPrimitive
    {
        public SquarePrimitive(GraphicsDevice graphicsDevice, float Height, float Width)
        {
            AddVertex(new Vector3(-1 * Width / 2, 0, -1 * Height / 2), Vector3.Down);
            AddVertex(new Vector3(-1 * Width / 2, 0, 1 * Height / 2), Vector3.Down);
            AddVertex(new Vector3(1 * Width / 2, 0, 1 * Height / 2), Vector3.Down);
            AddVertex(new Vector3(1 * Width / 2, 0, -1 * Height / 2), Vector3.Down);

            AddIndex(0);
            AddIndex(1);
            AddIndex(2);
            AddIndex(0);
            AddIndex(2);
            AddIndex(3);

            InitializePrimitive(graphicsDevice);
        }
    }
}
