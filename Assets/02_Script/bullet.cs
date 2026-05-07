using System;
using UnityEngine;

namespace _02_Script
{
    public class bullet : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponent<TargetObject>() != null)
            {
                other.gameObject.SetActive(false);
            }
        }
    }
}