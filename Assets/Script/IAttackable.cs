using UnityEngine;

public interface IAttackable
{
    // Respond to an Attack
    // Returns true if the IAttackable object died, false otherwise
    public bool ReceiveAttack(Attack attack);

    //used to make sure the target is being faced (might be removed later)
    public Vector3 GetPosition();
}

public interface IAttackableByPlayer : IAttackable
{

}

public interface IAttackableByEnemy : IAttackable
{

}

public struct Attack
{
    public enum AttackType
    {
        Sword,
        Spear,
        Axe,
        Mace,
        Bow,
        Slashing,
        Piercing,
        Blunt,
        Magic,
        Other,
        Effect
    }

    public enum AttackEffect
    {
        None,
        Entangle,
        Confuse
    }

    public float Value;
    public AttackType Type;
    public AttackEffect Effect;
}
