using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        public int UpgradeLevel { get; private set; } = 0;  // Tracks upgrade level (public getter)
        private double bulletCooldownTime = 0.5;  // Bullet cooldown time, default for base ship

        private Texture2D shipTexture;  // For updating the ship texture on upgrade
        private ContentManager contentManager;  // Reference to the ContentManager

        public Ship(Texture2D initialTexture, ContentManager content, int screenWidth, int screenHeight)
        {
            shipTexture = initialTexture;
            speed = baseSpeed;  // Start with the base speed
            contentManager = content;  // Store the reference to ContentManager
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

        // Getter for Bullet Cooldown Time
        public double GetBulletCooldownTime()
        {
            return bulletCooldownTime;
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
        public void UpgradeShip(int newSpeed, int newRadius, string textureName, double newBulletCooldownTime)
        {
            // Upgrade the ship's stats only if we are moving to a higher level
            if (UpgradeLevel < 2)
            {
                UpgradeLevel++;  // Increment upgrade level
                speed = newSpeed; // Set new speed
                radius = newRadius; // Set new radius
                bulletCooldownTime = newBulletCooldownTime; // Set new bullet cooldown time
                shipTexture = LoadTexture(textureName); // Change ship texture
            }
        }

        // Method to load texture based on the texture name
        private Texture2D LoadTexture(string textureName)
        {
            return contentManager.Load<Texture2D>(textureName);
        }

        // Get the current texture of the ship
        public Texture2D GetShipTexture()
        {
            return shipTexture;
        }

        // Draw method to render the ship
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the ship at its current position with its texture
            spriteBatch.Draw(shipTexture, position, Color.White);
        }
    }
}
