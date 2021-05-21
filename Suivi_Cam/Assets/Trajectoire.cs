using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Trajectoire : MonoBehaviour
{
    List<Vector3> position;
    List<Quaternion> rotation;

    int i = 0;

    public enum EApproximationType {DeCasteljau, Spline};
    public EApproximationType Approximation = EApproximationType.Spline;

    // degres des polynomes par morceaux
    public int degres = 5;
    // nombre d'itération de subdivision
    public int nombreIteration = 5;

    private List<Vector3> Approximation_curve = new List<Vector3>();
    private List<Quaternion> Interpolation_rotation = new List<Quaternion>();

    // Pas d'échantillonnage 
    public float pas = 0.01f;

    // Semantique : etant donnés k et n, calcule k parmi n 
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

    // semantique : construit un échantillonnage regulier 
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

    // semantique : renvoie la liste des points composant la courbe         //
    //              approximante selon un nombre de subdivision données     //
    Vector3 DeCasteljau(List<Vector3> PointsPosition, float t){
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
        for (int k = 0; k < n; k++){
            B = (float)(KparmiN(k, n-1) * Math.Pow(1 - t, n - k - 1) * Math.Pow(t, k));
            x = x + B * X[k];
            y = y + B * Y[k];
            z = z + B * Z[k];
        }
        return new Vector3(x, y, z);

    }

    List<float> duplicate(List<float> X)
    {
        List<float> Xres = new List<float>();
        for (int j = 0; j < X.Count; j++)
        {
            Xres.Add(X[j]); Xres.Add(X[j]);
        }
        return Xres;

    }

    List<float> midpoints(List<float> X)
    {
        List<float> Xsub = new List<float>();
        for (int i = 0; i < X.Count-1; i++)
        {
            Xsub.Add(0.5f *(X[i]+ X[i+1]));
        }
        Xsub.Add(0.5f * (X[0] + X.Last()));
        return Xsub;
    }

    // semantique : réalise nombreIteration subdivision pour des polys de   //
    //              degres degres                                           //
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

    Quaternion Slerp(Quaternion q1, Quaternion q2, float t){

        float cos_omega = q1.x*q2.x + q1.w*q2.w + q1.y*q2.y + q1.z*q2.z;
        if (cos_omega > 1.0f){
            cos_omega = 1.0f;
        }
        if (cos_omega < -1.0f){
            cos_omega = -1.0f;
        }
        float omega = (float) Math.Acos(cos_omega);
        float sin_omega = (float) Math.Sqrt(1.0f - cos_omega*cos_omega);
        float a = (float) Math.Sin((1.0 - t) * omega) / sin_omega;
        float b = (float) Math.Sin(t * omega) / sin_omega;
        return new Quaternion(a*q1.x + b*q2.x, a*q1.y + b*q2.y, a*q1.z + b*q2.z, a*q1.w + b*q2.w);

    }

    Vector3 Lerp(Vector3 p1, Vector3 p2, float t){
        float xres = (1-t) * p1.x + t * p2.x;
        float yres = (1-t) * p1.y + t * p2.y;
        float zres = (1-t) * p1.z + t * p2.z;
        return new Vector3(xres, yres, zres);        
    }

    // Start is called before the first frame update
    void Start()
    {
        position = new List<Vector3>();
        rotation = new List<Quaternion>();

        GameObject[] Points = GameObject.FindGameObjectsWithTag("PT");

        List<GameObject> SortedPoints = Points.OrderBy(go=>go.name).ToList();

        foreach(GameObject go in SortedPoints){
            position.Add(go.transform.position);
            rotation.Add(go.transform.rotation);
        }

        position.Reverse();
        rotation.Reverse();

        switch (Approximation)
        {
            case EApproximationType.DeCasteljau:
                List<float> T = buildEchantillonnage();
                List<Vector3> FinalLine = new List<Vector3>();

                foreach(float t in T){
                    Approximation_curve.Add(DeCasteljau(position, t));
                    FinalLine.Add(Lerp(position.Last(), position[0], t));
                }
                Approximation_curve.AddRange(FinalLine);
                break;
            case EApproximationType.Spline:
                Approximation_curve = Subdivise(position);
                break;
        }

        for (int i = 0; i < rotation.Count - 1; i++)
        {
            List<float> T = buildEchantillonnage();
            List<Quaternion> temp = new List<Quaternion>();
            foreach(float t in T){
                temp.Add(Quaternion.Slerp(rotation[i], rotation[i+1], t));
            }
            Interpolation_rotation.AddRange(temp);   
        }
        
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.blue;
        for (int i = 0; i < Approximation_curve.Count - 1; ++i){
            Gizmos.DrawLine(Approximation_curve[i], Approximation_curve[i + 1]);
        }
        Gizmos.matrix = transform.localToWorldMatrix;           // For the rotation bug
        Gizmos.DrawFrustum(transform.position, Camera.main.fieldOfView, Camera.main.nearClipPlane, Camera.main.farClipPlane, Camera.main.aspect);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Approximation_curve[i % Approximation_curve.Count];
        transform.rotation = Interpolation_rotation[i % Interpolation_rotation.Count];

        i++;
    }

}
