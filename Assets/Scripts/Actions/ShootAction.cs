using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    [SerializeField] private int maxShootDistance = 7;
    [SerializeField] private float aimingStateTime = 1f;
    [SerializeField] private float shootingStateTime = 0.1f;
    [SerializeField] private float cooloffStateTime = 0.5f;
    [SerializeField] private float rotateSpeed = 10f;

    public class OnShootEventArgs : EventArgs
    {
        public Unit shootingUnit;
        public Unit targetUnit;
    }

    public event EventHandler<OnShootEventArgs> OnShoot;

    private enum State
    {
        Aiming,
        Shooting,
        Cooloff
    }

    private State state;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;


    private void Update()
    {
        if (!isActive)
            return;

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Aiming:
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, rotateSpeed * Time.deltaTime);
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                state = State.Shooting;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Cooloff;
                stateTimer = cooloffStateTime;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }

    private void Shoot()
    {
        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            shootingUnit = unit,
            targetUnit = targetUnit
        });

        targetUnit.Damage(40);
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<GridPosition> GetValidGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                // Invalid GridPosition
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                // Out of Range
                if (Mathf.Abs(x) + Mathf.Abs(z) > maxShootDistance)
                    continue;

                // GridPosition is empty
                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    continue;

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                // Both Units on same team
                if (targetUnit.IsEnemy() == unit.IsEnemy())
                    continue;

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);

        state = State.Aiming;
        stateTimer = aimingStateTime;

        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        canShootBullet = true;
    }
}
