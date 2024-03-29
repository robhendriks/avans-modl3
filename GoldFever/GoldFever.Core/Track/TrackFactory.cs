﻿using GoldFever.Core.Model;
using System;
using System.Collections.Generic;

namespace GoldFever.Core.Track
{
    public sealed class TrackFactory
    {
        #region Static fields

        private static TrackFactory _instance;

        #endregion


        #region Properties

        private Dictionary<TrackType, Type> _bindings;

        public Dictionary<TrackType, Type> Bindings
        {
            get { return _bindings; }
        }

        #endregion


        #region Constructors

        private TrackFactory()
        {
            _bindings = new Dictionary<TrackType, Type>();

            _bindings.Add(TrackType.Default, typeof(DefaultTrack));
            _bindings.Add(TrackType.Start, typeof(StartTrack));
            _bindings.Add(TrackType.End, typeof(EndTrack));
            _bindings.Add(TrackType.SwitchIn, typeof(SwitchInTrack));
            _bindings.Add(TrackType.SwitchOut, typeof(SwitchOutTrack));
            _bindings.Add(TrackType.Drop, typeof(DropTrack));
            _bindings.Add(TrackType.Hold, typeof(HoldTrack));
        }

        #endregion


        #region Methods

        public BaseTrack Create(TrackModel data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Type type;
            if (!_bindings.TryGetValue(data.Type, out type))
                throw new TypeLoadException($"Track type {data.Type} has no binding.");

            object[] args;

            if (type.IsSubclassOf(typeof(SwitchTrack)))
                args = new object[] { data.Position, data.Direction, data.Key };
            else
                args = new object[] { data.Position, data.Direction };


            return (BaseTrack)Activator.CreateInstance(type, args);
        }

        public BaseTrack[] Create(LevelModel data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            else if (data.Tracks == null)
                throw new NullReferenceException("Track array cannot be null.");

            var results = new List<BaseTrack>();

            foreach(var track in data.Tracks)
                results.Add(Create(track));

            return results.ToArray();
        }

        #endregion


        #region Static Methods

        public static TrackFactory GetInstance()
        {
            return _instance ?? (_instance = new TrackFactory());
        }

        #endregion
    }
}
