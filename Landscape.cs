﻿using System;
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
        private int worldSize = 129; // 2^n + 1, where n = 3
        private float[,] heightMap;
        private int[] index;

        public Landscape(Game game)
        {
            DiamondSquareGenerator();

            BuildIndicesArray();
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
            indexBuffer = Buffer.Index.New(game.GraphicsDevice, index);
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
            game.GraphicsDevice.SetIndexBuffer(indexBuffer, true);

            // Apply the basic effect technique and draw the landscape
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.DrawIndexed(PrimitiveType.TriangleList, index.Length);
        }

        // Build vertex list using values from the heightmap
        private VertexPositionNormalColor[] BuildVertexArray()
        {
            Vector3 normalVector = new Vector3(0.0f, 0.0f, 0.0f);
            List<VertexPositionNormalColor> vertices = new List<VertexPositionNormalColor>();
            for (int z = 0; z < worldSize; z++)
            {
                float realZ = z - worldSize / 2f;
                for (int x = 0; x < worldSize; x++)
                {
                    float realX = x - worldSize / 2f;
                    vertices.Add(new VertexPositionNormalColor(
                        new Vector3((float)realX, heightMap[x, z], (float)realZ), normalVector, GetColor(heightMap[x, z])));
                }
            }

            // TODO: Calculate normal vectors and normalize them
           
            return vertices.ToArray();
        }

        // Chooses the color of a vertex at a particular height
        private Color GetColor(float height)
        {
            if (height > 50f)
            {    
                return Color.Snow;
            } 
            if (height > 30f)
            {
                return Color.SlateGray;
            }
            if (height > 0f)
            {
                return Color.ForestGreen;
            }
            return Color.SandyBrown;
        }
        
        // Creates an array of indices which are used to render the vertices
        private void BuildIndicesArray()
        {
            int sideLength = worldSize - 1;
            index = new int[(worldSize - 1) * (worldSize - 1) * 6];
            int j = 0;
            for (int i = 0; i < index.Length; i += 6)
                {
                    index[i] = j;
                    index[i + 1] = j + sideLength + 1;
                    index[i + 2] = j + sideLength + 2;
                    index[i + 3] = j;
                    index[i + 4] = j + sideLength + 2;
                    index[i + 5] = j + 1;
                    j++;
                    
                }
        }

        // Populate a 2D array with values by running the Diamond Square Algorithm
        private void DiamondSquareGenerator()
        {

            heightMap = new float[worldSize, worldSize];
            float range = 50.0f;
            Random generator = new Random();
            heightMap[0, 0] = heightMap[0, worldSize - 1] =
                heightMap[worldSize - 1, 0] = heightMap[worldSize - 1, worldSize - 1] =
                generator.NextFloat(-range, range);
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
                    for (int y = (x + halfDiagonal) % sideLength; y < worldSize - 1; y += sideLength / 2)
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
