#include <math.h>
#include "rlutil.h"
#include "Structures.h"

#define PI 3.14
int MATRIX[128 * 15][128 * 15], MonitorMatrix[84][35];
int WIDTH = 0, HEIGHT = 0;
int PlayerROT = 0;
COORDINATES Player(0, 0), PlayerM(0, 0);

HITPOINT CalculateDistance(COORDINATES Shift, int alfa)
{
	COORDINATES Ray(Player.X, Player.Y);
	HITPOINT Hit;
	while (true)
	{
		Ray = COORDINATES(Ray.X + Shift.X, Ray.Y + Shift.Y);
		Hit.DIST += cos((alfa / 2)* PI / 36);
		if (MATRIX[(int)Ray.X][(int)Ray.Y] != 0 && MATRIX[(int)Ray.X][(int)Ray.Y] != 5)
			break;
	}
	Hit.HIT = MATRIX[(int)Ray.X][(int)Ray.Y];
	return Hit;
}

void CalculateHitpoints()
{
	HITPOINT Hitpoint[42];
	for (int alfa = -21; alfa < 21; alfa++)
	{
		COORDINATES Shift(cos((alfa / 2 + PlayerROT / 10)*PI / 36), sin((alfa / 2 + PlayerROT / 10)*PI / 36));
		Hitpoint[alfa + 21] = CalculateDistance(Shift, alfa);
	}

	for (int x = 0; x < 84; x++)
		for (int y = 0; y < 35; y++)
			MonitorMatrix[x][y] = (y < 18) ? 1 : 2;

	for (int alfa = 0; alfa < 42; alfa++)
	{
		float height = 18 - pow(log10(Hitpoint[alfa].DIST*Hitpoint[alfa].DIST), 2);
		height = (height <= 0) ? 1 : height;
		for (int realHeight = 0; realHeight < height; realHeight++)
		{
			for (int column = 0; column < 2; column++)
			{
				MonitorMatrix[(alfa * 2) + column][18 + realHeight] = Hitpoint[alfa].HIT;
				MonitorMatrix[(alfa * 2) + column][18 - realHeight] = Hitpoint[alfa].HIT;
			}
		}
	}
}

void DisplayMatrix(COMPRESS Compress[35][64], int Length[35])
{
	for (int y = 0; y < 35; y++)
	{
		for (int x = 0; x <= Length[y]; x++)
		{
			rlutil::setBackgroundColor(   ((Compress[y][x].COLOR != -1) ? Compress[y][x].COLOR : 0)   );
			cerr << Compress[y][x].FILL;
		}
		cout << endl;
	}
	cout << endl;
	rlutil::setBackgroundColor(0);
}

void OptimizeAndDisplay()
{
	COMPRESS Compress[35][64];
	Compress[0][0].COLOR = 1;
	int Length[35];
	for (int i = 0; i < 35; i++)
		Length[i] = 0;
	for (int y = 0; y < 35; y++)
	{
		int i = 0;
		for (int x = 0; x < 84; x++)
		{
			if (Compress[y][i].COLOR != MonitorMatrix[x][y])
			{
				if (Compress[y][i].COLOR != -1)
				{
					i++;
					Length[y]++;
				}
				Compress[y][i].COLOR = MonitorMatrix[x][y];
			}
			Compress[y][i].FILL += " ";
		}
	}
	DisplayMatrix(Compress, Length);
}

void RefreshScreen()
{
	SetConsoleCursorPosition(GetStdHandle(STD_OUTPUT_HANDLE), { 0, 0 });
	CalculateHitpoints();
	OptimizeAndDisplay();
	cout << "X: " << Player.X << "   " << endl << "Y: " << Player.Y << "   " << endl << "Rotation: " << PlayerROT / 2 << "   " << endl;
}