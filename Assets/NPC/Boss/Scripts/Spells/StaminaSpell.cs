using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Spells/Stamina")]
public class StaminaSpell : SpellBase
{
    public float staminaRecoveryAmount = 40f;
    public float selfStunDuration = 1f;  // Brief vulnerability after casting

    public override void Cast()
    {
        BossController boss = castPoint.GetComponentInParent<BossController>();
        boss.RecoverStamina(staminaRecoveryAmount);
        
        Instantiate(castEffect, castPoint.position - castPoint.localPosition, Quaternion.identity);
    }
}
