using System.Collections;
using UnityEngine;
public class Npc : MonoBehaviour
{
    //User-defined settings in Unity inspector.
    public float speed = 0.3f;
    public int hitPoints = 3;
    public float bodyDespawnDelay = 5f;
    
    //Local variables.
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private bool _isDead;
    private Renderer _npcRenderer;
    private Color _initialColor;
    private Rigidbody _npcRb;
    private Headquarters _parentHq;

    //Sets the parentHq
    public Headquarters ParentHq
    {
        set => _parentHq = value;
    }
    private void Start()
    {
        _npcRenderer = gameObject.GetComponent<Renderer>();
        _initialColor = _npcRenderer.material.color;
        _npcRb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!_isDead)
        {
            transform.Translate(speed * Time.deltaTime * Vector3.forward);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //If collider is not a bullet, exits
        if (!other.gameObject.CompareTag("Bullet")) return;
        
        hitPoints--;
        if (hitPoints == 0) //Dies if hp reaches 0.
        {
            _isDead = true;
            DeathAnimation();
        }
        else //Still alive, plays hit animation.
        {
            StartCoroutine(HitAnimation());
        }
    }

    private void DeathAnimation()
    {
        //Stops any routines running (Eg.:Hit animations).
        StopAllCoroutines();
        //Changes color to red.
        _npcRenderer.material.SetColor(Color1,Color.red);
        //Turns off constraint on rotations.
        _npcRb.constraints = RigidbodyConstraints.None;
        //Pushes Npc back onto the floor.
        _npcRb.AddForce(-transform.forward*10f);
        //Invokes the DestroyBody function after a set bodySpawn delay to remove the body.
        Invoke(nameof(DestroyBody), bodyDespawnDelay);
    }

    //Method to remove body from scene.
    private void DestroyBody()
    {
        Destroy(gameObject);
        // _parentHq.DecrementCounter();
    }

    //Method to play hit animation (flash red twice).
    private IEnumerator HitAnimation()
    {
        //Flashes red twice.
        for (var i = 0; i < 2; i++)
        {
            //Sets color to red.
            _npcRenderer.material.color = Color.red;
            yield return new WaitForSeconds(.2f);
            
            //After 0.2 seconds, if character is dead, exits out the for loop and, consequently, the method.
            if (_isDead) break;
            
            //If character still alive, returns back to the initial color and waits 0.2 seconds again.
            _npcRenderer.material.color = _initialColor;
            yield return new WaitForSeconds(.2f);
        }
    }
}