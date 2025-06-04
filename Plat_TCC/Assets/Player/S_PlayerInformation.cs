using UnityEngine;

public class S_PlayerInformation : MonoBehaviour
{

    public int applesCollected;

    [SerializeField]
    private int applesLostOnDamage;

    void OnDamageTaken()
    {
        applesCollected -= applesLostOnDamage;

        if(applesCollected < 0)
        {
            //Die
        }
    }
}
