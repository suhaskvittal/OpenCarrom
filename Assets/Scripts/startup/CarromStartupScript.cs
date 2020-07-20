using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CarromStartupScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject blackPf, whitePf, redPf;

    void Start()
    {
        Transform bXform = Constants.board.GetComponent<Transform>();
        Tilemap bTileMap = bXform.GetComponentInChildren<Tilemap>();

        Vector2Int size = (Vector2Int) bTileMap.size;
        Vector2 pos = bTileMap.GetComponent<Transform>().position;

        float centerX = (2 * pos.x + size.x - 2 * Constants.borderSize) / 2.0f, 
            centerY = (2 * pos.y + size.x - 2 * Constants.borderSize) / 2.0f;
        Debug.Log("x, y: " + centerX + "," + centerY);
        // place red in the center
        Instantiate(redPf, new Vector3(centerX, centerY, 0), Quaternion.identity);
        // then place white and black in alternating fashion (in a hexagon)
        // 1st level is just a basic alternating hexagon
        // 2nd level is also alternating, but between each vertex, there is one man (so all vertices are the same color)
        float angle = 0.0f;
        bool type = false;  // 0 = black, 1 = white.
        for (int i = 0; i < 6; i++) {
            float flx = centerX + Mathf.Cos(angle) * Constants.menDiameter;  // first level x and y
            float fly = centerY + Mathf.Sin(angle) * Constants.menDiameter;
            GameObject m;
            if (type) { m = whitePf; }
            else { m = blackPf; }
            Instantiate(m, new Vector3(flx, fly, 0), Quaternion.identity);

            // now we have to instantiate two men for the outer circle
            // by convention, first one is white, second is black.
            float slx1 = flx + Mathf.Cos(angle) * Constants.menDiameter; 
            float sly1 = fly + Mathf.Sin(angle) * Constants.menDiameter;
            float slx2 = flx + Mathf.Cos(angle + Mathf.PI / 6.0f) * Constants.menDiameter;
            float sly2 = fly + Mathf.Sin(angle + Mathf.PI / 6.0f) * Constants.menDiameter;

            Instantiate(whitePf, new Vector3(slx1, sly1, 0), Quaternion.identity);
            Instantiate(blackPf, new Vector3(slx2, sly2, 0), Quaternion.identity);

            type = !type; angle += 2.0f * Mathf.PI / 6.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
