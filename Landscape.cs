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
        private int WORLD_SIZE = 9; // 2^n + 1, where n = 3
        private float[,] heightmap;

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
                Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f),
                World = Matrix.Identity
            };

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
            this.game = game;
        }

        public override void Update(GameTime gameTime)
        {
            // Rotate the cube.
            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            //basicEffect.World = Matrix.RotationX(time) * Matrix.RotationY(time * 2.0f) * Matrix.RotationZ(time * .7f);
            basicEffect.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);
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

        // Build vertex list using values from the heightmap
        private VertexPositionNormalColor[] BuildVertexArray()
        {
            Vector3 normalVector = new Vector3(0.0f, 0.0f, 0.0f);
            int sideLength = WORLD_SIZE - 1;
            VertexPositionNormalColor[] vertexList = new VertexPositionNormalColor[WORLD_SIZE * WORLD_SIZE];
            for (int x = 0; x < WORLD_SIZE; x++)
            {
                float realX = x - sideLength / 2f;
                for (int z = 0; z < WORLD_SIZE; z++)
                {
                    float realZ = z - sideLength / 2f;
                    vertexList[x * WORLD_SIZE + z] = new VertexPositionNormalColor(
                        new Vector3(realX, heightmap[x, z], realZ), normalVector, GetColor(heightmap[x, z]));
                }
            }
            return vertexList;
        }

        // Chooses the color of a vertex at a particular height
        private Color GetColor(float height)
        {
            return Color.Black;
        }
        // Populate a 2D array with values by running the Diamond Square Algorithm
        private void DiamondSquareGenerator()
        {
            heightmap = new float[WORLD_SIZE, WORLD_SIZE];
            float range = 50.0f;
            Random generator = new Random();
            heightmap[0, 0] = heightmap[0, WORLD_SIZE - 1] =
                heightmap[WORLD_SIZE - 1, 0] = heightmap[WORLD_SIZE - 1, WORLD_SIZE - 1] =
                generator.NextFloat(-range, range);
            // This for loop decreases the size of each square or diamond by 2 on each iteration,
            // simulating recursion.
            for (int sideLength = WORLD_SIZE - 1; sideLength > 1; sideLength /= 2, range /= 2)
            {
                // The diamond step of the algorithm. (x, y) is the top left of the square.
                for (int x = 0; x < WORLD_SIZE - 1; x += sideLength)
                {
                    for (int y = 0; y < WORLD_SIZE - 1; y += sideLength)
                    {
                        float average = (heightmap[x, y] + heightmap[x + sideLength, y] +
                                            heightmap[x, y + sideLength] +
                                            heightmap[x + sideLength, y + sideLength]) / 4.0f;
                        heightmap[x + sideLength / 2, y + sideLength / 2] = average + generator.NextFloat(-range, range);
                    }
                }
                // The square step of the algorithm. (x, y) is the center of the diamond.
                int halfDiagonal = sideLength / 2;
                for (int x = 0; x < WORLD_SIZE - 1; x += halfDiagonal)
                {
                    for (int y = (x + halfDiagonal) % sideLength; y < WORLD_SIZE - 1; y += sideLength /2 )
                    {
                        float average = (heightmap[x, (y - halfDiagonal + WORLD_SIZE - 1) % (WORLD_SIZE - 1)] +
                                         heightmap[(x + halfDiagonal) % (WORLD_SIZE - 1), y] +
                                         heightmap[x, (y + halfDiagonal) % (WORLD_SIZE - 1)] +
                                         heightmap[(x - halfDiagonal + WORLD_SIZE - 1) % (WORLD_SIZE - 1), y]) / 4.0f;
                        average = average + generator.NextFloat(-range, range);
                        heightmap[x, y] = average;
                        if (x == 0)
                        {
                            heightmap[WORLD_SIZE - 1, y] = average;
                        }
                        if (y == 0)
                        {
                            heightmap[x, WORLD_SIZE - 1] = average;
                        }
                    }
                }
            }
        }
    }
}
