﻿using GoldFever.Core.Ship;
using System;

namespace GoldFever.Core.Graphics.Terminal
{
    public sealed class FastRenderer : IRenderer
    {
        #region Constants

        const int OffsetX = 4,
                  OffsetY = 8;

        #endregion


        #region Private Fields

        private Game game;
        private DoubleBuffer buffer;

        #endregion


        #region Constructors

        public FastRenderer(Game game)
        {
            this.game = game;
            this.buffer = DoubleBuffer.GetInstance();
        }

        #endregion


        #region Methods

        private void RenderUI()
        {
            var info = new CharInfo();
            info.Attributes = Color.ForegroundGrey | Color.BackgroundBlack;

            var cur = game.Level.Port.Loading;

            if (game.State != GameState.Idle)
            {
                string score = $"{game.Score}".PadLeft(3, '0'),
                       carts = $"{game.Level.Carts.Count}".PadLeft(3, '0'),
                       ship = (cur != null ? $"{cur.Size}/{BaseShip.Capacity}" : "n/a");

                buffer.Write($"Punten: {score}", OffsetX, 2, info);
                buffer.Write($"Karren: {carts}", OffsetX, 3, info);
                buffer.Write($"Boten: {ship}", OffsetX, 4, info);
            }
            else
            {
                buffer.Write("Gepauzeerd", OffsetX, 2, info);
            }
        }

        Random r = new Random();

        private void RenderShips()
        {
            var info = new CharInfo();

            #region Water

            info.Char.AsciiChar = 177;
            info.Attributes = Color.ForegroundDarkCyan | Color.BackgroundDarkBlue;

            int maxWidth = game.Level.Port.Size,
                height = 2,
                x, 
                y = 6;

            for(int i = 0; i < maxWidth; i++)
            {
                x = OffsetX + (i * 2);

                for (int j = 0; j < height; j++)
                    buffer.Write(x, y + j, 2, info);
            }

            #endregion

            #region Ships

            info.Char.AsciiChar = 176;
            info.Attributes = Color.ForegroundYellow | Color.BackgroundDarkYellow;

            int width = BaseShip.Width,
                start,
                end;

            y = 7;

            foreach(var ship in game.Level.Port.Ships)
            {
                start = ship.Index;
                end = ship.Index + width;

                for(int i = start; i < end; i++)
                {
                    if (i < 0 || i >= maxWidth)
                        continue;

                    x = OffsetX + (i * 2);
                    buffer.Write(x, y, 2, info);
                }
            }

            #endregion
        }

        private void RenderTracks()
        {
            CharInfo info;
            int x, y;

            #region Tracks

            foreach(var track in game.Level.Tracks)
            {
                x = OffsetX + (track.X * 2);
                y = OffsetY + track.Y;

                info = new CharInfo();
                info.Attributes = track.Attributes();
                info.Char.AsciiChar = track.Char();

                buffer.Write(x, y, 2, info);
            }

            #endregion
        }

        private void RenderCarts()
        {
            var info = new CharInfo()
            {
                Attributes = Color.ForegroundGreen | Color.BackgroundDarkRed
            };

            int x, y;

            #region Carts

            foreach (var cart in game.Level.Carts)
            {
                if (cart?.Current == null)
                    continue;

                x = OffsetX + (cart.Current.X * 2);
                y = OffsetY + cart.Current.Y;

                info.Char.AsciiChar = cart.Char();
                
                buffer.Write(x, y, 2, info);
            }

            #endregion
        }

        public void Render()
        {
            buffer.Clear();

            RenderUI();
            RenderShips();
            RenderTracks();
            RenderCarts();

            buffer.Draw();
        }

        #endregion
    }
}
