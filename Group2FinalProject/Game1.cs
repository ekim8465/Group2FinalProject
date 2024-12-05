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
        Texture2D shipModel2Sprite;
        Texture2D shipModel3Sprite;
        Texture2D asteroidSprite;
        Texture2D planetSprite1;
        Texture2D starSprite;
        Texture2D spaceSprite;
        Texture2D bulletSprite;

        // Fonts
        SpriteFont gameFont;
        SpriteFont timerFont;

        // Sounds
        SoundEffect collisionSound;
        SoundEffect asteroidSound;
        SoundEffect shootingSound;
        SoundEffect victorySound;
        SoundEffectInstance victorySoundInstance;

        // Player and other game objects
        Ship player;
        List<Asteroid> asteroids = new List<Asteroid>();
        List<Planet> planets = new List<Planet>();
        List<Bullet> bullets = new List<Bullet>();
        List<Star> stars = new List<Star>();

        Random randomGenerator = new Random();

        // Time and score tracking
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private TimeSpan starElapsedTime = TimeSpan.Zero;
        private TimeSpan bulletCooldownTime = TimeSpan.Zero;
        private int secondsElapsed;
        private int score;

        // Spawn intervals
        private double asteroidSpawnInterval = 2;
        private double starSpawnInterval = 5;

        bool isGameOver = false;
        bool isGameWon = false;
        bool isVictorySoundPlayed = false;

        // Fire rate cooldown values for different ship models
        private double baseFireRate = 0.5;     // Default rate for base model
        private double model2FireRate = 0.3;   // Faster rate for model 2
        private double model3FireRate = 0.2;   // Fastest rate for model 3

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

            ResetGame();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load all textures and fonts
            shipSprite = Content.Load<Texture2D>("ship");
            shipModel2Sprite = Content.Load<Texture2D>("ShipModel2");
            shipModel3Sprite = Content.Load<Texture2D>("ShipModel3");
            asteroidSprite = Content.Load<Texture2D>("asteroid");
            planetSprite1 = Content.Load<Texture2D>("Planet1");
            spaceSprite = Content.Load<Texture2D>("space");
            starSprite = Content.Load<Texture2D>("star");
            bulletSprite = Content.Load<Texture2D>("bullet");

            gameFont = Content.Load<SpriteFont>("spaceFont");
            timerFont = Content.Load<SpriteFont>("timerFont");

            collisionSound = Content.Load<SoundEffect>("collisionSound");
            asteroidSound = Content.Load<SoundEffect>("asteroidSound");
            shootingSound = Content.Load<SoundEffect>("shootingSound");
            victorySound = Content.Load<SoundEffect>("victorySound");

            // Create instance for victory sound
            victorySoundInstance = victorySound.CreateInstance();

            // Initialize player
            player = new Ship(shipSprite, Content, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            // Spawn two planets with initial health
            planets.Add(new Planet(new Vector2(800, 200), 100, 100));
            planets.Add(new Planet(new Vector2(1100, 600), 150, 120));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (isGameOver || isGameWon)
            {
                // Restart game if 'R' key is pressed
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    ResetGame();
                }
                return;
            }

            KeyboardState keyboardState = Keyboard.GetState();

            // Check if the boost key (e.g., LeftShift) is being held
            bool boostActive = keyboardState.IsKeyDown(Keys.LeftShift);

            // Move the player ship with boost if active
            player.MoveShip(keyboardState, boostActive);

            // Determine bullet cooldown time based on ship upgrade level
            double currentFireRate = baseFireRate;
            if (player.UpgradeLevel == 1)
            {
                currentFireRate = model2FireRate;
            }
            else if (player.UpgradeLevel == 2)
            {
                currentFireRate = model3FireRate;
            }

            // Bullet Firing with Cooldown based on upgrade level
            bulletCooldownTime += gameTime.ElapsedGameTime;
            if (keyboardState.IsKeyDown(Keys.Space) && bulletCooldownTime.TotalSeconds >= currentFireRate)
            {
                bullets.Add(new Bullet(player.position, bulletSprite, 10));
                shootingSound.Play();
                bulletCooldownTime = TimeSpan.Zero; // Reset cooldown
            }

            // Update game objects if not game over or won
            UpdateGameObjects(gameTime);

            // Check for spaceship upgrades based on score
            CheckForShipUpgrade();

            // Update elapsed time and check for win condition
            elapsedTime += gameTime.ElapsedGameTime;
            secondsElapsed = (int)elapsedTime.TotalSeconds;

            // Check win condition
            if (secondsElapsed >= 40 || score >= 60)
            {
                isGameWon = true;
                if (!isVictorySoundPlayed)
                {
                    victorySoundInstance.Play();
                    isVictorySoundPlayed = true;
                }
            }

            base.Update(gameTime);
        }


        private void ResetGame()
        {
            // Stop the victory sound if it's playing
            if (victorySoundInstance != null && victorySoundInstance.State == SoundState.Playing)
            {
                victorySoundInstance.Stop();
            }

            // Reset game variables
            elapsedTime = TimeSpan.Zero;
            starElapsedTime = TimeSpan.Zero;
            bulletCooldownTime = TimeSpan.Zero;
            secondsElapsed = 0;
            score = 0;
            isGameOver = false;
            isGameWon = false;
            isVictorySoundPlayed = false;

            // Clear game objects
            asteroids.Clear();
            planets.Clear();
            bullets.Clear();
            stars.Clear();

            // Reinitialize player
            player = new Ship(shipSprite, Content, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            // Spawn two planets with initial health
            planets.Add(new Planet(new Vector2(800, 200), 100, 100));
            planets.Add(new Planet(new Vector2(1100, 600), 150, 120));
        }

        private void UpdateGameObjects(GameTime gameTime)
        {
            // Update bullets
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update();

                // Check collisions with asteroids and planets
                bool bulletRemoved = false;

                // Check collision with asteroids
                for (int j = asteroids.Count - 1; j >= 0 && !bulletRemoved; j--)
                {
                    if (Vector2.Distance(bullets[i].Position, asteroids[j].position) < 30)
                    {
                        asteroids[j].TakeDamage(25);
                        bullets.RemoveAt(i);
                        bulletRemoved = true;

                        if (asteroids[j].health <= 0)
                        {
                            asteroids.RemoveAt(j);
                            score += 10;
                        }
                    }
                }

                // Check collision with planets
                for (int j = planets.Count - 1; j >= 0 && !bulletRemoved; j--)
                {
                    if (Vector2.Distance(bullets[i].Position, planets[j].position) < 50)
                    {
                        planets[j].TakeDamage(20);
                        bullets.RemoveAt(i);
                        bulletRemoved = true;

                        if (planets[j].IsDestroyed())
                        {
                            planets.RemoveAt(j);
                            score += 20;
                        }
                    }
                }

                // Remove bullets that go off-screen
                if (!bulletRemoved && bullets[i].Position.X > _graphics.PreferredBackBufferWidth)
                {
                    bullets.RemoveAt(i);
                }
            }

            // Update asteroids
            for (int i = asteroids.Count - 1; i >= 0; i--)
            {
                asteroids[i].Update();

                // Remove asteroids that go off-screen
                if (asteroids[i].position.X < 0)
                {
                    asteroids.RemoveAt(i);
                    continue;
                }

                // Check collision with player
                if (Vector2.Distance(player.position, asteroids[i].position) < 40)
                {
                    isGameOver = true;
                    collisionSound.Play();
                    break;
                }
            }

            // Update planets
            for (int i = planets.Count - 1; i >= 0; i--)
            {
                planets[i].Update(2);

                // Remove planets that go off-screen
                if (planets[i].position.X < 0)
                {
                    planets.RemoveAt(i);
                    continue;
                }

                // Check collision with player for game over condition
                if (Vector2.Distance(player.position, planets[i].position) < 50)
                {
                    isGameOver = true;
                    collisionSound.Play();
                    break;
                }
            }

            // Update stars
            for (int i = stars.Count - 1; i >= 0; i--)
            {
                stars[i].updateStar();

                // Check collision with player for collection
                if (Vector2.Distance(player.position, stars[i].position) < 30)
                {
                    score += stars[i].value;
                    stars.RemoveAt(i);
                    continue; // Skip further checks since this star has been removed
                }

                // Remove stars that go off-screen
                if (stars.Count > i && stars[i].position.X < 0)
                {
                    stars.RemoveAt(i);
                }
            }


            // Spawn new asteroids and stars
            elapsedTime += gameTime.ElapsedGameTime;
            starElapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime.TotalSeconds >= asteroidSpawnInterval)
            {
                SpawnAsteroid();
                elapsedTime = TimeSpan.Zero;
            }

            if (starElapsedTime.TotalSeconds >= starSpawnInterval)
            {
                SpawnStar();
                starElapsedTime = TimeSpan.Zero;
            }
        }

        private void SpawnAsteroid()
        {
            float x = _graphics.PreferredBackBufferWidth;
            float y = randomGenerator.Next(0, _graphics.PreferredBackBufferHeight);

            int speed = randomGenerator.Next(2, 6);
            int health = randomGenerator.Next(50, 100);
            Asteroid newAsteroid = new Asteroid(speed, health) { position = new Vector2(x, y) };
            asteroids.Add(newAsteroid);
        }

        private void SpawnStar()
        {
            float x = _graphics.PreferredBackBufferWidth;
            float y = randomGenerator.Next(0, _graphics.PreferredBackBufferHeight);

            int speed = randomGenerator.Next(1, 3);
            Star newStar = new Star(speed) { position = new Vector2(x, y) };
            stars.Add(newStar);
        }

        private void CheckForShipUpgrade()
        {
            if (score >= 20 && player.UpgradeLevel < 1)
            {
                player.UpgradeShip(3, 25, "ShipModel2", model2FireRate);
            }
            if (score >= 40 && player.UpgradeLevel < 2)
            {
                player.UpgradeShip(4, 30, "ShipModel3", model3FireRate);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(spaceSprite, new Vector2(0, 0), Color.White);

            // Draw player ship
            player.Draw(_spriteBatch);

            // Draw bullets
            foreach (var bullet in bullets)
            {
                bullet.Draw(_spriteBatch);
            }

            // Draw asteroids
            foreach (var asteroid in asteroids)
            {
                asteroid.Draw(_spriteBatch, asteroidSprite);
            }

            // Draw planets
            foreach (var planet in planets)
            {
                planet.Draw(_spriteBatch, planetSprite1);
            }

            // Draw stars
            foreach (var star in stars)
            {
                _spriteBatch.Draw(starSprite, star.position, Color.Yellow);
            }

            // Draw score and timer
            _spriteBatch.DrawString(timerFont, $"Score: {score}", new Vector2(20, 20), Color.White);
            _spriteBatch.DrawString(timerFont, $"Time: {secondsElapsed}", new Vector2(20, 50), Color.White);

            if (isGameOver)
            {
                _spriteBatch.DrawString(gameFont, "Game Over! Press 'R' to Restart", new Vector2(400, 500), Color.Red);
            }

            if (isGameWon)
            {
                _spriteBatch.DrawString(gameFont, "Victory! Press 'R' to Restart", new Vector2(400, 500), Color.Green);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}