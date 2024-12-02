using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Group2FinalProject
{
	internal class Asteroid
	{
		public Vector2 position;
		private int speed;
		static public int radius = 60;

		public Asteroid(int speed)
		{
			this.speed = speed;
		}

		public void updateAsteroid()
		{
			position.X -= speed;
		}
	}
}
