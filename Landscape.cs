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
        private int worldSize;
        private float[,] heightMap;
        private Vector3[,] vertexNormals;
        private Vector3 lightDirection;
        private Vector3 specularColor;
        private Vector3 diffuseColor;
        private Vector3 ambientColor;
        private float rotationSpeed;

        private float zAngle;
        private float yAngle;

        public Landscape(Game game, float rotationSpeed, int worldSize)
        {
            this.worldSize = worldSize;
            DiamondSquareGenerator();
            BuildVertexNormals();
            VertexPositionNormalColor[] vertexList = BuildVertexArray();       
            vertices = Buffer.Vertex.New(game.GraphicsDevice, vertexList);
            rotationSpeed = 0.01f;
            this.rotationSpeed = rotationSpeed;

            zAngle = 0f;
            yAngle = (float)Math.PI;
            lightDirection = new Vector3(0f, (float)Math.Sin(yAngle), (float)Math.Cos(zAngle));
            specularColor = new Vector3(0.5f, 0.5f, 0.5f);
            diffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            ambientColor = new Vector3(0.2f, 0.2f, 0.2f);

            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                LightingEnabled = true,
                PreferPerPixelLighting = true,
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
            zAngle += rotationSpeed;
            yAngle += rotationSpeed;
            lightDirection = new Vector3(0f, (float)Math.Sin(yAngle), (float)Math.Cos(zAngle));

            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            zAngle += rotationSpeed;
            yAngle += rotationSpeed;
            lightDirection = new Vector3(0f, (float)Math.Sin(yAngle), (float)Math.Cos(zAngle));
            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f,
                (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);
            basicEffect.DirectionalLight0.Direction = lightDirection;
            basicEffect.DirectionalLight0.DiffuseColor = diffuseColor;
            basicEffect.DirectionalLight0.SpecularColor = specularColor;
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.AmbientLightColor = ambientColor;
        }

        public override void Draw(GameTime gameTime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw the landscape
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.SetBlendState(game.GraphicsDevice.BlendStates.AlphaBlend);
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }

        private void BuildVertexNormals()
        {
            vertexNormals = new Vector3[worldSize, worldSize];
            for (int x = 0; x < worldSize - 1; x++)
            {
                for (int z = 0; z < worldSize - 1; z++)
                {
                    Vector3 A = new Vector3((float)x, heightMap[x, z], (float)z);
                    Vector3 B = new Vector3((float)x, heightMap[x, z + 1], (float)z + 1);
                    Vector3 C = new Vector3((float)x + 1f, heightMap[x + 1, z + 1], (float)z + 1f);
                    Vector3 normalVector = Vector3.Cross(B - A, C - A);
                    vertexNormals[x, z] += normalVector;
                    vertexNormals[x, z + 1] += normalVector;
                    vertexNormals[x + 1, z + 1] += normalVector;
                    A = new Vector3((float)x + 1, heightMap[x + 1, z], (float)z);
                    B = new Vector3((float)x, heightMap[x, z], (float)z);
                    C = new Vector3((float)x + 1f, heightMap[x + 1, z + 1], (float)z + 1f);
                    normalVector = Vector3.Cross(B - A, C - A);
                    vertexNormals[x + 1, z] += normalVector;
                    vertexNormals[x, z] += normalVector;
                    vertexNormals[x + 1, z + 1] += normalVector;
                }
            }
            for (int x = 0; x < worldSize; x++)
            {
                for (int z = 0; z < worldSize; z++)
                {
                    vertexNormals[x, z].Normalize();
                }
            }
        }

        // Build vertex list using values from the heightmap
        private VertexPositionNormalColor[] BuildVertexArray()
        {
            List<VertexPositionNormalColor> vertices = new List<VertexPositionNormalColor>();
            int sideLength = worldSize - 1;

            for (int x = 0; x < sideLength; x++)
            {
                for (int z = 0; z < sideLength; z++)
                {
                    vertices.Add(new VertexPositionNormalColor(
                        new Vector3((float)x, heightMap[x, z], (float)z), vertexNormals[x, z], GetColor(heightMap[x, z])));
                    vertices.Add(new VertexPositionNormalColor(
                        new Vector3((float)x, heightMap[x, z + 1], (float)z + 1), vertexNormals[x, z + 1], GetColor(heightMap[x, z + 1])));
                    vertices.Add(new VertexPositionNormalColor(
                        new Vector3((float)x + 1f, heightMap[x + 1, z + 1], (float)z + 1f), vertexNormals[x + 1, z + 1], GetColor(heightMap[x + 1, z + 1])));
                    vertices.Add(new VertexPositionNormalColor(
                        new Vector3((float)x + 1f, heightMap[x + 1, z], (float)z), vertexNormals[x + 1, z], GetColor(heightMap[x + 1, z])));
                    vertices.Add(new VertexPositionNormalColor(
                        new Vector3((float)x, heightMap[x, z], (float)z), vertexNormals[x, z], GetColor(heightMap[x, z])));
                    vertices.Add(new VertexPositionNormalColor(
                        new Vector3((float)x + 1, heightMap[x + 1, z + 1], (float)z + 1), vertexNormals[x + 1, z + 1], GetColor(heightMap[x + 1, z + 1])));

                }
            }
            Color water = new Color(0,0,255,140);
            Vector3 normal = new Vector3(0f, 1f, 0f);
            vertices.Add(new VertexPositionNormalColor(new Vector3(0f, 0f, 0f), normal, water));
            vertices.Add(new VertexPositionNormalColor(new Vector3(0f, 0f, worldSize - 1), normal, water));
            vertices.Add(new VertexPositionNormalColor(new Vector3(worldSize - 1, 0f, worldSize - 1), normal, water));
            vertices.Add(new VertexPositionNormalColor(new Vector3(0f, 0f, 0f), normal, water));
            vertices.Add(new VertexPositionNormalColor(new Vector3(worldSize - 1, 0f, worldSize - 1), normal, water));
            vertices.Add(new VertexPositionNormalColor(new Vector3(worldSize - 1, 0f, 0f), normal, water));
            vertices.Add(new VertexPositionNormalColor(new Vector3(0f, 0f, 0f), normal, water));
            vertices.Add(new VertexPositionNormalColor(new Vector3(worldSize - 1, 0f, 0f), normal, water));
            vertices.Add(new VertexPositionNormalColor(new Vector3(worldSize - 1, 0f, worldSize - 1), normal, water));
            vertices.Add(new VertexPositionNormalColor(new Vector3(0f, 0f, worldSize - 1), normal, water));
            vertices.Add(new VertexPositionNormalColor(new Vector3(0f, 0f, 0f), normal, water));
            vertices.Add(new VertexPositionNormalColor(new Vector3(worldSize - 1, 0f, worldSize - 1), normal, water));
           


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
            return Color.SandyBrown;
        }

        // Returns the height of the heightMap at a certain coordinate without exposing heightMap to
        // external changes.
        public bool AllowMovement(Vector3 eye)
        {
            int x = (int)Math.Ceiling(eye.X);
            int z = (int)Math.Ceiling(eye.Z);
            if (x >= (float)worldSize || x <= 0 || z >= (float)worldSize || z <= 0)
            {
                return false;
            }
            return !(heightMap[x, z] >= eye.Y - 1f);
        }

        // Populate a 2D array with values by running the Diamond Square Algorithm
        private void DiamondSquareGenerator()
        {

            heightMap = new float[worldSize, worldSize];
            float range = 50.0f;
            Random generator = new Random();
            heightMap[0, 0] = heightMap[0, worldSize - 1] =
                heightMap[worldSize - 1, 0] = heightMap[worldSize - 1, worldSize - 1] =
                generator.NextFloat(0, 10);
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
