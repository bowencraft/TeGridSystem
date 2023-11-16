using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDataManager : MonoBehaviour
{

    public enum GridState
    {
        Placeable, // 可放置
        Occupied,  // 已放置
        Locked,     // 未解锁
        Merged
    }


    public GridState[,] gridStates;
    public GridPlaceableObject[,] gridObjects;

    public GridDisplayManager gridDisplayManager;
    public GridManager gridManager;

    void Start()
    {
        gridDisplayManager = GetComponent<GridDisplayManager>();
        gridManager = GetComponent<GridManager>();
        // 初始化 gridStates，例如：
        gridStates = new GridState[gridManager.gridSize.x, gridManager.gridSize.y]; // 假设网格大小为 10x10
        gridObjects = new GridPlaceableObject[gridManager.gridSize.x, gridManager.gridSize.y];

        for (int x = 0; x < gridStates.GetLength(0); x++)
        {
            for (int y = 0; y < gridStates.GetLength(1); y++)
            {
                gridStates[x, y] = GridState.Locked; // 或其他默认状态
            }
        }


        UpdateWithoutCoverGridState(2);
    }

    public void UpdateWithoutCoverGridState(int borderRadius)
    {
        for (int x = borderRadius -1; x < gridStates.GetLength(0) - borderRadius; x++)
        {
            for (int y = borderRadius -1; y < gridStates.GetLength(1) - borderRadius; y++)
            {
                if (gridStates[x, y] == GridState.Locked)
                    UpdateGridState(x, y, GridState.Placeable);
            }
        }
    }

    // 更新网格状态的方法
    public void UpdateGridState(int x, int y, GridState newState)
    {
        gridStates[x, y] = newState;
        gridDisplayManager.DisplayGridState(x, y, newState);
    }

    public GridState GetGridState(Vector2Int position)
    {
        // 返回指定位置的物品数据
        int x = position.x;
        int y = position.y;
        return gridStates[x, y];
    }


    public bool IsInMatrix(int row, int col, int matrixSize)
    {
        int rows = gridStates.GetLength(0);
        int cols = gridStates.GetLength(1);

        string matrixBorder = "";

        for (int i = -matrixSize / 2; i <= matrixSize / 2; i++)
        {
            for (int j = -matrixSize / 2; j <= matrixSize / 2; j++)
            {
                int newRow = row + i;
                int newCol = col + j;

                matrixBorder += newRow + "," + newCol;

                if (newRow < 0 || newRow >= rows || newCol < 0 || newCol >= cols || gridStates[newRow, newCol] != GridState.Occupied)
                {
                    Debug.Log(matrixBorder);
                    return false;
                }
            }
        }
        //Debug.Log(matrixBorder);

        return true;
    }

    public GridPlaceableObject[,] Check3x3Matrix(int row, int col)
    {
        if (IsInMatrix(row, col, 3))
        {
            GridPlaceableObject[,] matrixObjects = new GridPlaceableObject[3, 3];
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    matrixObjects[i + 1, j + 1] = gridObjects[row + i, col + j];
                }
            }
            return matrixObjects;
        }
        return null;
    }
}