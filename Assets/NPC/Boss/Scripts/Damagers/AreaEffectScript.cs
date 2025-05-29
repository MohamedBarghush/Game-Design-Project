using System.Collections;
using UnityEngine;

public class AreaEffectScript : MonoBehaviour
{
    public int damageAmount = 10;
    public float damageRadius = 4.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(AreaEffect());
    }

    private IEnumerator AreaEffect()
    {
        yield return new WaitForSeconds(0.3f);
        AudioManager.Instance?.PlaySound(SoundType.Ice);
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius, LayerMask.GetMask("Player"));
        if (colliders.Length == 1)
        {
            Player.PlayerHealth playerHealth = colliders[0].GetComponent<Player.PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
