using System;
using SwinGameSDK;

namespace RTS
{
	public class Button
	{
		private float _x;
		private float _y;
		private float _width;
		private float _height;
		private float _midPoint;
		private Color _color;
		private string _text;
		private Color _textColor;

		public Button (Color color, float x, float y, float width, float height, string text, Color textColor)
		{
			_color = color;
			_x = x;
			_y = y;
			_width = width;
			_height = height;
			_midPoint = y+ height / 2f;
			_text = text;
			_textColor = textColor;
		}

		public void Draw ()
		{
			SwinGame.DrawRectangle (_color, _x, _y, _width, _height);
			SwinGame.DrawText (_text, _textColor, _x, _midPoint);
		}
		public bool clicked ()
		{
			bool inX = SwinGame.MouseX () <= _x + _width && SwinGame.MouseX () >= _x;
			bool inY = SwinGame.MouseY () <= _y + _height && SwinGame.MouseY () >= _y;
			if (inX && inY)
				return true;
			return false;
		}
	}
}

