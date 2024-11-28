using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    GameObject ExplosionPrefab;
    [SerializeField] private Slider healthSlider;
    private void Start()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth.Value;
        }
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


    void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnemyKilled();
        }
        GameObject explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        NetworkObject networkObject = explosion.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
        Destroy(gameObject);
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = newHealth;
        }
        if (newHealth <= 0)
        {
            HandleDeathOnClient(); // die in client
        }
    }
    private void HandleDeathOnClient()
    {
        GameObject explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        NetworkObject networkObject = explosion.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
        Destroy(gameObject);
    }



}