using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace VladFallDemo
{

    //Much of this code come from this MonoGame 2D animation tutorial:
    //http://rbwhitaker.wikidot.com/monogame-texture-atlases-2
    internal class Explosion
    {
     
        private float _scale, _angle; //scale factor, angle
        private float _x, _y;   //x and y coordinates on the screen
        private Color _color;   //star color
        private Texture2D _explosionTexture;  //expolsion sprite sheet (multiple sprites on one image)

        private bool _expired;  //boolean to know when the explosion should be removed from the game

        private int _rows, _cols, _currentFrame, _totalFrames;  //these control the animation properties

        //Explosion constructor...lots to do here, in part because of the animations
        public Explosion(int rows, int columns, float x, float y, Color color, float scale, float angle, Texture2D explosionTexture)
        {

            //all of these attributes will be passed from the star that inspires the explosion
            _x = x;
            _y = y;
            _color = color;
            _scale = scale;
            _angle = angle;

            //explosion sprite
            _explosionTexture = explosionTexture;

            //explosion needs to start "not" expired
            _expired = false;

            //need to know the configuration of the sprite sheet
            _rows = rows;
            _cols = columns;
            _currentFrame = 0;
            _totalFrames = _rows * _cols;
        }


        public void Update(int timeStep)
        {
            //this is a hack to slow down the animation. We pass in the timer from the game engine (Game1.cs)
            //and update the animation every three timesteps, instead of every timestep.
            if (timeStep % 3 == 0)
            {
                //animation stuff
                _currentFrame++;

                //if we've played all the frames in the cycle, we're done with this explosion object. This
                //will let Game1.cs know that we can remove it from the explosion list.
                if (_currentFrame == _totalFrames) {
                    _expired = true;
                }
                
            }
        }

        //quick accessor
        public bool IsExpired() { return _expired; }

        //this is the draw for the animated sprite sheet.
        public void Draw(SpriteBatch spriteBatch)
        {
            int width = _explosionTexture.Width / _cols;
            int height = _explosionTexture.Height / _rows;
            int row = _currentFrame / _cols;
            int column = _currentFrame % _cols;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)_x, (int)_y, (int)(width * _scale), (int)(height * _scale));

            spriteBatch.Begin();
            spriteBatch.Draw(_explosionTexture, destinationRectangle, sourceRectangle, _color, _angle, new Vector2(width/2, height/2), SpriteEffects.None, 0);
            spriteBatch.End();
        }


    }
}
