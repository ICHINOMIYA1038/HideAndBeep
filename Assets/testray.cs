using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testray : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        float ViewAngle = 60f;
        float deltaAngle = -ViewAngle;
        RaycastHit hit;
        //Ray ray = new(agent.transform.position + new Vector3(0f, 15f, 0f), Quaternion.AngleAxis(deltaAngle, transform.up) * agent.transform.forward);
        //Ray ray2 = new Ray(agent.transform.position + new Vector3(0f, 15f, 0f), agent.transform.up);
        Ray ray3 = new Ray(this.transform.position, this.transform.forward);


        
            if (Physics.Raycast(ray3, out hit, 50f))
            {
                

            }

        //Debug.DrawLine(ray.origin, ray.direction * 30f, Color.blue, 1f);
        //Debug.DrawLine(ray2.origin, ray2.direction * 30f, Color.green, 1f);
        Debug.DrawLine(ray3.origin, ray3.origin + ray3.direction * 5f, Color.red, 1f); ;
            Debug.Log(ray3.direction);
        //transform.Translate(0f, 0f, 1f);

        
    }
}
