using System;
using System.Collections.Generic;
using UnityEngine;

namespace _02_Script
{
    public class TurretSound : MonoBehaviour
    {
        [SerializeField] private Sound sound;
        [SerializeField] private List<AudioSource> AudioClips;

        private void Awake()
        {
            AudioClips = new List<AudioSource>();
        }
        public void PlaySound()
        {
            if (AudioClips.Count <= 0)
            {
                AudioSource source = createSound();
                PlaySound(source);
            }

            else
            {
                bool _isPlay = false;
                foreach (AudioSource s in AudioClips)
                {
                    if (!s.isPlaying)
                    {
                        PlaySound(s);
                        _isPlay = true;
                    }
                }

                if (!_isPlay)
                {
                    AudioSource source = createSound();
                    PlaySound(source);
                }
            }
        }
        
        private AudioSource createSound()
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            AudioClips.Add(source);
            return source;
        }

        private void PlaySound(AudioSource source)
        {
            source.clip = sound.clip;
            source.volume = sound.volume;
            source.pitch = sound.pitch;
            source.Play();
        }
    }
        
}