using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using System;
using Sirenix.Serialization;

public class GridDataManager : SerializedMonoBehaviour
{
    public enum GridState
    {
        Placeable, // 可放置
        Occupied,  // 已放置
        Locked,     // 未解锁
        Merged
    }

    [TableMatrix(HorizontalTitle = "Grid States")]
    public GridState[,] gridStates;

    [TableMatrix(HorizontalTitle = "Grid Objects", SquareCells = true)]
    public GridPlaceableObject[,] gridObjects;

    public GridManager gridManager;
    public GridDisplayManager gridDisplayManager;

    public int unlockRadius; 

    void Start()
    {
        gridDisplayManager = gridManager.gridDisplayManager;
        // 初始化 gridStates，例如：
        unlockRadius = 2;


        gridStates = new GridState[gridManager.gridSize.x, gridManager.gridSize.y]; // 假设网格大小为 10x10
        gridObjects = new GridPlaceableObject[gridManager.gridSize.x, gridManager.gridSize.y];

        for (int x = 0; x < gridStates.GetLength(0); x++)
        {
            for (int y = 0; y < gridStates.GetLength(1); y++)
            {
                gridStates[x, y] = GridState.Locked; // 或其他默认状态
            }
        }


        UpdateWithoutCoverGridState(unlockRadius);
    }

    public void UpdateWithoutCoverGridState(int borderRadius)
    {
        //Debug.Log("Updated: " + borderRadius);
        if (borderRadius >= 0)
        {
            for (int x = borderRadius; x < gridStates.GetLength(0) - borderRadius; x++)
            {
                for (int y = borderRadius; y < gridStates.GetLength(1) - borderRadius; y++)
                {
                    if (gridStates[x, y] == GridState.Locked)
                        UpdateGridState(x, y, GridState.Placeable);
                }
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


    public GridPlaceableObject[,] Check3x3Matrix(int row, int col)
    {
        if (row < 1 || col < 1 || row >= gridStates.GetLength(0) - 1 || col >= gridStates.GetLength(1) - 1)
        {
            // 超出边界或不能形成一个完整的 3x3 矩阵
            return null;
        }

        GridPlaceableObject[,] matrixObjects = new GridPlaceableObject[3, 3];
        int count = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int newRow = row + i;
                int newCol = col + j;

                if (gridStates[newRow, newCol] != GridState.Occupied)
                {
                    // 如果任何单元格不是 Occupied，返回 null
                    return null;
                }
                count++;
                matrixObjects[i + 1, j + 1] = gridObjects[newRow, newCol];
            }
        }

        return matrixObjects; // 所有单元格都是 Occupied，返回对象矩阵
    }

}