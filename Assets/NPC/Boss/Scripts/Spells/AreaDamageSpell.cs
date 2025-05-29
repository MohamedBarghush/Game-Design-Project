using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Spells/AreaDamage")]
public class AreaDamageSpell : SpellBase
{
    public int damage = 50;
    public float radius = 5f;
    public int repeatCount = 3;
    public float inBetweenTime = 1.5f;
    public float warningTime = 1f;
    public GameObject projectilePrefab;
    public GameObject warningEffectPrefab;

    public override void Cast()
    {
        if (playerTarget == null) return;

        AudioManager.Instance?.PlaySoundAtSrc(SoundType.Bahnas_Ice, source: audioSource, 1.0f);
        
        // Start the casting sequence with repeating damage
        BossController.Instance.StartCoroutine(CastSequence());
    }

    private IEnumerator CastSequence()
    {
        Instantiate(castEffect, castPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < repeatCount; i++)
        {
            GameObject warning = Instantiate(warningEffectPrefab, playerTarget.position, Quaternion.identity);
            warning.transform.localScale = new Vector3(radius, radius*20, radius);
            yield return new WaitForSeconds(warningTime);
            // Instantiate effect at player's position
            if (playerTarget != null)
            {
                GameObject effect = Instantiate(projectilePrefab, warning.transform.position, Quaternion.identity);
                effect.GetComponent<AreaEffectScript>().damageAmount = damage;
                effect.GetComponent<AreaEffectScript>().damageRadius = radius;
            }
            Destroy(warning);
            
            yield return new WaitForSeconds(inBetweenTime);
        }
    }
}
