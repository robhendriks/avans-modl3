﻿using GoldFever.Core.Content;
using GoldFever.Core.Graphics;
using GoldFever.Core.Level;
using System;
using System.Threading;

namespace GoldFever.Core
{
    public sealed class Game
    {
        public static bool GodMode = false;

        private Thread inputThread;
        private Thread logicThread;

        private bool _running;

        public bool Running
        {
            get { return _running; }
        }

        private GameState _state;

        public GameState State
        {
            get { return _state; }
        }

        private GameOptions _options;

        public GameOptions Options
        {
            get { return _options; }
        }

        private ContentManager _contentManager;

        public ContentManager ContentManager
        {
            get { return _contentManager; }
        }

        private LevelManager _levelManager;

        public LevelManager LevelManager
        {
            get { return _levelManager; }
        }

        public BaseLevel Level
        {
            get { return _levelManager?.Level; }
        }

        private int _score;

        public int Score
        {
            get { return _score; }
            set { _score = value; }
        }

        public IRenderer Renderer { get; set; }

        public Game(GameOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            _options = options;

            NewGame();
            Initialize();
        }

        private void NewGame(bool clear = false)
        {
            _state = GameState.Paused;
            _running = false;
            _score = 0;

            if (!clear)
                return;

            _levelManager.Level.Clear();
        }

        private void Initialize()
        {
            _contentManager = new ContentManager(_options.ContentPath);
            _levelManager = new LevelManager(this);
        }

        private void Load()
        {
            _levelManager.Load();
        }

        private void Loop()
        {
            if (_state == GameState.Resumed)
                return;

            _state = GameState.Resumed;

            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);

            inputThread = new Thread(HandleInput);
            logicThread = new Thread(HandleLogic);

            inputThread.Start();
            logicThread.Start();
        }

        private void HandleInput()
        {
            while(_state == GameState.Resumed)
            {
                if (Console.KeyAvailable)
                {
                    var info = Console.ReadKey(true);

                    if (info.Key == ConsoleKey.Escape)
                        continue; // handle
                    else
                    {
                        foreach (var actor in _levelManager.Level.Switches)
                            if (actor.Key == info.Key)
                                actor.Toggle();
                    }

                    Renderer?.Render();
                }
            }
        }

        private void HandleLogic()
        {
            Renderer?.Render();

            while (_state == GameState.Resumed)
            {
                Renderer?.Render();
                Thread.Sleep(500);

                try
                {
                    Update();
                }
                catch (GameOverException ex)
                {
                    OnGameOver();
                }
            }
        }

        private void Update()
        {
            _levelManager.Level.Update();
        }

        public void Setup()
        {
            if (_running)
                throw new InvalidOperationException("Game already running.");

            _running = true;

            try
            {
                Load();
            }
            catch(GameException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw new GameException("Unable to load game.", ex);
            }
        }

        public void Resume()
        {
            if (_state == GameState.Resumed)
                return;

            NewGame();
            Loop();
        }

        public void Pause()
        {
            if (_state == GameState.Paused)
                return;
        }

        #region Events

        private void OnGameOver()
        {
            if (GameOver != null)
                GameOver(this);

            _state = GameState.Ended;
        }

        public event GameOverHandler GameOver;
        public delegate void GameOverHandler(object sender);

        #endregion
    }
}
