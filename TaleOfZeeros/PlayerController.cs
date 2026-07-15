using Microsoft.Xna.Framework;
using Reusable;
using Reusable.Services;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace TaleOfZeeros
{
	public class PlayerController
	{
		private RenderSystem renderSystem;
		public Vector2 Direction { get; set; }
		public Vector2 Velocity { get; set; }
		private float speed = 30;
		public int Id { get; set; }
		int x, y;
		public bool Stop { get; set; } = false;



		public PlayerController(Game game)
		{
			renderSystem = game.Services.GetService<RenderSystem>();
		}

		public void Update(GameTime gameTime, InputManager inputManager)
		{
			if (Stop)
				return;
			inputManager.Update();

			


			ReadInput(inputManager);

			MovePlayer(gameTime);



		
		}

		private void ReadInput(InputManager inputManager)
		{
			if (inputManager.Binding[InputManager.InputAction.MoveLeft].Invoke())
				x = -1;
			else if (inputManager.Binding[InputManager.InputAction.MoveRight].Invoke())
				x = 1;
			else
				x = 0;

			if (inputManager.Binding[InputManager.InputAction.MoveUp].Invoke())
				y = -1;
			else if (inputManager.Binding[InputManager.InputAction.MoveDown].Invoke())
				y = 1;
			else
				y = 0;

			if (x != 0 && y != 0)
			{
				Direction.Normalize();
			}

			Direction = new Vector2(x, y);
		}

		private void MovePlayer(GameTime gameTime)
		{
			float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
			Velocity = Direction * speed * dt;
			renderSystem.Data.Position[Id] += Velocity;
		}
	}
}
