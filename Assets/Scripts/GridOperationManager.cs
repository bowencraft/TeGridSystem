using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridDataManager;

public class GridOperationManager : MonoBehaviour
{
    private GridManager currentGridManager;
    private GridPlaceableObject currentObject;
    public GameObject objectPreview;

    public bool isInPlacementMode = false;

    public void Start()
    {
        if (objectPreview != null)
        {
            objectPreview = Instantiate(objectPreview);
            objectPreview.SetActive(false);
        }
    }

    public void EnterPlacementMode(GridManager gridManager, GridPlaceableObject placeableObject)
    {
        if (!isInPlacementMode)
        {
            isInPlacementMode = true;
            currentGridManager = gridManager;
            currentObject = Instantiate(placeableObject);
            currentObject.gameObject.SetActive(false);

            // 创建或更新物品预览
            if (objectPreview == null)
            {
                objectPreview = Instantiate(placeableObject.gameObject);
                // 可能需要禁用一些组件或调整预览样式
            }
            else
            {
                objectPreview.SetActive(true);
                objectPreview.transform.position = placeableObject.transform.position;
            }
        } else
        {
            Debug.Log("Already in placement mode");
        }
    }

    private bool CanPlace()
    {
        Vector2Int? gridPosition = (currentGridManager != null)?currentGridManager.GetGridPositionFromMouse():null;
        if (gridPosition.HasValue)
        {
            return currentGridManager.CanPlaceAtPosition(gridPosition.Value);
        }
        return false;
    }

    public Material finishedMaterial;

    public int unlockRadius = 2;

    public void ConfirmPlacement()
    {
        if (CanPlace())
        {
            Vector2Int gridPosition = currentGridManager.GetGridPositionFromMouse().Value;
            currentObject.transform.position = currentGridManager.GridToWorldPosition(gridPosition);
            currentObject.gameObject.SetActive(true);
            currentGridManager.gridDataManager.UpdateGridState(gridPosition.x, gridPosition.y, GridState.Occupied);

            currentGridManager.gridDataManager.gridObjects[gridPosition.x, gridPosition.y] = currentObject;

            GridPlaceableObject[,] matrixObjects = new GridPlaceableObject[3, 3];

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    matrixObjects = currentGridManager.gridDataManager.Check3x3Matrix(gridPosition.x + i - 1, gridPosition.y + j + 1);
                    Debug.Log("Click On:" + gridPosition+  " " + (gridPosition.x + i) + " " + (gridPosition.y + j));
                    if (matrixObjects != null) break;
                }
            }


            if (matrixObjects != null)
            {
                Debug.Log("Existed");
                for (int i =0; i< matrixObjects.GetLength(0); i++)
                {
                    for (int j = 0; j < matrixObjects.GetLength(0); j++)
                    {
                        GameObject matrixObject = matrixObjects[i, j].gameObject;

                        matrixObject.transform.GetChild(0).GetComponent<Renderer>().material = finishedMaterial;
                        currentGridManager.gridDataManager.UpdateGridState(gridPosition.x + i - 1, gridPosition.y + j - 1, GridState.Merged);

                    }
                }
                currentGridManager.gridDataManager.UpdateWithoutCoverGridState(unlockRadius);
                unlockRadius -= 1;
            }
            else
            {
                Debug.Log("Not Existed");
                // 不满足 3x3 Occupied 条件
            }


            // 退出放置模式
            ExitPlacementMode();
        }
    }

    public GameObject placeObject;

    void Update()
    {
        if (currentObject != null)
        {
            Vector2Int? gridPosition = currentGridManager.GetGridPositionFromMouse();
            if (gridPosition.HasValue)
            {
                objectPreview.transform.position = currentGridManager.GridToWorldPosition(gridPosition.Value);
            } else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int layerMask = LayerMask.GetMask("Plane");

                // 进行射线投射
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                {
                    objectPreview.transform.position = hit.point;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
            EnterPlacementMode(FindObjectOfType<GridManager>(), placeObject.GetComponent<GridPlaceableObject>());

        if (Input.GetMouseButtonDown(0) && CanPlace())
            ConfirmPlacement();

        //if (Input.GetKeyDown(KeyCode.Q))
        //{

        //    currentGridManager.gridDataManager.UpdateWithoutCoverGridState(unlockRadius);
        //    unlockRadius -= 1;
        //}
    }

    private void ExitPlacementMode()
    {
        if (objectPreview != null)
        {
            objectPreview.SetActive(false);
        }
        currentObject = null;
        currentGridManager = null;
        isInPlacementMode = false;
    }



}