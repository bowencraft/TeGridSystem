using UnityEngine;

[CreateAssetMenu(fileName = "GridSetting", menuName = "Grid/Grid Setting", order = 0)]

public class GridSettings : ScriptableObject
{
    public Vector2Int gridSize;
    public Vector3 gridPosition;

    public int unlockableBorder;
}

