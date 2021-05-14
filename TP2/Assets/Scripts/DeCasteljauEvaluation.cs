using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

//////////////////////////////////////////////////////////////////////////
///////////////// Classe qui gère l'évaluation via DCJ ///////////////////
//////////////////////////////////////////////////////////////////////////
public class DeCasteljauEvaluation : MonoBehaviour
{
    // Pas d'échantillonage construire la parametrisation
    public float pas = 1 / 100;
    // Liste des points composant la courbe
    private List<Vector3> ListePoints = new List<Vector3>();
    // Donnees i.e. points cliqués
    public GameObject Donnees;
    // Coordonnees des points composant le polygone de controle
    private List<float> PolygoneControleX = new List<float>();
    private List<float> PolygoneControleY = new List<float>();
    
    //////////////////////////////////////////////////////////////////////////
    // fonction : buildEchantillonnage                                       //
    // semantique : construit un échantillonnage regulier                    //
    // params : aucun                                                      //
    // sortie :                                                             //
    //          - List<float> tToEval : échantillonnage regulier             //
    //////////////////////////////////////////////////////////////////////////
    List<float> buildEchantillonnage()
    {
        List<float> tToEval = new List<float>() { 0.0f };
        int k = 0;
        while(tToEval.Last() < 1)
        {
            tToEval.Add(Math.Min(1, tToEval.Last() + pas)); 
            k = k + 1;
        }
        return tToEval;
    }

   //////////////////////////////////////////////////////////////////////////
    // fonction : DeCasteljau                                               //
    // semantique : renvoie le point approxime via l'aglgorithme de DCJ     //
    //              pour une courbe définie par les points de controle      //
    //              (X,Y) à l'instant t                                     //
    // params : - List<float> X : abscisses des point de controle           //
    //          - List<float> Y : odronnees des point de controle           //
    //          - float t : temps d'approximation                           //
    // sortie :                                                             //
    //          - Vector2 : point atteint au temps t                        //
    //////////////////////////////////////////////////////////////////////////
    Vector2 DeCasteljau(List<float> X, List<float> Y, float t)
    {
        /* version réccursive à éviter (O(2^n))
        int n = X.Count;
        float x = 0.0f;
        float y = 0.0f;
        Vector2 l1;
        Vector2 l2;
        if (n == 1)
        {
            x = X[0];
            y = Y[0];
        }
        else
        {
            l1 = DeCasteljau(X.GetRange(0, n - 1), Y.GetRange(0, n - 1), t);
            l2 = DeCasteljau(X.GetRange(1, n - 1), Y.GetRange(1, n - 1), t);
            x = t * l1.x + (1 - t) * l2.x;
            y = t * l1.y + (1 - t) * l2.y;
        }
        */

        /* version itérative (O(n^2)) */
        int n = X.Count;
        float x = 0.0f;
        float y = 0.0f;
        float B_k_t;

        for (int k = 0; k < n; k++)
        {
            B_k_t = (float)(KparmiN(k, n-1) * Math.Pow(1 - t, n - k - 1) * Math.Pow(t, k));
            x = x + B_k_t * X[k];
            y = y + B_k_t * Y[k];

        }
        return new Vector2(x, y);
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


    //////////////////////////////////////////////////////////////////////////
    //////////////////////////// NE PAS TOUCHER //////////////////////////////
    //////////////////////////////////////////////////////////////////////////

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            var ListePointsCliques = GameObject.Find("Donnees").GetComponent<Points>();
            if (ListePointsCliques.X.Count > 0)
            {
                for (int i = 0; i < ListePointsCliques.X.Count; ++i)
                {
                    PolygoneControleX.Add(ListePointsCliques.X[i]);
                    PolygoneControleY.Add(ListePointsCliques.Y[i]);
                }

                List<float> T = buildEchantillonnage();
                Vector2 point = new Vector2();
                foreach (float t in T)
                {
                    point = DeCasteljau(ListePointsCliques.X, ListePointsCliques.Y, t);
                    ListePoints.Add(new Vector3(point.x,-4.0f,point.y));
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < PolygoneControleX.Count - 1; ++i)
        {
            Gizmos.DrawLine(new Vector3(PolygoneControleX[i],-4.0f, PolygoneControleY[i]), new Vector3(PolygoneControleX[i + 1], -4.0f, PolygoneControleY[i + 1]));
        }

        Gizmos.color = Color.blue;
        for (int i = 0; i < ListePoints.Count - 1; ++i)
        {
            Gizmos.DrawLine(ListePoints[i], ListePoints[i + 1]);
        }
    }
}
