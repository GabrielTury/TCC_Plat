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

    protected ObstacleState currentState;

    protected void ChangeState(ObstacleState nextState)
    {
        switch (nextState)
        {
            case ObstacleState.Moving:
                StartCoroutine(Moving_Exec());
                break;
            case ObstacleState.Idle:
                StartCoroutine(Idle_Exec());
                break;
            case ObstacleState.Stunned:
                StartCoroutine(Stunned_Exec());
                break;
            case ObstacleState.Dying:
                StartCoroutine(Dying_Exec());
                break;
        }
    }

    protected abstract IEnumerator Moving_Exec();

    protected abstract IEnumerator Idle_Exec();

    protected abstract IEnumerator Stunned_Exec();

    protected abstract IEnumerator Dying_Exec();

}
