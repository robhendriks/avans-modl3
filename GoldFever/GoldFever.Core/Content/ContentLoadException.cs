﻿using System;

namespace GoldFever.Core.Content
{
    public class ContentLoadException : GameException
    {
        #region Constructors

        public ContentLoadException()
            : base()
        {

        }

        public ContentLoadException(string fileName)
            : this(fileName, null)
        {

        }

        public ContentLoadException(string fileName, Exception inner)
            : base($"Could not load resource {fileName}.", inner)
        {

        }

        #endregion
    }
}
