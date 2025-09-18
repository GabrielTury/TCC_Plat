using System.Collections;
using UnityEngine;

public class S_Zipline : MonoBehaviour
{
    [SerializeField]
    private float heightOffset = 1;

    [SerializeField]
    private bool isMovable = false;

    public Vector3[] ziplinePoints { get; private set; }

    public Transform endPoint {  get; private set; }
    private void Start()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        Transform otherPoint = transform.GetChild(0);
        endPoint = otherPoint;

        Vector3[] linePositions = new Vector3[2];
        linePositions[0] = transform.position + new Vector3(0, heightOffset,0);
        linePositions[1] = otherPoint.position + new Vector3(0, heightOffset, 0);

        lineRenderer.SetPositions(linePositions);

        if (isMovable)
            StartCoroutine(UpdateLineRenderer(lineRenderer, otherPoint));

        ziplinePoints = linePositions;
    }

    private IEnumerator UpdateLineRenderer(LineRenderer lineRenderer, Transform otherTransform)
    {
        while (true)
        {
            Vector3[] linePositions = new Vector3[2];
            linePositions[0] = transform.position + new Vector3(0, heightOffset, 0);
            linePositions[1] = otherTransform.position + new Vector3(0, heightOffset, 0);

            lineRenderer.SetPositions(linePositions);
            yield return new WaitForEndOfFrame();
        }
    }
}
