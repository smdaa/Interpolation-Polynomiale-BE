using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using System;

public class Trajectoire : MonoBehaviour
{
    // positions et rotations des points de controle
    List<Vector3> position;
    List<Quaternion> rotation;

    // compteur pour déplacer la caméra
    int i = 0;

    // choix de la méthode pour le suivi de la position approximation par DeCasteljau ou Spline
    public enum EApproximationType {DeCasteljau, Spline};
    public EApproximationType Approximation = EApproximationType.DeCasteljau;

    // degres des polynomes par morceaux
    public int degres = 9;
    // nombre d'itération de subdivision
    public int nombreIteration = 9;

    // postions et rotations du trajet 
    private List<Vector3> Approximation_curve = new List<Vector3>();
    private List<Quaternion> Interpolation_rotation = new List<Quaternion>();

    // Pas d'échantillonnage 
    public float pas = 0.01f;

    //////////////////////////////////////////////////////////////////////////
    // fonction   : buildEchantillonnage                                    //
    // semantique : construit un échantillonnage regulier                   //
    //////////////////////////////////////////////////////////////////////////
    List<float> buildEchantillonnage(float pas)
    {
        Assert.IsTrue(pas > 0);
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
    // fonction   : DeCasteljau                                             //
    // semantique : renvoie la liste des points composant la courbe         //
    //              approximante selon un nombre de subdivision données     //
    //              version itérative                                       //
    //////////////////////////////////////////////////////////////////////////
    Vector3 DeCasteljauIter(List<Vector3> PointsPosition, float t){
        List<float> X = new List<float>();
        List<float> Y = new List<float>();
        List<float> Z = new List<float>();
        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;
        float B;
        foreach(Vector3 v in PointsPosition){
            X.Add(v.x);
            Y.Add(v.y);
            Z.Add(v.z);
        }
        int n = PointsPosition.Count;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n-i-1; j++)
            {
                X[j] = (1 - t) * X[j] + t * X[j+1];
                Y[j] = (1 - t) * Y[j] + t * Y[j+1];
                Z[j] = (1 - t) * Z[j] + t * Z[j+1];
            }

        }
        return new Vector3(X[0], Y[0], Z[0]);
    }

    //////////////////////////////////////////////////////////////////////////
    // fonction   : DeCasteljau                                             //
    // semantique : renvoie la liste des points composant la courbe         //
    //              approximante selon un nombre de subdivision données     //
    //              version itérative                                       //
    //////////////////////////////////////////////////////////////////////////
    Vector3 DeCasteljauRec(List<Vector3> PointsPosition, float t){
        int n = PointsPosition.Count;
        if (n==1){
            return PointsPosition[0];
        } else {
            Vector3 p1 = DeCasteljauRec(PointsPosition.GetRange(0, n - 1), t);
            Vector3 p2 = DeCasteljauRec(PointsPosition.GetRange(1, n - 1), t);
            return Vector3.Lerp(p1, p2, t);
        }
    }
    //////////////////////////////////////////////////////////////////////////
    // fonction   : duplicate                                               //
    // semantique : Dupliquer les points de contrôle                        //
    //////////////////////////////////////////////////////////////////////////
    List<T> duplicate<T>(List<T> X)
    {
        List<T> Xres = new List<T>();
        for (int j = 0; j < X.Count; j++)
        {
            Xres.Add(X[j]); Xres.Add(X[j]);
        }
        return Xres;
    }

    //////////////////////////////////////////////////////////////////////////
    // fonction   : midpoints                                               //
    // semantique : Prendre le milieu de deux points consécutifs            //
    //////////////////////////////////////////////////////////////////////////
    List<float> midpoints(List<float> X)
    {
        List<float> Xres = new List<float>();
        for (int i = 0; i < X.Count-1; i++)
        {
            Xres.Add(0.5f *(X[i]+ X[i+1]));
        }
        Xres.Add(0.5f * (X[0] + X.Last()));
        return Xres;
    }

    //////////////////////////////////////////////////////////////////////////
    // fonction   : Subdivise                                               //
    // semantique : réalise nombreIteration subdivision pour des polys de   //
    //              degres degres                                           //
    //////////////////////////////////////////////////////////////////////////
    List<Vector3> Subdivise(List<Vector3> PointsPosition){
        List<float> X = new List<float>();
        List<float> Y = new List<float>();
        List<float> Z = new List<float>();

        List<float> Xres = new List<float>();
        List<float> Yres = new List<float>();
        List<float> Zres = new List<float>();

        List<Vector3> Pres = new List<Vector3>();

        foreach(Vector3 v in PointsPosition){
            X.Add(v.x);
            Y.Add(v.y);
            Z.Add(v.z);
        }

        for (int i = 0; i < nombreIteration; i++){
            Xres = duplicate(X);
            Yres = duplicate(Y);
            Zres = duplicate(Z);

            for (int k = 0; k < degres; k++){
                List<float> Xsubdivision = midpoints(Xres);
                List<float> Ysubdivision = midpoints(Yres);
                List<float> Zsubdivision = midpoints(Zres);

                Xres = new List<float>(Xsubdivision);
                Yres = new List<float>(Ysubdivision);
                Zres = new List<float>(Zsubdivision);
            }
            X = new List<float>(Xres);
            Y = new List<float>(Yres);
            Z = new List<float>(Zres);
        }
        Xres.Add(Xres[0]);
        Yres.Add(Yres[0]);
        Zres.Add(Zres[0]);

        for (int i = 0; i < Xres.Count; i++)
        {
            Pres.Add(new Vector3(Xres[i], Yres[i], Zres[i]));
        }
        return Pres;
    }

    //////////////////////////////////////////////////////////////////////////
    // fonction   : SlerpMultipleRec                                        //
    // semantique : réalise l’interpolation sphérique entre une liste des   //
    //              Quaternions version récursive                           //
    //////////////////////////////////////////////////////////////////////////
    Quaternion SlerpMultipleRec(List<Quaternion> PointsRotation, float t){
        int n = PointsRotation.Count;
        if (n == 1){
            return PointsRotation[0];
        } else {
            Quaternion q1 = SlerpMultipleRec(PointsRotation.GetRange(0, n - 1), t);
            Quaternion q2 = SlerpMultipleRec(PointsRotation.GetRange(1, n - 1), t);
            return Quaternion.Slerp(q1, q2, t);
        }
    }

    //////////////////////////////////////////////////////////////////////////
    // fonction   : SlerpMultipleIter                                       //
    // semantique : réalise l’interpolation sphérique entre une liste des   //
    //              Quaternions version itérative                           //
    //////////////////////////////////////////////////////////////////////////
    Quaternion SlerpMultipleIter(List<Quaternion> PointsRotation, float t){
        int n = PointsRotation.Count;
        List<Quaternion> PointsRotationCopy = new List<Quaternion>(PointsRotation);
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n-i-1; j++)
            {
                PointsRotationCopy[j] = Quaternion.Slerp(PointsRotationCopy[j], PointsRotationCopy[j+1], t);
            }
        }
        return PointsRotationCopy[0];
    }

    // Start is called before the first frame update
    void Start()
    {

        // On récupere les points de controle
        position = new List<Vector3>();
        rotation = new List<Quaternion>();
        GameObject[] Points = GameObject.FindGameObjectsWithTag("PT");
        
        // On fait un tri par le nom : PT1,PT2,PT3,PT4..
        List<GameObject> SortedPoints = Points.OrderBy(go=>go.name).ToList();

        foreach(GameObject go in SortedPoints){
            position.Add(go.transform.position);
            rotation.Add(go.transform.rotation);
        }

        switch (Approximation)
        {
            case EApproximationType.DeCasteljau:
                List<float> T = buildEchantillonnage(pas);

                foreach(float t in T){
                    Approximation_curve.Add(DeCasteljauIter(position, t));
                    Interpolation_rotation.Add(SlerpMultipleIter(rotation, t));
                }
                break;

            case EApproximationType.Spline:
                Approximation_curve = Subdivise(position);
                float passub = 1.0f / ((float)(Approximation_curve.Count - 1));
                T = buildEchantillonnage(passub);
                rotation.Add(rotation[0]);
                foreach(float t in T){
                    Interpolation_rotation.Add(SlerpMultipleIter(rotation, t));
                }
                break;
        }
        
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.green;
        
        for (int i = 0; i < Approximation_curve.Count - 1; ++i){
            Gizmos.DrawLine(Approximation_curve[i], Approximation_curve[i + 1]);
        }

        Gizmos.matrix = transform.localToWorldMatrix;           // For the rotation bug
        //Gizmos.DrawFrustum(transform.position, Camera.main.fieldOfView, Camera.main.nearClipPlane, Camera.main.farClipPlane, Camera.main.aspect);
        Debug.DrawRay(transform.position, transform.forward, Color.red, 0f, true);
    }



    // Update is called once per frame
    void Update()
    {
        transform.position = Approximation_curve[i % Approximation_curve.Count];
        transform.rotation = Interpolation_rotation[i % Interpolation_rotation.Count];
        i++;
    }

}
