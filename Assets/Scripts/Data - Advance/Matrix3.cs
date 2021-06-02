// Matrix3
using UnityEngine;

public struct Matrix3
{
	public float[,] m;

	public static Matrix3 CreateMtx()
	{
		Matrix3 result = default(Matrix3);
		result.m = new float[3, 3];
		return result;
	}

	public static Matrix3 CreateMtxCol(Vector3 col0, Vector3 col1, Vector3 col2)
	{
		Matrix3 result = default(Matrix3);
		result.m = new float[3, 3];
		for (int i = 0; i < 3; i++)
		{
			result.m[i, 0] = col0[i];
			result.m[i, 1] = col1[i];
			result.m[i, 2] = col2[i];
		}
		return result;
	}

	public static Matrix3 CreateMtxIdentity()
	{
		return CreateMtx(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);
	}

	public static Matrix3 CreateMtx(float _11, float _12, float _13, float _21, float _22, float _23, float _31, float _32, float _33)
	{
		Matrix3 result = default(Matrix3);
		result.m = new float[3, 3];
		result.m[0, 0] = _11;
		result.m[0, 1] = _12;
		result.m[0, 2] = _13;
		result.m[1, 0] = _21;
		result.m[1, 1] = _22;
		result.m[1, 2] = _23;
		result.m[2, 0] = _31;
		result.m[2, 1] = _32;
		result.m[2, 2] = _33;
		return result;
	}

	public void SetValue(ref Matrix3 mat)
	{
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				m[i, j] = mat.m[i, j];
			}
		}
	}

	public Vector3 getRow(int row)
	{
		return new Vector3(m[row, 0], m[row, 1], m[row, 2]);
	}

	public Vector3 getCol(int col)
	{
		return new Vector3(m[0, col], m[1, col], m[2, col]);
	}

	public void setCol(int col, Vector3 v)
	{
		for (int i = 0; i < 3; i++)
		{
			m[i, col] = v[i];
		}
	}

	public void setCol(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		setCol(0, v1);
		setCol(1, v2);
		setCol(2, v3);
	}

	public static Vector3 operator *(Matrix3 mat, Vector3 v)
	{
		return new Vector3(mat.m[0, 0] * v.x + mat.m[0, 1] * v.y + mat.m[0, 2] * v.z, mat.m[1, 0] * v.x + mat.m[1, 1] * v.y + mat.m[1, 2] * v.z, mat.m[2, 0] * v.x + mat.m[2, 1] * v.y + mat.m[2, 2] * v.z);
	}

	public static Matrix3 operator *(Matrix3 mat1, Matrix3 mat2)
	{
		return CreateMtx(mat1.m[0, 0] * mat2.m[0, 0] + mat1.m[0, 1] * mat2.m[1, 0] + mat1.m[0, 2] * mat2.m[2, 0], mat1.m[0, 0] * mat2.m[0, 1] + mat1.m[0, 1] * mat2.m[1, 1] + mat1.m[0, 2] * mat2.m[2, 1], mat1.m[0, 0] * mat2.m[0, 2] + mat1.m[0, 1] * mat2.m[1, 2] + mat1.m[0, 2] * mat2.m[2, 2], mat1.m[1, 0] * mat2.m[0, 0] + mat1.m[1, 1] * mat2.m[1, 0] + mat1.m[1, 2] * mat2.m[2, 0], mat1.m[1, 0] * mat2.m[0, 1] + mat1.m[1, 1] * mat2.m[1, 1] + mat1.m[1, 2] * mat2.m[2, 1], mat1.m[1, 0] * mat2.m[0, 2] + mat1.m[1, 1] * mat2.m[1, 2] + mat1.m[1, 2] * mat2.m[2, 2], mat1.m[2, 0] * mat2.m[0, 0] + mat1.m[2, 1] * mat2.m[1, 0] + mat1.m[2, 2] * mat2.m[2, 0], mat1.m[2, 0] * mat2.m[0, 1] + mat1.m[2, 1] * mat2.m[1, 1] + mat1.m[2, 2] * mat2.m[2, 1], mat1.m[2, 0] * mat2.m[0, 2] + mat1.m[2, 1] * mat2.m[1, 2] + mat1.m[2, 2] * mat2.m[2, 2]);
	}
}
