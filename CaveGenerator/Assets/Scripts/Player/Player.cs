using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody PlayerRigidbody { get; set; }
    private Vector3 Velocity { get; set; }

    [SerializeField]
    private int speed;

    private void Start()
    {
        PlayerRigidbody = GetComponent<Rigidbody>();
        
    }

    private void Update()
    {
        Velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * speed;
    }

    private void FixedUpdate()
    {
        PlayerRigidbody.MovePosition(PlayerRigidbody.position + Velocity * Time.fixedDeltaTime);
    }
}
