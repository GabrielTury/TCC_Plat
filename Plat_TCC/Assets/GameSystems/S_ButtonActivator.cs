using UnityEngine;
using UnityEngine.VFX;

public class S_ButtonActivator : MonoBehaviour
{
    [SerializeField] private GameObject[] objects;

    private Transform pressedButtonTransform;
    private Vector3 pressedButtonPosition;
    private int timer = 0;
    private bool isActivated = false;

    [SerializeField] private AudioClip buttonPress;
    [SerializeField] private VisualEffect rockVFX;
    [SerializeField] private float vfxDuration = 2f;
    [SerializeField] private bool oneTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pressedButtonTransform = transform;
        pressedButtonPosition = pressedButtonTransform.position;

        if (rockVFX != null)
            rockVFX.Stop();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timer > 0)
            timer -= 1;
    }

    private void ToggleButtonInteraction()
    {
        foreach (GameObject obj in objects)
        {
            IActivatableObjects activatable = obj.GetComponent<IActivatableObjects>();
            if (activatable != null)
                activatable.ToggleButtonInteraction();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (timer <= 0)
        {
            timer = 5;
            if (!isActivated)
            {
                ToggleButtonInteraction();

                pressedButtonTransform.position = new Vector3(
                    pressedButtonPosition.x,
                    pressedButtonPosition.y - 0.3f,
                    pressedButtonPosition.z
                );

                if (buttonPress != null)
                    AudioManager.instance.PlaySFX(buttonPress);

                PlayRockEffect(); // só dispara se tiver efeito configurado

                if(oneTime)
                    isActivated = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isActivated)
        {
            pressedButtonTransform.position = pressedButtonPosition;
        }
    }

    private void PlayRockEffect()
    {
        if (rockVFX != null)
        {
            rockVFX.Play();
            Invoke(nameof(StopRockEffect), vfxDuration);
        }
    }

    private void StopRockEffect()
    {
        if (rockVFX != null)
            rockVFX.Stop();
    }
}
