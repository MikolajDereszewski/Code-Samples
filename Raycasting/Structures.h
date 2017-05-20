#include <iostream>

using namespace std;

struct COORDINATES
{
	float X, Y;
	COORDINATES() {};
	COORDINATES(float X, float Y) : X(X), Y(Y) {};
};

struct COMPRESS
{
	int COLOR;
	string FILL;
	COMPRESS() : COLOR(-1), FILL("") {};
};

struct HITPOINT
{
	float DIST;
	int HIT;
	HITPOINT() : DIST(0), HIT(0) {};
};