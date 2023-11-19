using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static GridDataManager;

public class GridOperationManager : MonoBehaviour
{
    private GridManager currentGridManager;
    //private GridPlaceableObject currentObject;
    private GameObject currentObject;

    public GameObject objectPreview;

    public bool isInPlacementMode = false;
    public bool isMultipleObjects;

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

    //public void EnterPlacementMode(GridManager gridManager, GridPlaceableObject placeableObject)
    //{
    //    if (!isInPlacementMode)
    //    {
    //        isMultipleObjects = false;
    //        isInPlacementMode = true;
    //        currentGridManager = gridManager;
    //        currentObject = Instantiate(placeableObject).gameObject;
    //        currentObject.name = "Placing - " + currentObject.name;
    //        currentObject.transform.SetParent(placedObjects.transform);
    //        currentObject.gameObject.SetActive(false);

    //        singlePlaceableObject = placeableObject;

    //        // 创建或更新物品预览
    //        if (objectPreview == null)
    //        {
    //            objectPreview = Instantiate(placeableObject.gameObject);
    //            // 可能需要禁用一些组件或调整预览样式
    //        }
    //        else
    //        {
    //            objectPreview.SetActive(true);
    //            objectPreview.transform.position = placeableObject.transform.position;
    //        }
    //    } else
    //    {
    //        Debug.Log("Already in placement mode");
    //    }
    //}

    public void EnterPlacementMode(GridManager gridManager, GridPlaceableMultiObjects placeableObjects)
    {
        if (!isInPlacementMode)
        {
            isMultipleObjects = true;
            isInPlacementMode = true;
            currentGridManager = gridManager;
            currentObject = Instantiate(placeableObjects.gameObject);
            currentObject.name = "Placing - " + currentObject.name;
            currentObject.transform.SetParent(placedObjects.transform);
            currentObject.gameObject.SetActive(false);

            multiPlaceableObjects = currentObject.GetComponent<GridPlaceableMultiObjects>();

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
        Vector2Int? gridPosition = (currentGridManager != null) ? currentGridManager.GetGridPositionFromMouse() : null;
        if (!gridPosition.HasValue)
        {
            return false;
        }

        if (isMultipleObjects && multiPlaceableObjects != null)
        {
            for (int i=0; i< multiPlaceableObjects.objects.GetLength(0); i++) {

                for (int j = 0; j < multiPlaceableObjects.objects.GetLength(1); j++)
                {
                    if (multiPlaceableObjects.objects[i,j] != null)
                    {
                        GridPlaceableObject obj = multiPlaceableObjects.objects[i, j];
                        Vector2Int objPosition = gridPosition.Value + new Vector2Int(obj.offsetPosition.x, obj.offsetPosition.y);
                        if (!currentGridManager.CanPlaceAtPosition(objPosition))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        else
        {
            return currentGridManager.CanPlaceAtPosition(gridPosition.Value);
        }
    }


    public Material finishedMaterial;


    public void ConfirmPlacement()
    {
        if (CanPlace())
        {
            Vector2Int gridPosition = currentGridManager.GetGridPositionFromMouse().Value;

            GridPlaceableObject[,] matrixObjects = null;

            if (isMultipleObjects && multiPlaceableObjects != null)
            {
                currentObject.transform.position = currentGridManager.GridToWorldPosition(gridPosition) + new Vector3(0.5f,0,0.5f);
                currentObject.gameObject.SetActive(true);
                for (int i = 0; i < multiPlaceableObjects.objects.GetLength(0); i++)
                {

                    for (int j = 0; j < multiPlaceableObjects.objects.GetLength(1); j++)
                    {
                        GridPlaceableObject obj = multiPlaceableObjects.objects[i, j];

                        if (obj != null)
                        {
                            Vector2Int objPosition = gridPosition + new Vector2Int(obj.offsetPosition.x, obj.offsetPosition.y );
                            //GameObject objGameObject = Instantiate(obj.gameObject);
                            //objGameObject.transform.position = currentGridManager.GridToWorldPosition(objPosition) + obj.transform.localPosition - new Vector3(i,0,j);
                            //objGameObject.gameObject.SetActive(true);

                            obj.gridPosition = objPosition;
                            currentGridManager.gridDataManager.UpdateGridState(objPosition.x, objPosition.y, GridState.Occupied);
                            currentGridManager.gridDataManager.gridObjects[objPosition.x, objPosition.y] = obj;

                            matrixObjects = currentGridManager.gridDataManager.CheckMatrix(gridPosition.x, gridPosition.y);
                        }
                    }
                }
            }
            else
            {
                currentObject.transform.position = currentGridManager.GridToWorldPosition(gridPosition) + new Vector3(0.5f, 0, 0.5f);
                currentObject.gameObject.SetActive(true);
                currentGridManager.gridDataManager.UpdateGridState(gridPosition.x, gridPosition.y, GridState.Occupied);
                currentGridManager.gridDataManager.gridObjects[gridPosition.x, gridPosition.y] = singlePlaceableObject;

                matrixObjects = currentGridManager.gridDataManager.CheckMatrix(gridPosition.x, gridPosition.y);
            }



            if (matrixObjects != null)
            {
                Debug.Log(matrixObjects.Length);
                for (int i =0; i< matrixObjects.GetLength(0); i++)
                {
                    for (int j = 0; j < matrixObjects.GetLength(0); j++)
                    {
                        GameObject matrixObject = matrixObjects[i, j].gameObject;

                        Renderer renderer = matrixObject.transform.GetComponentInChildren<Renderer>();
                        if (renderer != null)
                        {
                            renderer.material = finishedMaterial;
                        } else
                        {
                            matrixObject.transform.GetComponent<Renderer>().material = finishedMaterial;
                        }
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
                objectPreview.transform.position = currentGridManager.GridToWorldPosition(gridPosition.Value) +new Vector3(0.5f, 0, 0.5f);
            } else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int layerMask = LayerMask.GetMask("Plane");

                // 进行射线投射
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                {
                    objectPreview.transform.position = hit.point + new Vector3(0.5f, 0, 0.5f);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
            EnterPlacementMode(FindObjectOfType<GridManager>(), placeObject.GetComponent<GridPlaceableMultiObjects>());

        if (Input.GetMouseButtonDown(0) && CanPlace())
            ConfirmPlacement();

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (isInPlacementMode && isMultipleObjects && multiPlaceableObjects != null)
            {
                // 旋转预览对象
                if (objectPreview != null)
                {
                    objectPreview.transform.Rotate(0, 90, 0);
                }

                // 旋转当前对象
                if (currentObject != null)
                {
                    currentObject.transform.Rotate(0, 90, 0);
                }

                // 更新多对象的内部状态
                multiPlaceableObjects.RotateMatrixRight();
            }
        } else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (isInPlacementMode && isMultipleObjects && multiPlaceableObjects != null)
            {
                // 旋转预览对象
                if (objectPreview != null)
                {
                    objectPreview.transform.Rotate(0, -90, 0);
                }

                // 旋转当前对象
                if (currentObject != null)
                {
                    currentObject.transform.Rotate(0, -90, 0);
                }

                // 更新多对象的内部状态
                multiPlaceableObjects.RotateMatrixLeft();
            }
        }
    }

    private void ExitPlacementMode()
    {
        //if (objectPreview != null)
        //{
        //    objectPreview.SetActive(false);
        //}
        Destroy(objectPreview);
        objectPreview = null;
        currentObject = null;
        currentGridManager = null;
        isInPlacementMode = false;
    }



}