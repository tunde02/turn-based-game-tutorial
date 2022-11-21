using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMeshpro;

    private object gridObject;


    protected virtual void Update()
    {
        textMeshpro.text = gridObject.ToString();
    }

    public virtual void SetGridObject(object gridObject)
    {
        this.gridObject = gridObject;
    }
}
