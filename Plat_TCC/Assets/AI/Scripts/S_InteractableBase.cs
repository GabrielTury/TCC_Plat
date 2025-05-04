using System.Collections;
using UnityEngine;

public abstract class S_InteractableBase : S_AIBase
{
    protected enum InteractableState
    {
        Moving,
        Idle,
        Interacting
    };

    protected InteractableState currentState = InteractableState.Idle;

    protected abstract IEnumerator Interacting_Exec();

    protected abstract IEnumerator Idle_Exec();

    protected abstract IEnumerator Moving_Exec();

    protected void ChangeState(InteractableState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case InteractableState.Moving:
                StartCoroutine(Moving_Exec());
                break;
            case InteractableState.Idle:
                StartCoroutine(Idle_Exec());
                break;
            case InteractableState.Interacting:
                StartCoroutine(Interacting_Exec());
                break;
        }
    }
}
