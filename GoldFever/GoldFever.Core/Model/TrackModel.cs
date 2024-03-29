﻿using GoldFever.Core.Generic;
using GoldFever.Core.Track;
using System;

namespace GoldFever.Core.Model
{
    public sealed class TrackModel
    {
        #region Properties

        public TrackType Type { get; set; }
        public Vector Position { get; set; }
        public Direction Direction { get; set; }
        public ConsoleKey Key { get; set; }

        #endregion
    }
}
