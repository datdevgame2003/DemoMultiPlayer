


using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : NetworkBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private int maxHealth = 100;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(100); //synchornize health giua server va client
    [SerializeField]
    GameObject ExplosionPrefab;
    private void Start()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }
        if (healthSlider != null) //neu co thanh mau thi mau hien tai = mau toi da
        {

            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth.Value;
        }
    }


    private void Update()
    {
        if (IsServer)
        {
            healthSlider.value = currentHealth.Value;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        currentHealth.Value -= damage;
        if (currentHealth.Value <= 0)
        {
            GameManage.Instance.ShowGameOverUI();
            Die();

        }
        currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth);

        //synchonize health -> server
        UpdateHealthServerRpc(currentHealth.Value);


    }
    void Die()
    {
        GameObject explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
       
        // Get the NetworkObject component attached to the explosion prefab
        NetworkObject networkObject = explosion.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            // Spawn the effect so it gets synchronized across all clients
            networkObject.Spawn();
        }


        Destroy(gameObject);

    }
  
 

    //public void Heal(int amount)
    //{
    //    if (!IsOwner) return;

    //    currentHealth.Value += amount;
    //    currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth);

    //ServerRpc: synchonize health -> server
    //    UpdateHealthServerRpc(currentHealth.Value);
    //}

    // ServerRpc:update health client->server
    [ServerRpc]
    private void UpdateHealthServerRpc(int newHealth)
    {
        currentHealth.Value = newHealth;
        UpdateHealthClientRpc(newHealth);
    }

    // ClientRpc :update health -> clients
    [ClientRpc]
    private void UpdateHealthClientRpc(int newHealth)
    {
        healthSlider.value = newHealth; // update healthbar -> diffirent client
    }

}