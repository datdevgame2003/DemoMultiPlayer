using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 5f;
    [SerializeField]
  
    private Vector2 direction;
    void Start()
    {
        
    }


    void Update()
    {

        transform.Translate(direction * moveSpeed * Time.deltaTime);

    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }

}

