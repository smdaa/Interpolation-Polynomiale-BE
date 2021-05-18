using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SurfaceBezierPT : MonoBehaviour
{
    public bool autoGenerateGrid;
    public GameObject particle;

    public float[,] X;
    public float[,] Hauteurs;
    public float[,] Z;

    public int NombreDeSubdivision = 3;

    private List<List<Vector3>> ListePoints = new List<List<Vector3>>();



    // Start is called before the first frame update
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

    (List<float>, List<float>, List<float>) DeCasteljauSub(List<float> X, List<float> Y, List<float> Z, int nombreDeSubdivision)
    {
        int n = X.Count;

        List<float> Q_x = new List<float>();
        List<float> Q_y = new List<float>();
        List<float> Q_z = new List<float>();

        List<float> R_x = new List<float>();
        List<float> R_y = new List<float>();
        List<float> R_z = new List<float>();

        List<float> Qsub_x = new List<float>();
        List<float> Qsub_y = new List<float>();
        List<float> Qsub_z = new List<float>();

        List<float> Rsub_x = new List<float>();
        List<float> Rsub_y = new List<float>();
        List<float> Rsub_z = new List<float>();

        Q_x.Add(X[0]); Q_y.Add(Y[0]); Q_z.Add(Z[0]);
        R_x.Add(X[n - 1]); R_y.Add(Y[n - 1]); R_z.Add(Z[n - 1]);

        int i = n - 2;
        while (i >= 0)
        {
            int j = 0;
            while (j <= i)
            {
                X[j] = (float)(0.5 * X[j + 1] + 0.5 * X[j]);
                Y[j] = (float)(0.5 * Y[j + 1] + 0.5 * Y[j]);
                Z[j] = (float)(0.5 * Z[j + 1] + 0.5 * Z[j]);
                j = j + 1;
            }
            Q_x.Add(X[0]); Q_y.Add(Y[0]); Q_z.Add(Z[0]);
            R_x.Add(X[i]); R_y.Add(Y[i]); R_z.Add(Z[i]);
            i = i - 1;
        }

        R_x.Reverse(); R_y.Reverse(); R_z.Reverse();

        if (nombreDeSubdivision == 1)
        {
            Q_x.AddRange(R_x); Q_y.AddRange(R_y); Q_z.AddRange(R_z);

            return (Q_x, Q_y, Q_z);
        }
        else
        {
            (Qsub_x, Qsub_y, Qsub_z) = DeCasteljauSub(Q_x, Q_y, Q_z, nombreDeSubdivision - 1);
            (Rsub_x, Rsub_y, Rsub_z) = DeCasteljauSub(R_x, R_y, R_z, nombreDeSubdivision - 1);
            Qsub_x.AddRange(Rsub_x); Qsub_y.AddRange(Rsub_y); Qsub_z.AddRange(Rsub_z);

            return (Qsub_x, Qsub_y, Qsub_z);
        }

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            List<List<float>> X3 = new List<List<float>>();
            List<List<float>> Z3 = new List<List<float>>();
            List<List<float>> Hauteurs3 = new List<List<float>>();

            for (int i = 0; i < 5; i++)
            {
                List<float> X1 = new List<float>();
                List<float> Hauteurs1 = new List<float>();
                List<float> Z1 = new List<float>();

                List<float> X2 = new List<float>();
                List<float> Hauteurs2 = new List<float>();
                List<float> Z2 = new List<float>();

                for (int j = 0; j < 5; j++)
                {
                    X1.Add(X[i, j]); Z1.Add(Z[i, j]); Hauteurs1.Add(Hauteurs[i, j]);
                }
                (X2, Hauteurs2, Z2) = DeCasteljauSub(X1, Hauteurs1, Z1, NombreDeSubdivision);
                X3.Add(X2);
                Z3.Add(Z2);
                Hauteurs3.Add(Hauteurs2);
            }

            for (int i = 0; i < X3[0].Count; i++)
            {
                List<float> x = new List<float>();
                List<float> h = new List<float>();
                List<float> y = new List<float>();

                List<float> X4 = new List<float>();
                List<float> Hauteurs4 = new List<float>();
                List<float> Z4 = new List<float>();
                List<Vector3> listvects = new List<Vector3>();

                for (int k = 0; k < 5; k++)
                {

                    X4.Add(X3[k][i]);
                    Hauteurs4.Add(Hauteurs3[k][i]);
                    Z4.Add(Z3[k][i]);
                }
                (x, h, y) = DeCasteljauSub(X4, Hauteurs4, Z4, NombreDeSubdivision);
                for (int j = 0; j < x.Count; j++)
                {
                    listvects.Add(new Vector3(x[j], h[j], y[j]));
                }
                ListePoints.Add(listvects);
            }
        }

    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
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
