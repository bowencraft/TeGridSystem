using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridDataManager;

public class GridDisplayManager : MonoBehaviour
{
    private GridDataManager dataManager;
    private GridManager gridManager;
    private MultiGridManager multiGridManager;
    private SpriteRenderer gridRenderer;

    public Sprite placeableSprite; // 可放置状态的 Sprite
    public Sprite occupiedSprite;  // 已放置状态的 Sprite
    public Sprite lockedSprite;    // 未解锁状态的 Sprite

    private List<GameObject> gridSprites = new List<GameObject>(); // 存储所有 Sprite 的列表

    void Awake()
    {
        dataManager = GetComponent<GridDataManager>();
        gridManager = GetComponent<GridManager>();
        multiGridManager = gridManager.multiGridManager;

        placeableSprite = multiGridManager.placeableSprite;
        occupiedSprite = multiGridManager.occupiedSprite;
        lockedSprite = multiGridManager.lockedSprite;

        InitializeGridSprites();
        StartGridDisplay();
    }

    void InitializeGridSprites()
    {
        for (int x = 0; x < dataManager.gridStates.GetLength(0); x++)
        {
            for (int y = 0; y < dataManager.gridStates.GetLength(1); y++)
            {
                Vector3 position = gridManager.GridToWorldPosition(new Vector2Int(x,y));
                position.x += 0.5f;
                position.z += 0.5f;
                GameObject gridSprite = new GameObject("Grid Sprite - " + x + "," + y);
                gridSprite.transform.position = position;
                gridSprite.transform.rotation = Quaternion.Euler(90f, 0f, 0f);



                gridSprite.transform.parent = transform;
                gridSprite.AddComponent<SpriteRenderer>();

                gridSprites.Add(gridSprite);
            }
        }
    }

    void StartGridDisplay()
    {
        for (int i = 0; i < gridSprites.Count; i++)
        {
            int x = i / dataManager.gridStates.GetLength(1);
            int y = i % dataManager.gridStates.GetLength(1);
            GridState state = dataManager.gridStates[x, y];
            DisplayGridState(gridSprites[i], state);
        }
    }



    public void DisplayGridState(GameObject gridSprite, GridState state)
    {
        SpriteRenderer spriteRenderer = gridSprite.GetComponent<SpriteRenderer>();
        switch (state)
        {
            case GridState.Placeable:
                spriteRenderer.sprite = placeableSprite;
                break;
            case GridState.Occupied:
                spriteRenderer.sprite = occupiedSprite;
                break;
            case GridState.Locked:
                spriteRenderer.sprite = lockedSprite;
                break;
            case GridState.Merged:
                spriteRenderer.sprite = lockedSprite;
                break;
        }
    }


    public void DisplayGridState(int x, int y, GridState newState)
    {
        int index = x * dataManager.gridStates.GetLength(1) + y ;
        if (index >= 0 && index < gridSprites.Count)
        {
            GameObject gridSprite = gridSprites[index];
            DisplayGridState(gridSprite, newState);
        }
    }

}