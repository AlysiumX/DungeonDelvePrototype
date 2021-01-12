using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace DungeonDelvePrototype
{
	public enum SpriteAction { Waiting, Moving, Attacking }

	public class MovableGameSprite
	{
		public string Name { get; set; }
		public int Life { get; set; }
		public Texture2D Image { get; set; }
		public Vector2 Position { get; set; }
		public MovableGameSprite AttackTarget { get; set; }
		public int Speed { get; set; }
		public int StandardDamage { get; set; }
		public Vector2 MoveToDestination { get; set; }
		public SpriteAction CurrentAction;

		public MovableGameSprite()
		{
			Position = new Vector2( 0, 0 );
			MoveToDestination = new Vector2( 0, 0 );
			CurrentAction = SpriteAction.Waiting;
		}

		public bool WasClicked( Point mousePosition )
		{
			return ( mousePosition.X > Position.X && mousePosition.X < Position.X + Image.Width ) &&
					( mousePosition.Y > Position.Y && mousePosition.Y < Position.Y + Image.Height );
		}

		private void DoAttackIfAble()
		{
			//AttackTarget.Life -= StandardDamage;
		}

		public void Update( GameTime gameTime )
		{
			switch( CurrentAction )
			{
				case SpriteAction.Moving:
					MoveIfRequired( gameTime );
					break;

				case SpriteAction.Attacking:
					DoAttackIfAble();
					break;
			}
		}

		private void MoveIfRequired( GameTime gameTime )
		{
			if( Position.X != MoveToDestination.X || Position.Y != MoveToDestination.Y )
			{
				var direction = Vector2.Normalize( MoveToDestination -Position );
				Position += direction * ( float )gameTime.ElapsedGameTime.TotalSeconds * Speed;
			}

			if( Position.X == MoveToDestination.X && Position.Y == MoveToDestination.Y )
				CurrentAction = SpriteAction.Waiting;
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

		private Rectangle _bossHealthBackDimensions;
		private Texture2D _bossHealthBack;

		private Rectangle _bossHealthDimensions;
		private Texture2D _bossHealth;

		private Texture2D _buttonTaunt;

		private MovableGameSprite _warrior;
		private MovableGameSprite _archer;
		private MovableGameSprite _mage;
		private MovableGameSprite _currentlySelectedSprite;

		private MovableGameSprite _dragon;

		private bool wasMouseLeftClicked = false;
		private bool wasMouseRightClicked = false;

		//TODO : Add character select to bottom screen.
		//TODO : MultiSelect
		//TODO : Healthbars
		//TODO : Healer
		//TODO : Implement Dragon Abilities.
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
			_currentlySelectedSprite = null;

			_bossHealthBackDimensions = new Rectangle( 280, 25, 200, 25 );
			var _bossHealthBackDimensions_Data = new Color[_bossHealthBackDimensions.Width * _bossHealthBackDimensions.Height];
			for( int i = 0; i < _bossHealthBackDimensions_Data.Length; ++i )
				_bossHealthBackDimensions_Data[i] = Color.Black;

			_bossHealthBack = new Texture2D( GraphicsDevice, _bossHealthBackDimensions.Width, _bossHealthBackDimensions.Height );
			_bossHealthBack.SetData( _bossHealthBackDimensions_Data );

			_bossHealthDimensions = new Rectangle( 280, 25, 1, 25 );
			var _bossHealthDimensions_Data = new Color[_bossHealthDimensions.Width * _bossHealthDimensions.Height];
			for( int i = 0; i < _bossHealthDimensions_Data.Length; ++i )
				_bossHealthDimensions_Data[i] = Color.Green;

			_bossHealth = new Texture2D( GraphicsDevice, _bossHealthDimensions.Width, _bossHealthDimensions.Height );
			_bossHealth.SetData( _bossHealthDimensions_Data );


			_warrior = new MovableGameSprite();
			_warrior.Name = "Warrior";
			_warrior.Life = 100;
			_warrior.Position = new Vector2( 350, 310 );
			_warrior.MoveToDestination = new Vector2( -100, -100 );
			_warrior.Speed = 30;

			_archer = new MovableGameSprite();
			_archer.Name = "Archer";
			_archer.Life = 100;
			_archer.Position = new Vector2( 450, 365 );
			_archer.MoveToDestination = new Vector2( -100, -100 );
			_archer.Speed = 40;

			_mage = new MovableGameSprite();
			_mage.Name = "Mage";
			_mage.Life = 100;
			_mage.Position = new Vector2( 300, 375 );
			_mage.MoveToDestination = new Vector2( -100, -100 );
			_mage.Speed = 35;

			_dragon = new MovableGameSprite();
			_dragon.Name = "Dragon";
			_dragon.Life = 500;
			_dragon.Position = new Vector2( 325, 175 );

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
			_dragon.Image = Content.Load<Texture2D>( "Dragon" );
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
				else if( _dragon.WasClicked( mousePosition ) )
					_dragon.Life -= 5;
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
				if( _currentlySelectedSprite == null )
					return;

				if( _dragon.WasClicked( Mouse.GetState().Position ) )
				{
					_currentlySelectedSprite.AttackTarget = _dragon;
					_currentlySelectedSprite.CurrentAction = SpriteAction.Attacking;
				}
				else
				{
					_currentlySelectedSprite.MoveToDestination = new Vector2( Mouse.GetState().Position.X - _currentlySelectedSprite.Image.Width / 2, Mouse.GetState().Position.Y - _currentlySelectedSprite.Image.Height / 2 );
					_currentlySelectedSprite.CurrentAction = SpriteAction.Moving;
				}

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
			 Fire Spread - Flys up and then shoots out fire bolts that then flare up from the outside in.
			 GroundSlam - Pounds down on the grand after ending flight.
			 */
		}

		protected override void Draw( GameTime gameTime )
		{
			GraphicsDevice.Clear( Color.White );

			// TODO: Add your drawing code here
			_spriteBatch.Begin();
			_dragon.Draw( _spriteBatch );
			_warrior.Draw( _spriteBatch );
			_archer.Draw( _spriteBatch );
			_mage.Draw( _spriteBatch );
			_spriteBatch.Draw( _statusBar, new Vector2( 0, 500 ), Color.White );
			_spriteBatch.Draw( _actionBarBack, new Vector2( 300, 520 ), Color.White );
			_spriteBatch.Draw( _bossHealthBack, new Vector2( _bossHealthBackDimensions.Left, _bossHealthBackDimensions.Top ), Color.White );
			var healthBarValue = ( int )( ( ( _dragon.Life )/500m ) * 200 );
			_spriteBatch.Draw( _bossHealth, new Rectangle( _bossHealthDimensions.Left, _bossHealthDimensions.Top, healthBarValue, 25 ), Color.White );
			_spriteBatch.DrawString( _basicFont, _currentlySelectedSprite?.Name ?? "", new Vector2( 10, 505 ), Color.Black );

			if( _currentlySelectedSprite == _warrior )
				_spriteBatch.Draw( _buttonTaunt, new Vector2( 303, 523 ), Color.White );

			_spriteBatch.End();

			base.Draw( gameTime );
		}
	}
}
