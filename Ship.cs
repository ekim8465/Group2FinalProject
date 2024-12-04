using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Assignment3_EunmiKim
{
    internal class Ship
    {
        public Vector2 position = new Vector2(100, 100); // Ship's position on screen
        int baseSpeed = 2; // Speed of the ship
        int radius = 30; // Collision radius of the ship

        public List<Bullet> bullets = new List<Bullet>(); // List of bullets

        public void setRadius(int radius)
        {
            this.radius = radius;
        }

        public int getRadius()
        {
            return this.radius;
        }

        // Move the ship based on keyboard input (with boost option)
        public void MoveShip(KeyboardState state, bool boostActive)
        {
            int speed = boostActive ? baseSpeed * 2 : baseSpeed;

            if (state.IsKeyDown(Keys.Left))
            {
                this.position.X -= speed;
            }
            if (state.IsKeyDown(Keys.Right))
            {
                this.position.X += speed;
            }

            if (state.IsKeyDown(Keys.Up))
            {
                this.position.Y -= speed;
            }
            if (state.IsKeyDown(Keys.Down))
            {
                this.position.Y += speed;
            }
        }

        // Shoot a bullet from the ship
        public void Shoot(Texture2D bulletTexture)
        {
            Bullet newBullet = new Bullet(new Vector2(this.position.X + 10, this.position.Y), bulletTexture);
            bullets.Add(newBullet); // Add the new bullet to the list
        }
    }
}
