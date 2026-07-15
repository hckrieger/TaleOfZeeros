using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Reusable;
using Reusable.Managers;
using Reusable.Services;
using SharpDX.DirectWrite;
using SharpDX.Win32;
using System;
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
		private CameraManager cameraManager;
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
			renderSystem = new RenderSystem(this, 100, Content.Load<Texture2D>);
			Services.AddService(renderSystem);
			

			displayManager = new DisplayManager(_graphics);
			displayManager.SetWindowSize(new Point(320, 180), 4);
			tilemapManager = new TilemapManager("Data/Tilemaps/level.json", Content.Load<Texture2D>, this);
			inputManager = new InputManager(displayManager, InputManager.BindingType.TopDownAdventure);

			var tilemap = tilemapManager.TiledMap;
			Rectangle worldBounds = new Rectangle(0, 0, tilemap.WorldPixelWidth, tilemap.WorldPixelHeight);
			Rectangle cameraBounds = new Rectangle(0, 0, displayManager.InternalResolution.X, displayManager.InternalResolution.Y);
			cameraManager = new CameraManager(cameraBounds, worldBounds);
			Services.AddService(cameraManager);


			playerController = new PlayerController(this);
			
			
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
			

			var playerPosition = renderSystem.Data.Position[playerController.Id] - renderSystem.Data.Origin[playerController.Id];
			playerHeadCollision = new Rectangle(1 + (int)playerPosition.X, 1 + (int)playerPosition.Y, 15, 12);
			playerFeetCollision = new Rectangle(1 + (int)playerPosition.X, 13 + (int)playerPosition.Y, 14, 3);

			var tileMap = tilemapManager.TiledMap;
			var tiledLayers = tileMap.Layers;

			var chosenRectangle = playerFeetCollision;

			int leftTile = chosenRectangle.Left / tileMap.TileWidth;
			int rightTile = (chosenRectangle.Right) / tileMap.TileWidth;
			int topTile = chosenRectangle.Top / tileMap.TileHeight;
			int bottomTile = (chosenRectangle.Bottom) / tileMap.TileHeight;

			playerController.Stop = false;

			int predictedXBuffer = 0;
			int predictedYBuffer = 0;

			Vector2 velocity = playerController.Velocity;
			

			if (velocity.X > 0)
			{
				predictedXBuffer = 1;
			}
			else if (velocity.X < 0)
			{
				predictedYBuffer = -1;
			}

			if (velocity.Y > 0)
			{
				predictedYBuffer = 1;
			}
			else if (velocity.Y < 0)
			{
				predictedYBuffer = -1;
			}

			var predictedPlayerBounds = new Rectangle((int)(playerFeetCollision.X + predictedXBuffer),
												(int)(playerFeetCollision.Y + predictedYBuffer),
												playerFeetCollision.Width,
												playerFeetCollision.Height);

			for (int y = topTile; y <= bottomTile; y++)
			{
				for (int x = leftTile; x <= rightTile; x++)
				{
					var boundsAroundPlayer = new Rectangle(x * tileMap.TileWidth, y * tileMap.TileHeight, tileMap.TileWidth, tileMap.TileHeight);

					foreach (var layer in tiledLayers)
					{
						if (layer.Type == "objectgroup")
							continue;

						if (layer.Properties.GetValue<string>("TileCollision") == "Impassable")
						{
							var tileIndex = Utils.CoordinateToIndex(x, y, tileMap.Width);

							if (layer.Data[tileIndex] == 0)
								continue;

							var tileBounds = new Rectangle(x * tileMap.TileWidth, y * tileMap.TileHeight, tileMap.TileWidth, tileMap.TileHeight);



							

							if (tileBounds.Intersects(predictedPlayerBounds))
							{
								 if (predictedXBuffer == -1 && playerController.Direction.X == 1)
									playerController.Direction = new Vector2(0, playerController.Direction.Y);
								else if (predictedXBuffer == 1)
									playerController.Direction = new Vector2(0, playerController.Direction.Y);

								if (predictedYBuffer == -1)
									playerController.Direction = new Vector2(playerController.Direction.X, 0);
								else if (predictedYBuffer == 1)
									playerController.Direction = new Vector2(playerController.Direction.X, 0);


							} 
						} 
						
						
					}
				}
			}

			playerController.Update(gameTime, inputManager);
			//foreach (var layer in tiledLayers)
			//{
			//	if (layer.Type == "objectgroup")
			//		continue;

			//	for (int i = 0; i < layer.Data.Count; i++)
			//	{
			//		if (layer.Data[i] == 0)
			//			continue;

			//		var tileCoordinate = Utils.IndexToCoordinate(layer.Data[i], layer.Width);

			//		var tileBounds = new Rectangle(tileCoordinate.X * tileMap.TileWidth, tileCoordinate.Y * tileMap.TileHeight, tileMap.TileWidth, tileMap.TileHeight);

			//		var playerCollision = playerFeetCollision.Intersects(tileBounds);

			//		if (playerCollision)
			//		{
			//			Debug.WriteLine("A tile collision happened");
			//		} else
			//		{
			//			Debug.WriteLine("Not");
			//		}
			//	}
			//}


			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			_graphics.GraphicsDevice.SetRenderTarget(displayManager.RenderTarget);
			_spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: cameraManager.TransformMatrix);
			tilemapManager.Draw(_spriteBatch);
			_spriteBatch.End();

			_spriteBatch.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack, transformMatrix: cameraManager.TransformMatrix);
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
