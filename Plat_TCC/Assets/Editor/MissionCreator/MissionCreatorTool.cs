using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MissionCreatorTool: MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Insert here object populate with information")]
    public SO_MissionInfo missionObject;

    [ButtonMono("Add Active Coins")]
    public void AddCoins()
    {
        List<GameObject> coins = FindObjectsByType<GameObject>(FindObjectsSortMode.None).ToList();
        foreach (GameObject obj in coins)
        {
            if(!obj.CompareTag("Coin"))
                coins.Remove(obj);
        }

        missionObject.SetCoins(coins.ToArray());
        
    }
#endif
}
