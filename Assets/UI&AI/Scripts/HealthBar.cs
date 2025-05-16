using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Image hpImage;           // Main health bar (immediate changes)
    public Image delayedHpImage;    // Delayed health bar
    public float maxHP = 100f;      // Maximum health value
    public float delayDuration = 0.3f; // Time before delayed bar follows
    public float catchUpSpeed = 30f; // HP/second for delayed bar movement

    private float currentHP;        // Actual current health
    private float delayedHP;        // Delayed health display
    private float lastDamageTime;   // Time of last HP decrease

    void Start()
    {
        currentHP = maxHP;
        delayedHP = maxHP;
        UpdateBars();
    }

    void Update()
    {
        HandleDelayedBar();
    }

    // Call this method from other scripts to update health
    public void SetHealth(float newHealth)
    {
        newHealth = Mathf.Clamp(newHealth, 0f, maxHP);
        
        if (newHealth < currentHP)
        {
            // HP decreased - start delay timer
            lastDamageTime = Time.time;
        }
        else if (newHealth > delayedHP)
        {
            // HP increased - update both bars immediately
            delayedHP = newHealth;
            UpdateBars();
        }

        currentHP = newHealth;
        hpImage.fillAmount = currentHP / maxHP;
    }

    private void HandleDelayedBar()
    {
        if (delayedHP > currentHP)
        {
            if (Time.time >= lastDamageTime + delayDuration)
            {
                // Move delayed bar toward current HP
                delayedHP = Mathf.MoveTowards(delayedHP, currentHP, catchUpSpeed * Time.deltaTime);
                delayedHpImage.fillAmount = delayedHP / maxHP;
            }
        }
        else if (delayedHP < currentHP)
        {
            // Ensure immediate update for healing
            delayedHP = currentHP;
            delayedHpImage.fillAmount = delayedHP / maxHP;
        }
    }

    private void UpdateBars()
    {
        hpImage.fillAmount = currentHP / maxHP;
        delayedHpImage.fillAmount = delayedHP / maxHP;
    }
}