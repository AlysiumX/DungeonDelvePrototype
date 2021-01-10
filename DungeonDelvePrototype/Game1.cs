using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace DungeonDelvePrototype
{
	public class MovableGameSprite
	{
		public string Name { get; set; }
		public Texture2D Image { get; set; }
		public Vector2 Position { get; set; }
		public int Speed { get; set; }
		public Vector2 MoveToDestination { get; set; }

		public MovableGameSprite()
		{
			Position = new Vector2( 0, 0 );
			MoveToDestination = new Vector2( 0, 0 );
		}

		public bool WasClicked( Point mousePosition )
		{
			return ( mousePosition.X > Position.X && mousePosition.X < Position.X + Image.Width ) &&
					( mousePosition.Y > Position.Y && mousePosition.Y < Position.Y + Image.Height );
		}

		public void Update( GameTime gameTime )
		{
			//Wacky Hacky
			if( MoveToDestination.X != -100 )
			{
				if( Position.X != MoveToDestination.X || Position.Y != MoveToDestination.Y )
				{
					var direction = Vector2.Normalize( MoveToDestination -Position );
					Position += direction * ( float )gameTime.ElapsedGameTime.TotalSeconds * Speed;
				}

				if( Position.X == MoveToDestination.X && Position.Y == MoveToDestination.Y )
					MoveToDestination = new Vector2( -100, -100 );
			}
		}

		public void Draw( SpriteBatch spriteBatch )
		{
			spriteBatch.Draw( Image, Position, Color.White );
		}
	}

	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private SpriteFont _basicFont;

		private Texture2D _statusBar;
		private Texture2D _actionBarBack;

		private Texture2D _buttonTaunt;

		private MovableGameSprite _warrior;
		private MovableGameSprite _archer;
		private MovableGameSprite _mage;
		private MovableGameSprite _currentlySelectedSprite;

		private Texture2D _dragon;
		private Vector2 _dragonPosition;

		private bool wasMouseLeftClicked = false;
		private bool wasMouseRightClicked = false;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager( this );
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			_graphics.PreferredBackBufferHeight = 600;
			_graphics.PreferredBackBufferWidth = 800;
		}

		protected override void Initialize()
		{
			_currentlySelectedSprite = new MovableGameSprite();

			_warrior = new MovableGameSprite();
			_warrior.Name = "Warrior";
			_warrior.Position = new Vector2( 400, 200 );
			_warrior.MoveToDestination = new Vector2( -100, -100 );
			_warrior.Speed = 30;

			_archer = new MovableGameSprite();
			_archer.Name = "Archer";
			_archer.Position = new Vector2( 450, 250 );
			_archer.MoveToDestination = new Vector2( -100, -100 );
			_archer.Speed = 40;

			_mage = new MovableGameSprite();
			_mage.Name = "Mage";
			_mage.Position = new Vector2( 350, 250 );
			_mage.MoveToDestination = new Vector2( -100, -100 );
			_mage.Speed = 35;

			_dragonPosition = new Vector2( 350, 25 );

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch( GraphicsDevice );
			_basicFont = Content.Load<SpriteFont>( "BasicFont" );
			_statusBar = Content.Load<Texture2D>( "StatusBar" );
			_actionBarBack = Content.Load<Texture2D>( "ActionBarBack" );
			_buttonTaunt = Content.Load<Texture2D>( "Button_Taunt" );
			_warrior.Image = Content.Load<Texture2D>( "Warrior" );
			_archer.Image = Content.Load<Texture2D>( "Archer" );
			_mage.Image = Content.Load<Texture2D>( "Mage" );
			_dragon = Content.Load<Texture2D>( "Dragon" );

			// TODO: use this.Content to load your game content here
		}

		protected override void Update( GameTime gameTime )
		{
			if( GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown( Keys.Escape ) )
				Exit();

			HandleLeftClickLogic( gameTime );
			HandleRightClickLogic( gameTime );
			HandleCharacterMovement( gameTime );
			ExecuteDragonLogic( gameTime );

			base.Update( gameTime );
		}

		private void HandleLeftClickLogic( GameTime gameTime )
		{
			if( Mouse.GetState().LeftButton == ButtonState.Pressed )
				wasMouseLeftClicked = true;

			if( wasMouseLeftClicked && Mouse.GetState().LeftButton == ButtonState.Released )
			{
				var mousePosition = Mouse.GetState().Position;
				if( _warrior.WasClicked( mousePosition ) )
					_currentlySelectedSprite = _warrior;

				else if( _archer.WasClicked( mousePosition ) )
					_currentlySelectedSprite = _archer;

				else if( _mage.WasClicked( mousePosition ) )
					_currentlySelectedSprite = _mage;
				else
					_currentlySelectedSprite = null;

				wasMouseLeftClicked = false;
			}
		}

		private void HandleRightClickLogic( GameTime gameTime )
		{
			if( Mouse.GetState().RightButton == ButtonState.Pressed )
				wasMouseRightClicked = true;

			if( wasMouseRightClicked && Mouse.GetState().RightButton == ButtonState.Released )
			{
				_currentlySelectedSprite.MoveToDestination = new Vector2( Mouse.GetState().Position.X, Mouse.GetState().Position.Y );
				wasMouseRightClicked = false;
			}
		}

		private void HandleCharacterMovement( GameTime gameTime )
		{
			_warrior.Update( gameTime );
			_archer.Update( gameTime );
			_mage.Update( gameTime );
		}

		private void ExecuteDragonLogic( GameTime gameTime )
		{
			/*
			 Abilities : 
			 FireLine - Shoots a line of fire straight forward.
			 Lose Aggro - Start attacking another member.
			 Fire Spread - Flys up and then shoots fire in multiple directions.
			 */
		}

		protected override void Draw( GameTime gameTime )
		{
			GraphicsDevice.Clear( Color.White );

			// TODO: Add your drawing code here
			_spriteBatch.Begin();
			_spriteBatch.Draw( _dragon, _dragonPosition, Color.White );
			_warrior.Draw( _spriteBatch );
			_archer.Draw( _spriteBatch );
			_mage.Draw( _spriteBatch );
			_spriteBatch.Draw( _statusBar, new Vector2( 0, 500 ), Color.White );
			_spriteBatch.Draw( _actionBarBack, new Vector2( 300, 520 ), Color.White );
			_spriteBatch.DrawString( _basicFont, _currentlySelectedSprite.Name ?? "", new Vector2( 10, 505 ), Color.Black );

			if( _currentlySelectedSprite == _warrior )
				_spriteBatch.Draw( _buttonTaunt, new Vector2( 303, 523 ), Color.White );

			_spriteBatch.End();

			base.Draw( gameTime );
		}
	}
}
