// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//authors: Kyra & Reese
using System;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;

    public class Project1Game : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private int worldSize;
        private Landscape model;
        private Sun sun;
        // camera movement variables
        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private KeyboardState keyboardState;
        private MouseState mouseState;
        private float mouseVelocity;
        private float moveVelocity;
        private float pitch;
        private float yaw;
        private float roll;
        private Vector3 eye;


        /// <summary>
        /// Initializes a new instance of the <see cref="Project1Game" /> class.
        /// </summary>
        public Project1Game()
        {
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            keyboardManager = new KeyboardManager(this);
            mouseManager = new MouseManager(this);
            worldSize = 129;
            pitch = -0.5f;
            yaw = 0f;
            roll = 0f;
            mouseVelocity = 0.05f;
            moveVelocity = 0.1f;
            eye = new Vector3(0f, 50, 0f);
            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            float x = worldSize/2f;
            float y = 0f;
            float z = (float)Math.PI;
            float rotationSpeed = 0.01f;
            float sideLength = 5f;
            model = new Landscape(this, rotationSpeed, worldSize);
            sun = new Sun(this, x, y, z, sideLength, rotationSpeed, worldSize);

            // Create an input layout from the vertices

            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "Project 1";

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            Matrix view = UpdateViewMatrix();

            model.Update(gameTime, Matrix.Identity, view);
            sun.Update(gameTime, Matrix.Identity, view);

            // Handle base.Update
            base.Update(gameTime);

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.Black);
            // Uncomment the following four lines to generate a wireframe image.
            //SharpDX.Direct3D11.RasterizerStateDescription rasterizer = SharpDX.Direct3D11.RasterizerStateDescription.Default();
            //rasterizer.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
            //RasterizerState rasterizerState = RasterizerState.New(this.GraphicsDevice, rasterizer);
            //GraphicsDevice.SetRasterizerState(rasterizerState);

            model.Draw(gameTime);
            sun.Draw(gameTime);

            // Handle base.Draw
            base.Draw(gameTime);
        }

        private Matrix UpdateViewMatrix()
        {
            keyboardState = keyboardManager.GetState();
            mouseState = mouseManager.GetState();
            float yawDx = 0.5f - mouseState.X;
            float pitchDy = 0.5f - mouseState.Y;
            yaw -= yawDx * mouseVelocity * gameTime.ElapsedGameTime.Milliseconds;
            pitch += pitchDy * mouseVelocity * gameTime.ElapsedGameTime.Milliseconds;
            if (keyboardState.IsKeyDown(Keys.Q))
            {
                roll += moveVelocity * gameTime.ElapsedGameTime.Milliseconds * .05f;
            }
            if (keyboardState.IsKeyDown(Keys.E))
            {
                roll -= moveVelocity * gameTime.ElapsedGameTime.Milliseconds * .05f;
            }
            Vector3 direction = new Vector3(
                (float)(Math.Cos(pitch) * Math.Sin(yaw)),
                (float)(Math.Sin(pitch)),
                (float)(Math.Cos(pitch) * Math.Cos(yaw)));
            Vector3 xAxis = new Vector3(
                (float)(Math.Sin(yaw + Math.PI / 2f) * Math.Cos(roll)),
                (float)Math.Sin(roll), (float)(Math.Cos(yaw + Math.PI / 2f) * Math.Cos(roll)));
            Vector3 up = Vector3.Cross(direction, xAxis);
            if (keyboardState.IsKeyDown(Keys.A))
            {
                Vector3 eyeChange = eye - moveVelocity * gameTime.ElapsedGameTime.Milliseconds * xAxis;
                if (model.AllowMovement(eyeChange))
                {
                eye = eyeChange;
                }
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                Vector3 eyeChange = eye + moveVelocity * gameTime.ElapsedGameTime.Milliseconds * xAxis;
                if (model.AllowMovement(eyeChange))
                {
                    eye = eyeChange;
                }
            }
            if (keyboardState.IsKeyDown(Keys.W))
            {
                Vector3 eyeChange = eye + moveVelocity * gameTime.ElapsedGameTime.Milliseconds * direction;
                if (model.AllowMovement(eyeChange))
                {
                    eye = eyeChange;
                }
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                Vector3 eyeChange = eye - moveVelocity * gameTime.ElapsedGameTime.Milliseconds * direction;
                if (model.AllowMovement(eyeChange))
                {
                    eye = eyeChange;
                }
            }
            mouseManager.SetPosition(new Vector2(0.5f, 0.5f));
            return Matrix.LookAtLH(eye, eye + direction, up);
        }
    }
}
