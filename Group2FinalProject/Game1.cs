using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Group2FinalProject
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		Texture2D shipSprite;
		Texture2D asteroidSprite;
		Texture2D spaceSprite;
		Texture2D starSprite;
		Texture2D bulletSprite; //
		SpriteFont gameFont;
		SpriteFont timerFont;

		SoundEffect collisionSound;
		SoundEffect asteroidSound;
		SoundEffect shootingSound; //

		Ship player = new Ship();
		List<Asteroid> asteroids = new List<Asteroid>();
		List<Star> stars = new List<Star>();

		Random randomAsteroids = new Random();
		Random randomStars = new Random();

		private TimeSpan elapsedTime;
		private TimeSpan starElapsedTime; 
		private int secondsElapsed;
		private int score;

		private double asteroidSpawnInterval = 2; 
		private double starSpawnInterval = 2; 

		Controller controller = new Controller();

		bool isGameOver = false;
		bool isGameWon = false;

		bool isBoostAvailable = false;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			_graphics.PreferredBackBufferWidth = 1300;
			_graphics.PreferredBackBufferHeight = 1000;
			_graphics.ApplyChanges();

			elapsedTime = TimeSpan.Zero;
			starElapsedTime = TimeSpan.Zero; 
			secondsElapsed = 0;
			score = 0;

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			shipSprite = Content.Load<Texture2D>("ship");
			asteroidSprite = Content.Load<Texture2D>("asteroid");
			spaceSprite = Content.Load<Texture2D>("space");
			starSprite = Content.Load<Texture2D>("star");
			bulletSprite = Content.Load<Texture2D>("bullet");  //

			gameFont = Content.Load<SpriteFont>("spaceFont");
			timerFont = Content.Load<SpriteFont>("timerFont");

			collisionSound = Content.Load<SoundEffect>("collisionSound");
			asteroidSound = Content.Load<SoundEffect>("asteroidSound");
			shootingSound = Content.Load<SoundEffect>("shootingSound");
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (isGameOver || isGameWon)
				return;

			KeyboardState keyboardState = Keyboard.GetState();
			bool boostActive = false;
			if (isBoostAvailable && keyboardState.IsKeyDown(Keys.Right) && keyboardState.IsKeyDown(Keys.Enter))
			{
				boostActive = true;
			}

			player.MoveShip(keyboardState, boostActive);

			secondsElapsed = controller.updateTime(gameTime);

			// Asteroid Update and Collision Handling
			for (int i = 0; i < asteroids.Count; i++)
			{
				var asteroid = asteroids[i];
				asteroid.updateAsteroid();

				if (controller.didCollisionHappen(player, asteroid))
				{
					collisionSound.Play();
					score -= 3;

					asteroids.RemoveAt(i);
					i--;
						 //continue;
				}

				// Remove Asteroids When They Move Off-Screen
				if (asteroid.position.X < 0)
				{
					asteroids.RemoveAt(i);
					i--;
				}
			}

			// Star Generation and Score Handling
			for (int i = 0; i < stars.Count; i++)
			{
				var star = stars[i];
				star.updateStar();

				if (controller.didCollectHappen(player, star))
				{
					score += 3;
					stars.RemoveAt(i); 
					i--; 

					continue;
				}

				// Remove Star When They Move Off-Screen
				if (star.position.X < 0)
				{
					stars.RemoveAt(i); 
					i--;
				}
			}

			elapsedTime += gameTime.ElapsedGameTime;
			starElapsedTime += gameTime.ElapsedGameTime; // Update Star Spawn Time


			if (secondsElapsed >= 30)
			{
				if (score > 0)
				{
					isGameWon = true;
					controller.gameWinEndScript(score);
				}
				else if (score <= 0)
				{
					isGameOver = true;
					controller.gameEndScript();
				}
				return;
			}

			// Asteroids Spawn
			if (elapsedTime.TotalSeconds >= asteroidSpawnInterval)
			{
				SpawnAsteroid();
				elapsedTime = TimeSpan.Zero;
			}

			// Star Spawn
			if (starElapsedTime.TotalSeconds >= starSpawnInterval)
			{
				SpawnStar();
				starElapsedTime = TimeSpan.Zero; 
			}

			HandleStarCollection();

			base.Update(gameTime);
		}

		private void SpawnAsteroid()
		{
			float x = _graphics.PreferredBackBufferWidth;
			float y = randomAsteroids.Next(0, _graphics.PreferredBackBufferHeight);

			int speed = randomAsteroids.Next(2, 6);
			Asteroid newAsteroid = new Asteroid(speed) { position = new Vector2(x, y) };
			asteroids.Add(newAsteroid);
		}

		private void SpawnStar()
		{
			// Set stars to move from right to left across the screen
			float x = _graphics.PreferredBackBufferWidth;
			float y = randomStars.Next(0, _graphics.PreferredBackBufferHeight);

			int speed = randomStars.Next(1, 3);
			Star newStar = new Star(speed) { position = new Vector2(x, y) };
			stars.Add(newStar);
		}

		private void HandleStarCollection()
		{
			for (int i = 0; i < stars.Count; i++)
			{
				var star = stars[i];

				if (Vector2.Distance(player.position, star.position) < 30) // Star Collection Range
				{
					score += star.value;
					
					stars.RemoveAt(i);
					i--;
					if (score >= 5)
					{
						isBoostAvailable = true;
					}
				}
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			_spriteBatch.Begin();

			_spriteBatch.Draw(spaceSprite, new Vector2(0, 0), Color.White);
			_spriteBatch.Draw(shipSprite, new Vector2(player.position.X - shipSprite.Width / 2, player.position.Y - shipSprite.Height / 2), Color.White);

			// star
			foreach (var star in stars)
			{
				
				_spriteBatch.Draw(starSprite, new Vector2(star.position.X - starSprite.Width / 2, star.position.Y - starSprite.Height / 2), Color.White);
			}

			// asteroid
			foreach (var asteroid in asteroids)
			{
				_spriteBatch.Draw(asteroidSprite, new Vector2(asteroid.position.X - asteroidSprite.Width / 2, asteroid.position.Y - asteroidSprite.Height / 2), Color.White);
			}

			// score
			_spriteBatch.DrawString(timerFont, "Score: " + score, new Vector2(_graphics.PreferredBackBufferWidth / 2, 10), Color.White);

			// timer
			_spriteBatch.DrawString(timerFont, "Time: " + secondsElapsed, new Vector2(_graphics.PreferredBackBufferWidth / 2, 30), Color.White);

			// message
			if (isGameOver)
			{
				_spriteBatch.DrawString(gameFont, controller.gameEndScript(), new Vector2(_graphics.PreferredBackBufferWidth / 4 - 100, _graphics.PreferredBackBufferHeight / 2), Color.White);
			}
			else if (isGameWon)
			{
				_spriteBatch.DrawString(gameFont, controller.gameWinEndScript(score), new Vector2(_graphics.PreferredBackBufferWidth / 4 - 100, _graphics.PreferredBackBufferHeight / 2), Color.White);
			}

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
