using System;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    [Header("타겟의 중심점입니다.")] 
    public Transform orbitPivot;
    
    [Header("타겟 스피드")]
    public float targetSpeed = 30.0f;

    private void Update()
    {
        if(orbitPivot == null) return;
        
        orbitPivot.Rotate(Vector3.up, targetSpeed * Time.deltaTime ,  Space.Self);
    }
}