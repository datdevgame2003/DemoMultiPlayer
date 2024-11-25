
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : NetworkBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private int maxHealth = 100; 
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(100); // NetworkVariable để đồng bộ máu
    [SerializeField]
    GameObject ExplosionPrefab;
    private void Start()
    {
        if (IsOwner)
        {
            currentHealth.Value = maxHealth; 
        }

        healthSlider.maxValue = maxHealth; 
        healthSlider.value = currentHealth.Value;
        
        
        }
    

    private void Update()
    {
        if (IsOwner)
        {
            healthSlider.value = currentHealth.Value; 
        }
    }

    public void TakeDamage(int damage)
    {
        if (!IsOwner) return;

        currentHealth.Value -= damage;
        if(currentHealth.Value <= 0)
        {
           
            Die();
           
        }
        currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth);

        // Goi ServerRpc đe đong bo lai mau tren server
        UpdateHealthServerRpc(currentHealth.Value);


    }
    void Die()
    {
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        GameManager.Instance.ShowGameOverUI();
        Destroy(gameObject);
       
    }

    public void Heal(int amount)
    {
        if (!IsOwner) return;

        currentHealth.Value += amount;
        currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth); 

        // Goi ServerRpc đe đong bo lai mau tren server
        UpdateHealthServerRpc(currentHealth.Value);
    }

    // ServerRpc:update health client->server
    [ServerRpc]
    private void UpdateHealthServerRpc(int newHealth)
    {
        currentHealth.Value = newHealth;
        UpdateHealthClientRpc(newHealth);
    }

    // ClientRpc :update health -> client 
    [ClientRpc]
    private void UpdateHealthClientRpc(int newHealth)
    {
        healthSlider.value = newHealth; // Cập nhật thanh máu trên các client khác
    }
  
}
