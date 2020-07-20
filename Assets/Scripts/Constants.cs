using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Constants {
    public static float borderSize = 3.0f;
    public static float menDiameter = 65.0f / 40.0f;
    public static float baseCircleDiameter = 64.0f / 40.0f;
    public static GameObject board = GameObject.FindWithTag("Board");

    public static float unitForce = 1000.0f;
}