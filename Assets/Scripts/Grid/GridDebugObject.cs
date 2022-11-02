using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMeshpro;
    private GridObject gridObject;


    private void Update()
    {
        textMeshpro.text = gridObject.ToString();
    }

    public void SetGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
    }
}
