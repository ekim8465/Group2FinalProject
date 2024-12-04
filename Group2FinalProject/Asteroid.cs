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
            spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, radius * 2, radius * 2), Color.White);

            // Draw health bar
            var healthBarWidth = 50; // Fixed width for health bar
            var healthBarHeight = 5;
            var healthPercentage = Math.Max(0, health / 100f); // Health as a fraction
            var healthBarColor = healthPercentage > 0.5f ? Color.Green : Color.Red;

            spriteBatch.Draw(
                texture: texture,
                destinationRectangle: new Rectangle((int)position.X, (int)position.Y - 10, (int)(healthBarWidth * healthPercentage), healthBarHeight),
                color: healthBarColor
            );
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
        }
    }
}
