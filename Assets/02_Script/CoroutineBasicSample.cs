using System.Collections;
using UnityEngine;

namespace _02_Script
{
    public class CoroutineBasicSample : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(MyCoroutine());
        }

        private IEnumerator MyCoroutine()
        {
            Debug.Log("MyCoroutine 시작.");
            
            float waitTime = 1.5f;
            yield return new WaitForSeconds(waitTime);
            
            Debug.Log($"MyCoroutine {waitTime}초 후 시작.");
        }
       
    }
}