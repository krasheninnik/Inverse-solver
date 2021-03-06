#include "task.h"
#define TASK_DIM 3
#define ELEM_DIM 8

void Task::init() {
	int n;
	std::ifstream fin;

	// ���� ���������
	fin.open("nodes.txt");
	// � ������ ������� ���������� �����
	fin >> n;
	nodes.resize(n);
	for (Point &node : nodes) {
		fin >> node.x >> node.y >> node.z;
	}
	fin.close();

	// ��������
	fin.open("elems.txt");
	// � ������ ������� ���������� ���������
	fin >> n;
	elems.resize(n);
	for (FiniteElem &elem : elems) {
		elem.nodes.resize(ELEM_DIM);
		for (int& node : elem.nodes) {
			fin >> node;
		}
	}
	fin.close();

	// ���������
	fin.open("measures.txt");
	// � ������ ������� ���������� ���������
	fin >> n;
	measures.resize(n);
	for (Measure& m : measures) {
		fin >> m.point.x >> m.point.y >> m.point.z;
		//fin >> m.B.x >> m.B.y >> m.B.z;
	}
	fin.close();
	
	gaussPoints = { -0.774596669, 0.0 , 0.774596669 };
	gaussWeights = { 0.555555556, 0.888888889, 0.555555556 };

	//������������� �������� ����
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

	// to storage calculated values of residual and magnetic induction
	assert(xAxisMeasures.size() > 0 && yAxisMeasures.size() > 0);
	residualValues.resize(xAxisMeasures.size() * yAxisMeasures.size());
	magneticInductionValues.resize(xAxisMeasures.size() * yAxisMeasures.size());
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


void Task::changeAlphaThings(double _alpha, double _pmin, double _pmax,
	double _firstAlpha, double _alphaStep, double _fittingProcentThreshold) {
	alpha = _alpha;
	pmin = _pmin;
	pmax = _pmax;
	firstAlpha = _firstAlpha;
	alphaStep = _alphaStep;
	fittingProcentThreshold = _fittingProcentThreshold;
}

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

void Task::init(double hxMeasure, int nxMeasure, double hyMeasure, int nyMeasure,
	Point p0Measure, std::vector<Point> B,
	double x0Grid, double x1Grid, int xStepsGrid,
	double y0Grid, double y1Grid, int yStepsGrid,
	double z0Grid, double z1Grid, int zStepsGrid,
	double _alpha, double _pmin, double _pmax,
	double _firstAlpha, double _alphaStep, double _fittingProcentThreshold) {

	fillAxisGrid(xAxisGrid, x0Grid, x1Grid, xStepsGrid);
	fillAxisGrid(yAxisGrid, y0Grid, y1Grid, yStepsGrid);
	fillAxisGrid(zAxisGrid, z0Grid, z1Grid, zStepsGrid);

	int pointsInX = xAxisGrid.size();
	int pointsInY = yAxisGrid.size();
	int pointsInZ = zAxisGrid.size();
	int pointsInXY = pointsInX * pointsInY;

	//���������� ������� �����
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

	//���������� ������� ���������
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

	//���������� ������� ���������
	measures.clear();
	measures.reserve(yAxisMeasures.size() * xAxisMeasures.size());
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
	alpha = _alpha;

	pmin = _pmin;
	pmax = _pmax;
	firstAlpha = _firstAlpha;
	alphaStep = _alphaStep;
	fittingProcentThreshold = _fittingProcentThreshold;

	// to storage calculated values of residual and magnetic induction
	residualValues.resize(xAxisMeasures.size() * yAxisMeasures.size());
	magneticInductionValues.resize(xAxisMeasures.size() * yAxisMeasures.size());

	//������������� �������� ����
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
	gridInfo.elemsInZ = pointsInZ - 1; 
	gridInfo.elemsSize = gridInfo.elemsInX * gridInfo.elemsInY * gridInfo.elemsInZ;
	gridInfo.pointsSize = nodes.size();
	gridInfo.yResultsLayersSize = yAxisGrid.size() - 1;
	gridInfo.yMeasureLayersSize = yAxisMeasures.size();
	gridInfo.xMeasureLayersSize = xAxisMeasures.size();

	gridInfo.dx = xAxisGrid[1] - xAxisGrid[0];
	gridInfo.xStart = xAxisGrid.front();
	gridInfo.xEnd = xAxisGrid.back();
	gridInfo.dz  = zAxisGrid[1] - zAxisGrid[0];
	gridInfo.zStart = zAxisGrid.front();
	gridInfo.zEndl = zAxisGrid.back();
}

Point Task::calculateB(int i) {
	Point res = Point (0,0,0);
	for (FiniteElem& elem : elems) {
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
	// ���������� ����� ���������
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

void Task::reset() {
	for (auto& row : matrix) std::fill(row.begin(), row.end(), 0);
	std::fill(rightPart.begin(), rightPart.end(), 0);
	std::fill(p.begin(), p.end(), 0);
}

auto get_time() { return std::chrono::high_resolution_clock::now(); }
void Task::buildMatrix() {
	auto start = get_time();
	std::cout << "Start solve task";
	concurrency::parallel_for(size_t(0), matrix.size(), [this](size_t q)
		{
			for (int s = 0; s < matrix[q].size(); s++) {
				for (Measure& m: measures) {
					matrix[q][s] += vectorsProduct(L(q, m.point), L(s, m.point));
				}
			}
			for (Measure&m : measures) {
				rightPart[q] += vectorsProduct(L(q, m.point), B(m));
			}
		});

	
	//_elems = elems;

	auto finish = get_time();
	auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(finish - start);
	std::cout << "Elapsed time = " << duration.count() << " ms\n";

	// store matrix (yeap, it's to much memory, but ....)
	matrixStored = matrix;
}

void Task::restoreMatrixAndSolve() {
	// restore builded matrix
	matrix = matrixStored;

	// solve
	alphaRegularization();
	Gauss(matrix, rightPart, p);

	// save results to elems
	for (int i = 0, j = 0; i < p.size(); i += 3, j++) {
		elems[j].p.x = p[i];
		elems[j].p.y = p[i + 1];
		elems[j].p.z = p[i + 2];
	}
}

void Task::solveWithAlphaSetted(std::vector<FiniteElem>& _elems, double* functionalVal) {

	restoreMatrixAndSolve();
	// save elems for output
	_elems = elems;

	// calculate additional residual and magnetic induction:
	std::vector<Point> parameters;
	std::vector<Point> calculatedB;

	parameters.reserve(elems.size());
	for (int i = 0; i < elems.size(); i++) {
		parameters.push_back(elems[i].p);
	}

	getB(parameters, calculatedB);


	double functionalCur = 0;
	for (int i = 0; i < residualValues.size(); i++) {
		double xResidual = abs(measures[i].B.x - calculatedB[i].x);
		double yResidual = abs(measures[i].B.y - calculatedB[i].y);
		double zResidual = abs(measures[i].B.z - calculatedB[i].z);

		// calculate functional:
		functionalCur += xResidual * xResidual + yResidual * yResidual + zResidual * zResidual;

		// calculate residual:
		residualValues[i] = Point(abs(measures[i].B.x - calculatedB[i].x) / abs(measures[i].B.x) * 100,
			abs(measures[i].B.y - calculatedB[i].y) / abs(measures[i].B.y) * 100,
			abs(measures[i].B.z - calculatedB[i].z) / abs(measures[i].B.z) * 100);
	}
	// save functional to output
	*functionalVal = functionalCur;

	// calculate magnetic induction:
	for (int i = 0; i < magneticInductionValues.size(); i++) {
		magneticInductionValues[i] = calculatedB[i];
	}
}

bool Task::isFindedParametersInTheRange() {
	bool inTheRange = true;

	for (int i = 0; i < elems.size(); i++) {
		if (pmin > elems[i].p.x || elems[i].p.x > pmax ||
			pmin > elems[i].p.y || elems[i].p.y > pmax ||
			pmin > elems[i].p.z || elems[i].p.z > pmax) {
			inTheRange = false;
			break;
		}
	}

	return inTheRange;
}

void Task::solveWithAlphaFitting(std::vector<FiniteElem>& _elems, double* _alpha, double* functionalVal) {
	// store setted alpha
	double settedAlpha = alpha;

	// init variables for calculating
	double functional0 = 0;
	double functionalCur = 0;
	std::vector<Point> parameters;
	std::vector<Point> calculatedB;
	parameters.resize(elems.size());

	// calculate with alpha = 0:
	alpha = 0;
	restoreMatrixAndSolve();

	for (int i = 0; i < elems.size(); i++) {parameters[i] = elems[i].p;}
	getB(parameters, calculatedB);

	std::cout << "parameters" << std::endl;
	for_each(parameters.begin(), parameters.end(), [](Point a) {std::cout << a.x << "\t" << a.y << "\t" << a.z << std::endl; });
	std::cout << std::endl << "calculatedB" << std::endl;
	for_each(calculatedB.begin(), calculatedB.end(), [](Point a) {std::cout << a.x << "\t" << a.y << "\t" << a.z << std::endl; });
	std::cout << std::endl << "measures" << std::endl;
	for_each(measures.begin(), measures.end(), [](auto a) {std::cout << a.B.x << "\t" << a.B.y << "\t" << a.B.z << std::endl; });
	std::cout << std::endl;


	// calculate functional:
	for (int i = 0; i < residualValues.size(); i++) {
		double xResidual = abs(measures[i].B.x - calculatedB[i].x);
		double yResidual = abs(measures[i].B.y - calculatedB[i].y);
		double zResidual = abs(measures[i].B.z - calculatedB[i].z);

		//residualValues[i] = Point(xResidual * 100, xResidual * 100, xResidual * 100);
		functional0 += xResidual * xResidual + yResidual * yResidual + zResidual * zResidual;
		if (isnan(functional0)) {
			std::cout << "functional0 is nan on the i-th elem: " << i << std::endl;
			return;
		}
	}

	bool inTheRange = isFindedParametersInTheRange();
	functionalCur = functional0;
	std::cout << "functional0: " << functional0 << std::endl;
	double functonalDiff = (abs(functional0 - functionalCur) / functional0) * 100;
	// debug things:
	std::cout << "Calculate functional with alpha = 0" << std::endl;
	std::cout << "alpha: " << alpha << ", functional diff: " << functonalDiff << ", in the range: " << inTheRange << std::endl;

	// set alpha to first alpha for fitting alpha
	alpha = firstAlpha;
	std::cout << "Start alpha fitting: " << std::endl;
	while (!inTheRange && functonalDiff < fittingProcentThreshold) {
		restoreMatrixAndSolve();
		/// output result alpha
		*_alpha = alpha;

		for (int i = 0; i < elems.size(); i++) { parameters[i] = elems[i].p; }
		getB(parameters, calculatedB);

		// calculate functional:
		functionalCur = 0;
		for (int i = 0; i < residualValues.size(); i++) {
			double xResidual = abs(measures[i].B.x - calculatedB[i].x);
			double yResidual = abs(measures[i].B.y - calculatedB[i].y);
			double zResidual = abs(measures[i].B.z - calculatedB[i].z);

			functionalCur += xResidual * xResidual + yResidual * yResidual + zResidual * zResidual;
		}

		// check if all the finded parameters in the possible range:
		inTheRange = isFindedParametersInTheRange();

		// check the diff between functional0 and current functional
		functonalDiff = (abs(functional0 - functionalCur) / functional0) * 100;

		// next interation alpha
		alpha *= alphaStep;
		// debug things:
		std::cout << "alpha: " << alpha << ", functional diff: " << functonalDiff << ", in the range: " << inTheRange << std::endl;
	}

	// save results for best (*** last ***) task solving...
	// calculate residual:
	for (int i = 0; i < residualValues.size(); i++) {
		double xResidual = abs(measures[i].B.x - calculatedB[i].x) / abs(measures[i].B.x);
		double yResidual = abs(measures[i].B.y - calculatedB[i].y) / abs(measures[i].B.y);
		double zResidual = abs(measures[i].B.z - calculatedB[i].z) / abs(measures[i].B.z);

		residualValues[i] = Point(xResidual * 100, xResidual * 100, xResidual * 100);
	}

	// calculate magnetic induction:
	for (int i = 0; i < magneticInductionValues.size(); i++) {
		magneticInductionValues[i] = calculatedB[i];
	}


	// save elems for output
	_elems = elems;
	// save functionalVal for output
	*functionalVal = functionalCur;

	// restore setted alpha
	alpha = settedAlpha;
}


void Task::getDiscrepancyByY(int y, std::vector<Point>& residual) {
	residual.resize(xAxisMeasures.size());
	int firstIndex = y * xAxisMeasures.size();
	int lastIndex = (y + 1) * xAxisMeasures.size();
	std::copy(residualValues.begin() + firstIndex, residualValues.begin() + lastIndex, residual.begin());
}

void Task::getDiscrepancyByX(int x, std::vector<Point>& residual) {
	residual.resize(yAxisMeasures.size());
	for (int k = 0, i = x; i < residualValues.size(); i += xAxisMeasures.size(), k++) {
		residual[k] = residualValues[i];
	}
}

void Task::getMagneticInductionByY(int y, std::vector<Point>& magneticInduction) {
	magneticInduction.resize(xAxisMeasures.size());
	int firstIndex = y * xAxisMeasures.size();
	int lastIndex = (y + 1) * xAxisMeasures.size();
	std::copy(magneticInductionValues.begin() + firstIndex, magneticInductionValues.begin() + lastIndex, magneticInduction.begin());
}

void Task::getMagneticInductionByX(int x, std::vector<Point>& magneticInduction) {
	magneticInduction.resize(yAxisMeasures.size());
	for (int k = 0, i = x; i < magneticInductionValues.size(); i += xAxisMeasures.size(), k++) {
		magneticInduction[k] = magneticInductionValues[i];
	}
}

void Task::alphaRegularization() {
	for (int i = 0; i < matrix.size(); i++) {
		matrix[i][i] += alpha;
	}
}