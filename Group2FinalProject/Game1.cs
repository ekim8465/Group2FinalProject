using Group2FinalProject;
using Microsoft.Xna.Framework;
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

        // Textures
        Texture2D shipSprite;
        Texture2D asteroidSprite;
        Texture2D spaceSprite;
        Texture2D bulletSprite; // Bullet texture

        // Fonts
        SpriteFont gameFont;
        SpriteFont timerFont;

        // Sound Effects
        SoundEffect collisionSound;
        SoundEffect boostSound;
        SoundEffect shootSound; // Shoot sound effect

        // Player's ship
        Ship player = new Ship();

        // List of asteroids
        List<Asteroid> asteroids = new List<Asteroid>();

        // Random number generator for asteroid spawning
        Random randomAsteroids = new Random();

        // Time tracking
        private TimeSpan elapsedTime;
        private int secondsElapsed;
        private int score;

        // Asteroid spawn interval in seconds
        private double asteroidSpawnInterval = 2;

        // Controller for handling game logic
        Controller controller = new Controller();

        // Game state flags
        bool isGameOver = false;
        bool isGameWon = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content"; // Set content directory
            IsMouseVisible = true; // Set mouse visibility
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.ApplyChanges(); // Apply graphics settings

            elapsedTime = TimeSpan.Zero;
            secondsElapsed = 0;
            score = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load textures (images) for ship, asteroid, background, and bullet
            shipSprite = Content.Load<Texture2D>("Images/ship");
            asteroidSprite = Content.Load<Texture2D>("Images/asteroid");
            spaceSprite = Content.Load<Texture2D>("Images/space");
            bulletSprite = Content.Load<Texture2D>("Images/laserBullet");

            // Load fonts
            gameFont = Content.Load<SpriteFont>("Fonts/spaceFont");
            timerFont = Content.Load<SpriteFont>("Fonts/timerFont");

            // Load sound effects
            shootSound = Content.Load<SoundEffect>("Sounds/retro-laser-shot-04");
            collisionSound = Content.Load<SoundEffect>("Sounds/metallic-crash");
            boostSound = Content.Load<SoundEffect>("Sounds/boostSound");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (isGameOver || isGameWon)
                return; // Skip update if game is over or won

            // Handle spaceship movement (with boost option)
            KeyboardState keyboardState = Keyboard.GetState();
            bool boostActive = keyboardState.IsKeyDown(Keys.Right) && keyboardState.IsKeyDown(Keys.Enter);

            player.MoveShip(keyboardState, boostActive);
            if (boostActive) boostSound.Play(); // Play boost sound when boost is active

            // Shooting logic
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                player.Shoot(bulletSprite);  // Create a new bullet
                shootSound.Play();           // Play the shooting sound
            }

            secondsElapsed = controller.updateTime(gameTime);

            // Update and check collisions for asteroids
            for (int i = 0; i < asteroids.Count; i++)
            {
                var asteroid = asteroids[i];
                asteroid.updateAsteroid();

                // Check collision with the ship
                if (controller.didCollisionHappen(player, asteroid))
                {
                    collisionSound.Play();
                    score -= 3; // Deduct score for collision

                    asteroids.RemoveAt(i); // Remove the asteroid that collided with the ship
                    i--; // Adjust the index
                    continue; // Continue with the next asteroid
                }

                // Increase score when an asteroid moves off-screen
                if (asteroid.position.X < 0)
                {
                    score++;
                    asteroids.RemoveAt(i);
                    i--;
                }

                // Check game over or win condition based on time and score
                if (secondsElapsed >= 15)
                {
                    if (score > 0)
                    {
                        isGameWon = true;
                        controller.gameWinEndScript(score);
                    }
                    else if (score < 0)
                    {
                        isGameOver = true;
                        controller.gameEndScript();
                    }
                    return;
                }
            }

            elapsedTime += gameTime.ElapsedGameTime;

            // Spawn new asteroids periodically
            if (elapsedTime.TotalSeconds >= asteroidSpawnInterval)
            {
                SpawnAsteroid();
                boostSound.Play();
                elapsedTime = TimeSpan.Zero;
            }

            // Update bullets
            foreach (var bullet in player.bullets)
            {
                bullet.Update(); // Move the bullet
            }

            // Remove bullets that are off-screen
            player.bullets.RemoveAll(b => b.Position.X > _graphics.PreferredBackBufferWidth);

            base.Update(gameTime);
        }

        private void SpawnAsteroid()
        {
            float x = _graphics.PreferredBackBufferWidth;
            float y = randomAsteroids.Next(0, _graphics.PreferredBackBufferHeight);

            int speed = randomAsteroids.Next(2, 6);
            Asteroid newAsteroid = new Asteroid(speed, asteroidSprite) { position = new Vector2(x, y) };
            asteroids.Add(newAsteroid);

            boostSound.Play();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue); // Clear the screen with a background color

            _spriteBatch.Begin();
            _spriteBatch.Draw(spaceSprite, new Vector2(0, 0), Color.White); // Draw the background
            _spriteBatch.Draw(shipSprite, new Vector2(player.position.X - shipSprite.Width / 2, player.position.Y - shipSprite.Height / 2), Color.White); // Draw the ship

            // Draw asteroids
            foreach (var asteroid in asteroids)
            {
                asteroid.Draw(_spriteBatch); // Draw each asteroid
            }

            // Draw bullets
            foreach (var bullet in player.bullets)
            {
                bullet.Draw(_spriteBatch); // Draw each bullet
            }

            // Displaying scores and timer
            _spriteBatch.DrawString(timerFont, "Score: " + score, new Vector2(_graphics.PreferredBackBufferWidth / 2, 10), Color.White);
            _spriteBatch.DrawString(timerFont, "Time: " + secondsElapsed, new Vector2(_graphics.PreferredBackBufferWidth / 2, 30), Color.White);

            // Display Game Over or Win messages
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
