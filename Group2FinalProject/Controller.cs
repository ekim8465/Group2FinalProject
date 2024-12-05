﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Group2FinalProject
{
	internal class Controller
	{
		private TimeSpan elapsedTime;
		private int secondsElapsed;

		private int score;

		public Controller()
		{
			elapsedTime = TimeSpan.Zero;
			secondsElapsed = 0;
			score = 0;
		}

		//Update the timer and return the seconds elapsed 
		public int updateTime(GameTime gameTime)
		{
			elapsedTime += gameTime.ElapsedGameTime;

			if (elapsedTime.TotalSeconds >= 1) //누적된 경과 시간을 초단위로 변환
			{
				secondsElapsed++;
				elapsedTime = TimeSpan.Zero;
			}

			return secondsElapsed; //Return the current seconds count 
		}

		public bool didCollisionHappen(Ship player, Asteroid asteroid)
		{
			int playerRadius = player.GetRadius();
			int astRadius = Asteroid.radius;
			int distance = playerRadius + astRadius;

			//충돌 여부만 판단 
			return Vector2.Distance(player.position, asteroid.position) < distance;
		}

		public bool didCollectHappen(Ship player, Star star)
		{
			int playerRadius = player.GetRadius();
			int starRadius = Star.radius;
			int distance = playerRadius + starRadius;

			//충돌 여부만 판단 
			return Vector2.Distance(player.position, star.position) < distance;
		}



		public String gameEndScript()
		{
			string gameEndMessage = "Game Lost!! Spaceship hit by Asteroid";
			elapsedTime = TimeSpan.Zero;
			secondsElapsed = 0;
			return gameEndMessage;
		}

		public String gameWinEndScript(int score)
		{
			string gameWinEndMessage = $"Game Won, You surpassed {score} asteroids";
			elapsedTime = TimeSpan.Zero;
			secondsElapsed = 0;
			return gameWinEndMessage;
		}
	}
}

