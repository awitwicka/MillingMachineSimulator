using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Windows.Storage;

namespace MillingMachineSimulator
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MillingMachine : Game
    {
        private GraphicsDeviceManager graphics;

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

        //frez movement
        public bool IsWorking = false;
        private List<Vector3> positions;
        private int StepCounter = 0;
        private Vector3 PositionBegin;
        private Vector3 PositionEnd;

        public void StartMilling()
        {
            if (FileHelper.IsFileLoaded)
            {
                PositionEnd = FileHelper.ReadNextLine(Brick.Resolution);
                IsWorking = true;
            }
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
            if (this.IsActive)
            {
                MouseState mouse = Mouse.GetState();
                if (moveMode)
                {
                    CameraArc.Rotation += MathHelper.ToRadians((mouse.X - screenCenter.X) / 2f);
                    CameraArc.Elevation += MathHelper.ToRadians((mouse.Y - screenCenter.Y) / 2f);
                    //CameraArc.Rotation += MathHelper.ToRadians(1);
                    //CameraArc.Elevation += MathHelper.ToRadians(1);
                    Mouse.SetPosition(screenCenter.X, screenCenter.Y);
                }
                if (mouse.RightButton == ButtonState.Pressed)
                {
                    if (!moveMode && previousMouse.RightButton == ButtonState.Released)
                    {
                        if (graphics.GraphicsDevice.Viewport.Bounds.Contains(new Point(mouse.X, mouse.Y)))
                        {
                            moveMode = true;
                            saveMousePoint.X = mouse.X;
                            saveMousePoint.Y = mouse.Y;
                            Mouse.SetPosition(screenCenter.X, screenCenter.Y);
                            this.IsMouseVisible = false;
                        }
                    }
                }
                else
                {
                    if (moveMode)
                    {
                        moveMode = false;
                        Mouse.SetPosition(saveMousePoint.X, saveMousePoint.Y);
                        this.IsMouseVisible = true;
                    }
                }
                if (mouse.ScrollWheelValue - previousMouse.ScrollWheelValue != 0)
                {
                    float wheelChange = mouse.ScrollWheelValue - previousMouse.ScrollWheelValue;
                    CameraArc.ViewDistance -= (wheelChange / 120) * scrollRate;
                }
                previousMouse = mouse;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);
            if (IsWorking) //i iz doing animation
            {
                if (positions == null || positions.Count == StepCounter)
                {
                    PositionBegin = PositionEnd;
                    PositionEnd = FileHelper.ReadNextLine(Brick.Resolution);
                    positions = FileHelper.GetPositions(PositionBegin, PositionEnd, Brick.Resolution);
                    StepCounter = 0;
                }
                if (positions.Count != 0) //WOOOOT?
                {
                    Brick.MoveFrezStep(positions[StepCounter], FileHelper.Diameter, FileHelper.Frez);
                    StepCounter++;
                }
                Brick.MoveFrez(positions, FileHelper.Diameter, FileHelper.Frez); 
            }
            Brick.Draw(CameraArc, Effect);
            base.Draw(gameTime);
        }
    }
}
