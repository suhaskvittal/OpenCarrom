using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SimulatedPhysicsScript : MonoBehaviour
{
    private float minX, maxX, minY, maxY;
    private const float boardSFric = 0.4f, boardDFric = 0.15f;

    // Start is called before the first frame update
    void Start()
    {
        // initialize minX, maxX, minY, maxY
        Transform bXform = Global.board.GetComponent<Transform>();
        Tilemap bTileMap = bXform.GetComponentInChildren<Tilemap>();

        Vector2Int size = (Vector2Int) bTileMap.size;
        Vector2 pos = bTileMap.GetComponent<Transform>().position;

        minX = pos.x + Global.borderSize; maxX = pos.x + (size.x - 3 * Global.borderSize); 
        minY = pos.y + Global.borderSize; maxY = pos.y + (size.x - 3 * Global.borderSize);
    }

    // Update is called once per frame
    void Update() {

    }    
    
    void FixedUpdate()
    {
        /*
            Check if object's position is at one of the borders; if so, rebound it.
        */
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 p = rb.position;
        Vector2 v = new Vector2(rb.velocity.x, rb.velocity.y);
        float x = p.x; float y = p.y;
        if (x <= minX || x >= maxX) {  // then rebound the object in the x direction
            v.x = -v.x;
            v *= (1 - Global.wallEnergyAbsorption);
        }
        if (y <= minY || y >= maxY) {
            v.y = -v.y;
            v *= (1 - Global.wallEnergyAbsorption);
        }
        rb.velocity = v;
    }
}
