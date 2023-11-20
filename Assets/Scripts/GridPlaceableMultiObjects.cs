using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridPlaceableMultiObjects : SerializedMonoBehaviour
{
    public GridPlaceableObject[,] objects = new GridPlaceableObject[4, 4]; // 存储多个物品的二维数组

    [TableMatrix(HorizontalTitle = "GObjects")]
    public GameObject[,] objectsG = new GameObject[4, 4]; // 存储多个物品的二维数组
    // 管理多物品的逻辑

    public void Awake()
    {
        objects = new GridPlaceableObject[4, 4];

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (objectsG[i, j] != null && objectsG[i, j].GetComponent<GridPlaceableObject>() != null)
                {
                    GridPlaceableObject gridPlaceableObject = objectsG[i, j].GetComponent<GridPlaceableObject>();
                    gridPlaceableObject.offsetPosition = new Vector2Int(i, j);
                    objects[i, j] = gridPlaceableObject;
                }

            }
        }
    }

    public int rotationAngle = 0;

    public void RotateMatrixRight()
    {
        int size = objects.GetLength(0); // 假设矩阵是正方形
        GridPlaceableObject[,] rotated = new GridPlaceableObject[size, size];
        GameObject[,] rotatedG = new GameObject[size, size];

        rotationAngle += 90;
        if (rotationAngle >= 360) rotationAngle = 0;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                //rotated[i, j] = objects[j, size - 1 - i];
                rotatedG[i, j] = objectsG[j, size - 1 - i];
                //rotated[j, size - 1 - i] = objects[i, j];

                if (objects[i, j] != null)
                {
                    if (rotationAngle == 90)
                    {
                        objects[i, j].offsetPosition = new Vector2Int(j,-i);
                    } else if (rotationAngle == 180)
                    {
                        objects[i, j].offsetPosition = new Vector2Int(-i,-j); //
                        Debug.Log(rotationAngle + ": " + objects[i, j].offsetPosition + ", " + new Vector2Int(-i, j));
                    } else if (rotationAngle == 270)
                    {
                        objects[i, j].offsetPosition = new Vector2Int(-j,i); //
                        Debug.Log(objects[i, j].offsetPosition);
                    } else if (rotationAngle == 0)
                    {
                        objects[i, j].offsetPosition = new Vector2Int(i, j);
                    }
                }

            }
        }

        //objects = rotated;
        objectsG = rotatedG;
    }

    public void RotateMatrixLeft()
    {
        int size = objects.GetLength(0); // 假设矩阵是正方形
        GridPlaceableObject[,] rotated = new GridPlaceableObject[size, size];
        GameObject[,] rotatedG = new GameObject[size, size];

        rotationAngle -= 90;
        if (rotationAngle < 0) rotationAngle = 270;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                rotated[i, j] = objects[j, i];
                //rotated[j, size - 1 - i] = objects[i, j];

                if (objects[i, j] != null)
                {
                    if (rotationAngle == 90)
                    {
                        objects[i, j].offsetPosition = new Vector2Int(j, i);
                    }
                    else if (rotationAngle == 180)
                    {
                        objects[i, j].offsetPosition = new Vector2Int(i, -j); //
                        Debug.Log(rotationAngle + ": " + objects[i, j].offsetPosition + ", " + new Vector2Int(-i, j));
                    }
                    else if (rotationAngle == 270)
                    {
                        objects[i, j].offsetPosition = new Vector2Int(-j, i); //
                        Debug.Log(objects[i, j].offsetPosition);
                    }
                    else if (rotationAngle == 0)
                    {
                        objects[i, j].offsetPosition = new Vector2Int(i, j);
                    }
                }

            }
        }

        //objects = rotated;
        objectsG = rotatedG;
    }

}