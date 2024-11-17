
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : NetworkBehaviour
{
    public Slider healthSlider;
    public float maxHealth = 100f;
    private float currentHealth;

    // Cập nhật khi mất máu
    public void TakeDamage(float damage)
    {
        if (!IsOwner) return; // Chỉ giảm máu cho owner

        currentHealth -= damage;

        // Cập nhật thanh máu
        healthSlider.value = currentHealth / maxHealth;

        // Nếu máu <= 0, bạn có thể thực hiện hành động như chết
        if (currentHealth <= 0)
        {
            Die();
        }

        // Đồng bộ lại máu cho tất cả các client
        UpdateHealthServerRpc(currentHealth);
    }

    // ServerRpc để đồng bộ hóa máu
    [ServerRpc]
    void UpdateHealthServerRpc(float newHealth)
    {
        currentHealth = newHealth;
        UpdateHealthClientRpc(currentHealth);
    }

    // ClientRpc để đồng bộ hóa máu cho tất cả client
    [ClientRpc]
    void UpdateHealthClientRpc(float newHealth)
    {
        if (!IsOwner)
        {
            currentHealth = newHealth;
            healthSlider.value = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        // Thực hiện hành động khi player chết (tắt nhân vật, respawn, v.v.)
        Debug.Log("Player died!");
    }

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }
}
