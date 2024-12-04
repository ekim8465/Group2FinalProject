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

        public Planet(Vector2 position, int initialHealth, int size)
        {
            this.position = position;
            this.health = initialHealth;
            this.size = size;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, size, size), Color.White);

            // Draw health bar
            var healthBarWidth = size; // Match planet size
            var healthBarHeight = 5;
            var healthPercentage = Math.Max(0, health / 100f);
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
