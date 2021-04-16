#include "task.h"

int main() {
	Task task;
	std::vector<Point>B;
	B.resize(1602, Point(0, 0, 0));
	std::cout << "Init task: " << std::endl;
	task.init(10, 800, 1, 1, Point(-2000, 0, 0), B, 2000, 3000, 2, 0, 1, 1, -1000, -500, 2, 1.0);
	std::vector<Point> parameters;
	parameters.resize(4, Point(1.0, 0, 0));

	std::cout << "Get B: " << std::endl;
	task.getB(parameters, B);
	task.init(10, 800, 1, 1, Point(-2000, 0, 0), B, 2000, 3000, 2, 0, 1, 1, -1000, -500, 2, 1.0);
	std::vector<FiniteElem>_elems;
	std::vector<double>fx;

	std::cout << "Solve Pips: " << std::endl;
	task.solve(_elems);
	return 0;
}