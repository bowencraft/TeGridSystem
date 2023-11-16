using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiGridManager : MonoBehaviour
{
    public Sprite placeableSprite; // 可放置状态的 Sprite
    public Sprite occupiedSprite;  // 已放置状态的 Sprite
    public Sprite lockedSprite;    // 未解锁状态的 Sprite

    public float gridSize;

    public List<GridSettings> gridSettingsList;

    private List<GridManager> grids = new List<GridManager>(); // 存储所有Grid实例的列表

    void Start()
    {
        foreach (var settings in gridSettingsList)
        {
            CreateGrid(settings);
        }
    }

    void CreateGrid(GridSettings settings)
    {
        // 实例化网格预制体
        GameObject gridObj = new GameObject(settings.name);
        GridManager gridManager = gridObj.AddComponent<GridManager>();
        gridManager.ApplySettings(settings);

        gridManager.multiGridManager = this;

        grids.Add(gridManager);

        // 将设置应用到 Grid 实例
    }

    public Grid GetGridAtMousePosition()
    {
        // 实现获取鼠标当前所在的网格的逻辑
        // 使用射线投射（Raycasting）来确定鼠标位置对应的网格
        // 返回对应的Grid实例
        return null;
    }
}