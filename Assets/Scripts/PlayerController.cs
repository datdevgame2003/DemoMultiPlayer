using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 2.5f;
    private Vector3 otherPos;
    public bool isFacingRight = true;
    public float left_right;
    private Animator anim;
    private Rigidbody2D rb;
    private float move;
    public bool allowJump;
    public float jumpForce;
    [SerializeField]
    Transform firePoint;
    [SerializeField]
    GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    [SerializeField]
    AudioSource shootingSound;

    [SerializeField]
    List<AudioClip> listAudios;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        shootingSound.Stop();

    }

    void Update()
    {
        if (IsOwner)
        {
            // Di chuyen
            left_right = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(left_right * moveSpeed, rb.velocity.y);
            flip();

            move = Mathf.Abs(left_right);
            anim.SetFloat("move", move);

            // jump
            if (Input.GetKeyDown(KeyCode.Space) && allowJump)
            {
                RequestJumpServerRpc(); // jump -> server
            }

            // shoot
            if (Input.GetButtonDown("Fire2"))
            {
                RequestShootServerRpc(isFacingRight); // shoot -> server
            }

            // Dong bo vị trí va trang thai flip len Server
            if (NetworkManager.Singleton.IsClient)
            {
                SyncPlayerPosServerRpc(transform.position, isFacingRight, move);
            }
        }
        else
        {
            // Nhan vi tri va trang thai flip tu Server (cho cac client)
            transform.position = otherPos;
            anim.SetFloat("move", move);
            flip();
        }
    }

   
    void flip()
    {
        if (IsOwner)
        {
            if (isFacingRight && left_right < 0 || !isFacingRight && left_right > 0)
            {
                isFacingRight = !isFacingRight;
                Vector3 size = transform.localScale;
                size.x = size.x * -1;
                transform.localScale = size;

               
            }
        }
        else
        {
            if (isFacingRight)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D otherhitbox)
    {
        if (otherhitbox.gameObject.tag == "ground")
        {
            allowJump = true;
        }
    }

    private void OnTriggerExit2D(Collider2D otherhitbox)
    {
        if (otherhitbox.gameObject.tag == "ground")
        {
            allowJump = false;
        }
    }

    // ServerRpc: synchonize postion, flip status client -> server
    [ServerRpc]
    void SyncPlayerPosServerRpc(Vector3 pos, bool facingRight, float move)
    {
        otherPos = pos;
        isFacingRight = facingRight;
        this.move = move;

        // Sau khi đong bo tu client, goi ClientRpc đe cap nhat cac client khac
        SyncPlayerPosClientRpc(pos, facingRight, move);
    }

   
    [ClientRpc]
    void SyncPlayerPosClientRpc(Vector3 pos, bool facingRight, float move)
    {
        if (!IsOwner)
        {
            otherPos = pos;
            isFacingRight = facingRight;
            this.move = move;
            anim.SetFloat("move", move);
            flip();
        }
    }

    
    [ServerRpc(RequireOwnership = false)]
    void RequestShootServerRpc(bool facingRight)
    {
        ShootClientRpc(facingRight);
    }

    [ClientRpc]
    void ShootClientRpc(bool facingRight)
    {
        
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Vector2 shootDirection = facingRight ? Vector2.right : Vector2.left;
        bullet.GetComponent<BulletController>().SetDirection(shootDirection);
        shootingSound.Play();
        Destroy(bullet, 1.5f);
    }

    
    [ServerRpc]
    void RequestJumpServerRpc()
    {
        JumpClientRpc(); 
    }

    
    [ClientRpc]
    void JumpClientRpc()
    {
        if (allowJump)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            allowJump = false; // không bi nhay lien tuc
        }
    }
   
  
}
