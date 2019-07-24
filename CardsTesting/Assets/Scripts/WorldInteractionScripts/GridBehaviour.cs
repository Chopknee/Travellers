using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    int masterGridSize = 8;
    public Cell[,] masterGrid = new Cell[8,8];

    /*
     * current format:
     * { cellSize,
     *   cellPos
     * }
     */

    Vector3 origin = Vector3.zero;
    Vector3 currentCellPos = Vector3.zero;
    float cellSize = 5;

    
    void Awake()
    {
        for (int i = 0; i < masterGridSize; i++) // left-to-right
        {
            currentCellPos.x += cellSize;
            for (int j = 0; j < masterGridSize; j++) // top-to-bottom
            {
                currentCellPos.z += cellSize;

                
                Cell t = new Cell(cellSize, currentCellPos.x, currentCellPos.y, currentCellPos.z);
                
                
                t.x = currentCellPos.x;
                t.z = currentCellPos.z;
                masterGrid[i, j] = t;

                
            }
            currentCellPos.x += cellSize;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        for (int i = 0; i < masterGrid.Length; i++) // left-to-right
        {
            for (int j = 0; j < masterGrid.Length; j++) // top-to-bottom
            {
                //Gizmos.DrawCube(new Vector3(masterGrid[i, j].x, masterGrid[i, j].y, masterGrid[i, j].z), Vector3.one * masterGrid[i,j].size); 
            }
        }
    }
}
public class Cell
{
    public float x = 0, y = 0, z = 0;
    public float size = 5;

    public Cell(float size, float x, float y, float z)
    {
        x = this.x;
        y = this.y;
        z = this.z;
        size = this.size;
    }
}
