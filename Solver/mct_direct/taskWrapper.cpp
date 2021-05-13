#include "taskWrapper.h"

Task* createTask() {
    return new Task();
}

void deleteTask(Task *task) {
    delete task;
}

void initInverseTask(Task* task,
    double hxMeasure, int nxMeasure, double hyMeasure, int nyMeasure,
    Point p0Measure, Value* measuredValues, int measuredValuesSize,
    double xStart, double xEnd, int xStepsAmount,
    double yStart, double yEnd, int yStepsAmount,
    double zStart, double zEnd, int zStepsAmount,
    double _alpha, double pmin, double pmax,
    double firstAlpha, double alphaStep, double fittingProcentThreshold) {

    // transform C-ctyle array to vector
    std::vector<Value> B(measuredValuesSize);
    B.assign(measuredValues, measuredValues + measuredValuesSize);

    task->init(hxMeasure, nxMeasure, hyMeasure, nyMeasure, p0Measure, B,
        xStart, xEnd, xStepsAmount,
        yStart, yEnd, yStepsAmount,
        zStart, zEnd, zStepsAmount,
        _alpha, pmin, pmax,
        firstAlpha, alphaStep, fittingProcentThreshold);
}

void getGridInformation(Task* task, GridInformation& gridInfo) {
    task->getGridInformation(gridInfo);
}


void buildMatrix(Task* task) {
    task->reset();
    task->buildMatrix();
}

void solveWithAlphaSetted(Task* task, FiniteElemProxy* felemsProxy, double* functionalVal) {
    std::vector<FiniteElem> felems;
    task->solveWithAlphaSetted(felems, functionalVal);

    for (int i = 0; i < felems.size(); i++) {
        auto& el = felems[i];
        felemsProxy[i] = FiniteElemProxy(el.nodes[0], el.nodes[1], el.nodes[2], el.nodes[3], el.nodes[4], el.nodes[5], el.nodes[6], el.nodes[7], el.p);
    }
}
void solveWithAlphaFitting(Task* task, FiniteElemProxy* felemsProxy, double* alpha, double* functionalVal) {
    std::vector<FiniteElem> felems;
    task->solveWithAlphaFitting(felems, alpha, functionalVal);

    for (int i = 0; i < felems.size(); i++) {
        auto& el = felems[i];
        felemsProxy[i] = FiniteElemProxy(el.nodes[0], el.nodes[1], el.nodes[2], el.nodes[3], el.nodes[4], el.nodes[5], el.nodes[6], el.nodes[7], el.p);
    }
}

void getResultGrids(Task* task, Point* nodes, double* yLayers) {
    std::vector<Point> nodesVec;
    std::vector<double> yLayersVec;
    task->getResultGrids(nodesVec, yLayersVec);

    // Convert vectors to C-style arrays
    for (int i = 0; i < nodesVec.size(); i++) nodes[i] = nodesVec[i];
    for (int i = 0; i < yLayersVec.size(); i++) yLayers[i] = yLayersVec[i];
}

void getMeasureGrids(Task* task, double* xGrid, double* yGrid) {
    std::vector<double> xGridVec;
    std::vector<double> yGridVec;
    task->getMeasureGrids(xGridVec, yGridVec);

    // Convert vectors to C-style arrays
    for (int i = 0; i < xGridVec.size(); i++) xGrid[i] = xGridVec[i];
    for (int i = 0; i < yGridVec.size(); i++) yGrid[i] = yGridVec[i];
}

void getDiscrepancyByY(Task* task, int yLayerIndex, Point* residual) {
    std::vector<Point> fxVec;
    task->getDiscrepancyByY(yLayerIndex, fxVec);

    // Convert vectors to C-style arrays
    for (int i = 0; i < fxVec.size(); i++) residual[i] = fxVec[i];
}

void getDiscrepancyByX(Task* task, int xLayerIndex, Point* residual) {
    std::vector<Point> fxVec;
    task->getDiscrepancyByX(xLayerIndex, fxVec);

    // Convert vectors to C-style arrays
    for (int i = 0; i < fxVec.size(); i++) residual[i] = fxVec[i];
}


void getMagneticInductionByY(Task* task, int yLayerIndex, Point* magneticInduction) {
    std::vector<Point> fxVec;
    task->getMagneticInductionByY(yLayerIndex, fxVec);

    // Convert vectors to C-style arrays
    for (int i = 0; i < fxVec.size(); i++) magneticInduction[i] = fxVec[i];
}

void getMagneticInductionByX(Task* task, int xLayerIndex, Point* magneticInduction) {
    std::vector<Point> fxVec;
    task->getMagneticInductionByX(xLayerIndex, fxVec);

    // Convert vectors to C-style arrays
    for (int i = 0; i < fxVec.size(); i++) magneticInduction[i] = fxVec[i];
}

void changeAlphaThings(Task* task, double alpha, double pmin, double pmax,
    double firstAlpha, double alphaStep, double fittingProcentThreshold) {
    task->changeAlphaThings(alpha, pmin, pmax, firstAlpha, alphaStep, fittingProcentThreshold);
}