using UnityEngine;
using System.Collections;

public abstract class S_PlatformBase : S_AIBase
{

    protected enum PlatformState
    {
        Moving,
        Idle,
        Acting
    };

    protected PlatformState currentState = PlatformState.Idle;
    protected void ChangeState(PlatformState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case PlatformState.Moving:
                StartCoroutine(Moving_Exec());
                break;
            case PlatformState.Idle:
                StartCoroutine(Idle_Exec());
                break;
            case PlatformState.Acting:
                StartCoroutine(Acting_Exec());
                break;
        }
    }

    protected abstract IEnumerator Moving_Exec();

    protected abstract IEnumerator Idle_Exec();

    protected abstract IEnumerator Acting_Exec();
}
