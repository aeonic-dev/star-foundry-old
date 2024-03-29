﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using StarFoundry.Engine.Core;
using StarFoundry.GameContent;
using StarFoundry.Engine.Input;

namespace StarFoundry {
    public class Client : Game {
        public static Client Instance { get; private set; } = null!;

        public static Container Container {
            get => Instance._container;
            set => Instance.SetContainer(value, new FadeTransition(Instance.GraphicsDevice, Color.Black, 0.5f));
        }

        public static GameTime GameTime { get; private set; } = null!;

        public static Size ScreenSize { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; } = null!;
        public static SpriteBatch SpriteBatch { get; private set; } = null!;
        public static ContentManager ContentManager { get; private set; } = null!;

        private readonly ScreenManager _screenManager;

        private Container _container = Container.Empty;
        private ResizeState _resizeState;

        public Client() {
            Instance = this;
            ContentManager = Content;
            ContentManager.RootDirectory = "Content";
            IsMouseVisible = true;

            // Containers
            _screenManager = new ScreenManager();
            Components.Add(_screenManager);

            // Window sizing
            ScreenSize = new Size(1280, 720);

            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferredBackBufferWidth = ScreenSize.Width;
            Graphics.PreferredBackBufferHeight = ScreenSize.Height;

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (_, _) => OnResize();
            _resizeState = new ResizeState(false, ScreenSize.Width, ScreenSize.Height);

            // Input
            InputEvents.Bootstrap(this);
            Components.Add(InputEvents.Instance);
        }

        public void SetContainer(Container container, Transition transition) {
            InputEvents.UnregisterInputEvents(_container);
            InputEvents.RegisterInputEvents(container);

            _container = container;
            _screenManager.LoadScreen(container, transition);
        }

        protected override void Initialize() {
            base.Initialize();

            Container = new TestContainer();
        }

        protected void OnResize() {
            _resizeState = new ResizeState(true, Window.ClientBounds.Width, Window.ClientBounds.Height);
        }

        protected override void LoadContent() {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime) {
            GameTime = gameTime;
            if (_resizeState.ResizePending) {
                Graphics.PreferredBackBufferWidth = _resizeState.Width;
                Graphics.PreferredBackBufferHeight = _resizeState.Height;

                ScreenSize = new Size(_resizeState.Width, _resizeState.Height);
                _resizeState.ResizePending = false;
                Graphics.ApplyChanges();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}