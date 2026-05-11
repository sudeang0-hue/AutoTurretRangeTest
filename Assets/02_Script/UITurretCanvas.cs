using System;
using UnityEngine;

namespace _02_Script
{
    public class UITurretCanvas : MonoBehaviour
    {
        [SerializeField] private Camera MainCamera;

        private void Awake()
        {
            if(MainCamera == null) MainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();
            SetMainCamera();
        }

        private void SetMainCamera()
        {
            Transform cameraTransform = MainCamera.transform;
            Vector3 Direction = cameraTransform.position - transform.position;
            Direction.Normalize();
            transform.rotation = Quaternion.LookRotation(Direction);
        }
    }
}