using UnityEngine;

public class Player : MonoBehaviour
{
    //User-defined settings in the Unity inspector.
    //Movement.
    public float rotationSpeed = 1.25f;
    public float moveSpeed = 5.0f;
    public float jumpForce = 7.5f;
    //Bullet.
    public GameObject bulletPrefab;
    public float shootForce = 500f;
    
    //Local variables.
    private Rigidbody _npcRb;
    private Vector3 _desiredRotation;
    private RaycastHit _hit;
    private float _currentRotation;
    private bool _isGrounded;
    

    // Start is called before the first frame update.
    private void Start()
    {
        _npcRb = gameObject.GetComponent<Rigidbody>();
        _desiredRotation = transform.eulerAngles;
    }

    // Update is called once per frame.
    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the target rotation angle based on the horizontal input.
        _currentRotation += horizontalInput * rotationSpeed;

        // Calculate the movement direction based on the current rotation.
        Vector3 moveDirection = Quaternion.Euler(0, _currentRotation, 0) * Vector3.forward * verticalInput;

        // Apply the movement and rotation to the player.
        _npcRb.velocity = new Vector3(moveDirection.x * moveSpeed, _npcRb.velocity.y, moveDirection.z * moveSpeed);
        transform.rotation = Quaternion.Euler(0, _currentRotation, 0);
        
        //Jumping.
        if (Input.GetButtonDown("Jump"))
        {
            _npcRb.velocity = new Vector3(_npcRb.velocity.x, jumpForce, _npcRb.velocity.z);
        }
        
        //Shooting Bullets.
        if (Input.GetButtonDown("Fire1"))
        {
            //Calculates the offset to spawn outside (in front of) the collider of the player.
            Vector3 playerForward = _npcRb.transform.forward;
            Vector3 offset = new Vector3(playerForward.x*0.5f, 0.3f, playerForward.z * 0.5f); 
            
            // Create a new sphere GameObject on the position of the player with the offset calculated above.
            GameObject bullet = Instantiate(bulletPrefab, _npcRb.position+offset, transform.rotation);

            // Add a Rigidbody component to the sphere, sets the collision to continuous to avoid missing a collision.
            Rigidbody bulletRb = bullet.AddComponent<Rigidbody>();
            bulletRb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // Add a force to the sphere to make it shoot forward.
            bulletRb.AddForce( transform.forward * shootForce);
        }

    }
    
}
