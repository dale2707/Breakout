///////////////////////////////////////////////////////////////////////////////
// PORTFOLIO ASSIGNMENT 2
//
// To complete this assignment:
//
// 1) Declare an array of GameSprites for the bricks and initialise them all.
// 2) Use a loop in the Draw method and draw them inside that loop.
// 3) Use another loop in Update that will check each brick for collision.
// 4) Upon collision with a brick...
//   4a) Gain points
//   4b) Remove the brick from play (eg by moving it off-screen)
//   4c) Make the ball respond to the collision (eg by bouncing toward ground)
// 5) Keep count of bricks destroyed.  When all are gone, restart level.
// 6) Add suitable comments to every non-trivial block of code you added
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using GameLibrary;
using UIControls;

namespace BrickBreaker
{
    ///////////////////////////////////////////////////////////////////////////
    // SHARED DATA: PUT VARIABLES HERE TO BE USED ACROSS DIFFERENT SCREENS
    // Remember to make them 'public' and 'static'.
    ///////////////////////////////////////////////////////////////////////////
    public static class sharedData
    {
        public static bool soundsOn = true;
        public static bool musicOn = true;
    }

    public class InGameScreen : Screen
    {
        ///////////////////////////////////////////////////////////////////////
        // DECLARATIONS
        ///////////////////////////////////////////////////////////////////////

        // content objects
        Texture2D batImage, ballImage, brickImage;
        SoundEffect bounceSound, pointScoredSound, loseSound;
        SoundEffectInstance bounceSoundInst;

        // gameplay objects
        GameSprite bat, ball;
        GameSprite[] bricks = new GameSprite[40];

        const float FRICTION_COEFFICIENT = 0.8f;

        int lives;
        int score;
        int highscore = 0;
        int destroyed;

        ///////////////////////////////////////////////////////////////////////
        // INITIALIZE THE SCREEN
        ///////////////////////////////////////////////////////////////////////
        public override void Initialize(ScreenManager mgr)
        {
            base.Initialize(mgr);
            batImage = Content.Load<Texture2D>("bat");
            ballImage = Content.Load<Texture2D>("ball");
            brickImage = Content.Load<Texture2D>("brick-bw");
            bricks[0] = new GameSprite(brickImage, new Vector2(0,0));
            bricks[0].image = brickImage;
           
            
            for(int brickNumber = 0; brickNumber < 10; brickNumber+=1)
            {
                bricks[brickNumber] = new GameSprite(brickImage, new Vector2(brickNumber * 100+60,100));
            }
            for (int brickNumber = 10; brickNumber < 20; brickNumber += 1)
            {
                bricks[brickNumber] = new GameSprite(brickImage, new Vector2(brickNumber * 100 - 940, 150));
            }
            for (int brickNumber = 20; brickNumber < 30; brickNumber += 1)
            {
                bricks[brickNumber] = new GameSprite(brickImage, new Vector2(brickNumber * 100 - 1940, 200));
            }
            for (int brickNumber = 30; brickNumber < 40; brickNumber += 1)
            {
                bricks[brickNumber] = new GameSprite(brickImage, new Vector2(brickNumber * 100 - 2940, 250));
            }
          

            bat = new GameSprite(batImage, Vector2.Zero);
            ball = new GameSprite(ballImage, Vector2.Zero);
            bounceSound = Content.Load<SoundEffect>("boinggg");
            bounceSoundInst = bounceSound.CreateInstance();
            pointScoredSound = Content.Load<SoundEffect>("break crate");
            loseSound = Content.Load<SoundEffect>("fail");
            ResetLevel();
        }


        ///////////////////////////////////////////////////////////////////////
        // START A NEW GAME
        ///////////////////////////////////////////////////////////////////////
        public override void Start()
        {
            lives = 3;
            score = 0;
            ResetLevel();
        }

        private void ResetLevel()
        {
            ResetBat();
            ResetBall();
            ResetBricks();
        }
        private void ResetBall()
        {
            ball.position = new Vector2(screenWidth / 2, screenHeight / 2);
            ball.velocity = new Vector2(5, -4);
        }

        private void ResetBat()
        {
            bat.position = new Vector2(screenWidth / 2, 
                screenHeight - batImage.Height / 2);
            bat.velocity = new Vector2(0, 0);
        }

        //Reset the bricks upon all being removed.
        
        private void ResetBricks()
        {
            for (int brickNumber = 0; brickNumber < 10; brickNumber += 1)
            {
                bricks[brickNumber] = new GameSprite(brickImage, new Vector2(brickNumber * 100 + 60, 100));
            }
            for (int brickNumber = 10; brickNumber < 20; brickNumber += 1)
            {
                bricks[brickNumber] = new GameSprite(brickImage, new Vector2(brickNumber * 100 - 940, 150));
            }
            for (int brickNumber = 20; brickNumber < 30; brickNumber += 1)
            {
                bricks[brickNumber] = new GameSprite(brickImage, new Vector2(brickNumber * 100 - 1940, 200));
            }
            for (int brickNumber = 30; brickNumber < 40; brickNumber += 1)
            {
                bricks[brickNumber] = new GameSprite(brickImage, new Vector2(brickNumber * 100 - 2940, 250));
            }
        }


        ///////////////////////////////////////////////////////////////////////
        // UPDATE THE GAME WORLD
        ///////////////////////////////////////////////////////////////////////
        public override void Update(GameTime gameTime)
        {
            MoveBat();
            MoveBall();
            BatBallCollisionCheck();
            BrickBallCollisionCheck();

            if (score > highscore)
            {
                highscore = score;  // keep high score up to date
            }

            if (lives <= 0)
            {
                manager.GoToScreen("gameOver");
            }

            base.Update(gameTime);
        }

        private void BatBallCollisionCheck()
        {
            if (ball.collision(bat))
            {
                // find the current speed of the ball, and speed it up a bit
                float newspeed = ball.velocity.Length() * 1.025f;
                
                // next, get find an angle it bounces off based on how far
                // along the bat the ball hit.
                float maxDistance = ball.origin.X + bat.origin.X;
                float newAngle = (ball.position.X - bat.position.X) /
                    maxDistance - (float)Math.PI / 2;
                
                // turn angle & speed into a Vector with X and Y values
                ball.velocity = new Vector2((float)Math.Cos(newAngle),
                    (float)Math.Sin(newAngle));
                ball.velocity *= newspeed;

                if (sharedData.soundsOn)
                    bounceSoundInst.Play(); // use instance to avoid distortion
            }
        }

        private void BrickBallCollisionCheck()
        {

            for (int brickNumber = 0; brickNumber < 40; brickNumber += 1)
            {
                if (ball.collision(bricks[brickNumber]))
                {
                    score += 100;
                    bricks[brickNumber].position.X = -1000;
                    ball.velocity.Y = Math.Abs(ball.velocity.Y);
                    destroyed += 1;
                    if (destroyed == 40)
                        ResetLevel();
                }
            }
            

           

            ///////////////////////////////////////////////////////////////////
            // TO DO: ADD YOUR CODE TO PERFORM COLLISION CHECK HERE
            ///////////////////////////////////////////////////////////////////
        }

        private void MoveBall()
        {
            ball.position += ball.velocity;             // move ball
            ball.rotation += ball.velocity.X * 0.02f;   // spin it

            // collision at left or right of screen
            if (ball.position.X < ball.origin.X)
            {
                ball.velocity.X *= -1;
                if (sharedData.soundsOn)
                    bounceSound.Play();
            }
            
            if(ball.position.X > screenWidth - ball.origin.X)
            {
                ball.velocity.X *= -1;
                ball.position.X = screenWidth - ball.origin.X;
                if (sharedData.soundsOn)
                    bounceSound.Play();
            }

            if (ball.position.X > ball.origin.X)
            {
                ball.velocity.X *= 1;
                if (sharedData.soundsOn)
                    bounceSound.Play();
            }


            // collision at top of screen
            if (ball.position.Y < ball.origin.Y)
            {
                ball.velocity.Y *= -1;
                if (sharedData.soundsOn)
                    bounceSound.Play();
            }

            // collision at bottom of screen
            if (ball.position.Y > screenHeight + ball.origin.Y)
            {
                ResetBall();
                lives--;
                if (sharedData.soundsOn)
                    loseSound.Play();
            }
        }

        private void MoveBat()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) bat.velocity.X -= 3;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) bat.velocity.X += 3;
            bat.velocity *= FRICTION_COEFFICIENT;
            bat.position += bat.velocity;
            if (bat.position.X < bat.origin.X) bat.position.X = bat.origin.X;
            if (bat.position.X > screenWidth - bat.origin.X) bat.position.X =
                screenWidth - bat.origin.X;
        }

        ///////////////////////////////////////////////////////////////////////
        // DRAW THE SCREEN
        ///////////////////////////////////////////////////////////////////////
        public override void Draw(GameTime gameTime)
        {
            DrawText.AlignedScaledAndRotated("score: " + score,
                HorizontalAlignment.Left, VerticalAlignment.Top,
                0, 0, 0, 0.5f, Color.White, manager.defaultFont);
            DrawText.AlignedScaledAndRotated("lives: " + lives,
                HorizontalAlignment.Right, VerticalAlignment.Top,
                1, 0, 0, 0.5f, Color.White, manager.defaultFont);
            DrawText.AlignedScaledAndRotated("Hiscore: " + highscore,
                HorizontalAlignment.Center, VerticalAlignment.Top,
                0.5f, 0, 0, 0.5f, Color.White, manager.defaultFont);
            
            bat.Draw(spriteBatch);
           
            for (int brickNumber = 0; brickNumber < 40; brickNumber += 1)
            {
                bricks[brickNumber].Draw(spriteBatch);
            }

            ///////////////////////////////////////////////////////////////////
            // TO DO: ADD YOUR CODE TO DO DRAW BRICKS HERE
            ///////////////////////////////////////////////////////////////////

            // only draw the ball if you are playing the game right now
            // so if this screen is drawn under another, the ball is hidden
            if(manager.ActiveScreen == this)
                ball.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
