using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Group2FinalProject
{
	internal class Ship
	{
		public Vector2 position = new Vector2(100, 100);
		int baseSpeed = 2;
		int radius = 0;

		// 반지름 설정
		public void setRadius(int radius)
		{
			this.radius = radius;
		}

		public int getRadius()
		{
			return this.radius;
		}

		// 우주선 이동 처리
		public void MoveShip(KeyboardState state, bool boostActive)
		{
			int speed = boostActive ? baseSpeed * 2 : baseSpeed;

			if (state.IsKeyDown(Keys.Left))
			{
				this.position.X -= speed;
			}
			if (state.IsKeyDown(Keys.Right))
			{
				this.position.X += speed;
			}

			if (state.IsKeyDown(Keys.Up))
			{
				this.position.Y -= speed;
			}
			if (state.IsKeyDown(Keys.Down))
			{
				this.position.Y += speed;
			}
		}
	}
}
