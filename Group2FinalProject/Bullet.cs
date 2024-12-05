using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Group2FinalProject
{
	public class Bullet
	{
		public Vector2 Position { get; private set; } // Make sure this is public and named Position
		private int speed;
		private Texture2D bulletTexture;

		// Constructor to initialize the bullet's position, texture, and speed
		public Bullet(Vector2 startPosition, Texture2D texture, int bulletSpeed = 10)
		{
			Position = startPosition;
			bulletTexture = texture;
			speed = bulletSpeed;
		}

		// Update the bullet's position based on speed
		public void Update()
		{
			Position = new Vector2(Position.X + speed, Position.Y);
		}

		// Draw the bullet to the screen using the provided SpriteBatch
		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(bulletTexture, Position, Color.White);
		}
	}
}
