using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;

    class Sun : ColoredGameObject
    {
        float x;
        float yAngle;
        float zAngle;
        float sideLength;
        float radius;
        public Sun(Game game, float x, float yAngle, float zAngle, float sideLength, float rotationSpeed, float radius)
        {
            this.rotationSpeed = rotationSpeed;
            this.x = x;
            this.yAngle = yAngle;
            this.zAngle = zAngle;
            this.sideLength = sideLength;
            this.radius = radius;
            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                LightingEnabled = false,
                PreferPerPixelLighting = true,
                View = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY),
                Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f,
                    (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 1000.0f),
                World = Matrix.Identity
            };
            this.game = game;
            BuildVertices();
        }

        public override void Update(GameTime gameTime, Matrix world, Matrix view)
        {
            
            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f,
                (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);
            yAngle += rotationSpeed;
            zAngle += rotationSpeed;
            BuildVertices();
        }

        public override void Draw(GameTime gameTime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw the rotating cube
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }

        private void BuildVertices()
        {
            float y = radius/2f * (float)Math.Sin(yAngle);
            float z = radius * (float)Math.Cos(zAngle);
            Color color = Color.Yellow;
            Vector3 frontBottomLeft = new Vector3(x, y, z);
            Vector3 frontTopLeft = new Vector3(x, y + sideLength, z);
                Vector3 frontTopRight = new Vector3(x + sideLength, y + sideLength, z);
                Vector3 frontBottomRight = new Vector3(x + sideLength, y, z);
                Vector3 backBottomLeft = new Vector3(x, y, z + sideLength);
                Vector3 backBottomRight = new Vector3(x + sideLength, y, z + sideLength);
                Vector3 backTopLeft = new Vector3(x, y + sideLength, z + sideLength);
                Vector3 backTopRight = new Vector3(x + sideLength, y + sideLength, z + sideLength);

                Vector3 frontBottomLeftNormal = new Vector3(-0.333f, -0.333f, -0.333f);
                Vector3 frontTopLeftNormal = new Vector3(-0.333f, 0.333f, -0.333f);
                Vector3 frontTopRightNormal = new Vector3(0.333f, 0.333f, -0.333f);
                Vector3 frontBottomRightNormal = new Vector3(0.333f, -0.333f, -0.333f);
                Vector3 backBottomLeftNormal = new Vector3(-0.333f, -0.333f, 0.333f);
                Vector3 backBottomRightNormal = new Vector3(0.333f, -0.333f, 0.333f);
                Vector3 backTopLeftNormal = new Vector3(-0.333f, 0.333f, 0.333f);
                Vector3 backTopRightNormal = new Vector3(0.333f, 0.333f, 0.333f);

                vertices = Buffer.Vertex.New(
                                game.GraphicsDevice,
                                new[]
                        {
                        new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, color), // Front
                        new VertexPositionNormalColor(frontTopLeft, frontTopLeftNormal, color),
                        new VertexPositionNormalColor(frontTopRight, frontTopRightNormal, color),
                        new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, color),
                        new VertexPositionNormalColor(frontTopRight, frontTopRightNormal, color),
                        new VertexPositionNormalColor(frontBottomRight, frontBottomRightNormal, color),
                        new VertexPositionNormalColor(backBottomLeft, backBottomLeftNormal, color), // BACK
                        new VertexPositionNormalColor(backTopRight, backTopRightNormal, color),
                        new VertexPositionNormalColor(backTopLeft, backTopLeftNormal, color),
                        new VertexPositionNormalColor(backBottomLeft, backBottomLeftNormal, color),
                        new VertexPositionNormalColor(backBottomRight, backBottomRightNormal, color),
                        new VertexPositionNormalColor(backTopRight, backTopRightNormal, color),
                        new VertexPositionNormalColor(frontTopLeft, frontTopLeftNormal, color), // Top
                        new VertexPositionNormalColor(backTopLeft, backTopLeftNormal, color),
                        new VertexPositionNormalColor(backTopRight, backTopRightNormal, color),
                        new VertexPositionNormalColor(frontTopLeft, frontTopLeftNormal, color),
                        new VertexPositionNormalColor(backTopRight, backTopRightNormal, color),
                        new VertexPositionNormalColor(frontTopRight, frontTopRightNormal, color),
                        new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, color), // Bottom
                        new VertexPositionNormalColor(backBottomRight, backBottomRightNormal, color),
                        new VertexPositionNormalColor(backBottomLeft, backBottomLeftNormal, color),
                        new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, color),
                        new VertexPositionNormalColor(frontBottomRight, frontBottomRightNormal, color),
                        new VertexPositionNormalColor(backBottomRight, backBottomRightNormal, color),
                        new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, color), // Left
                        new VertexPositionNormalColor(backBottomLeft, backBottomLeftNormal, color),
                        new VertexPositionNormalColor(backTopLeft, backTopLeftNormal, color),
                        new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, color),
                        new VertexPositionNormalColor(backTopLeft, backTopLeftNormal, color),
                        new VertexPositionNormalColor(frontTopLeft, frontTopLeftNormal, color),
                        new VertexPositionNormalColor(frontBottomRight, frontBottomRightNormal, color), // Right
                        new VertexPositionNormalColor(backTopRight, backTopRightNormal, color),
                        new VertexPositionNormalColor(backBottomRight, backBottomRightNormal, color),
                        new VertexPositionNormalColor(frontBottomRight, frontBottomRightNormal, color),
                        new VertexPositionNormalColor(frontTopRight, frontTopRightNormal, color),
                        new VertexPositionNormalColor(backTopRight, backTopRightNormal, color),
                    });

                inputLayout = VertexInputLayout.FromBuffer(0, vertices);
} 
 
    }   
}
