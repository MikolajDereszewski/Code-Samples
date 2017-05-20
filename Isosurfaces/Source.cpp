#include "Classes.h"
#define isosurfaceCount 4

SDL_Window *Window = SDL_CreateWindow("ISOSURFACES", 200, 200, 200, 200, SDL_WINDOW_SHOWN);
SDL_Renderer *Renderer = SDL_CreateRenderer(Window, -1, SDL_RENDERER_ACCELERATED | SDL_RENDERER_PRESENTVSYNC);
SDL_Event Event;
Isosurface Surfaces[isosurfaceCount] = {		Isosurface(20, 130, 300, 0, 0, 2, true),
												Isosurface(10, 100, 400, 2, 0, 1),
												Isosurface(150, 10, 200, 0, 2, 1),
												Isosurface(5, 5, 150, 2, 2, 1)				};

void PutPixelOnScreen(unsigned char x, unsigned char y, float d, signed char filterType)
{
	Color Color = Filter(x, y, d, filterType);
	if (Color.r + Color.g + Color.b == 0)
		return;
	SDL_SetRenderDrawColor(Renderer, Color.r, Color.g, Color.b, 255);
	SDL_RenderDrawPoint(Renderer, x, y);
}

void RenderPixels(signed char filterType)
{
	for (unsigned char y = 0; y < 200; y++)
	{
		for (unsigned char x = 0; x < 200; x++)
		{
			float d = 0;
			for (int i = 0; i < isosurfaceCount; i++)
				d += 10* Surfaces[i].r / Surfaces[i].DistanceFromCenter(x, y);
			PutPixelOnScreen(x, y, d, filterType);
		}
	}
	SDL_SetRenderDrawColor(Renderer, 0, 0, 0, 255);
}

int main(int, char**)
{
	signed char filterType = 0;
	while (true)
	{
		while (SDL_PollEvent(&Event))
		{
			if (Event.type == SDL_QUIT)
				return 0;
			if (Event.type == SDL_MOUSEBUTTONUP)
				filterType = (filterType != 6) ? filterType + 1 : 0;
		}
		for (int i = 0; i < isosurfaceCount; i++)
			Surfaces[i].Move();
		RenderPixels(filterType);
		SDL_RenderPresent(Renderer);
		SDL_RenderClear(Renderer);
	}
}