using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Group2FinalProject
{
    internal class Ship
    {
        public Vector2 position = new Vector2(100, 100);
        private int baseSpeed = 2;  // Default speed
        private int speed;          // Dynamic speed based on upgrades
        private int radius = 20;    // Initial radius of the ship
        private int upgradeLevel = 0;  // Tracks upgrade level
        private int score = 0;          // Tracks score

        private Texture2D shipTexture;  // For updating the ship texture on upgrade

        public Ship(Texture2D initialTexture)
        {
            shipTexture = initialTexture;
            speed = baseSpeed;  // Start with the base speed
        }

        // Getter and Setter for radius
        public void SetRadius(int newRadius)
        {
            this.radius = newRadius;
        }

        public int GetRadius()
        {
            return this.radius;
        }

        // Handle movement with optional boost
        public void MoveShip(KeyboardState state, bool boostActive)
        {
            int currentSpeed = boostActive ? speed * 2 : speed;

            if (state.IsKeyDown(Keys.Left))
            {
                this.position.X -= currentSpeed;
            }
            if (state.IsKeyDown(Keys.Right))
            {
                this.position.X += currentSpeed;
            }

            if (state.IsKeyDown(Keys.Up))
            {
                this.position.Y -= currentSpeed;
            }
            if (state.IsKeyDown(Keys.Down))
            {
                this.position.Y += currentSpeed;
            }
        }

        // Method to check and apply upgrades based on the score
        public void CheckUpgrade(int currentScore)
        {
            score = currentScore;

            // Upgrade logic based on score thresholds
            if (score >= 40 && upgradeLevel < 2)  // Upgrade to Level 3 at score 40
            {
                UpgradeShip(3, 4, "shipModel3");  // Speed, radius, and texture for model 3
            }
            else if (score >= 20 && upgradeLevel < 1)  // Upgrade to Level 2 at score 20
            {
                UpgradeShip(3, 3, "shipModel2");  // Speed, radius, and texture for model 2
            }
        }

        // Upgrade the ship's stats
        private void UpgradeShip(int newSpeed, int newRadius, string textureName)
        {
            if (upgradeLevel < 2)  // Prevent upgrading beyond Level 3
            {
                upgradeLevel++;  // Increment upgrade level
                speed = newSpeed; // Set new speed
                radius = newRadius; // Set new radius
                shipTexture = LoadTexture(textureName); // Change ship texture
            }
        }

        // Dummy method to simulate loading new textures (you can replace with actual texture loading)
        private Texture2D LoadTexture(string textureName)
        {
            // In the actual implementation, load a texture based on the texture name
            return null; // Replace with your actual texture loading code
        }

        public Texture2D GetShipTexture()
        {
            return shipTexture;
        }
    }
}
