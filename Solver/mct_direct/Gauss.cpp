#include "Gauss.h"

void forward_path(vector<vector<double>>& A) {
	int n = A.size();
	for (int j = 0; j < n; j++) {
		//поиск максимального элемента в столбце
		int max_line = j;
		for (int i = j + 1; i < n; i++) {
			if (abs(A[max_line][j]) < abs(A[i][j]))
				max_line = i;
		}
		//обмен строк
		if (max_line != j)
			A[j].swap(A[max_line]);
		//обнуление элементов ниже j
		if (A[j][j] != 0) {
			vector<double> tmp(A[j].size());
			//из каждой строки от i+1 до n вычитаем первую строку, умноженную на масштабирующий множитель
			for (int i = j + 1; i < n; i++) {
				if (A[i][j] != 0) {
					tmp = A[j];
					double m = A[i][j] / A[j][j];
					for (int k = 0; k < n + 1; k++)
						tmp[k] = tmp[k] * m;
					for (int k = 0; k < n + 1; k++)
						A[i][k] -= tmp[k];
				}
			}
		}
	}
}

void backward_path(vector<vector<double>>& A, vector<double>& x) {
	int n = A.size();
	for (int i = n - 1; i >= 0; i--) {
		x[i] = A[i][n];
		double sum = 0;
		for (int k = n - 1; k > i; k--)
			sum += A[i][k] * x[k];
		x[i] -= sum;
		x[i] /= A[i][i];
	}
}

void Gauss(vector<vector<double>>& A, vector<double>& b, vector<double>& x) {
	for (int i = 0; i < A.size(); i++) {
		A[i].push_back(b[i]);
	}
	forward_path(A);
	backward_path(A, x);
}