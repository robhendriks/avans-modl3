﻿using System;
using System.Runtime.InteropServices;

namespace GoldFever.Core.Graphics.Terminal
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Coord
    {
        public short X;
        public short Y;

        public Coord(short X, short Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
