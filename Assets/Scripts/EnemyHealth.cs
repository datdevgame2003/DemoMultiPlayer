using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(100);
    [SerializeField]
    GameObject ExplosionPrefab;
    [SerializeField] private Slider healthSlider;
    private void Start()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth.Value;

        currentHealth.OnValueChanged += OnHealthChanged;
    }


    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        currentHealth.Value -= damage;
        currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth);

        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    // Hủy đối tượng khi chết
    void Die()
    {
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        healthSlider.value = newHealth;
    }

}