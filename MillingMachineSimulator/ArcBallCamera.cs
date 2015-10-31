using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MillingMachineSimulator
{
    public class ArcBallCamera
    {
        #region Fields
        private Vector3 cameraPosition = Vector3.Zero;
        private Vector3 targetPosition = Vector3.Zero;
        private float elevation;
        private float rotation;
        private float minDistance;
        private float maxDistance;
        private float viewDistance = 12f;
        private Vector3 baseCameraReference = new Vector3(0, 0, 1);
        private bool needViewResync = true;
        private Matrix cachedViewMatrix;
        #endregion
        #region Properties
        public Matrix View
        {
            get
            {
                if (needViewResync)
                {
                    Matrix transformMatrix = Matrix.CreateFromYawPitchRoll(rotation,elevation,0f);
                    cameraPosition = Vector3.Transform(baseCameraReference,transformMatrix);
                    cameraPosition *= viewDistance;
                    cameraPosition += targetPosition;
                    cachedViewMatrix = Matrix.CreateLookAt(cameraPosition,targetPosition,Vector3.Up);
                }
                return cachedViewMatrix;
            }
        }
        
        public Matrix Projection { get; private set; }
        public Vector3 Target
        {
            get { return targetPosition; }
            set
            {
                targetPosition = value;
                needViewResync = true;
            }
        }
        public Vector3 Position
        {
            get
            {
                return cameraPosition;
            }
        }
        public float Elevation
        {
            get { return elevation; }
            set
            {
                elevation = MathHelper.Clamp(
                value,
                MathHelper.ToRadians(-70),
                MathHelper.ToRadians(-10));
                needViewResync = true;
            }
        }
        public float Rotation
        {
            get { return rotation; }
            set
            {
                rotation = MathHelper.WrapAngle(value);
                needViewResync = true;
            }
        }
        public float ViewDistance
        {
            get { return viewDistance; }
            set
            {
                viewDistance = MathHelper.Clamp(
                value,
                minDistance,
                maxDistance);
            }
        }
        #endregion
        #region Constructor
        public ArcBallCamera(
        Vector3 targetPosition,
        float initialElevation,
        float initialRotation,
        float minDistance,
        float maxDistance,
        float initialDistance,
        float aspectRatio,
        float nearClip,
        float farClip)
        {
            Target = targetPosition;
            Elevation = initialElevation;
            Rotation = initialRotation;
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
            ViewDistance = initialDistance;
            Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            aspectRatio,
            nearClip,
            farClip);
            needViewResync = true;
        }
        #endregion
    }
}
