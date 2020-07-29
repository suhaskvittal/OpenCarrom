using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Global {
    /**/
    public static float borderSize = 3.0f;
    public static float carromManDiameter = 65.0f / 40.0f;
    public static float carromStrikerDiameter = 82.0f / 40.0f;
    public static float baseCircleDiameter = 64.0f / 40.0f;
    public static float unitForce = 1.0f;
    public static float effZeroVelocity = 0.1f;

    public static float wallEnergyAbsorption = 0.04f;

    public static Vector3 outOfBoardPosition = new Vector3(-1000, -1000, 0);
    /*
        GAME VARIABLES
    */
    public static int currentPlayer = 0;
    public static GameObject board = GameObject.FindWithTag("Board");
    public static GameObject manCommonAncestor = GameObject.FindWithTag("ManAncestor");
    public static GameObject strkCommonAncestor = GameObject.FindWithTag("StrikerAncestor");
    public static Stack<GameObject> pocketStack = new Stack<GameObject>();
    public static int maxPlayers = strkCommonAncestor.transform.childCount;
}