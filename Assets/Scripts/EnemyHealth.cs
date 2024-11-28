using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    GameObject ExplosionPrefab;
    [SerializeField] private Slider healthSlider;//thanh mau
    private void Start()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth; //khoi tao mau ban dau: 100
        }
        if (healthSlider != null) //neu co thanh mau thi mau hien tai = mau toi da
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth.Value;
        }
        //update healthbar when value is change
        currentHealth.OnValueChanged += OnHealthChanged;
    }


    public void TakeDamage(int damage)
    {
        if (!IsServer) return; //server only manage damage

        currentHealth.Value -= damage; //tru mau theo dame
        currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth);//0 - 100

        if (currentHealth.Value <= 0) // mau hien tai <= 0 thi enemy bi chet
        {
            Die();
        }
    }

    //handle enemy die
    void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnemyKilled();
        }
        GameObject explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity); //hieu ung no khi chet

        // Get the NetworkObject component attached to the explosion prefab
        NetworkObject networkObject = explosion.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            // Spawn the effect so it gets synchronized across all clients
            networkObject.Spawn();
        }

        GetComponent<NetworkObject>().Despawn(); //huy enemy
    }
    //update giao dien when health change
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
    private void HandleDeathOnClient() //xu li quai chet tren client
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



}