#pragma once
#include <vector>

using namespace std;

vector<vector<double>> forward_path(vector<vector<double>> A);
vector<double> backward_path(vector<vector<double>> A, vector<double>x);
vector<double> Gauss(vector<vector<double>> A, vector<double>b, vector<double>x);