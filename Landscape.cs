using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    class Landscape : ColoredGameObject
    {
        private int worldSize = 129; // 2^n + 1, where n = 7
        private float[,] heightMap;

        public Landscape(Game game)
        {
            DiamondSquareGenerator();
            
            VertexPositionNormalColor[] vertexList = BuildVertexArray();
            
            vertices = Buffer.Vertex.New(
                                    game.GraphicsDevice, vertexList);

            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY),
                Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f,
                    (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f),
                World = Matrix.Identity
            };

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
            this.game = game;
        }

        public override void Update(GameTime gameTime, Matrix world, Matrix view)
        {
            // Rotate the cube.
            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f,
                (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);
        }

        public override void Draw(GameTime gameTime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw the landscape
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }

        // Build vertex list using values from the heightmap
        private VertexPositionNormalColor[] BuildVertexArray()
        {
            Vector3 normalVector = new Vector3(0.0f, 0.0f, 0.0f);
            List<VertexPositionNormalColor> vertices = new List<VertexPositionNormalColor>();
            int sideLength = worldSize - 1;
            for (int x = 0; x < sideLength; x++)
            {
                float realX = x - sideLength / 2f;
                for (int z = 0; z < sideLength; z++)
                {
                    float realZ = z - sideLength / 2f;
                    vertices.Add(new VertexPositionNormalColor(
                        new Vector3(realX, heightMap[x, z], realZ), normalVector, GetColor(heightMap[x, z])));
                    vertices.Add(new VertexPositionNormalColor(
                        new Vector3(realX, heightMap[x, z + 1], realZ + 1), normalVector, GetColor(heightMap[x, z + 1])));
                    vertices.Add(new VertexPositionNormalColor(
                        new Vector3(realX + 1f, heightMap[x + 1, z + 1], realZ + 1f), normalVector, GetColor(heightMap[x + 1, z + 1])));
                    vertices.Add(new VertexPositionNormalColor(
                        new Vector3(realX + 1f, heightMap[x + 1, z], realZ), normalVector, GetColor(heightMap[x + 1, z])));
                    vertices.Add(new VertexPositionNormalColor(
                        new Vector3(realX, heightMap[x, z], realZ), normalVector, GetColor(heightMap[x, z])));
                    vertices.Add(new VertexPositionNormalColor(
                        new Vector3(realX + 1, heightMap[x + 1, z + 1], realZ + 1), normalVector, GetColor(heightMap[x + 1, z + 1])));

                }
            }

            // TODO: Calculate normal vectors and normalize them
           
            return vertices.ToArray();
        }

        // Chooses the color of a vertex at a particular height
        private Color GetColor(float height)
        {
            if (height > 20f)
            {    
                return Color.Snow;
            } 
            if (height > 15f)
            {
                return Color.SlateGray;
            }
            if (height > 0f)
            {
                return Color.ForestGreen;
            }
            if (height > -10f)
            {
                return Color.SandyBrown;
            }
            return Color.Blue;
        }

        // Populate a 2D array with values by running the Diamond Square Algorithm
        private void DiamondSquareGenerator()
        {

            heightMap = new float[worldSize, worldSize];
            float range = 30.0f;
            Random generator = new Random();
            heightMap[0, 0] = heightMap[0, worldSize - 1] =
                heightMap[worldSize - 1, 0] = heightMap[worldSize - 1, worldSize - 1] =
                generator.NextFloat(-10f, 10f);
            // This for loop decreases the size of each square or diamond by 2 on each iteration,
            // as well as the range, simulating recursion.
            for (int sideLength = worldSize - 1; sideLength > 1; sideLength /= 2, range /= 2)
            {
                // The diamond step of the algorithm. (x, y) is the top left of the square.
                for (int x = 0; x < worldSize - 1; x += sideLength)
                {
                    for (int y = 0; y < worldSize - 1; y += sideLength)
                    {
                        float average = (heightMap[x, y] + heightMap[x + sideLength, y] +
                                            heightMap[x, y + sideLength] +
                                            heightMap[x + sideLength, y + sideLength]) / 4.0f;
                        heightMap[x + sideLength / 2, y + sideLength / 2] = average + generator.NextFloat(-range, range);
                    }
                }
                // The square step of the algorithm. (x, y) is the center of the diamond.
                int halfDiagonal = sideLength / 2;
                for (int x = 0; x < worldSize - 1; x += halfDiagonal)
                {
                    for (int y = (x + halfDiagonal) % (sideLength); y < worldSize - 1; y += sideLength)
                    {
                        float average = (heightMap[x, (y - halfDiagonal + worldSize - 1) % (worldSize - 1)] +
                                         heightMap[(x + halfDiagonal) % (worldSize - 1), y] +
                                         heightMap[x, (y + halfDiagonal) % (worldSize - 1)] +
                                         heightMap[(x - halfDiagonal + worldSize - 1) % (worldSize - 1), y]) / 4.0f;
                        average = average + generator.NextFloat(-range, range);
                        heightMap[x, y] = average;
                        if (x == 0)
                        {
                            heightMap[worldSize - 1, y] = average;
                        }
                        if (y == 0)
                        {
                            heightMap[x, worldSize - 1] = average;
                        }
                    }
                }
            }
        }

    }
}
