using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class S_Goomba : S_ObstacleBase
{
    private NavMeshAgent nav;

    [SerializeField]
    private Vector3[] path;

    [SerializeField, Header("Editor Only"), Space(2)]
    private GameObject handler;

    private int currentPathIndex = 0;



    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }
    void Start()
    {        
        ChangeState(ObstacleState.Moving); //Sets default state
    }

    #region State Coroutines
    protected override IEnumerator Dying_Exec()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator Moving_Exec()
    {
        //Code to entry
        nav.destination = path[currentPathIndex];

        while (currentState == ObstacleState.Moving)
        {
            if (!nav.pathPending &&nav.remainingDistance < 0.5f)
            {
               
                currentPathIndex++;

                if (currentPathIndex >= path.Length)
                    currentPathIndex = 0;

                nav.destination = path[currentPathIndex];
            }
            //this ensure it runs on the fixed update loop
            yield return new WaitForFixedUpdate();
        }

        //Code to exit
    }

    protected override IEnumerator Idle_Exec()
    {
        yield return null;
    }

    protected override IEnumerator Stunned_Exec()
    {
        throw new System.NotImplementedException();
    }
    #endregion //StateCoroutines

    [ContextMenu("Set Path")]
    private void SetEditorPath()
    {
        // Converter o array para uma lista temporária
        var pathList = new List<Vector3>(path);

        // Adicionar a posição do handler à lista
        pathList.Add(handler.transform.position);

        // Converter de volta para o array
        path = pathList.ToArray();
    }

    private void OnDrawGizmos()
    {
        if (path == null || path.Length == 0)
            return;

        // Desenhar os pontos do caminho
        Gizmos.color = Color.red;
        for (int i = 0; i < path.Length; i++)
        {
            Gizmos.DrawSphere(path[i], 0.2f);

            // Conectar os pontos com linhas
            if (i < path.Length - 1)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }

        // Conectar o último ponto ao primeiro (opcional, se for um caminho circular)
        Gizmos.color = Color.green;
        Gizmos.DrawLine(path[path.Length - 1], path[0]);
    }
}
