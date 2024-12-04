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

        // Textures for the game
        Texture2D shipSprite;
        Texture2D asteroidSprite;
        Texture2D spaceSprite;
        Texture2D starSprite;
        Texture2D bulletSprite;

        // Fonts
        SpriteFont gameFont;
        SpriteFont timerFont;

        // Sounds
        SoundEffect collisionSound;
        SoundEffect asteroidSound;
        SoundEffect shootingSound;

        // Player and other game objects
        Ship player;
        List<Asteroid> asteroids = new List<Asteroid>();
        List<Star> stars = new List<Star>();

        Random randomAsteroids = new Random();
        Random randomStars = new Random();

        // Time and score tracking
        private TimeSpan elapsedTime;
        private TimeSpan starElapsedTime;
        private int secondsElapsed;
        private int score;

        // Spawn intervals
        private double asteroidSpawnInterval = 2;
        private double starSpawnInterval = 2;

        // Game Controller
        Controller controller = new Controller();

        bool isGameOver = false;
        bool isGameWon = false;

        bool isBoostAvailable = false;  // Boost flag
       
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

            // Load all textures and fonts
            shipSprite = Content.Load<Texture2D>("shipmodel1");  // Default ship model
            asteroidSprite = Content.Load<Texture2D>("asteroid");
            spaceSprite = Content.Load<Texture2D>("space");
            starSprite = Content.Load<Texture2D>("star");
            bulletSprite = Content.Load<Texture2D>("bullet");

            gameFont = Content.Load<SpriteFont>("spaceFont");
            timerFont = Content.Load<SpriteFont>("timerFont");

            collisionSound = Content.Load<SoundEffect>("collisionSound");
            asteroidSound = Content.Load<SoundEffect>("asteroidSound");
            shootingSound = Content.Load<SoundEffect>("shootingSound");

            // Initialize player after textures have been loaded
            player = new Ship(shipSprite, Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (isGameOver || isGameWon)
                return;

            KeyboardState keyboardState = Keyboard.GetState();
            bool boostActive = false;

            // Activate boost if available (score >= 5)
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
            starElapsedTime += gameTime.ElapsedGameTime;

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

            // Upgrade player ship based on score
            if (score >= 10)
            {
                // Upgrade to model 2 with speed 3, radius 25
                player.UpgradeShip(3, 25, "ShipModel2");
            }
            if (score >= 20)
            {
                // Upgrade to model 3 with speed 4, radius 30
                player.UpgradeShip(4, 30, "ShipModel3");
            }


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
                        isBoostAvailable = true;  // Boost becomes available after score reaches 5
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

            // Draw stars
            foreach (var star in stars)
            {
                _spriteBatch.Draw(starSprite, new Vector2(star.position.X - starSprite.Width / 2, star.position.Y - starSprite.Height / 2), Color.White);
            }

            // Draw asteroids
            foreach (var asteroid in asteroids)
            {
                _spriteBatch.Draw(asteroidSprite, new Vector2(asteroid.position.X - asteroidSprite.Width / 2, asteroid.position.Y - asteroidSprite.Height / 2), Color.White);
            }

            // Draw score
            _spriteBatch.DrawString(timerFont, "Score: " + score, new Vector2(_graphics.PreferredBackBufferWidth / 2, 10), Color.White);

            // Draw timer
            _spriteBatch.DrawString(timerFont, "Time: " + secondsElapsed, new Vector2(_graphics.PreferredBackBufferWidth / 2, 30), Color.White);

            // Display game over or win message
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
