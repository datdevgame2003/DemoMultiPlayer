using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;
   
    private Vector2 direction;

    void Update()
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.CompareTag("Enemy"))
        {
           
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(10);  
            }

           
            Destroy(gameObject);
        }
    }

}
