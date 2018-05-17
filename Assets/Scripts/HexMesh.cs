using UnityEngine;
using System.Collections.Generic;
using System;


//Creates the mesh on screen to display each hexagon cell
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{

    public bool useCollider, useColors, useUVCoordinates, useUV2Coordinates;
    public bool useTerrainTypes;

    Mesh hexMesh;
    //[NonSerialized] List<Vector3> vertices, terrainTypes;
    static List<Vector3> vertices = new List<Vector3>();
    static List<Color> colors = new List<Color>();
    static List<int> triangles = new List<int>();

    public int quality = 1;

    public int debugCount = 0;

    MeshCollider meshCollider;

    //List<Color> colors;

    //Ran at the start of the scene
    void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name = "Hex Mesh";
    }
    
    //Gets an array of cells to display on screen
    public void Triangulate(HexCell[] cells)
    {
        /*hexMesh.Clear();
        vertices.Clear();
        colors.Clear();
        triangles.Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        hexMesh.vertices = vertices.ToArray();
        //hexMesh.colors = colors.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.RecalculateNormals();

        meshCollider.sharedMesh = hexMesh;*/

        
    }
    
    void Triangulate(HexCell cell)
    {
        if (cell != null)
        {
            Vector3 center = cell.transform.localPosition;
            for (int i = 0; i < 6; i++)
            {
                AddTriangle(
                    center,
                    center + HexMetric.corners[i],
                    center + HexMetric.corners[i + 1],
                    1,
                    cell
                );
                /*if(i%2==0) {
                    AddTriangle(
                        center + HexMetric.hollowHexCorners[i],
                        center + HexMetric.hollowHexCorners[i + 1],
                        center + HexMetric.hollowHexCorners[i + 2],
                        1,
                        cell
                    );
                }
                else
                {
                    AddTriangle(
                        center + HexMetric.hollowHexCorners[i],
                        center + HexMetric.hollowHexCorners[i + 2],
                        center + HexMetric.hollowHexCorners[i + 1],
                        1,
                        cell
                    );
                }*/

                AddTriangleColor(cell.color);
            }
        }
    }

    //Adds color to hex
    void AddTriangleColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    //Creates a triangle using the 3 vectors as the position of the corners
    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, int q, HexCell cell)
    {
        //if quality needs to be improved then create 4 more triangles instead of this one
        
        if (q < quality )
        {
            //center triangle
            AddTriangle(
                v1,
                (v1 + v2) / 2,
                (v1 + v3) / 2,
                q + 1,
                cell
                );
            AddTriangle(
                (v1 + v3) / 2,
                (v1 + v2) / 2,
                (v3 + v2) / 2,
                q + 1,
                cell
                );
            AddTriangle(
                (v1 + v2) / 2,
                v2,
                (v2 + v3) / 2,
                q + 1,
                cell
                );
            AddTriangle(
                v3,
                (v3 + v1) / 2,
                (v3 + v2) / 2,
                q + 1,
                cell
                );
        }
        else
        {
            AddTriangleColor(cell.color);
            debugCount++;
            //v1.y = Terrain.activeTerrain.SampleHeight(v1)+.2f;
            //v2.y = Terrain.activeTerrain.SampleHeight(v2)+.2f;
            //v3.y = Terrain.activeTerrain.SampleHeight(v3)+.2f;
            int vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }
    }

}
