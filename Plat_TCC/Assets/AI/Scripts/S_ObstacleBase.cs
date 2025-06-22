using System;
using System.Collections;
using UnityEngine;

public abstract class S_ObstacleBase : S_AIBase
{
    protected enum ObstacleState
    {
        Moving,
        Idle,
        Stunned,
        Dying
    };

    protected ObstacleState currentState = ObstacleState.Idle;
    protected Coroutine currentCoroutine = null;

    protected void ChangeState(ObstacleState nextState)
    {
        currentState = nextState;
        if(currentCoroutine != null)
            StopCoroutine(currentCoroutine); // Stop the current coroutine if it exists

        switch (nextState)
        {
            case ObstacleState.Moving:
                currentCoroutine = StartCoroutine(Moving_Exec());
                break;
            case ObstacleState.Idle:
                currentCoroutine = StartCoroutine(Idle_Exec());
                break;
            case ObstacleState.Stunned:
                currentCoroutine = StartCoroutine(Stunned_Exec());
                break;
            case ObstacleState.Dying:
                currentCoroutine = StartCoroutine(Dying_Exec());
                break;
        }
    }

    protected abstract IEnumerator Moving_Exec();

    protected abstract IEnumerator Idle_Exec();

    protected abstract IEnumerator Stunned_Exec();

    protected abstract IEnumerator Dying_Exec();

}
