using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment3_EunmiKim
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

        // Update the timer and return the seconds elapsed
        public int updateTime(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime.TotalSeconds >= 1) // Convert accumulated time to seconds
            {
                secondsElapsed++;
                elapsedTime = TimeSpan.Zero;
            }

            return secondsElapsed; // Return the current seconds count
        }

        // Check if collision happened between the player and asteroid
        public bool didCollisionHappen(Ship player, Asteroid asteroid)
        {
            int playerRadius = player.getRadius();
            int astRadius = Asteroid.radius;
            int distance = playerRadius + astRadius;

            // Check if distance between player and asteroid is less than their combined radius
            return Vector2.Distance(player.position, asteroid.position) < distance;
        }

        // Check if bullet hit an asteroid
        public bool didBulletHitAsteroid(Bullet bullet, Asteroid asteroid)
        {
            float distance = Vector2.Distance(bullet.Position, asteroid.position);
            return distance < Asteroid.radius; // Check if bullet radius is small enough to ignore
        }

        // Game over message
        public string gameEndScript()
        {
            string gameEndMessage = "Game Lost!! Spaceship hit by Asteroid";
            elapsedTime = TimeSpan.Zero;
            secondsElapsed = 0;
            return gameEndMessage;
        }

        // Game win message
        public string gameWinEndScript(int score)
        {
            string gameWinEndMessage = $"Game Won, You surpassed {score} asteroids";
            elapsedTime = TimeSpan.Zero;
            secondsElapsed = 0;
            return gameWinEndMessage;
        }

       
    }
}
