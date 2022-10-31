using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    [SerializeField] private LayerMask mousePlaneLayerMask;
    [SerializeField] private GameObject cursorObject;

    private static MouseWorld instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        cursorObject.transform.position = MouseWorld.GetPosition();
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);

        return raycastHit.point;
    }
}
