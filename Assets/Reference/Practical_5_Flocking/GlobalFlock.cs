using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
    public GameObject gull;

    static public Vector3 goalPos;

    // Start is called before the first frame update
    void Start()
    {
        //create seagulls
        for(var i = 0; i < 100; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            Instantiate(gull, pos, Quaternion.identity);
        }

        goalPos = new Vector3(0,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Random.Range(0,10000) < 50)
        {
            goalPos = new Vector3(Random.Range(-10.0f,10.0f), 0, Random.Range(-10.0f,10.0f));
        }
    }
}
