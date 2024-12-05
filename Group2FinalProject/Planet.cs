using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Group2FinalProject
{
    internal class Planet
    {
        public Vector2 position;
        public int health;
        public int size;
        private const int MaxHealth = 100; // Maximum health value for scaling

        public Planet(Vector2 position, int initialHealth, int size)
        {
            this.position = position;
            this.health = initialHealth;
            this.size = size;
        }

        // Method to update the planet's position (for movement if needed)
        public void Update(int speed)
        {
            position.X -= speed; // Move the planet to the left based on speed
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D planetTexture)
        {
            // Draw the planet
            spriteBatch.Draw(
                planetTexture,
                new Rectangle((int)position.X, (int)position.Y, size, size),
                Color.White
            );

            // Draw the health bar
            DrawHealthBar(spriteBatch);
        }

        private void DrawHealthBar(SpriteBatch spriteBatch)
        {
            int healthBarWidth = size; // Health bar width matches the planet's size
            int healthBarHeight = 5; // Fixed height for the health bar
            float healthPercentage = Math.Max(0, health / (float)MaxHealth);

            // Determine the health bar color based on the health percentage
            Color healthBarColor = Color.Lerp(Color.Red, Color.Green, healthPercentage);

            // Draw the background of the health bar (gray to show the full health area)
            spriteBatch.Draw(
                new Texture2D(spriteBatch.GraphicsDevice, 1, 1),
                new Rectangle((int)position.X, (int)position.Y - 10, healthBarWidth, healthBarHeight),
                Color.Gray
            );

            // Draw the actual health bar based on the current health percentage
            spriteBatch.Draw(
                new Texture2D(spriteBatch.GraphicsDevice, 1, 1),
                new Rectangle((int)position.X, (int)position.Y - 10, (int)(healthBarWidth * healthPercentage), healthBarHeight),
                healthBarColor
            );
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health < 0)
            {
                health = 0; // Ensure health doesn't go below zero
            }
        }

        public bool IsDestroyed()
        {
            return health <= 0;
        }
    }
}