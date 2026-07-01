using Jewely;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Reusable;
using Reusable.Managers;
using Reusable.Services;
using SharpDX.Win32;
using System.Diagnostics;
using System.Linq;

namespace TaleOfZeeros
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private DisplayManager displayManager;
		private TilemapManager tilemapManager;
		private RenderSystem renderSystem;
		private InputManager inputManager;
		private PlayerController playerController;
		private Rectangle playerHeadCollision, playerFeetCollision;
		public const int PLAYER = 0;
		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			renderSystem = new RenderSystem(this, 15, Content.Load<Texture2D>);

			Services.AddService(renderSystem);
			displayManager = new DisplayManager(_graphics);
			tilemapManager = new TilemapManager("Data/Tilemaps/level.json", Content.Load<Texture2D>);
			inputManager = new InputManager(displayManager, InputManager.BindingType.TopDownAdventure);
			playerController = new PlayerController(this);
			
			displayManager.SetWindowSize(new Point(320, 180), 4);
			//displayManager.ToggleFullScreen();

			tilemapManager.ForEachObject((layer, obj) =>
			{
				if (layer.Name == "Character")
				{
					switch (obj.Name)
					{
						case "Player":
							var renderableDataInstance = new RenderableDataInstance
							{
								Position = new Vector2(obj.X, obj.Y),
								TextureKey = obj.Properties.GetValue<string>("TextureKey"),
								SourceRectangle = Utils.IndexToSourceRectangle(0, new Point(16, 16), 4),
							};


							playerController.Id = renderSystem.AddDataEntity(renderableDataInstance);
							break;
					}
				}
			});
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
			playerController.Update(gameTime, inputManager);

			var playerPosition = renderSystem.Data.Position[PLAYER] - renderSystem.Data.Origin[PLAYER];
			playerHeadCollision = new Rectangle(1 + (int)playerPosition.X, 1 + (int)playerPosition.Y, 15, 12);
			playerFeetCollision = new Rectangle(1 + (int)playerPosition.X, 13 + (int)playerPosition.Y, 14, 3);


			

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			_graphics.GraphicsDevice.SetRenderTarget(displayManager.RenderTarget);
			_spriteBatch.Begin(samplerState: SamplerState.PointClamp);
			tilemapManager.Draw(_spriteBatch);
			renderSystem.Draw(_spriteBatch);
			_spriteBatch.End();
			_graphics.GraphicsDevice.SetRenderTarget(null);

			_spriteBatch.Begin(samplerState: SamplerState.PointClamp);
			_spriteBatch.Draw(displayManager.RenderTarget, displayManager.AdjustedViewport, Color.White);
			_spriteBatch.End();

			base.Draw(gameTime);
		}


	}
}
