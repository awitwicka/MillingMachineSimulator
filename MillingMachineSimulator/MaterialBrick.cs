using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MillingMachineSimulator 
{
    class MaterialBrick
    {
        GraphicsDevice device;
         
        public int Length = 150;
        public int Width = 250;
        public int Heigh = 50; //43
        public int Resolution = 1; //set max
        public float Unit = 0.5f;
        private int VertexOffset;
        //private VertexPositionNormalTexture[] BrickVertices;

        //public List<float>[,] VerticesRef;
        private VertexPositionColor[] Vertices;
        //public VertexPositionColor[] Vertices {
        //    get { return vertices; }
        //    set { vertices = value; VerticesRef[] }
        //}
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

        public MaterialBrick(GraphicsDevice device)
        {
            this.device = device;
            Length *= Resolution;
            Width *= Resolution;
            Heigh *= Resolution;
            Unit /= Resolution;
            VertexOffset = (Length+1) * (Width / 2) + (Length+1)/2;
            Vertices = new VertexPositionColor[(Length + 1) * (Width + 1)];
            InitializeBrick(Length, Width, Heigh, Unit);
        }

        public void MoveFrez(List<Vector3> positions)
        {
            //List<Vector3> positions = PathData.GetPositions(begin, end, Unit);
            foreach (var v in positions)
            {
                MoveFrezStep(v);
            }
        }

        public void MoveFrezStep(Vector3 v)
        {
            int r = 4;
            r *= Resolution;
            
            int index;
            for (float y = (v.Z - r); y <= (v.Z + r); y++) //do gory zaokraglić
            {
                for (float x = (v.X - r); x <= (v.X + r); x++)
                {
                    index = (int)((int)(y) * (Length + 1) + x) + VertexOffset; //TOdO: warunek glebiej niz na r 
                    if (index >= 0)
                    {
                        //bool freezable = ((x - v.X) * (x - v.X)) + ((y - v.Z) * (y - v.Z)) + ((Vertices[index].Position.Y / Unit - v.Y) * (Vertices[index].Position.Y / Unit - v.Y)) <= (r * r);
                        var sphereZ = ((float)-Math.Sqrt((r * r) - (x - v.X) * (x - v.X) - (y - v.Z) * (y - v.Z)) + v.Y);
                        bool freezable = ((x - v.X) * (x - v.X)) + ((y - v.Z) * (y - v.Z)) <= (r * r) &&
                            Vertices[index].Position.Y/Unit > sphereZ;
                        if (freezable)
                            Vertices[index].Position.Y = Unit * sphereZ; //TODO change later, upper bound
                    }
                }
            }
        }

        public void Draw(ArcBallCamera camera, BasicEffect effect)
        {
            //effect.EnableDefaultLighting();
            effect.VertexColorEnabled = true;
            effect.World = Matrix.Identity;
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            //effect.LightingEnabled = true;
            //effect.DirectionalLight0.DiffuseColor = Color.White.ToVector3(); // a red light
            //effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);  // coming along the x-axis/pnpphj
            //effect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0); // with green highlights

            //RasterizerState originalState = GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.FillMode = FillMode.WireFrame;
            rasterizerState.CullMode = CullMode.None; //how works?
            device.RasterizerState = rasterizerState;

            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                vertexBuffer.SetData<VertexPositionColor>(Vertices);
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (Length+1)*(Width+1), 0, 2*Length*Width);
                //MoveFrez();
            }
        }
        public void InitializeBrick(int length, int width, int heigh, float unit)
        {
            int count = 0;
            short[] indices = new short[6*length*width];
            for (int y = 0; y <= width; y++)
            {
                for (int x = 0; x <= length; x++)
                {
                    Vertices[count] = new VertexPositionColor(new Vector3(x*unit - (length*unit/2), heigh*unit, y* unit - (width * unit / 2)), Color.DarkBlue);
                    //VerticesRef[x, y].Add(heigh*unit);
                    count++;
                }
            }
            count = 0;
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    indices[count * 6] = (short)(y * (length + 1) + x);
                    indices[1 + (count * 6)] = (short)((y + 1) * (length + 1) + x);
                    indices[2 + (count * 6)] = (short)(y * (length + 1) + x + 1);
                    indices[3 + (count * 6)] = (short)((y + 1) * (length + 1) + x);
                    indices[4 + (count * 6)] = (short)((y + 1) * (length + 1) + x + 1);
                    indices[5 + (count * 6)] = (short)(y * (length + 1) + x + 1);
                    count++;
                }
            }

            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionColor), (Length + 1) * (Width + 1), BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(Vertices);

            indexBuffer = new IndexBuffer(device, typeof(short), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }

    }
}
