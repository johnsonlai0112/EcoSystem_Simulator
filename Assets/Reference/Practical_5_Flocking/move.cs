﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("up"))
        {
            this.transform.Translate(0,0.1f,0);
        }
        else if (Input.GetKey("down"))
        {
            this.transform.Translate(0, -0.1f,0);
        }
        else if (Input.GetKey("left"))
        {
            this.transform.Translate(-0.1f, 0, 0);
        }
        else if (Input.GetKey("right"))
        {
            this.transform.Translate(0.1f, 0, 0);
        }
    }
}
