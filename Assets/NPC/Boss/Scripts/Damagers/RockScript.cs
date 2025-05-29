using UnityEngine;

public class RockScript : MonoBehaviour
{
    public GameObject effect;
    public float radius = 1.0f;
    public Vector3 offset = new Vector3(0, 1f, 0);
    public int damage = 30;

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + offset, radius, LayerMask.GetMask("Player"));
        if (colliders.Length == 1) {
            Player.PlayerHealth playerHealth = colliders[0].GetComponent<Player.PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Instantiate(effect, transform.position, Quaternion.identity);
                AudioManager.Instance?.PlaySound(SoundType.Rock);
                Destroy(gameObject);
            }
        }

        Destroy(gameObject, 3f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + offset, radius);
    }
}
