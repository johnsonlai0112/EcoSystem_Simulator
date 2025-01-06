using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    public Texture orbspot;
    public Transform playerPos;
    float mapScale = 5;

    float radarSpotX;
    float radarSpotY;

    float radarWidth = 200;
    float radarHeight = 200;

    void OnGUI()
    {
        GUI.BeginGroup(new Rect(10, Screen.height - radarHeight - 10, radarWidth, radarHeight));
        GUI.Box(new Rect(0, 0, radarWidth, radarHeight), "Radar");
        DrawSpotsForOrbs();
        GUI.EndGroup();
    }

    void DrawRadarBlip(GameObject go, Texture spotTexture)
    {
        var gameObjPos = go.transform.position;

        //find distance between object and player
        var dist = Vector3.Distance(playerPos.position, gameObjPos);

        //find the horizontal distances along the x and z between player and object
        var dx = playerPos.position.x - gameObjPos.x;
        var dz = playerPos.position.z - gameObjPos.z;

        //determine the angle of rotation between the direction the player is facing and the location
        //of the object
        float deltay = Mathf.Atan2(dx, dz) * Mathf.Rad2Deg - 270 - playerPos.eulerAngles.y;

        //orient the object on the radar according to the direction the player is facing
        float radarSpotX = dist * Mathf.Cos(deltay * Mathf.Deg2Rad) * mapScale;
        float radarSpotY = dist * Mathf.Sin(deltay * Mathf.Deg2Rad) * mapScale;

        //draw a spot on the radar
        GUI.DrawTexture(new Rect(radarWidth / 2.0f + radarSpotX, radarHeight / 2.0f + radarSpotY, 2, 2), spotTexture);
    }

    void DrawSpotsForOrbs()
    {
        GameObject[] gos;
        //look for all objects with a tag of orb
        gos = GameObject.FindGameObjectsWithTag("Seagull");

        var distance = Mathf.Infinity;
        var position = transform.position;

        foreach (GameObject go in gos)
        {
            DrawRadarBlip(go, orbspot);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
