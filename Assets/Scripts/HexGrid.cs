using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class HexGrid : MonoBehaviour
{
    //Hex grid default size
    int chunkCountX = 1, chunkCountZ = 1;

    int cellCountX, cellCountZ;

    public Color defaultColor = Color.white;
    public Color touchedColor = Color.magenta;

    public HexCell cellPrefab;

    public Text cellLabelPrefab;

    public HexGridChunk chunkPrefab;

    HexCell[] cells;

    List<HexCell> selectedCells = new List<HexCell>();

    int totalCells;

    HexGridChunk[] chunks;

    private HexCell previousCell;
    private HexCell currentCell;

    //test variables
    float pulse = 1;
    int direction = 1;

    //Creates a hex grid using the cellCountX and cellCountZ on startup
    void Awake()
    {

        Debug.Log(Terrain.activeTerrain.terrainData.size);

        int terrainWidth = (int)Terrain.activeTerrain.terrainData.size.x;
        int terrainLength = (int)Terrain.activeTerrain.terrainData.size.z; 

        chunkCountZ += (int)((terrainLength - HexMetric.outerRadius * 2)/(HexMetric.outerRadius*1.5));
        chunkCountX += (int)(((terrainWidth/HexMetric.innerRadius)-3)/2);

        

        cellCountX = chunkCountX * HexMetric.chunkSizeX;
        totalCells = cellCountX;
        cellCountZ = chunkCountZ * HexMetric.chunkSizeZ;
        totalCells *= cellCountZ;

        CreateChunks();
        CreateCells();
    }

    private void Update()
    {
        pulse -= .01f * direction;
        if (pulse <= .7f && direction == 1)
        {
            direction = -direction;
        }
        else if (pulse >= 1 && direction == -1)
            direction = -direction;

        cellPrefab.transform.Find("Red Highlight").gameObject.GetComponent<Projector>().material.SetColor("_Color", new Vector4(pulse, 0, 0, 1));
        cellPrefab.transform.Find("Cyan Highlight").gameObject.GetComponent<Projector>().material.SetColor("_Color", new Vector4(0, pulse, pulse, 1));
    }

    public int GetChunkCountX()
    {
        return chunkCountX;
    }

    public int GetChunkCountZ()
    {
        return chunkCountZ;
    }

    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    //Creates a cell for the grid
    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetric.innerRadius * 2f);
        position.y = 0;
        position.z = z * (HexMetric.outerRadius * 1.5f);
        
        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        //cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;
        cell.transform.Find("Hex").transform.SetPositionAndRotation(new Vector3(position.x, Terrain.activeTerrain.SampleHeight(position) + 5,position.z),Quaternion.identity);

        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        

        //Text label = Instantiate<Text>(cellLabelPrefab);
        //label.rectTransform.SetParent(gridCanvas.transform, false);
        //label.rectTransform.anchoredPosition =
           // new Vector2(position.x, position.z);
        //label.text = cell.coordinates.ToStringOnSeparateLines();

        AddCellToChunk(x, z, cell);
    }

    //Gets the cell that was clicked by the user
    public void OnClickColorCell(Vector3 position,HexColor color, Boolean b, int radius)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        if (index>=0 && index<totalCells && CheckOuterEdges(index, coordinates)) { 
            HexCell cell = cells[index];
            ColorCells(cell, color, b, radius);
            Debug.Log("touched at " + coordinates.ToString());
        }
    }

    private void ColorCells(HexCell cell, HexColor color, Boolean b, int radius)
    {
        selectedCells.Clear();

        selectedCells.Add(cell);
        List<HexCell> temp = new List<HexCell>();
        for (int i = 1; i < radius; i++)
        {
            foreach (HexCell c in selectedCells)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (c.GetNeighbor(Numerical(j)) != null)
                    {
                        if (!c.GetNeighbor(Numerical(j)).IsImpassible() || color==HexColor.NONE)
                            
                            temp.Add(c.GetNeighbor(Numerical(j)));
                    }
                }
            }
            foreach (HexCell c in temp)
            {
                if (!selectedCells.Contains(c))
                {
                    selectedCells.Add(c);
                }
            }
            temp.Clear();

        }

        foreach (HexCell c in selectedCells)
        {
            if (color == HexColor.RED)
                c.RedHighlight(true);
            else if (color == HexColor.CYAN)
                c.CyanHighlight(true);
            else if (color == HexColor.WHITE)
                c.Highlight(true);
            else
                c.CellGrid(b);
        }
    }

    public void HighlightMouse(Vector3 position, int radius, HexColor color)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        if (index >= 0 && index < totalCells && CheckOuterEdges(index, coordinates))
        {
            HexCell cell = cells[index];
            currentCell = cell;
            // if color is red (for targeting) and cell is white
            if (previousCell != null && currentCell != previousCell)
            {
                if ((color == HexColor.RED && cell.GetWhite() == true) || color != HexColor.RED)
                    HighLightCellsCircle(cell, radius, color);
                else if (color == HexColor.RED)
                {
                    clearHighlightedList(color);
                }
            }

            previousCell = currentCell;
        }
    }


    // ~.002 seconds for a radius 1    good
    // ~.002 seconds for a radius 2    good
    // ~.003 seconds for a radius 3    good
    // ~.004 seconds for a radius 4    good
    // ~.006 seconds for a radius 5    ok    55 fps
    // ~.010 seconds for a radius 6    ok    47 fps
    // ~.014 seconds for a radius 7    bad   40 fps
    // ~.021 seconds for a radius 8    not good
    // ~.034 seconds for a radius 9    not good
    // ~.054 seconds for a radius 10   not good
    // ~.083 seconds for a radius 11   not good
    // Multiplay by 60 because it runs 60 times a second
    // try to run this only when the mouse changes to a different cell

    private void HighLightCellsCircle(HexCell cell, int radius, HexColor color)
    {
       // long milli = System.DateTime.Now.Ticks/ TimeSpan.TicksPerMillisecond;
        foreach(HexCell c in selectedCells)
        {
            if (color == HexColor.RED)
                c.RedHighlight(false);
            else if (color == HexColor.CYAN)
                c.CyanHighlight(false);
            else if (color == HexColor.WHITE)
                c.Highlight(false);
            else
                c.Hex(false);
        }

        selectedCells.Clear();

        selectedCells.Add(cell);
        List<HexCell> temp = new List<HexCell>();
        for(int i=1;i<radius;i++)
        {
            foreach(HexCell c in selectedCells)
            {
                for(int j=0;j<6;j++)
                {
                    if (c.GetNeighbor(Numerical(j)) != null)
                    {
                        if(!c.GetNeighbor(Numerical(j)).IsImpassible())
                            temp.Add(c.GetNeighbor(Numerical(j)));
                    }
                }
            }
            foreach(HexCell c in temp)
            {
                if(!selectedCells.Contains(c))
                {
                    selectedCells.Add(c);
                }
            }
            temp.Clear();

        }

        foreach(HexCell c in selectedCells)
        {
            if (color == HexColor.RED)
                c.RedHighlight(true);
            else if (color == HexColor.CYAN)
                c.CyanHighlight(true);
            else if (color == HexColor.WHITE)
                c.Highlight(true);
            else
                c.Hex(true);
        }

        //Debug.Log("Milliseconds: " + (System.DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - milli));

    }

    public void clearHighlightedList(HexColor color)
    {
        foreach (HexCell c in selectedCells)
        {
            if (color == HexColor.RED)
                c.RedHighlight(false);
            else if (color == HexColor.CYAN)
                c.CyanHighlight(false);
            else if (color == HexColor.WHITE)
                c.Highlight(false);
            else
                c.Hex(false);
        }
    }

    private HexDirection Numerical(int i)
    {
        switch (i)
        {
            case 0: return HexDirection.NE;
            case 1: return HexDirection.E;
            case 2: return HexDirection.SE;
            case 3: return HexDirection.SW;
            case 4: return HexDirection.W;
            default: return HexDirection.NW;
        }
    }

    //Probably a better way to check edge cases or ignore completly on mouse clicks
    private Boolean CheckOuterEdges(int index , HexCoordinates coordinates)
    {
         
        Boolean flag=true;
        int x = coordinates.X;
        int z = coordinates.Z;

        //Z is rows going up. starts at 0 and goes to cellCountZ - 1 ???
        //X goes up and Y goes down respectively, X starts at 0 and goes up 1 every other row
        //X goes to -7 or -(CellCountX-1)/2 

        //To find a point in the grid thats not out of bounds
        //First take the Z to find the X starting point. so z of 1 would have an x starting point of (z-1)/2 which is 0 and ends at (z-1)/2+15
        //Does y matter?

        // z=0 x   0 15
        // z=1 x   0 15
        // z=2 x  -1 14
        // z=3 x  -1 14
        // z=4 x  -2 13
        // z=5 x  -2 13
        // z=6 x  -3 12
        // z=7 x  -3 12
        // z=8 x  -4 11
        // z=9 x  -4 11
        // z=10 x -5 10
        // z=11 x -5 10
        // z=12 x -6 9
        // z=13 x -6 9
        // z=14 x -7 8
        // z=15 x -7 8

        if(x<(-(z-z%2)/2) || x>(-(z-z%2)/2)+(cellCountZ-1))
        {
            flag = false;
        }
        Debug.Log(cellCountZ);
        Debug.Log(flag);

        return flag;
    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetric.chunkSizeX;
        int chunkZ = z / HexMetric.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetric.chunkSizeX;
        int localZ = z - chunkZ * HexMetric.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetric.chunkSizeX, cell);
    }


}