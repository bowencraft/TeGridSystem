using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridDataManager;

public class GridOperationManager : MonoBehaviour
{
    private GridManager currentGridManager;
    //private GridPlaceableObject currentObject;
    private GameObject currentObject;

    public GameObject objectPreview;

    public bool isInPlacementMode = false;
    public bool isMultipleObjects = false;

    public int unlockRadius;

    private GameObject placedObjects;


    private GridPlaceableObject singlePlaceableObject;
    private GridPlaceableMultiObjects multiPlaceableObjects;

    public void Start()
    {
        if (objectPreview != null)
        {
            objectPreview = Instantiate(objectPreview);
            objectPreview.SetActive(false);
        }
        placedObjects = GameObject.Find("PlacedObjects");
        if (placedObjects == null)
        {
            placedObjects = new GameObject("PlacedObjects");
        }
    }

    public void EnterPlacementMode(GridManager gridManager, GridPlaceableObject placeableObject)
    {
        if (!isInPlacementMode)
        {
            isInPlacementMode = true;
            currentGridManager = gridManager;
            currentObject = Instantiate(placeableObject).gameObject;
            currentObject.name = "Placed - " + currentObject.name;
            currentObject.transform.SetParent(placedObjects.transform);
            currentObject.gameObject.SetActive(false);

            singlePlaceableObject = placeableObject;

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

    public void EnterPlacementMode(GridManager gridManager, GridPlaceableMultiObjects placeableObjects)
    {
        if (!isInPlacementMode)
        {
            isInPlacementMode = true;
            currentGridManager = gridManager;
            currentObject = Instantiate(placeableObjects).gameObject;
            currentObject.name = "Placed - " + currentObject.name;
            currentObject.transform.SetParent(placedObjects.transform);
            currentObject.gameObject.SetActive(false);

            multiPlaceableObjects = placeableObjects;

            // 创建或更新物品预览
            if (objectPreview == null)
            {
                objectPreview = Instantiate(placeableObjects.gameObject);
                // 可能需要禁用一些组件或调整预览样式
            }
            else
            {
                objectPreview.SetActive(true);
                objectPreview.transform.position = placeableObjects.transform.position;
            }
        }
        else
        {
            Debug.Log("Already in placement mode");
        }
    }

    private bool CanPlace()
    {
        // check if isMultipleObjects, if so, check if each block is placeable

        Vector2Int? gridPosition = (currentGridManager != null)?currentGridManager.GetGridPositionFromMouse():null;
        if (gridPosition.HasValue)
        {
            return currentGridManager.CanPlaceAtPosition(gridPosition.Value);
        }
        return false;
    }

    public Material finishedMaterial;


    public void ConfirmPlacement()
    {
        if (CanPlace())
        {
            Vector2Int gridPosition = currentGridManager.GetGridPositionFromMouse().Value;

            if (isMultipleObjects)
            {
                // assume mouse is at 0,0 of multiPlaceableObjects.objects;
                // set each gridPosition of placeableObject in the multiPlaceableObjects.objects
            }
            else
            {
                //currentObject.gridPosition = gridPosition;
            }

            currentObject.transform.position = currentGridManager.GridToWorldPosition(gridPosition);
            currentObject.gameObject.SetActive(true);
            currentGridManager.gridDataManager.UpdateGridState(gridPosition.x, gridPosition.y, GridState.Occupied);


            if (isMultipleObjects)
            {
                // put each placeableObject into currentGridManager.gridDataManager.gridObjects from the multiPlaceableObjects.objects
            }
            else
            {
                //currentGridManager.gridDataManager.gridObjects[gridPosition.x, gridPosition.y] = currentObject;
            }


            GridPlaceableObject[,] matrixObjects = currentGridManager.gridDataManager.CheckMatrix(gridPosition.x, gridPosition.y);


            if (matrixObjects != null)
            {
                Debug.Log(matrixObjects.Length);
                for (int i =0; i< matrixObjects.GetLength(0); i++)
                {
                    for (int j = 0; j < matrixObjects.GetLength(0); j++)
                    {
                        GameObject matrixObject = matrixObjects[i, j].gameObject;

                        matrixObject.transform.GetChild(0).GetComponent<Renderer>().material = finishedMaterial;
                        currentGridManager.gridDataManager.UpdateGridState(matrixObjects[i, j].gridPosition.x, matrixObjects[i, j].gridPosition.y, GridState.Merged);

                    }
                }
                currentGridManager.unlockableBorder -= 1;
                currentGridManager.gridDataManager.UpdateWithoutCoverGridState(currentGridManager.unlockableBorder);
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