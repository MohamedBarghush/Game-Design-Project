using UnityEngine;
using Unity.Cinemachine;
using System;

namespace Player
{
    public class CameraSwitcher : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera freeLookCamera;
        [SerializeField] private CinemachineCamera targetedCamera;
        public enum CameraMode { FreeLook, Targeted }
        public CameraMode cameraMode = CameraMode.FreeLook;
        [SerializeField] private float targetSphereRadius = 5f;
        [SerializeField] private LayerMask targetLayerMask;
        // [SerializeField] private bool canSwitchCamera = false;

        public void InitializeCameraSwitching()
        {
            // Initialize the cameras here if needed
            cameraMode = CameraMode.FreeLook;
            freeLookCamera.Priority = 10;
            targetedCamera.Priority = 0;
        }

        public void HandleCameraSwitching(PlayerManager playerManager, ref bool isCameraLocked)
        {
            Transform target = CheckTargetsInRange();
            if (target != null) {
                if (playerManager.inputHandler.targetInput) {
                    playerManager.inputHandler.targetInput = false;
                    SwitchCameraMode(ref isCameraLocked, target);
                }
            } else {
                cameraMode = CameraMode.FreeLook;
                targetedCamera.LookAt = null;
                targetedCamera.Priority = 0;
                freeLookCamera.Priority = 10;
                isCameraLocked = false;
            }
        }

        private void SwitchCameraMode(ref bool isCameraLocked, Transform target)
        {
            cameraMode = cameraMode == CameraMode.FreeLook ? CameraMode.Targeted : CameraMode.FreeLook;
            if (cameraMode == CameraMode.Targeted) 
            {
                targetedCamera.LookAt = target;
                targetedCamera.Priority = 10;
                freeLookCamera.Priority = 0;
                isCameraLocked = true;
            } else {
                targetedCamera.LookAt = null;
                targetedCamera.Priority = 0;
                freeLookCamera.Priority = 10;
                isCameraLocked = false;
            }
        }

        private Transform CheckTargetsInRange()
        {
            Collider[] allColliders = Physics.OverlapSphere(transform.position, targetSphereRadius, targetLayerMask);
            Collider closestCollider = null;
            float closestDistance = float.MaxValue;
            
            Vector3 forwardDirection = Camera.main.transform.forward * 2f;
            
            foreach (Collider collider in allColliders)
            {
                // Calculate direction to target and check if it's in front using dot product
                Vector3 directionToTarget = (collider.transform.position - Camera.main.transform.position).normalized;
                float dotProduct = Vector3.Dot(forwardDirection, directionToTarget);
                
                // Only consider objects in front of the camera (dot product > 0)
                if (dotProduct > 0)
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (closestCollider == null || distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestCollider = collider;
                    }
                }
            }
            
            if (closestCollider == null) return null;

            return closestCollider.transform;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, targetSphereRadius);
        }
    }
}