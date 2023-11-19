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
        unlockRadius = gridManager.unlockableBorder;


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

    public GridPlaceableObject[,] CheckMatrix(int row, int col)
    {
        int maxSize = Math.Min(gridStates.GetLength(0), gridStates.GetLength(1));
        GridPlaceableObject[,] largestMatrix = null;
        int largestSize = 0;

        for (int startRow = 0; startRow < gridStates.GetLength(0); startRow++)
        {
            for (int startCol = 0; startCol < gridStates.GetLength(1); startCol++)
            {
                for (int size = 3; size <= maxSize; size++)
                {
                    if (startRow + size > gridStates.GetLength(0) || startCol + size > gridStates.GetLength(1))
                    {
                        break; // 超出边界，不再检查更大的矩阵
                    }

                    if (row >= startRow && row < startRow + size && col >= startCol && col < startCol + size)
                    {
                        if (IsMatrixOccupied(startRow, startCol, size) && size > largestSize)
                        {
                            largestSize = size;
                            largestMatrix = ExtractMatrix(startRow, startCol, size);
                        }
                    }
                }
            }
        }

        return largestMatrix;
    }


    private bool IsMatrixOccupied(int startRow, int startCol, int size)
    {
        if (startRow < 0 || startCol < 0 || startRow + size > gridStates.GetLength(0) || startCol + size > gridStates.GetLength(1))
        {
            return false;
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (gridStates[startRow + i, startCol + j] != GridState.Occupied)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private GridPlaceableObject[,] ExtractMatrix(int startRow, int startCol, int size)
    {
        GridPlaceableObject[,] matrixObjects = new GridPlaceableObject[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                matrixObjects[i, j] = gridObjects[startRow + i, startCol + j];
            }
        }

        return matrixObjects;
    }

}