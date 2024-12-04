using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        public void Draw(SpriteBatch spriteBatch, Texture2D planetTexture, Texture2D healthBarTexture)
        {
            // Draw the planet
            spriteBatch.Draw(
                planetTexture,
                new Rectangle((int)position.X, (int)position.Y, size, size),
                Color.White
            );

            // Draw the health bar
            int healthBarWidth = size; // Match width to the planet size
            int healthBarHeight = 5; // Fixed height for the health bar
            float healthPercentage = Math.Max(0, health / (float)MaxHealth);

            // Determine health bar color
            Color healthBarColor = Color.Lerp(Color.Red, Color.Green, healthPercentage);

            // Draw the background of the health bar (gray to show max health area)
            spriteBatch.Draw(
                healthBarTexture,
                new Rectangle((int)position.X, (int)position.Y - 10, healthBarWidth, healthBarHeight),
                Color.Gray
            );

            // Draw the actual health bar based on the current health percentage
            spriteBatch.Draw(
                healthBarTexture,
                new Rectangle((int)position.X, (int)position.Y - 10, (int)(healthBarWidth * healthPercentage), healthBarHeight),
                healthBarColor
            );
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health < 0) health = 0; // Ensure health doesn't go below zero
        }

        public bool IsDestroyed()
        {
            return health <= 0;
        }
    }
}
