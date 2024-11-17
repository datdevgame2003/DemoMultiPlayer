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

      
    }

    void Update()
    {
        if (IsOwner)
        {
            // Di chuyển
            left_right = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(left_right * moveSpeed, rb.velocity.y);
            flip();

            move = Mathf.Abs(left_right);
            anim.SetFloat("move", move);

            // Nhảy
            if (Input.GetKeyDown(KeyCode.Space) && allowJump)
            {
                RequestJumpServerRpc(); // Gửi yêu cầu nhảy lên server
            }

            // Bắn đạn
            if (Input.GetButtonDown("Fire2"))
            {
                RequestShootServerRpc(isFacingRight); // Gửi yêu cầu bắn đến server
            }

            // Đồng bộ vị trí và trạng thái flip lên Server
            if (NetworkManager.Singleton.IsClient)
            {
                SyncPlayerPosServerRpc(transform.position, isFacingRight, move);
            }
        }
        else
        {
            // Nhận vị trí và trạng thái flip từ Server (cho các client)
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

    // ServerRpc: Đồng bộ vị trí và trạng thái flip từ client lên server
    [ServerRpc]
    void SyncPlayerPosServerRpc(Vector3 pos, bool facingRight, float move)
    {
        otherPos = pos;
        isFacingRight = facingRight;
        this.move = move;

        // Sau khi đồng bộ từ client, gọi ClientRpc để cập nhật các client khác
        SyncPlayerPosClientRpc(pos, facingRight, move);
    }

    // ClientRpc: Cập nhật vị trí và trạng thái flip cho tất cả các client
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

    // ServerRpc: Đồng bộ yêu cầu bắn đạn từ client lên server
    [ServerRpc(RequireOwnership = false)]
    void RequestShootServerRpc(bool facingRight)
    {
        ShootClientRpc(facingRight);
    }

    // ClientRpc: Đồng bộ hành động bắn đạn cho tất cả các client
    [ClientRpc]
    void ShootClientRpc(bool facingRight)
    {
        // Tạo viên đạn tại vị trí firePoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Xác định hướng bắn dựa trên facingRight
        Vector2 shootDirection = facingRight ? Vector2.right : Vector2.left;
        bullet.GetComponent<BulletController>().SetDirection(shootDirection);
        shootingSound = GetComponent<AudioSource>();
        shootingSound.clip = listAudios[0];
        shootingSound.Play();
        Destroy(bullet, 1.5f);
    }

    // ServerRpc: Đồng bộ yêu cầu nhảy từ client lên server
    [ServerRpc]
    void RequestJumpServerRpc()
    {
        JumpClientRpc(); // Cập nhật nhảy cho tất cả các client
    }

    // ClientRpc: Đồng bộ hành động nhảy cho tất cả các client
    [ClientRpc]
    void JumpClientRpc()
    {
        if (allowJump)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            allowJump = false; // Đặt lại allowJump sau khi nhảy để không bị nhảy liên tục
        }
    }
   
  
}
