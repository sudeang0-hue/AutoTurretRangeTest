using System.Collections;
using UnityEngine;

namespace _02_Script
{
    public class UIFader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.5f;

        public void FadeIn()
        {
            StartCoroutine(FadeRoutine(1f, 0f));
        }

        public void FadeOut()
        {
            StartCoroutine(FadeRoutine(0f, 1f));
        }
        
        private IEnumerator FadeRoutine(float from, float to)
        {
            float elapsed = 0f;
            
            canvasGroup.alpha = from;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                
                float t  = elapsed / fadeDuration;
                canvasGroup.alpha = Mathf.Lerp(from, to, t);
                
                yield return null;
            }
            
            canvasGroup.alpha = to;
        }
    }
}