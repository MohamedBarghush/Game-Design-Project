using UnityEngine;

namespace Player
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private bool isCameraLocked = false;

        private CameraHandler cameraHandler;
        private CameraSwitcher cameraSwitcher;
        [SerializeField] private PlayerManager playerManager;

        void Awake()
        {
            cameraHandler = GetComponent<CameraHandler>();
            cameraSwitcher = GetComponent<CameraSwitcher>();
            cameraSwitcher.InitializeCameraSwitching();
        }

        void Update()
        {
            cameraSwitcher.HandleCameraSwitching(playerManager, ref isCameraLocked);
            if (playerManager != null) playerManager.isTargeting = isCameraLocked;
        }

        void LateUpdate()
        {
            cameraHandler.HandleCameraControl(isCameraLocked);
        }
    }
}