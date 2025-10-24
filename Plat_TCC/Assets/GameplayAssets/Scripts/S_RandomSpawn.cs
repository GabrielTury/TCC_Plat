using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class S_RandomSpawn : MonoBehaviour
{
    [SerializeField]
    private Vector3[] locations;

#if UNITY_EDITOR
    [ContextMenu("Set Location")]
    private void SetEditorPath()
    {
        // Converter o array para uma lista temporária
        var locationList = new List<Vector3>(locations);

        // Adicionar a posição do handler à lista
        locationList.Add(transform.position);

        // Converter de volta para o array
        locations = locationList.ToArray();
    }
#endif //UNITY_EDITOR
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int index = Random.Range(0, locations.Length-1);
        transform.position = locations[index];
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Vector3.zero);
    }
}
