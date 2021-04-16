#pragma once
#include "task.h"

#ifdef INVERSESOLVERLIB_EXPORTS
#define INVERSESOLVERLIB __declspec(dllexport)
#else
#define INVERSESOLVERLIB __declspec(dllimport)
#endif

using Value = Point;

class FiniteElemProxy {
public:
	FiniteElemProxy(int n0, int n1, int n2, int n3, int n4, int n5, int n6, int n7, Value p)
	{
		N0 = n0;
		N1 = n1;
		N2 = n2;
		N3 = n3;
		N4 = n4;
		N5 = n5;
		N6 = n6;
		N7 = n7;
		P = p;
	}

	// Map local point into global
	int N0;
	int N1;
	int N2;
	int N3;
	int N4;
	int N5;
	int N6;
	int N7;

	// Magnetization intensity
	Value P;
};

extern "C" INVERSESOLVERLIB Task* createTask();

extern "C" INVERSESOLVERLIB void solve(Task*, FiniteElemProxy*);

extern "C" INVERSESOLVERLIB void deleteTask(Task * task);

extern "C" INVERSESOLVERLIB  void initInverseTask(Task * task,
    double hxMeasure, int nxMeasure, double hyMeasure, int nyMeasure,
    Point p0Measure, Value * measuredValues, int measuredValuesSize,
    double xStart, double xEnd, int xStepsAmount,
    double yStart, double yEnd, int yStepsAmount,
    double zStart, double zEnd, int zStepsAmount,
    double _alpha);

extern "C" INVERSESOLVERLIB void getGridInformation(Task* task, GridInformation& gridInfo);

extern "C" INVERSESOLVERLIB void getResultGrids(Task* task, Point* nodes, double* yLayers);
extern "C" INVERSESOLVERLIB void getMeasureGrids(Task* task, double* xGrid, double* yGrid);

extern "C" INVERSESOLVERLIB void getDiscrepancy(Task* task, int yLayer, std::vector<double> fx);