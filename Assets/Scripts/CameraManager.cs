using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject actionCameraGameObject;


    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

        HideActionCamera();
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                Vector3 cameraCharacterHeight = Vector3.up * 1.7f;

                Vector3 shooterPosition = shootAction.GetUnit().GetWorldPosition() + cameraCharacterHeight;
                Vector3 targetPosition = shootAction.GetTargetUnit().GetWorldPosition() + cameraCharacterHeight;
                Vector3 shootDirection = (targetPosition - shooterPosition).normalized;

                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDirection * 0.5f;

                Vector3 actionCameraPosition = shooterPosition + shoulderOffset + (shootDirection * -1);

                actionCameraGameObject.transform.position = actionCameraPosition;
                actionCameraGameObject.transform.LookAt(targetPosition);

                ShowActionCamera();
                break;
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction:
                HideActionCamera();
                break;
        }
    }

    private void ShowActionCamera()
    {
        actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCameraGameObject.SetActive(false);
    }
}
