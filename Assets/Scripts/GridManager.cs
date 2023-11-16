using UnityEngine;
using static GridDataManager;

public class GridManager : MonoBehaviour
{
    public Vector2Int gridSize; // 网格的大小

    public GridDataManager gridDataManager;
    public GridDisplayManager gridDisplayManager;
    public MultiGridManager multiGridManager;

    void Start()
    {

        // 添加 Box Collider
        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.size = new Vector3(gridSize.x, 0.1f, gridSize.y); // 假设高度为 1
        collider.center = new Vector3(gridSize.x / 2f, 0f, gridSize.y / 2f);
        gameObject.layer = 8;

        // 初始化GridDataManager和GridDisplayManager
        gridDataManager = gameObject.AddComponent<GridDataManager>();
        gridDisplayManager = gameObject.AddComponent<GridDisplayManager>();
    }

    public void ApplySettings(GridSettings settings)
    {
        // 应用设置，比如设置网格的大小、位置等
        gridSize = settings.gridSize;
        transform.position = settings.gridPosition;
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        Vector3 localPos = transform.InverseTransformPoint(worldPosition);
        int x = Mathf.FloorToInt(localPos.x);
        int z = Mathf.FloorToInt(localPos.z);
        return new Vector2Int(x, z);
    }

    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        Vector3 localPos = new Vector3(gridPosition.x, 0, gridPosition.y);
        return transform.TransformPoint(localPos);
    }

    public bool CanPlaceAtPosition(Vector2Int gridPosition)
    {
        if (gridPosition.x < 0 || gridPosition.x >= gridSize.x ||
            gridPosition.y < 0 || gridPosition.y >= gridSize.y)
        {
            return false; // 坐标超出网格范围
        }
        return gridDataManager.gridStates[gridPosition.x, gridPosition.y] == GridState.Placeable;
    }

    public Vector2Int? GetGridPositionFromMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int layerMask = LayerMask.GetMask("Grid");


            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.gameObject == gameObject) // 确保射线击中的是网格对象
            {
                Vector3 worldPosition = hit.point;
                Vector2Int gridPosition = WorldToGridPosition(worldPosition);
                return gridPosition;
            }
        }
        return null; // 如果没有击中网格，则返回 null
    }

    void Update()
    {
        Vector2Int? gridPosition = GetGridPositionFromMouse();
        if (gridPosition.HasValue)
        {
            //Debug.Log(gridDataManager.GetGridState(gridPosition.Value));
        }
    }

}
