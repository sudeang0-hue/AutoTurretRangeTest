using System.Collections;
using UnityEngine;

namespace _02_Script
{
    public class skillController  : MonoBehaviour
    {
        [SerializeField] private float cooldown = 2f;

        private bool canUseSkill = true;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryUseSkill();
            }
        }

        private void TryUseSkill()
        {
            if (!canUseSkill)
            {
                Debug.Log("쿨 타임 중 ");
                return;
            }
            
            Debug.Log("스킬 사용");
            StartCoroutine(CoolDownRoutine());
        }

        private IEnumerator CoolDownRoutine()
        {
            canUseSkill = false;
            Debug.Log("설정해둔 스킬 사용.");
            
            //설정해둔 스킬 재사용시간 
            yield return new WaitForSeconds(cooldown);
            
            canUseSkill = true;
        }
    }
}