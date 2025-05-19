using UnityEngine;

public class RockScript : MonoBehaviour
{
    public GameObject effect;

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.0f, LayerMask.GetMask("Player"));
        if (colliders.Length == 1) {
            Player.PlayerHealth playerHealth = colliders[0].GetComponent<Player.PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(50);
                Instantiate(effect, transform.position, Quaternion.identity);
                AudioManager.Instance.PlaySound(SoundType.Rock);
                Destroy(gameObject);
            }
        }

        Destroy(gameObject, 3f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.0f);
    }
}
