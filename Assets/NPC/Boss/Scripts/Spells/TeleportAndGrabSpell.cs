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
        
        Vector3 direction = (BossController.Instance.transform.position - playerTarget.position).normalized;
        Vector3 teleportPosition = playerTarget.position + direction * grabOffset;
        Instantiate(castEffect, BossController.Instance.transform.position, Quaternion.identity);
        BossController.Instance.transform.position = teleportPosition;
        BossController.Instance.GetComponent<Animator>().applyRootMotion = true;
        BossController.Instance.GetComponent<Collider>().isTrigger = true;

        BossController.Instance.StartCoroutine(GrabPlayer());
    }

    public override float GetTotalDuration()
    {
        return 3.0f + 0.4f;
    }

    private IEnumerator GrabPlayer()
    {
        // Wait for a short duration before applying damage
        yield return new WaitForSeconds(0.2f);

        Collider[] playerCollider = Physics.OverlapBox(castPoint.position, new Vector3(grabDistance, grabDistance, grabDistance), Quaternion.identity, LayerMask.GetMask("Player"));

        if (playerCollider.Length > 0) {
            if (playerCollider[0].TryGetComponent(out PlayerManager playerManager)) {
                // if (playerTarget.GetComponentInParent<PlayerManager>().isInvulnerable) yield break;
                if (!playerManager.isInvulnerable){
                    // Apply damage to the player
                    playerManager.playerHealth.GetGrapped(damage);
                    playerManager.transform.rotation = Quaternion.LookRotation(new Vector3(BossController.Instance.transform.position.x, 0, BossController.Instance.transform.position.z) - playerCollider[0].transform.position);
                    // playerManager.transform.position = BossController.Instance.transform.position;
                    // playerTarget.GetComponentInParent<PlayerManager>().playerHealth.GetGrapped(damage);
                    // BossController.Instance.transform.position = playerCollider[0].transform.position + new Vector3(Camera.main.transform.forward.x * grabOffset, 0, Camera.main.transform.forward.z * grabOffset);
                    BossController.Instance.GetComponent<Animator>().SetTrigger("Throw");
                    delay = 3.0f;
                    // Add any additional effects or logic here
                }
            }
        }

        yield return new WaitForSeconds(delay);
        delay = 1.0f;

        BossController.Instance.GetComponent<Animator>().applyRootMotion = false;
        BossController.Instance.GetComponent<Collider>().isTrigger = false;
        Instantiate(castEffect, BossController.Instance.transform.position, Quaternion.identity);
        BossController.Instance.transform.position = startPosition;

        BossController.Instance.FinishSpell();
    }
}
