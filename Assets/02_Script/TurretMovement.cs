using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        [Header("감지용 Collider")]
        public SphereCollider TurretCollider;
        [Header("감지된 Target")]
        public List<GameObject> targetObjects = new List<GameObject>();
        [Header("총알 오브젝트를 모을 폴더")]
        public Transform bulletParent;
        [Header("상호작용 스크립트")]
        public Image ReloadSlider;
        public TurretSound turretSound;
        public Transform gunBarrel;
        public GameObject gunFireEffect;
        /*public Transform TargetTransform;*/
        
        public float YawSpeed = 120.0f;
        public float PitchSpeed = 60.0f;

        public float currentpitch;
        public float minPitch = -45.0f;
        public float maxPitch = 20.0f;

        public bool YawMatch;
        public bool PitchMatch;
        public float fireAngleThreshold = 0.5f;

        public float firecurrentime = -999f;
        public float fireInterval = 0.4f;
        public float projectileSpeed = 60.0f;
        public float projectilelifetime = 3.0f;

        private void Awake()
        {
            if(TurretCollider== null) TurretCollider = GetComponent<SphereCollider>();
            if (gunFireEffect != null)
            {
                if (gunFireEffect.activeSelf)
                {
                    gunFireEffect.SetActive(false);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!other.gameObject.activeSelf)return;
            
            if(other.gameObject.GetComponent<TargetObject>() != null)
            {
                if (!targetObjects.Contains(other.gameObject))
                {
                    targetObjects.Add(other.gameObject);
                }
            }
        }
        private void Update()
        {
            if (TurretPivot == null)
            {
                TurretPivot = gameObject.GetComponent<Transform>();
            }
            if(orbitYawPivot == null) return;
            if(orbitPitchPivot == null) return;

            ReloadSliderUpdate();
            if (targetObjects != null && targetObjects.Count > 0)
            {
                
                yawAngleRegulator(orbitYawPivot, targetObjects[0].transform);
                pitchAngleRegulator(orbitPitchPivot, targetObjects[0].transform);

                if (YawMatch && PitchMatch)
                {
                    if (Time.time < firecurrentime + fireInterval)
                    {
                        return;
                    }
                    StartCoroutine(BulletFireSequence());
                }
            }

        }

     

        private void FixedUpdate()
        {
            if (targetObjects != null && targetObjects.Count > 0)
            {
                for (int i = 0; i < targetObjects.Count; i++)
                {
                    if (!targetObjects[i].activeSelf)
                    {
                        targetObjects.Remove(targetObjects[i]);
                    }
                }
            }
        }
        private void ReloadSliderUpdate()
        {
            float _timeLerp = Mathf.InverseLerp(firecurrentime, firecurrentime + fireInterval, Time.time);
            float _fillAmount = Mathf.Lerp(0, 100, _timeLerp)/100;
            if (_fillAmount < 0.99f)
            {
                ReloadSlider.color = Color.red;
            }
            else
            {
                ReloadSlider.color = Color.green;
            }
            ReloadSlider.fillAmount = _fillAmount;

        }

        private IEnumerator BulletFireSequence()
        {
           
            firecurrentime = Time.time;
            //1.반동 모션
            StartCoroutine(gunBarrelMoving());
            //2.사운드 출력
            playFireSound();
            //3. 이펙트 출력
            StartCoroutine(setGunFireEffect());
            yield return null;
            //4. 총알 발사
            bulletFire();
            yield return null;
        }

        private void playFireSound()
        {
            if(turretSound == null) return;
            turretSound.PlaySound();
        }
        private IEnumerator setGunFireEffect()
        {
            
            gunFireEffect.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            gunFireEffect.SetActive(false);
        }

        private IEnumerator gunBarrelMoving()
        {
            if (gunBarrel == null)
            {
                Debug.LogWarning("gunBarrelMoving - gunBarrel is null");
            }
            float elapsed = 0f; // 경과 시간
            Vector3 currentPosition = gunBarrel.localPosition; // 바렐 위치
            Vector3 targetPosition = currentPosition + Vector3.back; // 목표 위치

            while (elapsed < fireInterval/2)
            {
                elapsed += Time.deltaTime;
                float t =  elapsed / (fireInterval/2);
                gunBarrel.localPosition = Vector3.Lerp(currentPosition, targetPosition, t);
                
                yield return null;
            }
            elapsed = 0f;
            while (elapsed < fireInterval/2)
            {
                elapsed += Time.deltaTime;
                float t =  elapsed / (fireInterval/2);
                gunBarrel.localPosition = Vector3.Lerp(targetPosition, currentPosition, t);
                
                yield return null;
            }
        }
        private void bulletFire()
        {
            if (FirePosition == null)
            {
                Debug.Log("FirePosition이 설정되어있지 않습니다.");
                return;
            }
            GameObject bullet = Instantiate(bulletPrefab, FirePosition.position, Quaternion.identity , bulletParent);
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.chartreuse;
            Gizmos.DrawWireSphere(TurretPivot.position + TurretCollider.center, TurretCollider.radius);
        }
    }
}