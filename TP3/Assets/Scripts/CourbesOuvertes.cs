using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class CourbesOuvertes : MonoBehaviour
{
    // Liste des points composant la courbe
    private List<Vector3> ListePoints = new List<Vector3>();
    // Donnees i.e. points cliqués
    public GameObject Donnees;
    // Coordonnees des points composant le polygone de controle
    private List<float> PolygoneControleX = new List<float>();
    private List<float> PolygoneControleY = new List<float>();

    // degres des polynomes par morceaux
    public int degres = 5;
    // nombre d'itération de subdivision
    public int nombreIteration = 5;

    List<float> midpoints(List<float> X)
    {
        List<float> Xsub = new List<float>();
        Xsub.Add(X[0]);
        for (int i = 0; i < X.Count - 1; i++)
        {
            Xsub.Add(0.5f * (X[i] + X[i + 1]));
        }
        Xsub.Add(X.Last());

        return Xsub;
    }

    // duplicate points
    List<float> duplicate(List<float> X)
    {
        List<float> Xres = new List<float>();
        for (int j = 0; j < X.Count; j++)
        {
            Xres.Add(X[j]); Xres.Add(X[j]);
        }
        return Xres;

    }
    //////////////////////////////////////////////////////////////////////////
    // fonction : subdivise                                                 //
    // semantique : réalise nombreIteration subdivision pour des polys de   //
    //              degres degres                                           //
    // params : - List<float> X : abscisses des point de controle           //
    //          - List<float> Y : odronnees des point de controle           //
    // sortie :                                                             //
    //          - (List<float>, List<float>) : points de la courbe          //
    //////////////////////////////////////////////////////////////////////////
    (List<float>, List<float>) subdivise(List<float> X, List<float> Y)
    {
        List<float> Xres = new List<float>();
        List<float> Yres = new List<float>();

        for (int i = 0; i < nombreIteration; i++)
        {
            //Dupliquer les points de contrôle (subdivision au degré 0).
            Xres = duplicate(X);
            Yres = duplicate(Y);

            //Prendre le milieu de deux points consécutifs dans le polynôme de de contrôle.
            for (int k = 0; k < degres; k++)
            {
                List<float> Xsubdivision = midpoints(Xres);
                List<float> Ysubdivision = midpoints(Yres);

                Xres = new List<float>(Xsubdivision);
                Yres = new List<float>(Ysubdivision);

            }
            X = new List<float>(Xres);
            Y = new List<float>(Yres);
        }


        return (Xres, Yres);
    }

    //////////////////////////////////////////////////////////////////////////
    //////////////////////////// NE PAS TOUCHER //////////////////////////////
    //////////////////////////////////////////////////////////////////////////

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

                List<float> Xres = new List<float>();
                List<float> Yres = new List<float>();

                (Xres, Yres) = subdivise(ListePointsCliques.X, ListePointsCliques.Y);
                for (int i = 0; i < Xres.Count; ++i)
                {
                    ListePoints.Add(new Vector3(Xres[i], -4.0f, Yres[i]));
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < PolygoneControleX.Count - 1; ++i)
        {
            Gizmos.DrawLine(new Vector3(PolygoneControleX[i], -4.0f, PolygoneControleY[i]), new Vector3(PolygoneControleX[i + 1], -4.0f, PolygoneControleY[i + 1]));
        }
        if (PolygoneControleX.Count > 0)
        {
            Gizmos.DrawLine(new Vector3(PolygoneControleX[PolygoneControleX.Count - 1], -4.0f, PolygoneControleY[PolygoneControleY.Count - 1]), new Vector3(PolygoneControleX[0], -4.0f, PolygoneControleY[0]));
        }

        Gizmos.color = Color.blue;
        for (int i = 0; i < ListePoints.Count - 1; ++i)
        {
            Gizmos.DrawLine(ListePoints[i], ListePoints[i + 1]);
        }
        if (ListePoints.Count > 0)
        {
            Gizmos.DrawLine(ListePoints[ListePoints.Count - 1], ListePoints[0]);
        }
    }

}
