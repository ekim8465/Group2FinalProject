using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Group2FinalProject
{
    internal class Asteroid
    {
        public Vector2 position;
        private int speed;
        public int health; // Health for asteroid
        public static int radius = 60;

        public Asteroid(int speed, int initialHealth)
        {
            this.speed = speed;
            this.health = initialHealth;
            this.position = new Vector2(800, Random.Shared.Next(50, 400)); // Random Y position
        }

        public void Update()
        {
            position.X -= speed; // Move left
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            // Draw the asteroid
            spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, radius * 2, radius * 2), Color.White);

            // Draw health bar
            int healthBarWidth = 50; // Fixed width for health bar
            int healthBarHeight = 5;
            float healthPercentage = Math.Max(0, health / 100f); // Health as a fraction
            Color healthBarColor = Color.Lerp(Color.Red, Color.Green, healthPercentage); // Health bar color interpolates between red and green

            // Draw the actual health bar
            spriteBatch.Draw(
                texture: new Texture2D(spriteBatch.GraphicsDevice, 1, 1),
                destinationRectangle: new Rectangle((int)position.X, (int)position.Y - 15, (int)(healthBarWidth * healthPercentage), healthBarHeight),
                color: healthBarColor
            );
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
        }
    }
}
