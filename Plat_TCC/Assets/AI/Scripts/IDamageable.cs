using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// Method to apply damage to the object implementing this interface.
    /// </summary>
    /// <param name="damageAmount">Ammount to damage</param>
    /// <param name="animParameter">Trigger parameter from animation</param>
    /// <returns>returns whether is dead or not</returns>
    public bool TakeDamage(int damageAmount = 1, string animParameter = "Hit");
}
