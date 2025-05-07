using UnityEngine;

public class S_PlayStep: MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    private AudioClip[] footstepSounds;
    private AudioClip jumpSound;
    private AudioClip landSound;
    private AudioClip ropeSound;

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