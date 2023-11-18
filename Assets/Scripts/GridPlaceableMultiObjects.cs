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
                    objects[i, j] = objectsG[i, j].GetComponent<GridPlaceableObject>();
                    Debug.Log(objects[i, j]);
                }

            }
        }
    }
}