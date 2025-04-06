using UnityEngine;

public class S_ObjectIdentifier : MonoBehaviour
{
    public int id { get; private set; }

    public void SetId(int id)
    {
        this.id = id;
    }
}
