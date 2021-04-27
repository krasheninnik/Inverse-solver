#pragma once
#define _USE_MATH_DEFINES
#include <iostream>
#include <fstream>
#include <vector>
#include <math.h>
#include <ppl.h>
#include <chrono>
#include "Gauss.h"
#include <assert.h>

class GridInformation
{
public:
	int elemsInX;
	int elemsInY;
	int elemsInZ;
	int elemsSize;
	int pointsSize;
	int yResultsLayersSize;
	int yMeasureLayersSize;
	int xMeasureLayersSize;
	double dx;
	double xStart;
	double xEnd;
	double dz;
	double zStart;
	double zEndl;
};


class Point {
public:
	Point() = default;
	Point(double _x, double _y, double _z) {
		x = _x;
		y = _y;
		z = _z;
	}
	double x, y, z;
};

class FiniteElem {
public:
	FiniteElem() = default;
	FiniteElem(std::vector<int> _nodes, Point _p) {
		nodes = _nodes;
		p = _p;
	}
	// map local point into global
	std::vector<int> nodes;
	// magnetization intensity
	Point p;
};

class Measure {
public:
	Measure() = default;
	Measure(Point _point, Point _B) {
		point = _point;
		B = _B;
	}
	// measure point
	Point point;
	// measured value
	Point B;
};

class Task {
public:
	//��� ��� � �������� Measure ��������� � ����� ���������
	//��� ��� � �������� Grid ��������� � ����� ��
	//yResidual - ����� Y, �� �������� ����� ������� ���������� �������
	void init(double hxMeasure, int nxMeasure, double hyMeasure, int nyMeasure, Point p0Measure, std::vector<Point> B,
		double x0Grid, double x1Grid, int xStepsGrid,
		double y0Grid, double y1Grid, int yStepsGrid,
		double z0Grid, double z1Grid, int zStepsGrid,
		double alpha);
	void init();
	void solve(std::vector<FiniteElem>&_elems);
	void reset();
	void getB(std::vector<Point> parameters, std::vector<Point>& B);

	void getGridInformation(GridInformation& gridInfo);
	void getResultGrids(std::vector<Point>& nodes, std::vector<double>& yLayers);
	void getMeasureGrids(std::vector<double>& xGrid, std::vector<double>& yGrid);
	void getDiscrepancyByY(int y, std::vector<double>& fx);
	void getDiscrepancyByX(int x, std::vector<double>& fx);
	void getMagneticInductionByY(int y, std::vector<double>& magneticInduction);
	void getMagneticInductionByX(int x, std::vector<double>& magneticInduction);


private:
	double alpha;

	std::vector<Measure> measures;
	std::vector<Point> nodes;
	std::vector<FiniteElem> elems;
	std::vector<double> gaussWeights;
	std::vector<double> gaussPoints;

	// grid parameters:
	GridInformation gridInfo;

	std::vector<double> xAxisGrid;
	std::vector<double> yAxisGrid;
	std::vector<double> zAxisGrid;

	std::vector<double> xAxisMeasures;
	std::vector<double> yAxisMeasures;
	double zMeasure;

	int yResidual;

	std::vector<std::vector<double>> matrix;
	std::vector<double> rightPart;

	// for save results values:
	std::vector<double> p;
	std::vector<double> residualValues;
	std::vector<Point> magneticInductionValues;


	void fillAxisGrid(std::vector<double>& axis, double a, double b, int steps);
	void fillAxisMeasures(std::vector<double>& axis, double a, int steps, double h);
	
	// ���������� �������� B � i-����� ��������� 
	Point calculateB(int i);
	// ���������� �������� B � i-����� ��������� �� k-��������
	Point B(int i, FiniteElem elem);
	// calculate element volume
	double mes(FiniteElem elem);
	// calculate distance between p1 and p2 
	double r(Point p1, Point p2);
	// ���������� ��������� ����� p1 ��� �������� ������ ��������� � p2
	Point calculateCoords(Point p1, Point p2);
	//������ ���������� � ��������� ��������������
	double variableChange(double var, double a, double b);

	std::vector<double>L(int number, Point point);
	std::vector<double>Lx(FiniteElem elem, Point point);
	std::vector<double>Ly(FiniteElem elem, Point point);
	std::vector<double>Lz(FiniteElem elem, Point point);
	std::vector<double>B(Measure m);

	void alphaRegularization();

	void calcResidual(int y, std::vector<double>& residual);

	double vectorsProduct(std::vector<double>v1, std::vector<double> v2);
};