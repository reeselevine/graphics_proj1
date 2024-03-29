﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    abstract public class GameObject
    {
        public BasicEffect basicEffect;
        public VertexInputLayout inputLayout;
        public Game game;
        public float rotationSpeed;

        public abstract void Update(GameTime gametime, Matrix world, Matrix view);
        public abstract void Draw(GameTime gametime);
    }
}
