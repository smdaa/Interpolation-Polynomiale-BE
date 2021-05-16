using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CalculHodographe : MonoBehaviour
{
    // Nombre de subdivision dans l'algo de DCJ
    public int NombreDeSubdivision = 3;
    // Liste des points composant la courbe de l'hodographe
    private List<Vector3> ListePoints = new List<Vector3>();
    // Donnees i.e. points cliqués

    public GameObject Donnees;
    public GameObject particle;

    //////////////////////////////////////////////////////////////////////////
    // fonction : DeCasteljauSub                                            //
    // semantique : renvoie la liste des points composant la courbe         //
    //              approximante selon un nombre de subdivision données     //
    // params : - List<float> X : abscisses des point de controle           //
    //          - List<float> Y : odronnees des point de controle           //
    //          - int nombreDeSubdivision : nombre de subdivision           //
    // sortie :                                                             //
    //          - (List<float>, List<float>) : liste des abscisses et liste //
    //            des ordonnées des points composant la courbe              //
    //////////////////////////////////////////////////////////////////////////
    (List<float>, List<float>) DeCasteljauSub(List<float> X, List<float> Y, int nombreDeSubdivision)
    {

        int n = X.Count;

        List<float> Q_x = new List<float>();
        List<float> Q_y = new List<float>();

        List<float> R_x = new List<float>();
        List<float> R_y = new List<float>();

        List<float> Qsub_x = new List<float>();
        List<float> Qsub_y = new List<float>();

        List<float> Rsub_x = new List<float>();
        List<float> Rsub_y = new List<float>();

        Q_x.Add(X[0]); Q_y.Add(Y[0]);
        R_x.Add(X[n - 1]); R_y.Add(Y[n - 1]);

        int i = n - 2;
        while (i >= 0)
        {
            int j = 0;
            while (j <= i)
            {
                X[j] = (float)(0.5 * X[j + 1] + 0.5 * X[j]);
                Y[j] = (float)(0.5 * Y[j + 1] + 0.5 * Y[j]);
                j = j + 1;
            }
            Q_x.Add(X[0]); Q_y.Add(Y[0]);
            R_x.Add(X[i]); R_y.Add(Y[i]);
            i = i - 1;
        }

        R_x.Reverse(); R_y.Reverse();

        if (nombreDeSubdivision == 1)
        {
            Q_x.AddRange(R_x); Q_y.AddRange(R_y);

            return (Q_x, Q_y);
        }
        else
        {
            (Qsub_x, Qsub_y) = DeCasteljauSub(Q_x, Q_y, nombreDeSubdivision - 1);
            (Rsub_x, Rsub_y) = DeCasteljauSub(R_x, R_y, nombreDeSubdivision - 1);
            Qsub_x.AddRange(Rsub_x); Qsub_y.AddRange(Rsub_y);

            return (Qsub_x, Qsub_y);
        }

    }

    //////////////////////////////////////////////////////////////////////////
    // fonction : Hodographe                                                //
    // semantique : renvoie la liste des vecteurs vitesses entre les paires //
    //              consécutives de points de controle                      //
    //              approximante selon un nombre de subdivision données     //
    // params : - List<float> X : abscisses des point de controle           //
    //          - List<float> Y : odronnees des point de controle           //
    //          - float Cx : offset d'affichage en x                        //
    //          - float Cy : offset d'affichage en y                        //
    // sortie :                                                             //
    //          - (List<float>, List<float>) : listes composantes des       //
    //            vecteurs vitesses sous la forme (Xs,Ys)                   //
    //////////////////////////////////////////////////////////////////////////
    (List<float>, List<float>) Hodographe(List<float> X, List<float> Y, float Cx = 1.5f, float Cy = 0.0f)
    {
        List<float> Xs = new List<float>();
        List<float> Ys = new List<float>();

        int n = X.Count;
        for (int i = 0; i < n-1; i++)
        {
            Xs.Add(n * (X[i + 1] - X[i]) - Cx);
            Ys.Add(n * (Y[i + 1] - Y[i]) - Cy);
        }

        return (Xs, Ys);
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
            Instantiate(particle, new Vector3(1.5f, -4.0f, 0.0f), Quaternion.identity);
            var ListePointsCliques = GameObject.Find("Donnees").GetComponent<Points>();
            if (ListePointsCliques.X.Count > 0)
            {
                List<float> XSubdivision = new List<float>();
                List<float> YSubdivision = new List<float>();
                List<float> dX = new List<float>();
                List<float> dY = new List<float>();
                
                (dX, dY) = Hodographe(ListePointsCliques.X, ListePointsCliques.Y);

                (XSubdivision, YSubdivision) = DeCasteljauSub(dX, dY, NombreDeSubdivision);
                for (int i = 0; i < XSubdivision.Count; ++i)
                {
                    ListePoints.Add(new Vector3(XSubdivision[i], -4.0f, YSubdivision[i]));
                }
            }

        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < ListePoints.Count - 1; ++i)
        {
            Gizmos.DrawLine(ListePoints[i], ListePoints[i + 1]);
        }
    }
}
