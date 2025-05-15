using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Spells/Rock")]
public class RockSpell : SpellBase
{
    public float projectileSpeed = 10f;
    public GameObject projectilePrefab;

    public override void Cast()
    {
        if (playerTarget == null) return;

        Vector3 direction = (playerTarget.position - castPoint.position).normalized;
        GameObject rock = Instantiate(projectilePrefab, castPoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = rock.GetComponent<Rigidbody>();
        rb.AddForce(direction * projectileSpeed, ForceMode.Impulse);
        rock.GetComponent<RockScript>().effect = castEffect;
    }

    // public override float GetTotalDuration()
    // {
    //     return 1.5f; // Duration of rock throw animation
    // }
}