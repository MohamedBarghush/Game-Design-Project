using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Spells/Heal")]
public class HealSpell : SpellBase
{
    public float healAmount = 200f;

    public override void Cast()
    {
        BossController boss = castPoint.GetComponentInParent<BossController>();
        boss.RecoverHealth(healAmount);
        Instantiate(castEffect, castPoint.position - castPoint.localPosition, Quaternion.identity);
    }
}