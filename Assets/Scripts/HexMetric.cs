using UnityEngine;

//Creates a hexagon for a hex cell
public static class HexMetric
{

    public const float outerRadius = 10f;

    public const float innerRadius = outerRadius * 0.866025404f;

    public const int chunkSizeX = 1, chunkSizeZ = 1;

    public const float outerCorner = .8f, innerCorner = .7f;

    //6 triangle corners for initial hexagon creation
    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };


    //12 triangle corners for initial hollow hexagon creation
    public static Vector3[] hollowHexCorners =
    {
        new Vector3(0f, 0f, outerRadius*innerCorner),
        new Vector3(0f, 0f, outerRadius*outerCorner),

        new Vector3(innerRadius*innerCorner, 0f, 0.5f * outerRadius * innerCorner),
        new Vector3(innerRadius * outerCorner, 0f, 0.5f * outerRadius * outerCorner),

        new Vector3(innerRadius*innerCorner, 0f, -0.5f * outerRadius * innerCorner),
        new Vector3(innerRadius*outerCorner, 0f, -0.5f * outerRadius * outerCorner),

        new Vector3(0f, 0f, -outerRadius*innerCorner),
        new Vector3(0f, 0f, -outerRadius*outerCorner),

        new Vector3(-innerRadius*innerCorner, 0f, -0.5f * outerRadius*innerCorner),
        new Vector3(-innerRadius*outerCorner, 0f, -0.5f * outerRadius*outerCorner),

        new Vector3(-innerRadius*innerCorner, 0f, 0.5f * outerRadius*innerCorner),
        new Vector3(-innerRadius*outerCorner, 0f, 0.5f * outerRadius*outerCorner),

        new Vector3(0f, 0f, outerRadius*innerCorner),
        new Vector3(0f, 0f, outerRadius*outerCorner)
    };

}