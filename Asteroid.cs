using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment3_EunmiKim
{
    internal class Asteroid
    {
        public Vector2 position;
        private int speed;
        public static int radius = 60; // Static radius for collision detection

        private Texture2D asteroidTexture; // Texture for rendering the asteroid

        // Constructor to set speed and initialize asteroid texture
        public Asteroid(int speed, Texture2D texture)
        {
            this.speed = speed;
            this.asteroidTexture = texture; // Assign the asteroid texture
        }

        // Update the asteroid's position
        public void updateAsteroid()
        {
            position.X -= speed;
        }

        // Draw the asteroid on the screen
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(asteroidTexture, position, Color.White);
        }
    }
}
