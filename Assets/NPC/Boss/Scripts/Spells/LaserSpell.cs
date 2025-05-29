using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Boss/Spells/Laser")]
public class LaserSpell : SpellBase
{
    public float laserDuration = 2f;
    public LineRenderer laserPrefab;
    public int damage = 50;
    public bool damaged = false;

    private Vector3 playerShootPos;

    public override void Cast()
    {
        AudioManager.Instance?.PlaySoundAtSrc(SoundType.Bahnas_Laser, source: audioSource, 1.0f);
        playerShootPos = playerTarget.position;
        damaged = false;
        if (castEffect != null)
            Instantiate(castEffect, castPoint.position, Quaternion.identity);
        BossController.Instance.StartCoroutine(LaserRoutine());
    }

    private IEnumerator LaserRoutine()
    {
        LineRenderer laser = Instantiate(laserPrefab);
        float timer = 0f;

        while (timer < laserDuration)
        {
            AudioManager.Instance?.PlaySound(SoundType.Laser, 0.5f);
            laser.SetPosition(0, castPoint.position);
            laser.SetPosition(1, playerShootPos);
            
            if (!damaged) {
                if (Physics.Raycast(castPoint.position, (playerShootPos - castPoint.position).normalized, out RaycastHit hit, 200, playerLayer))
                {
                    hit.collider.GetComponent<Player.PlayerHealth>().TakeDamage(damage);
                    damaged = true;
                }
            }
            
            timer += Time.deltaTime;
            yield return null;
        }
        
        Destroy(laser.gameObject);
    }
}