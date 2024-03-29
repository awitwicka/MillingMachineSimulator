﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows;
using Windows.UI.Popups;

namespace MillingMachineSimulator
{
    public class MaterialBrick
    {
        GraphicsDevice device;
        public delegate void CritErrorHandler(object sender, EventArgs e);
        public event CritErrorHandler CritError;

        public int Length = 150; //150
        public int Width = 250; //250
        public int Heigh = 50; //50
        public int Resolution = 2; //set max
        public float Unit = 0.5f;
        private int VertexOffset;
        private int offsetX;
        private int offsetY;

        //dusplay mode
        public bool IsWireframeOn = false;

        private VertexPositionColorNormal[] Vertices;
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

        public MaterialBrick(GraphicsDevice device)
        {
            this.device = device;
            Length *= Resolution;
            Width *= Resolution;
            Heigh *= Resolution;
            Unit /= Resolution;
            Vertices = new VertexPositionColorNormal[(Length + 1) * (Width + 1)];
            offsetX = (Length + 1) / 2;
            offsetY = (Width / 2);
            VertexOffset = (Length + 1) * (Width / 2) + (Length + 1) / 2;
            InitializeBrick(Length, Width, Heigh, Unit);
        }

        public void MoveFrez(List<Vector3> positions, int diameter, FileHelper.FrezType frez, double critDepth)
        {
            //List<Vector3> positions = PathData.GetPositions(begin, end, Unit);
            foreach (var v in positions)
            {
                MoveFrezStep(v, diameter, frez, critDepth);
            }
        }

        public async void MoveFrezStep(Vector3 v, int diameter, FileHelper.FrezType frez, double critDepth)
        {
            int r = diameter / 2;
            r *= Resolution;

            //make origin point of a sphere-like frez at the bottom instead of middle
            if (frez == FileHelper.FrezType.K)
                v.Y += r;

            int index;
            float sphereZ = 0;
            for (float y = (v.Z - r); y <= (v.Z + r); y++) //do gory zaokraglić
            {
                for (float x = (v.X - r); x <= (v.X + r); x++)
                {
                    index = (int)((int)(y) * (Length + 1) + x) + VertexOffset; //TOdO: warunek glebiej niz na r 
                    //if (index < (y) * (Length + 1) + x) + VertexOff)
                    if (index >= 0 && x + offsetX > -1 && x + offsetX < (Length+1) && y+offsetY>-1 && y+offsetY<(Width))
                    {
                        Vertices[index].Normal = Vector3.Zero;
                        if (frez == FileHelper.FrezType.K)
                        {
                            //sphereZ = ((float)-Math.Sqrt(Math.Abs((r * r) - (x - v.X) * (x - v.X) - (y - v.Z) * (y - v.Z))) + v.Y);
                            sphereZ = ((float)-Math.Sqrt((r * r) - (x - v.X) * (x - v.X) - (y - v.Z) * (y - v.Z)) + v.Y);
                        }
                        else if (frez == FileHelper.FrezType.F)
                            sphereZ = v.Y;

                        bool freezable = ((x - v.X) * (x - v.X)) + ((y - v.Z) * (y - v.Z)) <= (r * r) && Vertices[index].Position.Y / Unit > sphereZ;
                        if (sphereZ <= critDepth*Resolution)
                        {
                                CritError(this, null);
                                var dialog = new MessageDialog("Milling under critical Level!");
                                await dialog.ShowAsync();
                                goto outer; //TODO SEROIUSLY DO STH WITH IT!
                        }
                        if (freezable)
                            Vertices[index].Position.Y = Unit * sphereZ; //TODO change later, upper bound
                    }
                }
            }
            outer:;
            //calculate normals
            for (float y = (v.Z - r - 1); y <= (v.Z + r + 1); y++) //do gory zaokraglić
            {
                for (float x = (v.X - r - 1); x <= (v.X + r + 1); x++)
                {
                    index = (int)((int)(y) * (Length + 1) + x) + VertexOffset;
                    if (index >= 0 && x + offsetX > -1 && x + offsetX < (Length + 1) && y + offsetY > -1 && y + offsetY < (Width))
                    {
                        RecalculateNormalsForTriangle(index);
                    }
                }
            }
            vertexBuffer.SetData<VertexPositionColorNormal>(Vertices);
        }

        private void RecalculateNormalsForTriangle(int posStart)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 side1 = Vertices[posStart].Position - Vertices[posStart + 1].Position;
                Vector3 side2 = Vertices[posStart].Position - Vertices[(Length) + 1].Position;
                Vector3 normal = Vector3.Cross(side1, side2);
                normal.Normalize();
                Vertices[posStart].Normal += normal;
                Vertices[posStart + (Length + 1)].Normal += normal;
                Vertices[posStart + 1].Normal += normal;
            }
        }

        public void Draw(ArcBallCamera camera, BasicEffect effect)
        {
            effect.EnableDefaultLighting();
            effect.VertexColorEnabled = true;
            effect.World = Matrix.Identity;
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            //effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.DiffuseColor = Color.White.ToVector3(); // a red light
            effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);  // coming along the x-axis/pnpphj
            effect.DirectionalLight0.SpecularColor = Color.White.ToVector3();//new Vector3(0, 1, 0); // with green highlights
            //effect.PreferPerPixelLighting = true;

            //RasterizerState originalState = GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            if (IsWireframeOn)
                rasterizerState.FillMode = FillMode.WireFrame;
            else
                rasterizerState.FillMode = FillMode.Solid;

            device.RasterizerState = rasterizerState;
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer; //CHECK: send only once?

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //vertexBuffer.SetData<VertexPositionColorNormal>(Vertices);
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (Length + 1) * (Width + 1), 0, 2 * Length * Width);
            }
        }
        public void InitializeBrick(int length, int width, int heigh, float unit)
        {
            int count = 0;
            int[] indices = new int[6 * length * width];
            for (int y = 0; y <= width; y++)
            {
                for (int x = 0; x <= length; x++)
                {
                    Vertices[count] = new VertexPositionColorNormal(new Vector3(x * unit - (length * unit / 2), heigh * unit, y * unit - (width * unit / 2)), Color.LightGray, Vector3.Zero);
                    count++;
                }
            }
            count = 0;
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    indices[count * 6] = (int)(y * (length + 1) + x);
                    indices[1 + (count * 6)] = (int)((y + 1) * (length + 1) + x);
                    indices[2 + (count * 6)] = (int)(y * (length + 1) + x + 1);
                    indices[3 + (count * 6)] = (int)((y + 1) * (length + 1) + x);
                    indices[4 + (count * 6)] = (int)((y + 1) * (length + 1) + x + 1);
                    indices[5 + (count * 6)] = (int)(y * (length + 1) + x + 1);

                    RecalculateNormalsForTriangle(y * length + x);
                    count++;
                }
            }
            //TODO: Calculate normalsfor last column of x
            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionColorNormal), (Length + 1) * (Width + 1), BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColorNormal>(Vertices);

            indexBuffer = new IndexBuffer(device, typeof(int), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }

    }
}
