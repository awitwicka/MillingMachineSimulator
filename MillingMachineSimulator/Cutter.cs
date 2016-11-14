using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillingMachineSimulator
{
    public class Cutter
    {
        GraphicsDevice device;
        private SpherePrimitive primitive;

        public Vector3 Position;
        public FileHelper.FrezType Type = FileHelper.FrezType.K;
        float diameter;
        public float Diameter{ get { return diameter; } set { ChangeCutterDiameter(value); } }

        public Cutter(GraphicsDevice graphics, float diameter)
        {
            primitive = new SpherePrimitive(graphics, diameter, 10);
            this.diameter = diameter;
            device = graphics;
        }

        private void ChangeCutterDiameter(float newVal)
        {
            if (diameter != newVal) {
                primitive = new SpherePrimitive(device, newVal, 10);
                diameter = newVal;
             }
        }

        public void Draw(ArcBallCamera camera, BasicEffect effect)
        {

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                //make origin point of a sphere-like frez at the bottom instead of middle
                Vector3 pos = Position;
                if (Type == FileHelper.FrezType.K)
                    pos.Y += diameter/2.0f;

                pass.Apply();
                primitive.Draw(Matrix.CreateTranslation(pos), camera.View, camera.Projection, Color.Gray);
            }
        }

    }
}
