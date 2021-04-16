#include "task.h"
#define TASK_DIM 3
#define ELEM_DIM 8

void Task::init() {
	int n;
	std::ifstream fin;

	// узлы элементов
	fin.open("nodes.txt");
	// в начале считать количество узлов
	fin >> n;
	nodes.resize(n);
	for (Point &node : nodes) {
		fin >> node.x >> node.y >> node.z;
	}
	fin.close();

	// элементы
	fin.open("elems.txt");
	// в начале считать количество элементов
	fin >> n;
	elems.resize(n);
	for (FiniteElem &elem : elems) {
		elem.nodes.resize(ELEM_DIM);
		for (int &node : elem.nodes) {
			fin >> node;
		}
	}
	fin.close();

	// измерения
	fin.open("measures.txt");
	// в начале считать количество измерений
	fin >> n;
	measures.resize(n);
	for (Measure &m : measures) {
		fin >> m.point.x >> m.point.y >> m.point.z;
		//fin >> m.B.x >> m.B.y >> m.B.z;
	}
	fin.close();
	
	gaussPoints = { -0.774596669, 0.0 , 0.774596669 };
	gaussWeights = { 0.555555556, 0.888888889, 0.555555556 };

	//инициализация структур СЛАУ
	double dim = TASK_DIM * elems.size();
	matrix.resize(dim);
	rightPart.resize(dim);
	p.resize(dim);
	for (int i = 0; i < matrix.size(); i++) {
		matrix[i].resize(dim);
	}
	yResidual = 0;
	alpha = 1.0;
	xAxisGrid.resize(3);
}

//void Task::calculateB(Measure &m) {
//	m.B = Point(0, 0, 0);
//	for (FiniteElem elem:elems) {
//		Point b = B(m, elem);
//		m.B.x += b.x;
//		m.B.y += b.y;
//		m.B.z += b.z;
//	}
//}



void Task::getGridInformation(GridInformation& _gridInfo) {
	_gridInfo = gridInfo;
}

void Task::getResultGrids(std::vector<Point>& _nodes, std::vector<double>& _yLayers) {
	_nodes = nodes;
	_yLayers.resize(yAxisGrid.size() - 1);
	for (int i = 1; i < yAxisGrid.size(); i++) {
		_yLayers[i - 1] = (yAxisGrid[i] + yAxisGrid[i - 1]) / 2;
	}
}

void Task::getMeasureGrids(std::vector<double>& xGrid, std::vector<double>& yGrid) {
	xGrid = xAxisMeasures;
	yGrid = yAxisMeasures;
}

void Task::getDiscrepancy(int yLayer, std::vector<double> fx) {
	calcResidual(yLayer, fx);
}

void Task::init(double hxMeasure, int nxMeasure, double hyMeasure, int nyMeasure,
	Point p0Measure, std::vector<Point> B,
	double x0Grid, double x1Grid, int xStepsGrid,
	double y0Grid, double y1Grid, int yStepsGrid,
	double z0Grid, double z1Grid, int zStepsGrid,
	double _alpha) {

	fillAxisGrid(xAxisGrid, x0Grid, x1Grid, xStepsGrid);
	fillAxisGrid(yAxisGrid, y0Grid, y1Grid, yStepsGrid);
	fillAxisGrid(zAxisGrid, z0Grid, z1Grid, zStepsGrid);

	int pointsInX = xAxisGrid.size();
	int pointsInY = yAxisGrid.size();
	int pointsInZ = zAxisGrid.size();
	int pointsInXY = pointsInX * pointsInY;

	//заполнение массива узлов
	nodes.resize(pointsInX * pointsInY * pointsInZ);
	int ni = 0;
	for (int zi = 0; zi < pointsInZ; zi++)
	{
		for (int yi = 0; yi < pointsInY; yi++)
		{
			for (int xi = 0; xi < pointsInX; xi++)
			{
				nodes[ni++] = Point(xAxisGrid[xi], yAxisGrid[yi], zAxisGrid[zi]);
			}
		}
	}

	//заполнение массива элементов
	elems.resize((pointsInX - 1) * (pointsInY - 1) * (pointsInZ - 1));

	int ei = 0;
	for (int zi = 0; zi < pointsInZ-1; zi++)
	{
		for (int yi = 0; yi < pointsInY - 1; yi++)
		{
			for (int xi = 0; xi < pointsInX - 1; xi++)
			{
				int n0 = zi * pointsInXY + yi * pointsInX + xi;
				int n1 = zi * pointsInXY + yi * pointsInX + xi + 1;
				int n2 = zi * pointsInXY + (yi + 1) * pointsInX + xi;
				int n3 = zi * pointsInXY + (yi + 1) * pointsInX + xi + 1;
				int n4 = (zi + 1) * pointsInXY + yi * pointsInX + xi;
				int n5 = (zi + 1) * pointsInXY + yi * pointsInX + xi + 1;
				int n6 = (zi + 1) * pointsInXY + (yi + 1) * pointsInX + xi;
				int n7 = (zi + 1) * pointsInXY + (yi + 1) * pointsInX + xi + 1;
				int value = (xi + 1) * (yi + 1) * (zi + 1);

				Point p = Point(0, 0, 0);
				elems[ei++] = FiniteElem(std::vector<int>{n0, n1, n2, n3, n4, n5, n6, n7}, p);
			}
		}
	}

	//заполнение массива измерений
	measures.clear();
	fillAxisMeasures(xAxisMeasures, p0Measure.x, nxMeasure, hxMeasure);
	fillAxisMeasures(yAxisMeasures, p0Measure.y, nyMeasure, hyMeasure);
	zMeasure = p0Measure.z;
	int bi = 0;
	for (int j = 0; j < yAxisMeasures.size(); j++) {
		for (int i = 0; i < xAxisMeasures.size(); i++) {
			measures.push_back(Measure(Point(xAxisMeasures[i], yAxisMeasures[j], zMeasure), B[bi++]));
		}
	}

	gaussPoints = { -0.774596669, 0.0 , 0.774596669 };
	gaussWeights = { 0.555555556, 0.888888889, 0.555555556 };

	// just be:
	yResidual = 0;

	//инициализация структур СЛАУ
	double dim = TASK_DIM * elems.size();
	matrix.resize(dim);
	rightPart.resize(dim);
	p.resize(dim);
	for (int i = 0; i < matrix.size(); i++) {
		matrix[i].resize(dim);
	}

	// save grid info:
	gridInfo.elemsInX = pointsInX - 1;
	gridInfo.elemsInY = pointsInY - 1;
	gridInfo.elemsInZ = pointsInY - 1;
	gridInfo.elemsSize = gridInfo.elemsInX * gridInfo.elemsInY * gridInfo.elemsInZ;
	gridInfo.yResultsLayersSize = yAxisGrid.size() - 1;
	gridInfo.yMeasureLayersSize = yAxisMeasures.size() - 1;
	gridInfo.xMeasureLayersSize = xAxisMeasures.size() - 1;

	gridInfo.dx = xAxisGrid[1] - xAxisGrid[0];
	gridInfo.xStart = xAxisGrid.front();
	gridInfo.xEnd = xAxisGrid.back();
	gridInfo.dz  = zAxisGrid[1] - zAxisGrid[0];
	gridInfo.zStart = zAxisGrid.front();
	gridInfo.zEndl = zAxisGrid.back();
}

Point Task::calculateB(int i) {
	Point res = Point (0,0,0);
	for (FiniteElem elem : elems) {
		Point b = B(i, elem);
		res.x += b.x;
		res.y += b.y;
		res.z += b.z;
	}
	return res;
}

Point Task::B(int i, FiniteElem elem) {
	double sumx = 0;
	double sumy = 0;
	double sumz = 0;
	for (int gx = 0; gx < gaussPoints.size();gx++) {
		for (int gy = 0; gy < gaussPoints.size(); gy++) {
			for (int gz = 0; gz < gaussPoints.size(); gz++) {
				Point gaussPoint = Point(
					variableChange(gaussPoints[gx], nodes[elem.nodes[0]].x, nodes[elem.nodes[elem.nodes.size() - 1]].x),
					variableChange(gaussPoints[gy], nodes[elem.nodes[0]].y, nodes[elem.nodes[elem.nodes.size() - 1]].y),
					variableChange(gaussPoints[gz], nodes[elem.nodes[0]].z, nodes[elem.nodes[elem.nodes.size() - 1]].z));
				double rj = r(measures[i].point, gaussPoint);
				Point pj = calculateCoords(measures[i].point, gaussPoint);

				sumx += gaussWeights[gx] * gaussWeights[gy] * gaussWeights[gz] / (4.0 * M_PI * pow(rj, 3)) *
					(elem.p.x * (3.0 * pow(pj.x, 2) / pow(rj, 2) - 1.0) +
					 elem.p.y * (3.0 * pj.x * pj.y / pow(rj, 2)) +
					 elem.p.z * (3.0 * pj.x * pj.z / pow(rj, 2)));

				sumy += gaussWeights[gx] * gaussWeights[gy] * gaussWeights[gz] / (4.0 * M_PI * pow(rj, 3)) *
					(elem.p.y * (3.0 * pow(pj.y, 2) / pow(rj, 2) - 1.0) +
						elem.p.x * (3.0 * pj.x * pj.y / pow(rj, 2)) +
						elem.p.z * (3.0 * pj.y * pj.z / pow(rj, 2)));

				sumz += gaussWeights[gx] * gaussWeights[gy] * gaussWeights[gz] / (4.0 * M_PI * pow(rj, 3)) *
					(elem.p.z * (3.0 * pow(pj.z, 2) / pow(rj, 2) - 1.0) +
						elem.p.x * (3.0 * pj.x * pj.z / pow(rj, 2)) +
						elem.p.y * (3.0 * pj.y * pj.z / pow(rj, 2)));
			}
		}
	}
	return Point(mes(elem) * sumx, mes(elem) * sumy, mes(elem) * sumz);
}

double Task::mes(FiniteElem elem) {
	
	double a = abs(nodes[elem.nodes[elem.nodes.size()-1]].x - nodes[elem.nodes[0]].x);
	double b = abs(nodes[elem.nodes[elem.nodes.size() - 1]].y - nodes[elem.nodes[0]].y);
	double c = abs(nodes[elem.nodes[elem.nodes.size() - 1]].z - nodes[elem.nodes[0]].z);
	return a * b * c;
}

double Task::r(Point p1, Point p2) {
	return sqrt(pow(p2.x - p1.x, 2) + pow(p2.y - p1.y, 2)+ pow(p2.z - p1.z, 2));
}


Point Task::calculateCoords(Point p1, Point p2) {
	return Point(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
}

double Task::variableChange(double var, double a, double b) {
	return (b + a) / 2.0 + (b - a) / 2.0 * var;
}

void Task::getB(std::vector<Point> parameters, std::vector<Point>& B) {
	B.resize(measures.size());
	int k = 0;
	// запоминаем новые параметры
	for (int i = 0; i < elems.size();i++) {
		elems[i].p = parameters[i];
	}
	for (int i = 0; i < B.size(); i++) {
		B[i] = calculateB(i);
	}
	// debug
	std::fstream fout;
	fout.open("experimentB.txt");
	for (int i = 0; i < B.size(); i++) {
		fout << B[i].x << "\t" << B[i].y << "\t" << B[i].z << endl;
	}
	fout.close();
}

std::vector<double> Task::Lx(FiniteElem elem, Point point) {
	std::vector<double> L(TASK_DIM, 0);
	for (int gx = 0; gx < gaussPoints.size(); gx++) {
		for (int gy = 0; gy < gaussPoints.size(); gy++) {
			for (int gz = 0; gz < gaussPoints.size(); gz++) {
				Point gaussPoint = Point(
					variableChange(gaussPoints[gx], nodes[elem.nodes[0]].x, nodes[elem.nodes[elem.nodes.size() - 1]].x),
					variableChange(gaussPoints[gy], nodes[elem.nodes[0]].y, nodes[elem.nodes[elem.nodes.size() - 1]].y),
					variableChange(gaussPoints[gz], nodes[elem.nodes[0]].z, nodes[elem.nodes[elem.nodes.size() - 1]].z));
				double rj = r(point, gaussPoint);
				Point pj = calculateCoords(point, gaussPoint);
				double coef = gaussWeights[gx] * gaussWeights[gy] * gaussWeights[gz] / (4.0 * M_PI * pow(rj, 3));
				L[0] += coef * ((3.0 * pow(pj.x, 2)) / pow(rj, 2) - 1.0);
				L[1] += coef * ((3.0 * pj.x * pj.y) / pow(rj, 2));
				L[2] += coef * ((3.0 * pj.x * pj.z) / pow(rj, 2));
			}
		}
	}
	L[0] *= mes(elem);
	L[1] *= mes(elem);
	L[2] *= mes(elem);
	return L;
}

std::vector<double> Task::Ly(FiniteElem elem, Point point) {
	std::vector<double> L(TASK_DIM, 0);
	for (int gx = 0; gx < gaussPoints.size(); gx++) {
		for (int gy = 0; gy < gaussPoints.size(); gy++) {
			for (int gz = 0; gz < gaussPoints.size(); gz++) {
				Point gaussPoint = Point(
					variableChange(gaussPoints[gx], nodes[elem.nodes[0]].x, nodes[elem.nodes[elem.nodes.size() - 1]].x),
					variableChange(gaussPoints[gy], nodes[elem.nodes[0]].y, nodes[elem.nodes[elem.nodes.size() - 1]].y),
					variableChange(gaussPoints[gz], nodes[elem.nodes[0]].z, nodes[elem.nodes[elem.nodes.size() - 1]].z));
				double rj = r(point, gaussPoint);
				Point pj = calculateCoords(point, gaussPoint);
				double coef = gaussWeights[gx] * gaussWeights[gy] * gaussWeights[gz] / (4.0 * M_PI * pow(rj, 3));
				L[0] += coef * ((3.0 * pj.x * pj.z) / pow(rj, 2));
				L[1] += coef * ((3.0 * pj.y * pj.z) / pow(rj, 2));
				L[2] += coef * ((3.0 * pow(pj.z, 2)) / pow(rj, 2) - 1.0);
			}
		}
	}
	L[0] *= mes(elem);
	L[1] *= mes(elem);
	L[2] *= mes(elem);
	return L;
}

std::vector<double>Task::Lz(FiniteElem elem, Point point) {
	std::vector<double> L(TASK_DIM, 0);
	for (int gx = 0; gx < gaussPoints.size(); gx++) {
		for (int gy = 0; gy < gaussPoints.size(); gy++) {
			for (int gz = 0; gz < gaussPoints.size(); gz++) {
				Point gaussPoint = Point(
					variableChange(gaussPoints[gx], nodes[elem.nodes[0]].x, nodes[elem.nodes[elem.nodes.size() - 1]].x),
					variableChange(gaussPoints[gy], nodes[elem.nodes[0]].y, nodes[elem.nodes[elem.nodes.size() - 1]].y),
					variableChange(gaussPoints[gz], nodes[elem.nodes[0]].z, nodes[elem.nodes[elem.nodes.size() - 1]].z));
				double rj = r(point, gaussPoint);
				Point pj = calculateCoords(point, gaussPoint);
				double coef = gaussWeights[gx] * gaussWeights[gy] * gaussWeights[gz] / (4.0 * M_PI * pow(rj, 3));
				L[0] += coef * ((3.0 * pj.x * pj.y) / pow(rj, 2));
				L[1] += coef * ((3.0 * pow(pj.y, 2)) / pow(rj, 2) - 1.0);
				L[2] += coef * ((3.0 * pj.y * pj.z) / pow(rj, 2));
			}
		}
	}
	L[0] *= mes(elem);
	L[1] *= mes(elem);
	L[2] *= mes(elem);
	return L;
}

std::vector<double>Task::L(int number, Point point) {
	int k = number % TASK_DIM;
	int elemNumber = number / TASK_DIM;
	switch (k) 
	{
	case 0:
		return Lx(elems[elemNumber], point);
		break;
	case 1:
		return Ly(elems[elemNumber], point);
		break;
	case 2:
		return Lz(elems[elemNumber], point);
		break;
	}
}

double Task::vectorsProduct(std::vector<double>v1, std::vector<double> v2) {
	double sum = 0;
	for (int i = 0; i < v1.size(); i++) {
		sum += v1[i] * v2[i];
	}
	return sum;
}

void Task::fillAxisGrid(std::vector<double>& axis, double a, double b, int steps) {
	double step = (b-a)/steps;
	double point = a;

	axis.clear();

	if (axis.empty()) axis.push_back(point);

	for (int i = 0; i < steps; i++) {
		point += step;
		axis.push_back(point);
	}
}

void Task::fillAxisMeasures(std::vector<double>& axis, double a, int steps, double h) {
	double point = a;
	axis.clear();
	for (int i = 0; i <= steps; i++) {
		axis.push_back(point);
		point += h;
	}
}

std::vector<double> Task::B(Measure m) {
	std::vector<double> B(TASK_DIM);
	B[0] = m.B.x;
	B[1] = m.B.y;
	B[2] = m.B.z;
	return B;
}

void Task::solve(std::vector<FiniteElem>& _elems) {
	for (int q = 0; q < matrix.size(); q++) {
		for (int s = 0; s < matrix[q].size(); s++) {
			for (Measure m : measures) {
				matrix[q][s] += vectorsProduct(L(q, m.point), L(s, m.point));
			}
		}
		for (Measure m : measures) {
			rightPart[q] += vectorsProduct(L(q, m.point), B(m));
		}
	}
	alphaRegularization();
	p = Gauss(matrix, rightPart, p);
	for (int i = 0, j=0; i < p.size();i+=3, j++) {
		elems[j].p.x = p[i];
		elems[j].p.y = p[i + 1];
		elems[j].p.z = p[i + 2];
	}
	_elems = elems;

	//calcResidual(yResidual, _f);
}

void Task::calcResidual(int y, std::vector<double>& residual) {
	std::vector<Point>parameters;
	std::vector<Point>calculatedB;
	for (int i = 0; i < elems.size(); i++) {
		parameters.push_back(elems[i].p);
	}
	getB(parameters, calculatedB);
	for (int i = y * xAxisGrid.size(); i < y * xAxisGrid.size() + xAxisGrid.size(); i++) {
		residual.push_back(abs(measures[i].B.x - calculatedB[i].x) / abs(measures[i].B.x) * 100);
	}
}

void Task::alphaRegularization() {
	for (int i = 0; i < matrix.size(); i++) {
		matrix[i][i] += alpha;
	}
}