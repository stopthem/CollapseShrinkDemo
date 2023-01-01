using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CanTemplate.Extensions
{
    public static class ParticleExtensions
    {
        public static void StopClear(this ParticleSystem p)
        {
            p.Stop();
            p.Clear();
        }

        public static void PlayOrStop(this ParticleSystem p, bool status, bool clearIfStopped = true, bool dontPlayIfPlaying = true)
        {
            if (status)
            {
                if (dontPlayIfPlaying && p.isPlaying) return;
                p.Play();
            }
            else
            {
                if (clearIfStopped) p.StopClear();
                else p.Stop();
            }
        }
    }
}