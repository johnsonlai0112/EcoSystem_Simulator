using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flock : MonoBehaviour
{
    float speed = 0.001f;
    float rotationSpeed = 5.0f;
    Vector3 averagePosition;
    float neighbourDistance = 5.0f; // radius of a group
    float socialDistance = 0.5f; //disance of each bird

    static string flockCreature = "Seagull";
    GameObject[] gos; //array for store bird
        
    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(0.1f, 1.0f); //random speed
        gos = GameObject.FindGameObjectsWithTag(flockCreature); //assign all bird into array
    }

    // Update is called once per frame
    void Update()
    {
        // make the bird delay in action to make more realistic
        if (Random. Range(0, 5) < 1){
            ApplyRules();
        }

        transform.Translate(0, 0, Time.deltaTime* speed); //move
    }

    void ApplyRules()
    {
        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0;
        Vector3 wind = new Vector3(1,0,1);
        Vector3 goalPos = GlobalFlock.goalPos;
        float dist = 0;
        int groupSize = 0;
        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist <= neighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;
                    if(dist < socialDistance)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }
                    gSpeed = gSpeed + go.GetComponent<flock>().speed;
                }
            }
        }
        if (groupSize != 0)
        {
                vcentre = vcentre / groupSize + wind + (goalPos - this.transform.position);;
                speed = gSpeed / groupSize;
                Vector3 direction = (vcentre + vavoid) - transform.position;
                //var direction = vcentre - transform.position;
                if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion. LookRotation(direction), rotationSpeed * Time.deltaTime);

        }
    }
}
