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

		// Texture 및 Sound 관련 변수들
		Texture2D shipSprite;
		Texture2D asteroidSprite;
		Texture2D spaceSprite;
		Texture2D starSprite;
		SpriteFont gameFont;
		SpriteFont timerFont;

		SoundEffect collisionSound;
		SoundEffect boostSound;

		Ship player = new Ship();
		List<Asteroid> asteroids = new List<Asteroid>();
		List<Star> stars = new List<Star>();

		Random randomAsteroids = new Random();
		Random randomStars = new Random();

		private TimeSpan elapsedTime;
		private TimeSpan starElapsedTime; // 별 생성 시간 관리 변수 추가
		private int secondsElapsed;
		private int score;

		private double asteroidSpawnInterval = 2; // 소행성 생성 시간 간격
		private double starSpawnInterval = 2; // 별 생성 시간 간격

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
			starElapsedTime = TimeSpan.Zero; // 별 생성 시간을 초기화
			secondsElapsed = 0;
			score = 0;

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// 텍스처 및 사운드 파일 로드
			shipSprite = Content.Load<Texture2D>("ship");
			asteroidSprite = Content.Load<Texture2D>("asteroid");
			spaceSprite = Content.Load<Texture2D>("space");
			starSprite = Content.Load<Texture2D>("star");

			gameFont = Content.Load<SpriteFont>("spaceFont");
			timerFont = Content.Load<SpriteFont>("timerFont");

			collisionSound = Content.Load<SoundEffect>("collisionSound");
			boostSound = Content.Load<SoundEffect>("boostSound");
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

			// 소행성 업데이트 및 충돌 처리
			for (int i = 0; i < asteroids.Count; i++)
			{
				var asteroid = asteroids[i];
				asteroid.updateAsteroid();

				if (controller.didCollisionHappen(player, asteroid))
				{
					collisionSound.Play();
					score -= 3;

					asteroids.RemoveAt(i);
					i--; // 소행성 인덱스 수정
						 //continue;
				}

				// 화면 밖으로 나가면 소행성 제거
				if (asteroid.position.X < 0)
				{
					asteroids.RemoveAt(i);
					i--;
				}
			}

			// 별 생성 및 점수 처리
			for (int i = 0; i < stars.Count; i++)
			{
				var star = stars[i];
				star.updateStar();

				if (controller.didCollectHappen(player, star))
				{
					score += 3;
					stars.RemoveAt(i); // 별을 제거
					i--; // 별 인덱스 수정


					continue;
				}

				// 화면 밖으로 나가면 별 제거
				if (star.position.X < 0)
				{
					stars.RemoveAt(i); // 별을 제거
					i--;
				}
			}

			elapsedTime += gameTime.ElapsedGameTime;
			starElapsedTime += gameTime.ElapsedGameTime; // 별 생성 시간을 업데이트


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

			// 소행성 생성
			if (elapsedTime.TotalSeconds >= asteroidSpawnInterval)
			{
				SpawnAsteroid();
				elapsedTime = TimeSpan.Zero;
			}

			// 별 생성
			if (starElapsedTime.TotalSeconds >= starSpawnInterval)
			{
				SpawnStar();
				starElapsedTime = TimeSpan.Zero; // 별 생성 시간 초기화
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
			// 별은 화면 오른쪽에서 왼쪽으로 이동하도록 설정
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

				if (Vector2.Distance(player.position, star.position) < 30) // 별 수집 범위
				{
					score += star.value;
					isBoostAvailable = true;
					boostSound.Play();
					stars.RemoveAt(i);
					i--;
				}
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			_spriteBatch.Begin();

			_spriteBatch.Draw(spaceSprite, new Vector2(0, 0), Color.White);
			_spriteBatch.Draw(shipSprite, new Vector2(player.position.X - shipSprite.Width / 2, player.position.Y - shipSprite.Height / 2), Color.White);

			// 별 그리기
			foreach (var star in stars)
			{
				// 별 위치 그리기
				_spriteBatch.Draw(starSprite, new Vector2(star.position.X - starSprite.Width / 2, star.position.Y - starSprite.Height / 2), Color.White);
			}

			// 소행성 그리기
			foreach (var asteroid in asteroids)
			{
				_spriteBatch.Draw(asteroidSprite, new Vector2(asteroid.position.X - asteroidSprite.Width / 2, asteroid.position.Y - asteroidSprite.Height / 2), Color.White);
			}

			// 점수 표시
			_spriteBatch.DrawString(timerFont, "Score: " + score, new Vector2(_graphics.PreferredBackBufferWidth / 2, 10), Color.White);

			// 타이머 표시
			_spriteBatch.DrawString(timerFont, "Time: " + secondsElapsed, new Vector2(_graphics.PreferredBackBufferWidth / 2, 30), Color.White);

			// 게임 오버/게임 승리 메시지
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
