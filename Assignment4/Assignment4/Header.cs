using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Primitives
{
    struct Cube
    {
        public Vector3 position;
        public float scale;
        public Color color;
        public Vector3 velocity;
    }

    struct Light
    {
        public Vector3 position;
        public Color color;
        public Vector3 velocity;
    }
}
