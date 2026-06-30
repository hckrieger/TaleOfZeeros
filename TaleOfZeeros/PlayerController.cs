using Jewely;
using Microsoft.Xna.Framework;
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
		int x, y;
		private float speed = 50;
		public int Id { get; set; }



		public PlayerController(Game game)
		{
			renderSystem = game.Services.GetService<RenderSystem>();
		}

		public void Update(GameTime gameTime, InputManager inputManager)
		{
			inputManager.Update();

			float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

			
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

			Vector2 direction = new Vector2(x, y);

			if (x != 0 &&  y != 0)
			{
				direction.Normalize();
			}
			Debug.WriteLine($"{direction}");
			renderSystem.Data.Position[Id] += direction * speed * dt;


		
		}
	}
}
