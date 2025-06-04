using UnityEngine;
using UnityEngine.VFX;

public class S_PlayerFX : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip ropeSound;

    [SerializeField] private GameObject footstepVFX;
    [SerializeField] private GameObject fallburstVFX;
    [SerializeField] private Transform vfxSpawnPoint;

    private VisualEffect footstep;
    private VisualEffect fallburst;

    private void Awake()
    {
        GameObject vfxInstance = Instantiate(footstepVFX, vfxSpawnPoint.position, vfxSpawnPoint.rotation);
        footstep = vfxInstance.GetComponent<VisualEffect>();

        vfxInstance = Instantiate(fallburstVFX, vfxSpawnPoint.position, vfxSpawnPoint.rotation);
        fallburst = vfxInstance.GetComponent<VisualEffect>();
    }

    public void PlayStep()
    {
        if (footstepSounds.Length == 0) return;
        int randomIndex = Random.Range(0, footstepSounds.Length);
        AudioClip clip = footstepSounds[randomIndex];
        audioSource.PlayOneShot(clip);

        if(footstep != null)
        {
            footstep.transform.position = vfxSpawnPoint.position;
            footstep.Play();
        }
        
        /*if (footstepVFX != null && vfxSpawnPoint != null)
        {
            GameObject vfxInstance = Instantiate(footstepVFX, vfxSpawnPoint.position, vfxSpawnPoint.rotation);
            VisualEffect vfx = vfxInstance.GetComponent<VisualEffect>();
            if (vfx != null)
            {
                vfx.SendEvent("OnPlay");
            }
        }*/
    }
    public void PlayJump()
    {
        audioSource.PlayOneShot(jumpSound);
    }

    public void PlayLanding()
    {
        audioSource.PlayOneShot(landSound);
        if(fallburst != null)
        {
            fallburst.transform.position = vfxSpawnPoint.position;
            fallburst.Play();
        }

        /*if (fallburstVFX != null && vfxSpawnPoint != null)
        {
            GameObject vfxInstance = Instantiate(fallburstVFX, vfxSpawnPoint.position, vfxSpawnPoint.rotation);
            VisualEffect vfx = vfxInstance.GetComponent<VisualEffect>();
            if (vfx != null)
            {
                vfx.SendEvent("OnPlay");
            }
        }*/
    }

    public void PlayRope()
    {
        audioSource.PlayOneShot(ropeSound);
    }

}