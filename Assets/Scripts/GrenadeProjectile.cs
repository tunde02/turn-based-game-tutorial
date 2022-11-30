using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    [SerializeField] private float grenadeSpeed = 15;
    [SerializeField] private int grenadeDamage = 30;
    [SerializeField] private float damageRadius = 4f;
    [SerializeField] private Transform grenadeExplodeVfxPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;


    public static event EventHandler OnAnyGrenadeExploded;

    private Action onGrenadeActionComplete;
    private Vector3 targetPosition;
    private float totalDistance;
    private Vector3 positionXZ;


    private void Update()
    {
        Vector3 moveDirection = (targetPosition - positionXZ).normalized;

        positionXZ += moveDirection * grenadeSpeed * Time.deltaTime;

        float currentDistance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - currentDistance / totalDistance;
        float maxHeight = totalDistance / 3f;
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;

        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        float reachedTargetDistance = 0.2f;
        if (Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
        {
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);

            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(grenadeDamage);
                }

                if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate destructibleCrate))
                {
                    destructibleCrate.Damage();
                }
            }

            trailRenderer.transform.parent = null;
            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);
            Instantiate(grenadeExplodeVfxPrefab, targetPosition + Vector3.up, Quaternion.identity);

            Destroy(gameObject);

            onGrenadeActionComplete();
        }
    }

    public void Setup(GridPosition targetGridPosition, Action onGrenadeActionComplete)
    {
        this.onGrenadeActionComplete = onGrenadeActionComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        totalDistance = Vector3.Distance(transform.position, targetPosition);
        positionXZ = transform.position;
        positionXZ.y = 0;
    }
}
