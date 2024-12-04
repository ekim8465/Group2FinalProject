using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Group2FinalProject
{
    internal class Bullet
    {
        public Vector2 Position { get; private set; } // Position of the bullet
        private int speed = 10; // Speed at which the bullet moves
        private Texture2D bulletTexture; // Texture to represent the bullet

        // Constructor to initialize bullet's position and texture
        public Bullet(Vector2 startPosition, Texture2D texture)
        {
            Position = startPosition; // Set the initial position of the bullet
            bulletTexture = texture; // Set the texture that will be used for rendering the bullet
        }

        // Update the bullet's position
        public void Update()
        {
            // Move the bullet horizontally to the right
            Position = new Vector2(Position.X + speed, Position.Y);
        }

        // Draw the bullet to the screen using the provided SpriteBatch
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bulletTexture, Position, Color.White); // Draw the bullet at its current position
        }
    }
}
