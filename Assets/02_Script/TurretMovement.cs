using UnityEngine;

namespace _02_Script
{
    public class TurretMovement : MonoBehaviour
    {
        [Header("계산용 포탑 위치.")]
        public Transform TurretPivot;
        [Header("위치 이동용 Transform")]
        public Transform orbitYawPivot;
        public Transform orbitPitchPivot;
        public Transform FirePosition;
        [Header("탄환 오브젝트")]
        public GameObject bulletPrefab;

        public float YawSpeed = 30.0f;
        public float PitchSpeed = 10.0f;

        public float currentpitch;
        public float minPitch = -45.0f;
        public float maxPitch = 20.0f;

        public bool YawMatch;
        public bool PitchMatch;
        public float fireAngleThreshold = 5.0f;

        public float firecurrentime = -999f;
        public float fireInterval = 0.5f;
        public float projectileSpeed = 25.0f;
        public float projectilelifetime = 3.0f;
        public Transform TargetTransform;
        private void Update()
        {
            if (TurretPivot == null)
            {
                TurretPivot = gameObject.GetComponent<Transform>();
            }
            if(orbitYawPivot == null) return;
            if(orbitPitchPivot == null) return;
            
            yawAngleRegulator(orbitYawPivot, TargetTransform);
            pitchAngleRegulator(orbitPitchPivot, TargetTransform);

            if (YawMatch && PitchMatch)
            {
                bulletFire();
            }
        }

        private void bulletFire()
        {
            if (FirePosition == null)
            {
                Debug.Log("FirePosition이 설정되어있지 않습니다.");
                return;
            }

            
            if (Time.time < firecurrentime + fireInterval)
            {
                Debug.Log("재장전중...");
                return;
            }

            firecurrentime = Time.time;
            GameObject bullet = Instantiate(bulletPrefab, FirePosition.position, Quaternion.identity);
            if (bullet.GetComponent<Rigidbody>() != null)
            {
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.AddForce(FirePosition.forward * projectileSpeed, ForceMode.Impulse);
            }
            Destroy(bullet, projectilelifetime);
            
        }

        private void yawAngleRegulator(Transform turret , Transform target)
        {
            if(turret == null || target == null) return;
            //거리 구하기 y값 (높이)는 필요 없으니 0 으로 처리. 
            Vector3 myposition = turret.position;
            Vector3 direction = target.position - turret.position;
            direction.y = 0.0f;
            // 좌표에서 각도 구하기 = arctangent  -> radian 값을 Degree로 변환.
            float angle = Vector3.SignedAngle(turret.forward,direction, Vector3.up);
            
            float turretMotion = angleDirection(angle) * YawSpeed * Time.deltaTime;
            
            turret.Rotate(Vector3.up, turretMotion,Space.Self);
            
            YawMatch = Mathf.Abs(angle)<fireAngleThreshold;
            
        }

        private void pitchAngleRegulator(Transform turret, Transform target)
        {
            if (turret == null || target == null) return;

            // 방향 계산
            Vector3 dir = target.position - turret.position;
            Vector3 localDir = turret.InverseTransformDirection(dir);

            // 목표까지 남은 Pitch
            float angle = -Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;

            float step = PitchSpeed * Time.deltaTime;
            float rotateAmount = Mathf.Clamp(angle, -step, step);

            // 현재 Pitch
            float currentPitch = turret.localEulerAngles.x;
            if (currentPitch > 180f) currentPitch -= 360f;

            this.currentpitch = currentPitch;

            //다음 프레임 Pitch 예측
            float nextPitch = currentPitch + rotateAmount;

            // Clamp 적용
            if (nextPitch < minPitch)
                rotateAmount = minPitch - currentPitch;

            if (nextPitch > maxPitch)
                rotateAmount = maxPitch - currentPitch;

            // 회전
            turret.Rotate(Vector3.right, rotateAmount, Space.Self);

            /*Debug.Log($"Pitch Error: {angle:F4} | Current: {currentPitch:F2}");*/

            PitchMatch = Mathf.Abs(angle) < fireAngleThreshold;
        }
        private float angleDirection(float angle)
        {
            if (angle > 0f) return 1f;
            if (angle < 0f) return -1f;
            return 0f;
        }
    }
}