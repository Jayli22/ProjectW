using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsHub
{ 
    public static float GetAngleBetweenVectors(Vector2 vector1, Vector2 vector2)
{
    Vector2 difference = vector2 - vector1;
    float rawTargetAngle = Vector2.Angle(Vector2.up, vector2);
    float angle = rawTargetAngle;

    difference *= -1;


    if (difference.x < 0 && difference.y >= 0)
    {
        angle = 360 - rawTargetAngle;
    }
    else if (difference.x < 0 && difference.y <= 0)
    {
        angle = 360 - rawTargetAngle;
    }

    //Debug.Log("Angle: " + angle + " ---- Difference: " + difference + " ---- Raw: " + rawTargetAngle);
    return angle;
}
}
