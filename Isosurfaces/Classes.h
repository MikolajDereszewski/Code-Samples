#include <Math.h>
#include "SDL.h"

class Vector2
{
	public:
		int x = 0, y = 0;
		Vector2() {};
		Vector2(int x, int y) : x(x), y(y) {};
};

class Color
{
	public:
		int r = 0, g = 0, b = 0;
		Color() {};
		Color(int r, int g, int b) : r((r > 255) ? 255 : r), g((g > 255) ? 255 : g), b((b > 255) ? 255 : b) {};
};

class Isosurface
{
	public:
		Vector2 Position = Vector2(0, 0);
		Vector2 Speed = Vector2(0, 0);
		short int r = 1, shape = 1;
		bool controlled = false;
		Isosurface() {};
		Isosurface(int x, int y, short int r, int sx, int sy, int shape, bool controlled = false) : r(r), controlled(controlled), shape(shape)
		{
			Position = Vector2(x, y);
			Speed = Vector2(sx, sy);
		}

		float DistanceFromCenter(int x, int y)
		{
			float d = sqrt(pow(x - Position.x, shape*2) + pow(y - Position.y, shape*2))*pow(shape, -4);
			return (d != 0) ? d : 1;
		}

		void Move()
		{
			if (controlled)
			{
				SDL_GetMouseState(&Position.x, &Position.y);
				return;
			}
			Position = Vector2(Position.x + Speed.x, Position.y + Speed.y);
			if (Position.x < 0 || Position.x > 200)
				Speed.x *= -1;
			if (Position.y < 0 || Position.y > 200)
				Speed.y *= -1;
		}
};

Color Filter(short int x, short int y, float d, unsigned char filterType)
{
	if (filterType == 0)
	{
		return Color(d, d, d);
	}
	if (filterType == 1)
	{
		return Color(0, d, d);
	}
	if (filterType == 2)
	{
		if (d <= 200 || d >= 220)
			return Color(0, 0, 0);
		return Color(d, d, d);
	}
	if (filterType == 3)
	{
		if (d <= 200)
			return Color(0, 0, 0);
		float c = (d < 240) ? 255 : 100;
		return Color(0, c / 2, c);
	}
	if (filterType == 4)
	{
		if (d <= 200)
			return Color(0, 0, 0);
		return Color(y + 50, x + 50, d / 2 + 100);
	}
	if (filterType == 5)
	{
		if (d <= 200 || d >= 240)
			return Color(0, 0, 0);
		return Color(y + 50, x + 50, d / 2 + 50);
	}
	if (filterType == 6)
	{
		if (d <= 200)
			return Color(0, pow(log10(d / 2), 5) * 3, 0);
		float c = (d < 240) ? 0 : 255;
		return Color(c / 2, c, c / 2);
	}
}