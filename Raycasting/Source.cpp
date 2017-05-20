#include <fstream>
#include <windows.h>
#include "Engine.h"

bool LoadFile()
{
	ifstream file;
	file.open("MAP.txt");
	if (!file.good())
		return false;

	char bufor[3];
	file.getline(bufor, 3, ';');
	WIDTH = atoi(bufor);
	file.getline(bufor, 3, ';');
	HEIGHT = atoi(bufor);
	for (int y = 0; y < HEIGHT; y++)
	{
		for (int x = 0; x < WIDTH; x++)
		{
			file.getline(bufor, 3, ';');
			if (atoi(bufor) == 5)
				Player = COORDINATES(x * 15 + 7, y * 15 + 7);
			for (int m1 = 0; m1 < 15; m1++)
				for (int m2 = 0; m2 < 15; m2++)
					MATRIX[x * 15 + m1][y * 15 + m2] = atoi(bufor);
		}
	}
	file.close();
	PlayerM = COORDINATES(Player.X, Player.Y);
	return true;
}

bool MoveAndCheckForExit(int move)
{
	if (move == 14 && (MATRIX[(int)Player.X + (int)cos((PlayerROT / 10)*PI / 36)][(int)Player.Y + (int)sin((PlayerROT / 10)*PI / 36)] == 0 || MATRIX[(int)Player.X + (int)cos((PlayerROT / 10)*PI / 36)][(int)Player.Y + (int)sin((PlayerROT / 10)*PI / 36)] == 5))
		PlayerM = COORDINATES(PlayerM.X + 2 * cos((PlayerROT / 10)*PI / 36), PlayerM.Y + 2 * sin((PlayerROT / 10)*PI / 36));
	else if (move == 15 && (MATRIX[(int)Player.X - (int)cos((PlayerROT / 10)*PI / 36)][(int)Player.Y - (int)sin((PlayerROT / 10)*PI / 36)] == 0 || MATRIX[(int)Player.X - (int)cos((PlayerROT / 10)*PI / 36)][(int)Player.Y - (int)sin((PlayerROT / 10)*PI / 36)] == 5))
		PlayerM = COORDINATES(PlayerM.X - 2 * cos((PlayerROT / 10)*PI / 36), PlayerM.Y - 2 * sin((PlayerROT / 10)*PI / 36));
	PlayerROT += (move == 16) ? -20 :
		(move == 17) ? 20 : 0;
	PlayerROT = (PlayerROT == 720 || PlayerROT == -720) ? 0 : PlayerROT;

	if (MATRIX[(int)PlayerM.X][(int)PlayerM.Y] != 0 && MATRIX[(int)PlayerM.X][(int)PlayerM.Y] != 5)
	{
		PlayerM.X += (move == 14) ? -2 * cos((PlayerROT / 10)*PI / 36) : 2 * cos((PlayerROT / 10)*PI / 36);
		PlayerM.Y += (move == 14) ? -2 * sin((PlayerROT / 10)*PI / 36) : 2 * sin((PlayerROT / 10)*PI / 36);
	}
	Player = COORDINATES((int)PlayerM.X, (int)PlayerM.Y);
	return (move == 0) ? true : false;
}

int main()
{
	if (!LoadFile())
		return 1;
	while (true)
	{
		RefreshScreen();
		if (MoveAndCheckForExit(rlutil::getkey()))
			break;
		Sleep(25);
	}
	return 0;
}