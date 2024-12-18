﻿using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 4f;
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
    [SerializeField] private int gold = 0;

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
                RequestJumpServerRpc(); // jump -> server(yeu cau player nhay tu client)
            }

            // shoot
            if (Input.GetButtonDown("Fire2"))
            {
                RequestShootServerRpc(isFacingRight); // shoot -> server(player ban tu client)
            }

            // Dong bo vị trí va trang thai flip client len Server
            if (NetworkManager.Singleton.IsClient)
            {
                SyncPlayerPosServerRpc(transform.position, isFacingRight, move);
            }
        }
        else
        {
            // Nhan vi tri va flip tu Server (cho cac client)
            transform.position = otherPos;
            anim.SetFloat("move", move);
            flip();
        }
    }

   
    void flip()
    {
        if (IsOwner) //kiem tra chu so huu cua client ,dao nguoc vertical cua nhan vat khi thay doi
        {
            if (isFacingRight && left_right < 0 || !isFacingRight && left_right > 0)
            {
                isFacingRight = !isFacingRight;
                Vector3 size = transform.localScale;
                size.x = size.x * -1;
                transform.localScale = size;

               
            }
        }
        else //cap nhat chieu rong nhan vat tu server
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
    public void AddGold(int amount) //them vang trong tong vang
    {
        gold += amount;
        Debug.Log("Gold collected: " + gold);
       
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

        // Sau khi đong bo tu client, ClientRpc cap nhat tu server cho cac client khac
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
    void RequestShootServerRpc(bool facingRight) //goi shoot tu client
    {
        ShootClientRpc(facingRight);
    }

    [ClientRpc]
    void ShootClientRpc(bool facingRight) //cap nhat trang thai cua client
    {
        
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Vector2 shootDirection = facingRight ? Vector2.right : Vector2.left;
        bullet.GetComponent<BulletController>().SetDirection(shootDirection);
        shootingSound.clip = listAudios[0];
        shootingSound.Play();
        Destroy(bullet, 1.5f);
    }

    
    [ServerRpc]
    void RequestJumpServerRpc() //yeu cau nhay tu client
    {
        JumpClientRpc(); //can jump
    }

    
    [ClientRpc]
    void JumpClientRpc()
    {
        if (allowJump)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); //luc nhay
            allowJump = false; // không bi nhay lien tuc
        }
    }
   
  
}
