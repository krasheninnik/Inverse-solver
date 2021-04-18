#pragma once
#include <vector>

using namespace std;

void forward_path(vector<vector<double>>& A);
void backward_path(vector<vector<double>>& A, vector<double>& x);
void Gauss(vector<vector<double>>& A, vector<double>& b, vector<double>& x);