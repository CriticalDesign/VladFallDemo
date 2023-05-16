using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace StarFallDemo
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Variables for our game assets
        private Texture2D _backgroundSprite;
        private Texture2D _starSprite;
        private Texture2D _explosionSprite;
        private Texture2D _gameOverSprite;
        private SpriteFont _gamefont;
        private SoundEffect _popSound;
        private Song _music; //see attribution at the bottom of this file

        //this is a list-based demo, so here's the list that tracks the stars on the screen
        private List<Star> _starList;

        //and another list to track our explosions >:)
        private List<Explosion> _explosionList;

        //this is just a fun way to make the background colour a bit different each time you run the game
        private Color _backgroundColor;

        //some variables we need - timer for when to release a wave, waveCounter for which wave we are on, starsMissed so that the game eventually ends
        private int _timer, _waveCounter, _starsMissed;

        //this is to properly register a single mouse click
        private MouseState _lastMouseState, _currentMouseState;

        //random makes games fun - why does this say System.Random? Well, I was getting an error about namespaces conflicting. Something in the 
        //using lines (at the top) is conflicting with the System libraries, so, I changed this to System.Random to address the issue.
        System.Random _rng;

        //the only thing I changed in here is the resolution of the game. If you change that (800x600), you'll need to make other changes in the code
        //so that the stars spawn across the whole screen.
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();


            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //lots to initialize
            _timer = 0;  //timer starts at zero
            _starList = new List<Star>();  //new empty star list
            _explosionList = new List<Explosion>();  //new empty star list
            _rng = new System.Random();  //new _rng object
            _waveCounter = 0;   //waves starts at zero
            _starsMissed = 0;  //no missed stars to start

            //random background color - by using 0 to 128 the background should stay dark and contrast with the bright stars
            _backgroundColor = new Color(_rng.Next(0, 128), _rng.Next(0, 128), _rng.Next(0, 128));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //load all our assets!!! Notice the different types.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _backgroundSprite = Content.Load<Texture2D>("background");
            _starSprite = Content.Load<Texture2D>("star");
            _explosionSprite = Content.Load<Texture2D>("smallExplosion");
            _gameOverSprite = Content.Load<Texture2D>("gameOver");
            _gamefont = Content.Load<SpriteFont>("GameFont");
            _popSound = Content.Load<SoundEffect>("popSound");
            _music = Content.Load<Song>("song");
            
            //start the music!
            MediaPlayer.Play(_music);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            //most of the game logic follows here.....

            //we only run the game if we have missed less than 5 stars, so most of the game is in this if-statement.
            //this can easily be changed to more or less missed stars.
            if (_starsMissed < 5)
            {
                _timer++; //this updates ever time step.
                if (_timer % 300 == 0)  //every 300 time steps - if there are 60 time steps per second, this means every 5 seconds
                {
                    _waveCounter++;  //another wave is incoming!!!
                    
                    for (int i = 0; i < _waveCounter; i++)  //add "waveCounter" stars to the list (wave 1 = 1 star, wave 10 = 10 stars)
                    {
                        //set up some random fun for the star arguments.
                        float randomXcoord = (float)_rng.Next(50, 550);  //random x position
                        float yCoord = -300;  //constant y coordinate, 100 pixels above the scene
                        float randomScale = (_rng.Next(100, 201) / 100f) + 0.25f; //random scaling factor between 1.25 and 2.25. If the stars are too small, they are hard to click.
                        float randomSpeed = _rng.Next(1, 301) / 100f + 1.5f;  //a random fall speed that is somewhere between 1.5 and about 2.5ish

                        //call the star constructor to add a new star to the list.
                        Star StarToBeAdded = new Star(randomXcoord, yCoord, randomScale, randomSpeed, _starSprite); //note, star sprite comes from the attributes at the top
                        _starList.Add(StarToBeAdded);  //add our new star to the star list!!
                    }

                }

                //for every star in the list
                for (int i = 0; i < _starList.Count; i++)
                {
                    _starList[i].Update();  //update = rotation + fall (see Star class Update())
                    
                    //if we are below the screen's bottom....(screen is 600 pixels high)
                    if (_starList[i].getY() > 625f)
                    {
                        _starsMissed++;  //we've missed a star, track the miss
                        _starList.RemoveAt(i);  //remove the star from the list
                    }
                }

                //Code to record a single mouse click...
                _lastMouseState = _currentMouseState;       // The active state from the last frame is now old
                _currentMouseState = Mouse.GetState();      // Get the mouse state relevant for this frame
                if (_lastMouseState.LeftButton == ButtonState.Released && _currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    //we clicked the mouse, let's see if we clicked on a star.

                    //for every star in the list...(inefficient, but effective)
                    for (int i = 0; i < _starList.Count; i++)
                    {
                        //get the star's bounds (collision box)
                        Rectangle starBounds = _starList[i].getBounds();

                        //if the mouse click coordinates are withing the star's bounds...
                        if (starBounds.Contains(new Vector2(_currentMouseState.X,_currentMouseState.Y)))
                        {
                            //replace the current star with an Explosion object that is at the same locaton. Also pass the colour, scale and angle so that 
                            //the explosion matched the star's properties.
                            _explosionList.Add(new Explosion(3, 3, _starList[i].getX(), _starList[i].getY(), _starList[i].getColor(), _starList[i].getScale(), _starList[i].getAngle(), _explosionSprite));
                            _starList.RemoveAt(i);  //remove the star from the star list
                            _popSound.Play();       //play a cool popping sound
                        }
                    }
                }

                //for each explosion in the explosion list...
                for (int i = 0; i < _explosionList.Count; i++)
                {
                    _explosionList[i].Update(_timer);  //update the explosion's info, pass the time to control the animation speed (cheap hack)
                    
                    //if the explosion's life time has passed (i.e., the animation cycle is complete)
                    if (_explosionList[i].IsExpired())
                        _explosionList.RemoveAt(i);  //remove it from the list.
                }

            }//end if-statement that says we have missed 5 stars


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            DrawBackground();   //helper method to draw the background (see below)

            //for each star in the list...
            foreach (Star star in _starList)
            {
                star.Draw(_spriteBatch);  //...call the draw method
            }

            DrawUI();  //helper method to draw the UI elements

            //oops! we missed 5x stars
            if(_starsMissed >= 5)
            {
                DrawGameOver();  //show game over image
            }

            //for each explosion still in the list, draw what needs drawing
            for (int i = 0; i < _explosionList.Count; i++)
            {
                _explosionList[i].Draw(_spriteBatch);
            }

            base.Draw(gameTime);
        }

        //draw the background with the random background colour
        public void DrawBackground()
        {
            GraphicsDevice.Clear(_backgroundColor);
            _spriteBatch.Begin();
            _spriteBatch.Draw(_backgroundSprite, new Vector2(0, 0), Color.White * 0.65f);   //notice the * 0.65f that makes the starry background image slightly transparent so that 
                                                                                            //we can see the background colour show through.
            _spriteBatch.End();
        }

        //draw the UI elements
        public void DrawUI()
        {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_gamefont, "Wave:", new Vector2(25, 25), Color.White);
            _spriteBatch.DrawString(_gamefont, _waveCounter + "", new Vector2(125, 25), Color.White);

            _spriteBatch.DrawString(_gamefont, "Stars  Missed:", new Vector2(555, 25), Color.White);
            _spriteBatch.DrawString(_gamefont, _starsMissed + "", new Vector2(750, 25), Color.White);
            _spriteBatch.End();
        }

        //draw the game over screen
        public void DrawGameOver()
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_gameOverSprite, new Vector2(400-_gameOverSprite.Width/2, 300-_gameOverSprite.Height/2), Color.White);
            _spriteBatch.End();
        }
    }
}


//Music by <a href="https://pixabay.com/users/sergequadrado-24990007/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=15549">SergeQuadrado</a> from <a href="https://pixabay.com//?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=15549">Pixabay</a>