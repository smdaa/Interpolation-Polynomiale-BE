using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;

public class InterpolateurDeSurface : MonoBehaviour
{
    /*
    
    public GameObject particle;
    public float[,] X;
    public float[,] Hauteurs;
    public float[,] Z;
    public bool autoGenerateGrid;
    public float pas;
    private List<List<Vector3>> ListePoints = new List<List<Vector3>>();
    public enum EParametrisationType { Reguliere, Distance, RacineDistance, Tchebytcheff };
    public EParametrisationType ParametrisationType = EParametrisationType.Reguliere;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            List<float> T = new List<float>();
            List<float> tToEval = new List<float>();

            switch (ParametrisationType)
            {
                case EParametrisationType.Reguliere:
                    int n = X.GetLength(1);
                    List<float> X_i = new List<float>();
                    List<float> Z_i = new List<float>();
                    (T, tToEval) = buildParametrisationReguliere(n, pas);
                    for (int j = 0; j < n; ++j)
                    {
                        X_i.Add(X[1, j]);
                        Z_i.Add(Z[1, j]);
                    }
                    applyLagrangeParametrisation(X_i, Z_i, T, tToEval);
                    break;
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
    (List<float>, List<float>) buildParametrisationReguliere(int nbElem, float pas)
    {
        // Vecteur des pas temporels
        List<float> T = new List<float>();
        // Echantillonage des pas temporels
        List<float> tToEval = new List<float>();

        // Construction des pas temporels
        for (int i = 0; i < nbElem; i++)
        {
            T.Add(i);
        }

        // Construction des échantillons
        for (int i = 0; i < nbElem - 1; ++i)
        {
            tToEval.Add(T[i]);
            while (tToEval.Last() < T[i + 1])
            {
                tToEval.Add(Math.Min(tToEval.Last() + pas, T.Last()));
            }
        }

        return (T, tToEval);
    }

    void applyLagrangeParametrisation(List<float> X, List<float> Y, List<float> T, List<float> tToEval)
    {
        for (int i = 0; i < tToEval.Count; ++i)
        {
            // Calcul de xpoint et ypoint
            float xpoint = lagrange(tToEval[i], T, X);
            float ypoint = lagrange(tToEval[i], T, Y);
            flaot zpoint = 
            Vector3 pos = new Vector3(xpoint, 0.0f, ypoint);
            P2DRAW.Add(pos);
        }
    }
    private float lagrange(float x, List<float> X, List<float> Y)
    {
        float res = 0.0f;
        float temp = 1.0f;
        for (int k = 0; k < Y.Count; k++)
        {
            temp = 1.0f;
            for (int i = 0; i < X.Count; i++)
            {
                if (i != k)
                {
                    temp = temp * (x - X[i]) / (X[k] - X[i]);
                }
            }
            res = res + Y[k] * temp;
        }
        return res;
    }
    */
}

