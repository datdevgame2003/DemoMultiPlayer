
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
    [Header("Audio Settings")]
    [SerializeField] private AudioClip hitSound; 
    private AudioSource audioSource;
    public GameObject enemy;
    private void Start()
    {
        if (IsServer) //kiem tra chay tren server
        {
            currentHealth.Value = maxHealth;
        }
        if (healthSlider != null) //neu co thanh mau thi mau hien tai = mau toi da
        {

            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth.Value;
        }
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }


    private void Update()
    {
        if (IsServer)
        {
            healthSlider.value = currentHealth.Value; //cap nhat gia tri thanh mau voi health present,dong bo voi clients
        }
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        currentHealth.Value -= damage;
        PlayHitSound();
        if (currentHealth.Value <= 0)
        {
            GameManage.Instance.ShowGameOverUI();
            Die();
            Destroy(enemy);
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
            networkObject.Spawn();//dong bo hieu ung no giua cac client
        }

        Destroy(gameObject);

    }

        // ServerRpc:goi tu server to update health player
        [ServerRpc]
    private void UpdateHealthServerRpc(int newHealth)
    {
        currentHealth.Value = newHealth;
        UpdateHealthClientRpc(newHealth);//dong bo mau ->clients
    }

    // ClientRpc :goi tu server to update health -> clients
    [ClientRpc]
    private void UpdateHealthClientRpc(int newHealth)
    {
        healthSlider.value = newHealth; // update healthbar -> diffirent clients
    }

   
    private void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound); 
        }
    }

}