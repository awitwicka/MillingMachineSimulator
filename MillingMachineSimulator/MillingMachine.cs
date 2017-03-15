using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Popups;

namespace MillingMachineSimulator
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MillingMachine : Game
    {
        public bool showPath = false;

        private int count = 0;
        VertexPositionColor[] limitSquareVerts;
        VertexPositionColor[] vertexPositionColors = new VertexPositionColor[100000];
        private GraphicsDeviceManager graphics;

        Vector3 pos = new Vector3(0, 0, 0);
        public SquarePrimitive MillingLimit;
        public Cutter Cutter1;
        public MaterialBrick Brick;
        public FileHelper FileHelper;
        private BasicEffect Effect;
        private ArcBallCamera CameraArc;

        //mouse control
        private Point screenCenter;
        private Point saveMousePoint;
        private bool moveMode = false;
        private float scrollRate = 1.0f;
        private MouseState previousMouse;
        private KeyboardState oldState;

        //milling movement
        public bool IsWorking = false;
        private List<Vector3> positions;
        private int StepCounter = 0;
        private Vector3 PositionBegin;
        private Vector3 PositionEnd;

        //user settable
        private float speed = 50;
        public float Speed { get { return speed; } set { speed = value; } }
        private double criticalMillingDepth = 2.0;
        public double CriticalMillingDepth
        {
            get { return criticalMillingDepth; }
            set
            {
                criticalMillingDepth = value;
                pos.Y = (float)CriticalMillingDepth * Brick.Unit * Brick.Resolution;
                MillingLimit = new SquarePrimitive(graphics.GraphicsDevice, 10, 140);
                /*SetProperty(ref criticalMillingDepth, value);*/
            }
        }

        public void StartMilling()
        {
            if (FileHelper.IsFileLoaded)
            {
                PositionEnd = FileHelper.ReadNextLine(Brick.Resolution);
                tmpPos = new Vector3(PositionEnd.X * Brick.Unit * Brick.Resolution, PositionEnd.Y * Brick.Unit * Brick.Resolution + (FileHelper.Diameter * Brick.Unit * Brick.Resolution / 2), PositionEnd.Z * Brick.Unit * Brick.Resolution);
                vertexPositionColors[count] = new VertexPositionColor(tmpPos, Color.Red);
                count++;
                IsWorking = true;

            }
        }

        public void StopMilling(object sender, EventArgs e)
        {
            IsWorking = false;
        }

        public MillingMachine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            screenCenter.X = this.Window.ClientBounds.Width / 2;
            screenCenter.Y = this.Window.ClientBounds.Height / 2;
            previousMouse = Mouse.GetState();
            Mouse.SetPosition(screenCenter.X, screenCenter.Y);

            Effect = new BasicEffect(graphics.GraphicsDevice);
            CameraArc = new ArcBallCamera(new Vector3(0f, 0f, 0f), MathHelper.ToRadians(-200), 0f, 32f, 300f, 128f, GraphicsDevice.Viewport.AspectRatio, 0.1f, 512f); //rad -30 192
            FileHelper = new FileHelper(graphics.GraphicsDevice);
            Brick = new MaterialBrick(graphics.GraphicsDevice);
            Brick.CritError += new MaterialBrick.CritErrorHandler(StopMilling);
            Cutter1 = new Cutter(graphics.GraphicsDevice, 10);
            MillingLimit = new SquarePrimitive(graphics.GraphicsDevice, 10, 140);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            //if (this.IsActive)
            //{
            //    MouseState mouse = Mouse.GetState();
            //    if (moveMode)
            //    {
            //        CameraArc.Rotation += MathHelper.ToRadians((mouse.X - screenCenter.X) / 2f);
            //        CameraArc.Elevation += MathHelper.ToRadians((mouse.Y - screenCenter.Y) / 2f);
            //        //CameraArc.Rotation += MathHelper.ToRadians(1);
            //        //CameraArc.Elevation += MathHelper.ToRadians(1);
            //        Mouse.SetPosition(screenCenter.X, screenCenter.Y);
            //    }
            //    if (mouse.RightButton == ButtonState.Pressed)
            //    {
            //        if (!moveMode && previousMouse.RightButton == ButtonState.Released)
            //        {
            //            if (graphics.GraphicsDevice.Viewport.Bounds.Contains(new Point(mouse.X, mouse.Y)))
            //            {
            //                moveMode = true;
            //                saveMousePoint.X = mouse.X;
            //                saveMousePoint.Y = mouse.Y;
            //                Mouse.SetPosition(screenCenter.X, screenCenter.Y);
            //                this.IsMouseVisible = false;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (moveMode)
            //        {
            //            moveMode = false;
            //            Mouse.SetPosition(saveMousePoint.X, saveMousePoint.Y);
            //            this.IsMouseVisible = true;
            //        }
            //    }
            //    if (mouse.ScrollWheelValue - previousMouse.ScrollWheelValue != 0)
            //    {
            //        float wheelChange = mouse.ScrollWheelValue - previousMouse.ScrollWheelValue;
            //        CameraArc.ViewDistance -= (wheelChange / 120) * scrollRate;
            //    }
            //    previousMouse = mouse;
            //}
            //base.Update(gameTime);

            MouseState mouse = Mouse.GetState();
            KeyboardState newState = Keyboard.GetState();
            if (newState.IsKeyDown(Keys.W))
            {
                //pressed
                if (!oldState.IsKeyDown(Keys.W))
                {
                    CameraArc.Elevation -= MathHelper.ToRadians(2);
                }
            }
            if (newState.IsKeyDown(Keys.S))
            {
                //pressed
                if (!oldState.IsKeyDown(Keys.S))
                {
                    CameraArc.Elevation += MathHelper.ToRadians(2);
                }
            }
            if (newState.IsKeyDown(Keys.A))
            {
                //pressed
                if (!oldState.IsKeyDown(Keys.A))
                {
                    CameraArc.Rotation -= MathHelper.ToRadians(2);
                }
            }
            if (newState.IsKeyDown(Keys.D))
            {
                //pressed
                if (!oldState.IsKeyDown(Keys.D))
                {
                    CameraArc.Rotation += MathHelper.ToRadians(2);
                }
            }
            if (mouse.ScrollWheelValue - previousMouse.ScrollWheelValue != 0)
            {
                float wheelChange = mouse.ScrollWheelValue - previousMouse.ScrollWheelValue;
                CameraArc.ViewDistance -= (wheelChange / 120) * scrollRate;
            }

            previousMouse = mouse;
            //oldState = newState;
        }

        public void DoFastSimulation()
        {
            if (FileHelper.IsFileLoaded)
            {
                PositionEnd = FileHelper.ReadNextLine(Brick.Resolution);
                tmpPos = new Vector3(PositionEnd.X * Brick.Unit * Brick.Resolution, PositionEnd.Y * Brick.Unit * Brick.Resolution + (FileHelper.Diameter * Brick.Unit * Brick.Resolution / 2), PositionEnd.Z * Brick.Unit * Brick.Resolution);
                vertexPositionColors[count] = new VertexPositionColor(tmpPos, Color.Red);
                count++;
                IsWorking = false;
                while (!FileHelper.reader.EndOfStream)
                {
                    PositionBegin = PositionEnd;
                    PositionEnd = FileHelper.ReadNextLine(Brick.Resolution);
                    tmpPos = new Vector3(PositionEnd.X * Brick.Unit * Brick.Resolution, PositionEnd.Y * Brick.Unit * Brick.Resolution + (FileHelper.Diameter * Brick.Unit * Brick.Resolution / 2), PositionEnd.Z * Brick.Unit * Brick.Resolution);
                    vertexPositionColors[count] = new VertexPositionColor(tmpPos, Color.Red);
                    count++;
                    positions = FileHelper.GetPositions(PositionBegin, PositionEnd, Brick.Resolution);
                    Brick.MoveFrez(positions, FileHelper.Diameter, FileHelper.Frez, CriticalMillingDepth);
                }
                StepCounter = 0;
            }
        }
        Vector3 tmpPos;
        private int TotalTimeElapsed = 0;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);
            var elapsed = gameTime.ElapsedGameTime.Milliseconds;
            if (IsWorking && TotalTimeElapsed >= 1000 / Speed) //i iz doing animation
            {
                TotalTimeElapsed = 0;
                if (positions == null || positions.Count == StepCounter)
                {
                    PositionBegin = PositionEnd;
                    PositionEnd = FileHelper.ReadNextLine(Brick.Resolution);

                    tmpPos = new Vector3(PositionEnd.X*Brick.Unit*Brick.Resolution, PositionEnd.Y * Brick.Unit * Brick.Resolution + (FileHelper.Diameter*Brick.Unit*Brick.Resolution/2), PositionEnd.Z * Brick.Unit * Brick.Resolution);
                    vertexPositionColors[count] = new VertexPositionColor(tmpPos, Color.Red);
                    count++;

                    if (PositionEnd.X == -100 && PositionEnd.Y == -100 && PositionEnd.Z == -100)
                    {
                        PositionEnd = PositionBegin;
                        count--;
                    }
                    positions = FileHelper.GetPositions(PositionBegin, PositionEnd, Brick.Resolution);
                    StepCounter = 0;
                }
                if (positions.Count != 0) //WOOOOT?
                {
                    Brick.MoveFrezStep(positions[StepCounter], FileHelper.Diameter, FileHelper.Frez, CriticalMillingDepth);
                    Cutter1.Position = positions[StepCounter] * Brick.Unit;
                    Cutter1.Diameter = FileHelper.Diameter * (Brick.Unit * Brick.Resolution);
                    Cutter1.Type = FileHelper.Frez;
                    StepCounter++;
                }
            }
            /*if (FileHelper.Frez == FileHelper.FrezType.F)
            {
                if (PositionBegin.Y != PositionEnd.Y)
                {
                    var dialog = new MessageDialog("K-frez cannot move along Y axis");
                    dialog.ShowAsync();
                }
            }*/

            TotalTimeElapsed += elapsed;
            Brick.Draw(CameraArc, Effect);
            Cutter1.Draw(CameraArc, Effect);        
            MillingLimit.Draw(Matrix.CreateTranslation(pos), CameraArc.View, CameraArc.Projection, Color.Red);

            EffectTechnique effectTechnique = Effect.Techniques[0];
            EffectPassCollection effectPassCollection = effectTechnique.Passes;
            foreach (EffectPass pass in effectPassCollection)
            {
                pass.Apply();
                if (showPath && count > 1)
                    GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertexPositionColors, 0, count-1);
            }

            base.Draw(gameTime);
        }
    }


}
