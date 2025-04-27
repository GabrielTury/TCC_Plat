using UnityEngine;

public class S_PlayStep: MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] footstepSounds;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip ropeSound;

    public void PlayStepSound()
    {
        if (footstepSounds.Length == 0) return;

        int randomIndex = Random.Range(0, footstepSounds.Length);
        AudioClip clip = footstepSounds[randomIndex];
        audioSource.PlayOneShot(clip);
    }
    public void PlayJumpSound()
    {
        audioSource.PlayOneShot(jumpSound);
    }

    public void PlayLandingSound()
    {
        audioSource.PlayOneShot(landSound);
    }

    public void PlayRopeSound()
    {
        audioSource.PlayOneShot(ropeSound);
    }
}