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
        Texture2D planetSprite1;
        Texture2D planetSprite2;
        Texture2D spaceSprite;
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
        List<Planet> planets = new List<Planet>();
        List<Bullet> bullets = new List<Bullet>();

        Random randomGenerator = new Random();

        // Time and score tracking
        private TimeSpan elapsedTime;
        private int secondsElapsed;
        private int score;

        // Spawn intervals
        private double asteroidSpawnInterval = 2;

        // Game Controller
        Controller controller = new Controller();

        bool isGameOver = false;

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
            secondsElapsed = 0;
            score = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load all textures and fonts
            shipSprite = Content.Load<Texture2D>("shipmodel1");
            asteroidSprite = Content.Load<Texture2D>("asteroid");
            planetSprite1 = Content.Load<Texture2D>("Planet1");
            planetSprite2 = Content.Load<Texture2D>("Planet2");
            spaceSprite = Content.Load<Texture2D>("space");
            bulletSprite = Content.Load<Texture2D>("bullet");

            gameFont = Content.Load<SpriteFont>("spaceFont");
            timerFont = Content.Load<SpriteFont>("timerFont");

            collisionSound = Content.Load<SoundEffect>("collisionSound");
            asteroidSound = Content.Load<SoundEffect>("asteroidSound");
            shootingSound = Content.Load<SoundEffect>("shootingSound");

            // Initialize player
            player = new Ship(shipSprite, Content);

            // Spawn two planets with initial health
            planets.Add(new Planet(new Vector2(800, 200), 100, 100)); // Position (800, 200), health = 100
            planets.Add(new Planet(new Vector2(1100, 600), 150, 120)); // Position (1100, 600), health = 150
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (isGameOver)
                return;

            // Player movement and shooting
            KeyboardState keyboardState = Keyboard.GetState();
            player.MoveShip(keyboardState);

            if (keyboardState.IsKeyDown(Keys.Space)) // Fire bullet
            {
                bullets.Add(new Bullet(player.position, bulletSprite, 10)); // Bullet speed = 10
                shootingSound.Play();
            }

            // Update bullets
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Update();

                // Remove bullets that go off-screen
                if (bullets[i].position.X > _graphics.PreferredBackBufferWidth)
                {
                    bullets.RemoveAt(i);
                    i--;
                }
            }

            // Update asteroids
            for (int i = 0; i < asteroids.Count; i++)
            {
                asteroids[i].Update();

                // Check collisions with bullets
                for (int j = 0; j < bullets.Count; j++)
                {
                    if (Vector2.Distance(asteroids[i].position, bullets[j].position) < 30) // Collision range
                    {
                        asteroids[i].TakeDamage(25); // Reduce asteroid health by 25
                        bullets.RemoveAt(j);
                        j--;

                        if (asteroids[i].health <= 0)
                        {
                            asteroids.RemoveAt(i);
                            i--;
                            score += 10;
                            asteroidSound.Play();
                        }
                    }
                }

                // Remove asteroids that go off-screen
                if (asteroids[i].position.X < 0)
                {
                    asteroids.RemoveAt(i);
                    i--;
                }
            }

            // Update planets
            foreach (var planet in planets)
            {
                for (int j = 0; j < bullets.Count; j++)
                {
                    if (Vector2.Distance(planet.position, bullets[j].position) < 50) // Collision range
                    {
                        planet.TakeDamage(20); // Reduce planet health by 20
                        bullets.RemoveAt(j);
                        j--;

                        if (planet.health <= 0)
                        {
                            // Handle planet destruction logic here
                            collisionSound.Play();
                            isGameOver = true; // Game over if a planet is destroyed
                        }
                    }
                }
            }

            // Spawn new asteroids
            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime.TotalSeconds >= asteroidSpawnInterval)
            {
                SpawnAsteroid();
                elapsedTime = TimeSpan.Zero;
            }

            base.Update(gameTime);
        }

        private void SpawnAsteroid()
        {
            float x = _graphics.PreferredBackBufferWidth;
            float y = randomGenerator.Next(0, _graphics.PreferredBackBufferHeight);

            int speed = randomGenerator.Next(2, 6);
            int health = randomGenerator.Next(50, 100); // Random health for asteroids
            Asteroid newAsteroid = new Asteroid(speed, health) { position = new Vector2(x, y) };
            asteroids.Add(newAsteroid);
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

            // Draw asteroids and their health bars
            foreach (var asteroid in asteroids)
            {
                asteroid.Draw(_spriteBatch, asteroidSprite);
            }

            // Draw planets and their health bars
            foreach (var planet in planets)
            {
                planet.Draw(_spriteBatch, planetSprite1);
            }

            // Draw score
            _spriteBatch.DrawString(timerFont, "Score: " + score, new Vector2(20, 20), Color.White);

            if (isGameOver)
            {
                _spriteBatch.DrawString(gameFont, "Game Over!", new Vector2(500, 500), Color.Red);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
