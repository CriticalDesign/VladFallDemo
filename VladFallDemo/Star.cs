using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace VladFallDemo
{
    //This is a (relatively) straightforward class that describes a star object. Most
    //of the attributes are to make the star seem more dynamic.
    internal class Star
    {

        //star attributes
        private float _fallSpeed;   //how fast will we fall?
        private float _scale, _angle, _rotation; //scale factor, starting angle, per-step rotation
        private float _x, _y;   //x and y coordinates on the screen
        private Color _color;   //star color
        private Texture2D _starTexture;  //star sprite
        private Random _rng;  //I love RNG!!

        //argumented constructor - note: some arguments,  some hard-coded features inside.
        public Star(float x, float y, float scale, float fallSpeed, Texture2D starTexture)
        {
            //incoming arguments that are set by the game engine
            _fallSpeed = fallSpeed;
            _scale = scale;
            _x = x;
            _y = y;
            _starTexture = starTexture;

            //hard coded for ease...
            _rng = new Random();    //this is for the random attributes that are set inside the star and
                                    //not passed as arguments.
            
            //colors are made up of RGB components. Each color has Red, Green, and Blue values between 0 and 255.
            //By setting the colors components to values from 128 and up, we are ensuring that the stars stay on
            //the bright side of the color wheel.
            _color = new Color(_rng.Next(128, 256), _rng.Next(128, 256), _rng.Next(128, 256));
            
            //random starting angle between 0 and 1 - we typecast it because there's no NextFloat() method in Random.
            _angle = (float)_rng.NextDouble();

            //let's rotate by a small amount each time step - negative for left rotation, positive for right.
            //This creates a small negative to positive number.
            _rotation = ((float)_rng.Next(-200,201))/1000f;
        }

        //all we do when updating (per step) is change our angle by the rotation amount and fall by changing our y coordinate.
        public void Update()
        {
            _angle += _rotation;
            _y += _fallSpeed;
        }

        //we need an accessor for Y so that we can tell when stars have fallen off the screen.
        public float getY() { return _y; }

        //an accessor for X so that we can access this attribute if needed.
        public float getX() { return _x; }

        //we need the star's bounds so that we can tell if we've clicked on it or not.
        public Rectangle getBounds()
        {
            //looks complicated but what we're doing is figuring out the stars center, and creating a rectangle that 
            //covers the height and width of the star. We also need the scale factor so that our "collision box" is the right
            //size with the scaling.
            return new Rectangle((int)(_x - _starTexture.Width / 2), (int)(_y - _starTexture.Height / 2), (int)(_starTexture.Width*_scale), (int)(_starTexture.Height*_scale));
        }

        //a colour accessor to access our color
        public Color getColor() { return _color; }

        //an accessor to get our angle
        public float getAngle() { return _angle; }

        //an accessor to get our scale
        public float getScale() { return _scale; }

        //finally, use the complex draw method to draw, scale, and rotate the stars.
        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_starTexture, new Vector2(_x, _y), null, _color, _angle, new Vector2(_starTexture.Width/2, _starTexture.Height/2), new Vector2(_scale, _scale), SpriteEffects.None, 0);
            _spriteBatch.End();
        }
    }
}
