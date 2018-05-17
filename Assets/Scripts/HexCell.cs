using UnityEngine;
using UnityEditor;
using System;

//Hex Cell class used to add any properties to the cells in the grid
public class HexCell : MonoBehaviour
{

    public HexCoordinates coordinates;

    public Color color;

    public HexGridChunk chunk;

    public Texture grid,target,friendly,enemy;

    Boolean impassible = false;
    Boolean gridCell = true;
    Boolean red, cyan, white= false;

    
    [SerializeField]
    HexCell[] neighbors;

    public Boolean GetRed()
    {
        return red;
    }
    public Boolean GetCyan()
    {
        return cyan;
    }
    public Boolean GetWhite()
    {
        return white;
    }
    public Boolean GetGridCell()
    {
        return gridCell;
    }

    public void Highlight(Boolean b)
    {
        white = b;
        transform.Find("White Highlight").gameObject.SetActive(white);
        PriorityHighlight();
    }

    public void RedHighlight(Boolean b)
    {
        red = b;
        transform.Find("Red Highlight").gameObject.SetActive(red);
        PriorityHighlight();
    }

    public void CyanHighlight(Boolean b)
    {
        cyan = b;
        transform.Find("Cyan Highlight").gameObject.SetActive(b);
        PriorityHighlight();
    }

    public void CellGrid(Boolean b)
    {
        transform.Find("GridCell Projector").gameObject.SetActive(b);
        impassible = !b;
    }

    public void Hex(Boolean b)
    {
        transform.Find("Hex").gameObject.SetActive(b);
    }

    public Boolean IsImpassible()
    {
        return impassible;
    }

    private void PriorityHighlight()
    {
        if(impassible)
        {
            transform.Find("White Highlight").gameObject.SetActive(false);
            transform.Find("Cyan Highlight").gameObject.SetActive(false);
            transform.Find("Red Highlight").gameObject.SetActive(false);
        }
        else if(red)
        {
            transform.Find("White Highlight").gameObject.SetActive(false);
            transform.Find("Cyan Highlight").gameObject.SetActive(false);
        }
        else if(cyan)
        {
            transform.Find("White Highlight").gameObject.SetActive(false);
            transform.Find("Cyan Highlight").gameObject.SetActive(true);
        }
        else if(white)
        {
            transform.Find("White Highlight").gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        //transform.Find("Hex Projector").gameObject.transform.Rotate(new Vector3(0,0,-1));
    }

    void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    

}