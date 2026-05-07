using UnityEngine.Pool;

namespace _02_Script
{
   using System.Collections.Generic;
using UnityEngine;

namespace TurretDemo
{
    /// <summary>
    /// 랜덤 SpawnPoint에서 Enemy를 생성하고 최대 개수를 관리합니다.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EnemySpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private GameObject TargetPrefab;
        private ObjectPool<GameObject> targetPool;
        
        [SerializeField]
        [Tooltip("랜덤 생성 위치 목록.")]
        private Transform[] spawnPoints;

        [SerializeField]
        [Tooltip("생성된 Enemy의 부모(선택).")]
        private Transform enemyRoot;

        [Header("Spawn Settings")]
        [SerializeField]
        [Min(1)]
        private int initialSpawnCount = 4;

        [SerializeField]
        [Min(1)]
        private int maxAliveCount = 8;

        [SerializeField]
        [Min(0.1f)]
        private float spawnIntervalSeconds = 1f;

        [SerializeField]
        [Tooltip("생성 시 Turret 중앙점을 향하도록 forward를 배치합니다.")]
        private Transform lookAtCenter;

        [SerializeField]
        [Min(0.1f)]
        private float enemyMoveSpeedUnitsPerSecond = 3f;

        [SerializeField]
        [Min(0.1f)]
        private float enemyLifeTimeSeconds = 10f;

      
        
        private readonly List<GameObject> aliveEnemies = new List<GameObject>(32);
        private float nextSpawnTimeSeconds;

        private void Awake()
        {
            if (lookAtCenter == null)
            {
                GameObject Turret = GameObject.FindGameObjectWithTag("Turret");
                lookAtCenter = Turret.transform;
            }

            targetPool = new ObjectPool<GameObject>(
                CreateFunc,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                true,
                initialSpawnCount,
                maxAliveCount
            );

           
        }
        private GameObject CreateFunc()
        {
            GameObject spawned = Instantiate(TargetPrefab, enemyRoot.position, Quaternion.identity, enemyRoot);
            return spawned;
        }


        private void OnTakeFromPool(GameObject obj)
        {
            obj.SetActive(true);
            TargetObject target = obj.GetComponent<TargetObject>();
            if (target != null)
            {
                target.Initialize(enemyMoveSpeedUnitsPerSecond,enemyLifeTimeSeconds);
            }
            if (obj.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            aliveEnemies.Add(obj);
        }

        private void OnReturnedToPool(GameObject obj)
        {
            obj.SetActive(false);
            aliveEnemies.Remove(obj);
        }

        private void OnDestroyPoolObject(GameObject obj)
        {
            Destroy(obj);
        }

        public bool createTarget()
        {
            if (TargetPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            {
                return false;
            }

            //pool에서 꺼내기
            GameObject obj = targetPool.Get();
            //위치 및 각도 설정
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (spawnPoint == null)
            {
                return false;
            }
            Quaternion rotation = spawnPoint.rotation;
            if (lookAtCenter != null)
            {
                Vector3 toCenter = lookAtCenter.position - spawnPoint.position;
                if (toCenter.sqrMagnitude > 1e-8f)
                {
                    rotation = Quaternion.LookRotation(toCenter.normalized, Vector3.up);
                }
            }
            // 타겟 위치 및 각도 적용.
            obj.transform.position = spawnPoint.position;
            obj.transform.rotation = rotation;
            // 타겟 활성화
            return true;
        }

        public void ReturnToPool(GameObject obj)
        {
            targetPool.Release(obj);
        }

        private void Update()
        { // 뒤에서부터 조사하면 원소를 삭제해도 Index가 밀리지 않아 에러가 안 납니다.
            for (int i = aliveEnemies.Count - 1; i >= 0; i--)
            {
                // 누군가(예: 총알에 맞음)에 의해 비활성화된 경우 풀에 반납
                if (!aliveEnemies[i].activeSelf)
                {
                    ReturnToPool(aliveEnemies[i]);
                }
            }
            /*RemoveDestroyedEntries();*/
            if (Time.time < nextSpawnTimeSeconds)
            {
                return;
            }

            if (aliveEnemies.Count >= maxAliveCount)
            {
                return;
            }

            if (createTarget())
            {
                nextSpawnTimeSeconds = Time.time + spawnIntervalSeconds;
            }
        }
        
    }
}
}