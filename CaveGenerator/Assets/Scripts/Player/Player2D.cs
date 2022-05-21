using UnityEngine;

public class Player2D : MonoBehaviour
{
    private Rigidbody2D PlayerRigidbody { get; set; }
    private Vector2 Velocity { get; set; }

    [SerializeField]
    private int speed;

    private void Start()
    {
        PlayerRigidbody = GetComponent<Rigidbody2D>();
        
    }

    private void Update()
    {
        Velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * speed;
    }

    private void FixedUpdate()
    {
        PlayerRigidbody.MovePosition(PlayerRigidbody.position + Velocity * Time.fixedDeltaTime);
    }
}
