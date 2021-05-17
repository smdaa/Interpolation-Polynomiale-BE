using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class SurfaceBezierTriangulaire : MonoBehaviour
{
    // Liste des points composant la surface   
    private List<List<Vector3>> ListePoints = new List<List<Vector3>>();
    public GameObject particle;
    public float[,] X;
    public float[,] Hauteurs;
    public float[,] Z;

    public bool autoGenerateGrid;


    // Pas d'échantillonage construire la parametrisation
    public float pas = (float)1 / (float)100;

    void Start()
    {
        if (autoGenerateGrid)
        {
            int n = 5;
            X = new float[5, 5];
            Hauteurs = new float[5, 5];
            Z = new float[5, 5];
            for (int i = 0; i < n; ++i)
            {
                X[i, 0] = 0.00f;
                X[i, 1] = 0.25f;
                X[i, 2] = 0.50f;
                X[i, 3] = 0.75f;
                X[i, 4] = 1.00f;
                Z[0, i] = 0.00f;
                Z[1, i] = 0.25f;
                Z[2, i] = 0.50f;
                Z[3, i] = 0.75f;
                Z[4, i] = 1.00f;
            }
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    float XC2 = (X[i, j] - (1.0f / 2.0f)) * (X[i, j] - (1.0f / 2.0f));
                    float ZC2 = (Z[i, j] - (1.0f / 2.0f)) * (Z[i, j] - (1.0f / 2.0f));
                    Hauteurs[i, j] = (float)Math.Exp(-(XC2 + ZC2));
                    Instantiate(particle, new Vector3(X[i, j], Hauteurs[i, j], Z[i, j]), Quaternion.identity);
                }
            }
        }
        else
        {

        }
    }

    long KparmiN(int k, int n)
    {

        decimal result = 1;
        for (int i = 1; i <= k; i++)
        {
            result *= n - (k - i);
            result /= i;
        }
        return (long)result;
    }

    float Bernstein(int m, int i, float u)
    {
        float ber;
        ber = (float)(KparmiN(i, m) * Math.Pow(1 - u, m - i) * Math.Pow(u, i));
        return ber;
    }


    Vector3 DeCasteljau(float[,] X, float[,] Hauteurs, float[,] Z, float u, float v)
    {
        int m = 5;
        int n = 5;
        float x = 0.0f;
        float hauteur = 0.0f;
        float z = 0.0f;

        for (int j = 0; j < n; j++)
        {
            float B_j_n = Bernstein(n, j, v);
            for (int i = 0; i < m; i++)
            {
                float B_i_m = Bernstein(m, i, u);
                x = x + B_j_n * B_i_m * X[i, j];
                z = z + B_j_n * B_i_m * Z[i, j];
                hauteur = hauteur + B_j_n * B_i_m * Hauteurs[i, j];
            }
        }
        return new Vector3(x, hauteur, z);
    }

    List<float> buildEchantillonnage2()
    {
        List<float> tToEval = new List<float>() { 0.0f };
        int k = 0;
        while (tToEval.Last() < 1)
        {
            tToEval.Add(Math.Min(1, tToEval.Last() + pas));
            k = k + 1;
        }
        return tToEval;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            List<float> T = buildEchantillonnage2();
            int n = T.Count;

            Vector3 point = new Vector3();

            for (int i = 0; i < n; i++)
            {
                List<Vector3> temp = new List<Vector3>();
                for (int j = 0; j < n-i; j++)
                {
                    float u = T[i];
                    float v = T[j];
                    point = DeCasteljau(X, Hauteurs, Z, u, v);
                    temp.Add(point);
                }
                ListePoints.Add(temp);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        if (autoGenerateGrid)
        {
            for (int j = 0; j < ListePoints.Count; ++j)
            {
                for (int i = 0; i < ListePoints[j].Count - 1; ++i)
                {
                    Gizmos.DrawLine(ListePoints[j][i], ListePoints[j][i + 1]);
                }
            }
        }
    }
}

