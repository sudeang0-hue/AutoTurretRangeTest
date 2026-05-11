using System;
using UnityEngine;

namespace _02_Script
{
    public class TargetObject : MonoBehaviour
    {
        public GameObject HitSphere;
        public float TargetSpeed = 2.0f;
        private float lifeTimeSeconds = 10f;
        private float remainingLifeSeconds;

        public static event Action OnObjectKilled;
        
        public void Initialize(float targetSpeed, float lifeTime)
        {
            TargetSpeed = targetSpeed;
            lifeTimeSeconds = lifeTime;
            remainingLifeSeconds = lifeTimeSeconds;
        }
        private void OnEnable()
        {
            remainingLifeSeconds = lifeTimeSeconds;
        }

        private void Update()
        {
            transform.position +=  transform.forward * (TargetSpeed * Time.deltaTime);
            remainingLifeSeconds -= Time.deltaTime;
            if (remainingLifeSeconds <= 0f)
            {
                gameObject.SetActive(false);
            }
        }

        public void HitTarget()
        {
            OnObjectKilled?.Invoke();
            gameObject.SetActive(false);
        }
        private void OnDisable()
        {
            GameObject obj = Instantiate(HitSphere , transform.position, Quaternion.identity);
            Destroy(obj,0.5f);
        }
    }
}