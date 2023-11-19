using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PrefabStack : MonoBehaviour
{
    public List<GameObject> prefabs; // 用于堆栈的prefab列表
    //public int AmountToSpawn;
    public Transform stackParent; // 用于存放堆栈的父对象
    public float offset = 0.5f; // 每个prefab之间的偏移量

    public int stackAmount;

    private List<GameObject> stack = new List<GameObject>();

    public void Start()
    {
        InitializeStack(stackAmount);
    }

    public void Update()
    {
        //stackParent.transform.rotation = 
    }

    // 初始化堆栈

    public void InitializeStack(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Count)]; // 随机选择一个Prefab
            GameObject go = Instantiate(randomPrefab, stackParent);
            SetLayer(go.transform.gameObject, 5);

            go.transform.localPosition = new Vector3(0, stack.Count * offset, 0);
            stack.Add(go);
        }
    }


    private void SetLayer(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayer(child.gameObject, layer);
        }
    }

    // 弹出堆栈的顶部prefab
    public GridPlaceableMultiObjects PreviewPop()
    {
        if (stack.Count > 0)
        {
            return stack[stack.Count - 1].GetComponent<GridPlaceableMultiObjects>();
        }
        return null;
    }


    public GridPlaceableMultiObjects Pop()
    {
        if (stack.Count > 0)
        {
            GridPlaceableMultiObjects top = stack[stack.Count - 1].GetComponent<GridPlaceableMultiObjects>();
            Destroy(stack[stack.Count - 1]);
            stack.RemoveAt(stack.Count - 1);
            return top;
        }
        return null;
    }

    //public void RotateTop()
    //{
    //    if (stack.Count > 0)
    //    {
    //        GameObject top = stack[stack.Count - 1];
    //        Transform objTransform = top.transform;
    //        objTransform.GetChild(1).rotation *= Quaternion.Euler(0, 60f, 0);
    //        //objTransform.rotation *= Quaternion.Euler(0, 60, 0);

    //        //top.GetComponent<PrefabCell>().RotateClockwise();

    //    }
    //}

    public Quaternion GetRotation()
    {
        if (stack.Count > 0)
        {
            GameObject top = stack[stack.Count - 1];

            return top.transform.rotation;
        }
        return Quaternion.Euler(0, 0, 0);
    }

    public int Count()
    {
        return stack.Count;
    }
}
