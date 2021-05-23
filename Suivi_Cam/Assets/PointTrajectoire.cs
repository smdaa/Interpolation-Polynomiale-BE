using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTrajectoire : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(transform.position, transform.forward, Color.red, 0f, true);
    }

    void OnDrawGizmosSelected(){
        Gizmos.matrix = this.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(3.0f, 3.0f, 0.2f));
        Debug.DrawRay(transform.position, transform.forward, Color.green, 0f, true);
    }
}
