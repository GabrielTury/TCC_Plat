using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class S_HookBird : S_PlatformBase
{

    [SerializeField]
    private float speed;

    [SerializeField]
    private float angularSpeed;

    [SerializeField]
    private Vector3[] path;

    private int currentPathIndex = 0;

#if UNITY_EDITOR
    [SerializeField, Header("Editor")]
    private GameObject pathEditor;
#endif

    private void Start()
    {
        ChangeState(PlatformState.Moving); //Sets default state
    }

    #region State Coroutines
    protected override IEnumerator Acting_Exec()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator Idle_Exec()
    {
        yield return null;
    }

    protected override IEnumerator Moving_Exec()
    {
        while(currentState == PlatformState.Moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, path[currentPathIndex], Time.fixedDeltaTime * speed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(path[currentPathIndex] - transform.position), Time.fixedDeltaTime * angularSpeed);

            if (Vector3.Distance(transform.position, path[currentPathIndex]) < 0.5f)
            {
                currentPathIndex++;
                if (currentPathIndex >= path.Length)
                    currentPathIndex = 0;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion //State Coroutines

    [ContextMenu("Set Path")]
    private void SetEditorPath()
    {
        // Converter o array para uma lista temporária
        var pathList = new List<Vector3>(path);

        // Adicionar a posição do handler à lista
        pathList.Add(pathEditor.transform.position);

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
