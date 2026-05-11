using TMPro;
using UnityEngine;

namespace _02_Script
{
    public class TargetCounter : MonoBehaviour
    {
        [SerializeField] private EnemySpawner enemySpawner;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private UIFader  uiFader;

        [SerializeField] private int maxTargetCount;
        [SerializeField] private int killTargetCount;
        private void Awake()
        {
            if(enemySpawner == null)enemySpawner = FindFirstObjectByType<EnemySpawner>();
            if(uiFader == null)uiFader = FindFirstObjectByType<UIFader>();
            if (text == null)
            {
                Debug.LogError("No TextMeshProUGUI found");
            }

            SetData();
        }

        private void Start()
        {
            uiFader.FadeIn();
        }

        private void Update()
        {
            text.text = remainingEnemies();
            if (maxTargetCount <= killTargetCount)
            {
                uiFader.FadeOut();
            }
        }

        private void OnEnable()
        {
            TargetObject.OnObjectKilled += TargetCount;
        }

        private void OnDisable()
        {
            TargetObject.OnObjectKilled -= TargetCount;
        }

        private void SetData()
        {
            //데이터 받아오기.
            maxTargetCount = enemySpawner.TargetSet;
        }

        private string remainingEnemies()
        {
            string _text = "remaining enemies\n";
            _text += $"{killTargetCount} / {maxTargetCount}";
            return _text;
        }

        private void TargetCount()
        {
            killTargetCount++;
        }
    }
}