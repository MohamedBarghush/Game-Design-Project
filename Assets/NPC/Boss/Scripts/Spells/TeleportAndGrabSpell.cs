using System.Collections;
using Player;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Spells/TeleportAndGrab")]
public class TeleportAndGrabSpell : SpellBase
{
    public float teleportDistance = 5f;
    public float grabDistance = 2f;
    public int damage = 40;
    public float grabOffset;
    public float delay = 1.0f;

    public override void Cast()
    {
        if (playerTarget == null) return;

        // Teleport the boss to a position near the player
        // startPosition = BossController.Instance.transform.position;

        AudioManager.Instance?.PlaySoundAtSrc(SoundType.Bahnas_Grab, source: audioSource, 1.0f);
        
        Vector3 direction = (BossController.Instance.transform.position - playerTarget.position).normalized;
        direction.y = 0f; // Ensure the direction is horizontal
        Vector3 offsetFromPlayer = direction * teleportDistance;
        BossController.Instance.GetComponent<Collider>().isTrigger = true;
        Vector3 teleportPosition = playerTarget.position + offsetFromPlayer;
        Instantiate(castEffect, BossController.Instance.transform.position, Quaternion.identity);
        BossController.Instance.transform.position = teleportPosition;

        BossController.Instance.isCasting = true;
        BossController.Instance.StartCoroutine(GrabPlayer());
    }

    public override float GetTotalDuration()
    {
        return 5f + 0.4f;
    }

    private IEnumerator GrabPlayer()
    {
        // Wait for a short duration before applying damage
        yield return new WaitForSeconds(0.2f);

        Collider[] playerCollider = Physics.OverlapBox(
            castPoint.position,
            new Vector3(grabDistance * 5.0f, grabDistance, grabDistance * 20.0f),
            Quaternion.identity,
            LayerMask.GetMask("Player")
        );

        if (playerCollider.Length > 0) {
            if (playerCollider[0].TryGetComponent(out PlayerManager playerManager)) {
                // if (playerTarget.GetComponentInParent<PlayerManager>().isInvulnerable) yield break;
                if (!playerManager.isInvulnerable){
                    // Apply damage to the player
                    BossController.Instance.transform.rotation = Quaternion.LookRotation(new Vector3(playerManager.transform.position.x, 0, playerManager.transform.position.z) - playerCollider[0].transform.position);
                    BossController.Instance.transform.position = playerCollider[0].transform.position + 
                                                                new Vector3(BossController.Instance.transform.forward.x * teleportDistance, 
                                                                0.0f, 
                                                                BossController.Instance.transform.forward.z * teleportDistance);
                    BossController.Instance.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                    playerManager.playerHealth.GetGrapped(damage);
                    // playerManager.transform.rotation = Quaternion.LookRotation(new Vector3(BossController.Instance.transform.position.x, 0, BossController.Instance.transform.position.z) - playerCollider[0].transform.position);
                    // BossController.Instance.transform.position = playerManager.transform.position + (playerManager.transform.forward * grabOffset);
                    // playerTarget.GetComponentInParent<PlayerManager>().playerHealth.GetGrapped(damage);
                    // BossController.Instance.transform.position = playerCollider[0].transform.position + new Vector3(Camera.main.transform.forward.x * grabOffset, 0, Camera.main.transform.forward.z * grabOffset);
                    BossController.Instance.GetComponent<Animator>().SetBool("Attacking", true);
                    BossController.Instance.GetComponent<Animator>().SetTrigger("Throw");
                    delay = 5.0f;
                    // Add any additional effects or logic here
                }
            }
        }

        yield return new WaitForSeconds(delay);
        delay = 1.0f;

        BossController.Instance.GetComponent<Animator>().SetBool("Attacking", false);
        BossController.Instance.GetComponent<Collider>().isTrigger = false;
        Instantiate(castEffect, BossController.Instance.transform.position, Quaternion.identity);
        BossController.Instance.transform.position = startPosition;

        BossController.Instance.FinishSpell();
    }
}
