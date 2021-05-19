using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Trajectoire : MonoBehaviour
{
    List<Vector3> position;
    List<Quaternion> rotation;
    // Start is called before the first frame update
    void Start()
    {
        position = new List<Vector3>();
        rotation = new List<Quaternion>();

        GameObject[] Points = GameObject.FindGameObjectsWithTag("PT");

        foreach(GameObject go in Points){
            position.Add(go.transform.position);
            rotation.Add(go.transform.rotation);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // interpolation sphérique
    Quaternion Slerp(Quaternion q1, Quaternion q2, float t){

        Quaternion q_res;

        // calculer l'angle entre q1 et q2
        float cos_theta_sur_deux = q1.x * q2.x + q1.w * q2.w + q1.y * q2.y + q1.z * q2.z;

        if (cos_theta_sur_deux >= 1.0f){
            // q1 = q2
            q_res = new Quaternion(q1.x, q1.y, q1.z, q1.w);
            return q_res;

        } else {
            float theta_sur_deux     = (float) Math.Acos(cos_theta_sur_deux);
            float sin_theta_sur_deux = (float) Math.Sqrt(1 - Math.Pow(cos_theta_sur_deux, 2));

            float a = (float) (Math.Sin((1 - t) * theta_sur_deux) / sin_theta_sur_deux);
            float b = (float) (Math.Sin(t * theta_sur_deux) / sin_theta_sur_deux);

            q_res = new Quaternion(a*q1.x + b*q2.x, a*q1.y + b*q2.y, a*q1.z + b*q2.z, a*q1.w + b*q2.w);
            return q_res;
        }

    }

    // interpolation neville
    Vector3 Neville(List<float> X, List<float> Y, List<float> Z, float t){
        
    }

    // interpolation lagrange
    Vector3 Lagrange(List<float> X, List<float> Y, List<float> Z, float t){

    }


}
