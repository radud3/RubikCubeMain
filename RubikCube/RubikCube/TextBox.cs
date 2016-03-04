﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys; /*
using System.Of[A].Down; */

namespace RubikCube
{
    class TextBox
    {
        public string textbox = ""; // the entirety of what u wrote
        public string drawBox = ""; // what u see on screen
        int drawTab = 0; // physical place of tab in vector.x
        string physTab = ""; // the flashing tab
        public int movedTo = 0; // how much u moved to the right
        int timeSincePress = 0; // time passed since last click
        int uselessWordSize = 0; //the vector.x size of "Text: "
        int timeSinceLetterPress = 0;//time passed since you started pressing a letter
        int tabTimer = 0; //timer of the tab
        Keys oldKey = Keys.F24; //old key last pressed
        public int boxSize = 20; //the boxsize in letters, not in vector.x!!! this size changes depending on the string vector.x size!
        public const int realBoxSize = 266; //the real box size in vectors.x, const!
        public int tabPlace = 0; //the logical place of the tab in the string
        public TextBox()
        {
        }
        public void Update(KeyboardState state, KeyboardState oldState, GameTime gameTime, SpriteFont font)
        {
            if (state.IsKeyDown(Keys.D1) && oldState.IsKeyUp(Keys.D1))
                Debug.WriteLine("");
            //34
            uselessWordSize = (int)(font.MeasureString("Text: ")).X;
            CheckForDeviation(font);
            tabTimer += gameTime.ElapsedGameTime.Milliseconds % 1000;
            //(Keys)(Enum.Parse(typeof(Keys), "A"));
            string usedKeys = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < usedKeys.Length; i++)
            {
                CheckForClick(ref state, ref oldState, gameTime, (Keys)(Enum.Parse(typeof(Keys), usedKeys.Substring(i, 1)))); //converts string letter from usedKeys to Keys, and send to cheak.
            }
            CheckForClick(ref state, ref oldState, gameTime, Keys.Space);
            CheckForClick(ref state, ref oldState, gameTime, Keys.OemSemicolon); //nope
            if ((state.IsKeyDown(Keys.Enter)) && (oldState.IsKeyUp(Keys.Enter)))
            {
                textbox = textbox.Insert(movedTo + tabPlace, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            }
            //checks if one of the keys that count how much time has passed since you pressed them have been...um...Un-pressed?
            if ((CheakForKeyChange(state, oldState, Keys.Right)) || (CheakForKeyChange(state, oldState, Keys.Left)) || (CheakForKeyChange(state, oldState, Keys.Back)) || (CheakForKeyChange(state, oldState, Keys.Space)))
            {
                timeSincePress = 0;
                oldKey = Keys.F24;
                tabTimer = 0;
            }
            if (CheakForKeyChange(state, oldState, oldKey))
            {
                timeSinceLetterPress = 0;
                oldKey = Keys.F24;
                tabTimer = 0;
            }

            if ((state.IsKeyDown(Keys.Back)) && (textbox.Length > 0))
            {
                timeSincePress += gameTime.ElapsedGameTime.Milliseconds;
                if ((oldState.IsKeyUp(Keys.Back) || (timeSincePress > 250)))
                {
                    if (timeSincePress > 250)
                    {
                        timeSincePress = 225;
                    }
                    if ((tabPlace + movedTo) > 0)
                    {
                        if (textbox.Length <= boxSize)
                        {
                            tabPlace--;
                        }
                        else if ((MovedToRight() > 0 && (movedTo > 0)))
                        {
                            if (tabPlace > 0)
                            {
                                tabPlace--;
                            }
                            else
                            {
                                movedTo--;
                            }
                        }
                        else if (MovedToRight() > 0 && (movedTo == 0))
                        {
                            tabPlace--;
                        }
                        else if ((MovedToRight() == 0) && (movedTo > 0))
                        {
                            movedTo--;
                        }
                        textbox = textbox.Remove(movedTo + tabPlace, 1);
                        tabTimer = 0;
                    }
                    else
                    {
                        Console.Beep();
                    }

                }
            }
            else if (state.IsKeyDown(Keys.Right))
            {
                timeSincePress += gameTime.ElapsedGameTime.Milliseconds;
                if ((oldState.IsKeyUp(Keys.Right) || (timeSincePress > 250)))
                {
                    if (!((tabPlace == boxSize) && (MovedToRight() == 0)))
                    {
                        if (timeSincePress > 250)
                        {
                            timeSincePress = 225;
                        }
                        if ((tabPlace == boxSize) && (MovedToRight() > 0))
                        {
                            movedTo++;
                        }
                        else if ((tabPlace < boxSize) && (textbox.Length - tabPlace > 0))
                        {
                            tabPlace++;
                        }
                        tabTimer = 0;
                    }
                    else
                    {
                        Console.Beep();
                    }
                }
            }
            else if (state.IsKeyDown(Keys.Left))
            {
                timeSincePress += gameTime.ElapsedGameTime.Milliseconds;
                if ((oldState.IsKeyUp(Keys.Left) || (timeSincePress > 250)))
                {
                    if (movedTo + tabPlace > 0)
                    {
                        if (timeSincePress > 250)
                        {
                            timeSincePress = 225;
                        }
                        if (tabPlace > 0)
                        {
                            tabPlace--;
                        }
                        if ((tabPlace == 0) && (movedTo > 0))
                        {
                            movedTo--;
                        }
                        tabTimer = 0;
                    }
                    else
                    {
                        Console.Beep();
                    }

                }
            }
            if (textbox.Length > boxSize && (movedTo + boxSize + MovedToRight()) == textbox.Length)
            {
                drawBox = textbox.Substring(movedTo, boxSize);
            }
            else
            {
                drawBox = textbox;
            }

            //}
            //else if (drawBox.Length < (boxSize - 1))
            //{
            //    if (font.MeasureString(textbox.Substring(movedTo, drawBox.Length + 1)).X <= realBoxSize)
            //    {
            //        boxSize++;
            //    }

            //}
            CheckForDeviation(font);

            if (textbox.Length > boxSize && (movedTo + boxSize + MovedToRight()) == textbox.Length)
            {
                drawBox = textbox.Substring(movedTo, boxSize);
            }
            else
            {
                drawBox = textbox;
            }
            if (tabTimer < 500)
            {
                physTab = "|";
            }
            else
            {
                physTab = "";
            }
            if (tabTimer >= 1000)
            {
                tabTimer = 0;
            }
            oldState = state;

            //END OF UPDATE
        }

        private void CheckForDeviation(SpriteFont font)
        {
            Vector2 boxVector = (font.MeasureString(drawBox));
            if (boxVector.X >= realBoxSize)
            {
                if (drawBox.Length > 0)
                {
                    if (drawBox.Last<char>() == 'M')
                        Debug.WriteLine("");
                }
                int check = 1;
                Debug.WriteLine((font.MeasureString(drawBox.Substring(0, drawBox.Length - check)).X));
                while ((font.MeasureString(drawBox.Substring(0, drawBox.Length - check)).X) >= realBoxSize)
                {
                    Debug.WriteLine("size of string" + font.MeasureString(drawBox.Substring(0, drawBox.Length - check)).X);
                    if (tabPlace >= boxSize)
                    {
                        tabPlace--;
                    }
                    boxSize--;
                    movedTo++;
                    check++;
                }
            }
            else if ((drawBox.Length >= boxSize))
            {
                boxSize++;
            }
        }

        private int MovedToRight()
        {
            if ((textbox.Length - boxSize - movedTo) <= 0)
            {
                return 0;
            }
            else
                Debug.Write("");
            return textbox.Length - boxSize - movedTo;
        }
        private void CheckForClick(ref KeyboardState keyboardState, ref KeyboardState oldKeyboardState, GameTime gameTime, Keys key)
        {
            if (keyboardState.IsKeyDown(key))
            {
                timeSinceLetterPress += gameTime.ElapsedGameTime.Milliseconds;
                if (key != oldKey)
                {
                    timeSinceLetterPress = 0;
                }
                oldKey = key;
                if ((oldKeyboardState.IsKeyUp(key)) || (timeSinceLetterPress > 250))
                {
                    tabTimer = 0;
                    if (timeSinceLetterPress > 250)
                    {
                        timeSinceLetterPress = 225;
                    }
                    textbox = textbox.Insert(movedTo + tabPlace, KeyToChar(key, keyboardState, oldKeyboardState));
                    if (tabPlace < boxSize)
                    {
                        if (tabPlace >= 52)
                            Debug.Write("");
                        tabPlace++;
                    }
                    else
                    {
                        if (tabPlace >= 52)
                            Debug.Write("");
                        tabPlace = boxSize;
                        movedTo++;
                    }
                }
            }
        }
        private bool CheakForKeyChange(KeyboardState keyboardState, KeyboardState oldKeyboardState, Keys key)
        {
            if (((keyboardState.IsKeyUp(key)) && (oldKeyboardState.IsKeyDown(key))) || (keyboardState.IsKeyDown(key) && oldKeyboardState.IsKeyUp(key)))
            {
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            //265
            // should it get to the if before this error
            Vector2 tabVector = (font.MeasureString("Text: " + drawBox.Substring(0, tabPlace)));
            tabVector = new Vector2((tabVector.X + 295), (375));
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "DraBoxSize.X " + font.MeasureString(drawBox).X, new Vector2(300, 325), Color.Black);
            spriteBatch.DrawString(font, "BoxMax.X " + realBoxSize, new Vector2(475, 325), Color.Black);
            spriteBatch.DrawString(font, ("TabX: " + (tabVector.X - uselessWordSize)), new Vector2(300, 350), Color.Black);
            if (drawBox.Length > 0)
            {
                if (drawBox.Last<char>() == 'M')
                    Debug.WriteLine("");
            }
            spriteBatch.DrawString(font, ("Text: " + drawBox), new Vector2(300, 375), Color.Black);
            spriteBatch.DrawString(font, ("|"), new Vector2(300 + realBoxSize + uselessWordSize, 375), Color.Red);
            spriteBatch.DrawString(font, (physTab), tabVector, Color.Black);
            spriteBatch.DrawString(font, ("Length: " + textbox.Length), new Vector2(300, 400), Color.Black);
            spriteBatch.DrawString(font, ("BoxSize: " + boxSize), new Vector2(450, 400), Color.Black);
            spriteBatch.DrawString(font, ("MovedTo: " + movedTo), new Vector2(300, 425), Color.Black);
            spriteBatch.DrawString(font, ("MovedToRight: " + MovedToRight()), new Vector2(450, 425), Color.Black);
            spriteBatch.DrawString(font, ("TabPlace: " + tabPlace), new Vector2(300, 450), Color.Black);
            spriteBatch.End();
        }
        public string KeyToChar(Keys key, KeyboardState state, KeyboardState oldstate)
        {

            if (key == Keys.Space)
            {
                return " ";
            }
            if (key == Keys.OemSemicolon)
            {
                return "\'";
            }
            if (key.ToString().Length == 1)
            {
                return (key.ToString());
            }

            return "";
        }
    }
}
