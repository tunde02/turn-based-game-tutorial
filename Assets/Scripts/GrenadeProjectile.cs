using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    [SerializeField] private float grenadeSpeed = 15;
    [SerializeField] private int grenadeDamage = 30;
    [SerializeField] private float damageRadius = 4f;

    private Action onGrenadeActionComplete;
    private Vector3 targetPosition;


    private void Update()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        transform.position += moveDirection * grenadeSpeed * Time.deltaTime;

        float reachedTargetDistance = 0.2f;
        if (Vector3.Distance(transform.position, targetPosition) < reachedTargetDistance)
        {
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);

            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(grenadeDamage);
                }
            }

            Destroy(gameObject);

            onGrenadeActionComplete();
        }
    }

    public void Setup(GridPosition targetGridPosition, Action onGrenadeActionComplete)
    {
        this.onGrenadeActionComplete = onGrenadeActionComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
    }
}
