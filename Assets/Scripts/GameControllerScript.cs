using System.Collections;
using System.Collections.Generic;

using System;
using System.Net;
using System.Net.Sockets;

using UnityEngine;
using UnityEngine.Tilemaps;

public class GameControllerScript : MonoBehaviour
{
    public GameObject blackPf, whitePf, redPf;
    /*
        state 0: ready to be launched.
        state 1: launched.
        state 2: has stopped on the board.
        state 3: has stopped in a pocket.
    */

    void Start()
    {
        SetupBoard();
    }

    // Update is called once per frame
    void Update()
    {
        // check pocket stack
        while (Global.pocketStack.Count > 0) {
            GameObject g = Global.pocketStack.Pop(); 
            if (g.tag == "Striker") {
                // TODO: fine player a man
                StartCoroutine(ShrinkSprite(g, false));
            } else {
                // TODO: Give player a man
                StartCoroutine(ShrinkSprite(g, true));
            }
        }
    }

    private IEnumerator ShrinkSprite(GameObject g, bool delete) {
        while (g != null && g.transform.localScale != Vector3.zero) {
            g.transform.localScale -= new Vector3(1, 1, 1) * 0.01f;
            yield return new WaitForSeconds(.1f);;
        }
        if (delete && g != null) { Destroy(g); }
    }

    private void SetupBoard() {
        Transform bXform = Global.board.GetComponent<Transform>();
        Tilemap bTileMap = bXform.GetComponentInChildren<Tilemap>();

        Vector2Int size = (Vector2Int) bTileMap.size;
        Vector2 pos = bTileMap.GetComponent<Transform>().position;

        float centerX = (2 * pos.x + size.x - 2 * Global.borderSize) / 2.0f, 
            centerY = (2 * pos.y + size.x - 2 * Global.borderSize) / 2.0f;
        Debug.Log("x, y: " + centerX + "," + centerY);
        // place red in the center
        
        GameObject redMan = Instantiate(redPf, new Vector3(centerX, centerY, 0), Quaternion.identity);
        redMan.transform.parent = Global.manCommonAncestor.transform;
        // then place white and black in alternating fashion (in a hexagon)
        // 1st level is just a basic alternating hexagon
        // 2nd level is also alternating, but between each vertex, there is one man (so all vertices are the same color)
        float angle = 0.0f;
        bool type = false;  // 0 = black, 1 = white.
        for (int i = 0; i < 6; i++) {
            float flx = centerX + Mathf.Cos(angle) * Global.carromManDiameter;  // first level x and y
            float fly = centerY + Mathf.Sin(angle) * Global.carromManDiameter;
            GameObject m;
            if (type) { m = whitePf; }
            else { m = blackPf; }
            GameObject gInnerMan = Instantiate(m, new Vector3(flx, fly, 0), Quaternion.identity);
            gInnerMan.transform.parent = Global.manCommonAncestor.transform;
            // now we have to instantiate two men for the outer circle
            // by convention, first one is white, second is black.
            float slx1 = flx + Mathf.Cos(angle) * Global.carromManDiameter; 
            float sly1 = fly + Mathf.Sin(angle) * Global.carromManDiameter;
            float slx2 = flx + Mathf.Cos(angle + Mathf.PI / 6.0f) * Global.carromManDiameter;
            float sly2 = fly + Mathf.Sin(angle + Mathf.PI / 6.0f) * Global.carromManDiameter;

            GameObject gOuterManW = Instantiate(whitePf, new Vector3(slx1, sly1, 0), Quaternion.identity);
            GameObject gOuterManB = Instantiate(blackPf, new Vector3(slx2, sly2, 0), Quaternion.identity);
            gOuterManW.transform.parent = Global.manCommonAncestor.transform; gOuterManB.transform.parent = Global.manCommonAncestor.transform;

            type = !type; angle += 2.0f * Mathf.PI / 6.0f;
        }
    }
}
