using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Group2FinalProject
{
	internal class Star
	{
		public Vector2 position;
		private int speed;
		public int value = 1; // Value of star for score increment
		static public int radius = 5;


		public Star(int speed)
		{
			this.speed = speed;
		}
		public void updateStar()
		{ 
			position.X -= speed;
		}
	}
}
