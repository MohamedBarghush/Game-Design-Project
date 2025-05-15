using UnityEngine;

[CreateAssetMenu(fileName = "Combo Attack", menuName = "ScriptableObjects/Attacks", order = 1)]
public class AttackOverrideSO : ScriptableObject
{
    public AnimatorOverrideController animatorOverrideController;
    public float exitTime = 0.8f;
    public int damage;
}
