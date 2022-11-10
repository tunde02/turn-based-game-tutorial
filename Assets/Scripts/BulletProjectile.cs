using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVFXPrefab;
    [SerializeField] private float bulletSpeed = 200f;

    private Vector3 targetPosition;


    private void Update()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float distanceBeforeMoving, distanceAfterMoving;

        distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);

        transform.position += moveDirection * bulletSpeed * Time.deltaTime;

        distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

        if (distanceBeforeMoving < distanceAfterMoving)
        {
            transform.position = targetPosition;
            trailRenderer.transform.parent = null;

            Destroy(gameObject);

            Instantiate(bulletHitVFXPrefab, targetPosition, Quaternion.identity);
        }
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
}
