using UnityEngine;
using UnityEngine.VFX;

public class S_GoombaSounds : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip potStep1;
    [SerializeField] private AudioClip potStep2;
    [SerializeField] private AudioClip potDeath;

    public void PlayPotStep1()
    {
        audioSource.PlayOneShot(potStep1);
    }

    public void PlayPotStep2()
    {
        audioSource.PlayOneShot(potStep2);
    }

    public void PlayPotDeath()
    {
        audioSource.PlayOneShot(potDeath);
    }
}
