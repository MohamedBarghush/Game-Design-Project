using UnityEngine;

public class Enemy : EnemyDefiner
{
    public Transform playerTransform;

    public override void TakeDamage(int damage)
    {
        // throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            // Get direction to player
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            
            // Zero out the y component to only rotate on the y-axis
            directionToPlayer.y = 0;
            
            // Only rotate if direction is not zero
            if (directionToPlayer != Vector3.zero)
            {
                // Make the enemy look at the player (only y rotation)
                transform.rotation = Quaternion.LookRotation(directionToPlayer);
            }
        }
    }
}
