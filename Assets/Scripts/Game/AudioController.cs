using UnityEngine;

namespace Game
{
    /// <summary>
    /// Fades between audio clips when changing scenes.
    /// </summary>
    public class AudioController : MonoBehaviour
    {
        public AudioClip BattleMusic;
        public AudioClip AmbientMusic;
        public AudioSource Source;
        public Animator FadeAnimator;


        public void Battle()
        {
            FadeAnimator.SetTrigger("Change");
            FadeAnimator.SetTrigger("Battle");
        }

        public void SwapMusicBattle()
        {
            Source.Stop();
            Source.clip = BattleMusic;
            Source.Play();
        }

        public void SwapMusicNormal()
        {
            Source.Stop();
            Source.clip = AmbientMusic;
            Source.Play();
        }

        public void ExitBattle()
        {
            FadeAnimator.SetTrigger("Change");
            FadeAnimator.SetTrigger("Normal");
        }
    }
}