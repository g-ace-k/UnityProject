using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class HexMapEditor : MonoBehaviour
{

    public Color[] colors;

    public HexGrid hexGrid;

    private HexColor activeColor;

    private int radius;

    private Boolean brush;

    private long milliBefore=0;
    private long milliAfter=0;

    Boolean selected = true;

    void Awake()
    {
        SelectColor(0);
        SelectRadius(1);
        brushMode(true);
        
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 200, 100, 300), "Milliseconds: " + (milliAfter - milliBefore));
    }

    void Update()
    {
        if (brush)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                selected = !selected;
            }

            if (Input.GetMouseButton(0) &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                HandleInput();
            }

            if (activeColor != HexColor.NONE)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                    HandleMousePointerInput();
            }
        }

    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            Debug.Log("Hit Point: " + hit.point);
            hexGrid.OnClickColorCell(hit.point,activeColor, selected,radius);
        }
    }

    void HandleMousePointerInput()
    {
        milliBefore = System.DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            hexGrid.HighlightMouse(hit.point, radius,activeColor);
        }
        else
        {
            hexGrid.clearHighlightedList(activeColor);
        }

        milliAfter = System.DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        
    }

    public void SelectColor(int index)
    {
        hexGrid.clearHighlightedList(activeColor);

        if (index == 0)
            activeColor = HexColor.RED;
        else if (index == 1)
            activeColor = HexColor.CYAN;
        else if (index == 2)
            activeColor = HexColor.WHITE;
        else if (index == 3)
            activeColor = HexColor.HEX;
    }

    public void SelectRadius(float r)
    {
        radius = (int)r;
        
    }

    public void brushMode(Boolean b)
    {
        brush = b;
    }
}