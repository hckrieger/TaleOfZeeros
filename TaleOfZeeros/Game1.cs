using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Reusable.Managers;
using Reusable.Services;

namespace TaleOfZeeros
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private DisplayManager displayManager;
		private TilemapManager tilemapManager;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			displayManager = new DisplayManager(_graphics);
			tilemapManager = new TilemapManager("Data/Tilemaps/level.json", Content.Load<Texture2D>);
			displayManager.SetWindowSize(new Point(320, 180), 4);

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			_graphics.GraphicsDevice.SetRenderTarget(displayManager.RenderTarget);
			_spriteBatch.Begin();
			tilemapManager.Draw(_spriteBatch);
			_spriteBatch.End();
			_graphics.GraphicsDevice.SetRenderTarget(null);

			_spriteBatch.Begin(samplerState: SamplerState.PointClamp);
			_spriteBatch.Draw(displayManager.RenderTarget, displayManager.AdjustedViewport, Color.White);
			_spriteBatch.End();

			base.Draw(gameTime);
		}


	}
}
