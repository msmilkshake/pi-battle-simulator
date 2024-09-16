using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Soldier : MonoBehaviour, IEngageable
{
    // Static Distribution Generators
    public static GaussianDistribution accuracyGenerator =
        new(0, 2.25f, -7f, 7f);

    public static GaussianBimodalDistribution defenseWalkGenerator =
        new(-1f, 0.5f, 4f, 0.75f, 0.125f);

    public static GaussianDistribution attackWalkGenerator = new();

    public static MixedExpDistribution engageDelay = new();

    // Static stats
    private static float[] _accuracies = { 1.0f, 0.7f, 0.4f, 0.25f, 0.1f };
    private static float[] _ranges = { 20f, 25f, 30f, 35f, 40f };
    private static float[] _bulletTravelTime = { 0f, 0.5f, 1f, 1.5f, 2f };


    // Game Over Variable
    public bool GameOver { get; set; } = false;

    // IEngageable interface
    public Transform ObjectTransform => transform;

    // User-defined settings in Unity inspector.
    public int hitPoints = 3;
    public float bodyDespawnDelay = 5f;
    public Healthbar _healthbar;
    private float _currentHealth;

    // Bullet.
    public GameObject bulletPrefab;
    public float shootForce = 500f;
    public float shootCooldownSeconds = 0.5f;
    public float engageCooldownSeconds;

    // Local variables.
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private bool _isDead;
    private Renderer _soldierRenderer;
    private Color _initialColor;
    private Rigidbody _soldierRb;
    private Headquarters _parentHq;

    private GameObject _enemyHq;

    // Movement.
    public static float _proximityFactor = 75f;
    private static long _idGen;
    private float _waypointRange;
    private NavMeshAgent _agent;
    private readonly long _id = _idGen++;

    private bool _isEngaging = false;
    private bool _isShooting = false;
    private IEngageable _engagedEnemy = null;
    private bool _isOnEngageCooldown = false;
    private Quaternion rotationBeforeEngaging = Quaternion.identity;
    private bool _isEngagingHQ;

    private List<GameObject> enemiesInRange = new();

    private int _level;
    private float _accModifier;
    private float _range;

    public Headquarters ParentHq
    {
        set => _parentHq = value;
    }

    public GameObject EnemyHq
    {
        set => _enemyHq = value;
    }

    void Start()
    {
        name += " #" + _id;
        _waypointRange = Random.Range(7.5f, 15f);
        Vector3 initialDestination = transform.position + transform.forward * _waypointRange;
        _agent = GetComponent<NavMeshAgent>();
        _agent.SetDestination(initialDestination);

        _soldierRenderer = transform.GetChild(0).GetComponent<Renderer>();
        _initialColor = _soldierRenderer.material.color;
        _soldierRb = transform.GetChild(0).GetComponent<Rigidbody>();

        shootCooldownSeconds = Random.Range(1.5f, 3.5f);
        engageCooldownSeconds = Random.Range(1.0f, 2.0f);

        _currentHealth = hitPoints;
        _healthbar.UpdateHealthbar(hitPoints, _currentHealth);
    }

    void Update()
    {
        if (GameOver)
        {
            return;
        }

        if (_enemyHq == null || _enemyHq.GetComponent<Headquarters>().IsDead())
        {
            GameOver = true;
            StartGameOverAnimation();
            return;
        }

        if (!_isEngaging && _agent.enabled && (_agent.remainingDistance < 0.5f || !_agent.hasPath))
        {
            SetNewDestination();
        }

        if (!_isEngaging && !_isOnEngageCooldown && (_isEngagingHQ || enemiesInRange.Count > 0))
        {
            StartCoroutine(EngageEnemy());
        }

        if (_isEngaging && !_isShooting)
        {
            StartCoroutine(ShootBullet());
        }

        if (_isEngagingHQ && _enemyHq.GetComponent<Headquarters>().IsDead())
        {
            Disengage();
            _isEngagingHQ = false;
        }

        if (_currentHealth == 0)
        {
            Die();
        }
    }

    private void StartGameOverAnimation()
    {
        StopAllCoroutines();
        if (_agent.isOnNavMesh)
        {
            _agent.ResetPath();
            _agent.isStopped = true;
            _agent.ResetPath();
            _agent.enabled = false;
        }
        _soldierRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _soldierRb.isKinematic = false;
        StartCoroutine(JumpAnimation());
    }

    private IEnumerator JumpAnimation()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 3.5f));
            _soldierRb.AddForce(new Vector3(0, 9f, 0), ForceMode.Impulse);
        }
    }

    private IEnumerator EngageEnemy()
    {
        // Debug.Log(name + " Starting engage...");
        GameObject closestEnemy = null;
        if (_isEngagingHQ)
        {
            closestEnemy = _enemyHq;
            _engagedEnemy = _enemyHq.transform.GetComponent<IEngageable>();
        }
        else
        {
            float shortestDistance = Mathf.Infinity;
            Vector3 currentPos = transform.position;
            enemiesInRange.RemoveAll(item => item == null);
            foreach (GameObject obj in enemiesInRange)
            {
                Vector3 objPos = obj.transform.position;
                float distanceToObject = Vector3.Distance(currentPos, objPos);
                float angleToObject = Vector3.Angle(transform.forward, objPos - currentPos);

                if (distanceToObject < shortestDistance && angleToObject < 90f)
                {
                    shortestDistance = distanceToObject;
                    closestEnemy = obj;
                }
            }
        }

        if (closestEnemy == null)
        {
            yield break;
        }

        float delay = engageDelay.Get();
        // Debug.Log(name + " Delaying engage for " + delay + "seconds...");
        yield return new WaitForSeconds(delay);
        // Debug.Log(name + " Engaging enemy!");
        rotationBeforeEngaging = transform.rotation;
        if (!_isEngagingHQ && closestEnemy != null)
        {
            _engagedEnemy = closestEnemy.GetComponent<Soldier>();
        }

        _agent.isStopped = true;
        _agent.ResetPath();
        _isEngaging = true;
    }

    private void Disengage()
    {
        transform.rotation = rotationBeforeEngaging;
        _agent.enabled = false;
        _agent.transform.rotation = rotationBeforeEngaging;
        _agent.enabled = true;
        _isEngaging = false;
        _isOnEngageCooldown = true;
        StartCoroutine(PerformEngageCooldown());
        _engagedEnemy = null;
        SetNewDestination();
    }

    // Movement
    private void SetNewDestination()
    {
        Vector3 playerDirection = transform.forward;
        Vector3 directionToEntity = (_enemyHq.transform.position - transform.position).normalized;
        ;
        float distance = Vector3.Distance(transform.position, _enemyHq.transform.position);
        ;
        float targetAngle = Vector3.SignedAngle(playerDirection, directionToEntity, Vector3.up);

        Quaternion desiredRotation;
        float sign = targetAngle > 0 ? 1 : -1;
        if (distance > _proximityFactor)
        {
            desiredRotation = Quaternion.Euler(0, defenseWalkGenerator.Get() * sign, 0);
        }
        else
        {
            float mu = targetAngle / 4f;
            float sigma = 2 * distance / _proximityFactor + 0.1f;
            desiredRotation = Quaternion.Euler(0, attackWalkGenerator.Get(mu, sigma) * sign, 0);
        }

        // Quaternion randomRotation = Quaternion.Euler(0, Random.Range(-15f, 15f), 0);
        Vector3 newDirection = desiredRotation * playerDirection;

        Vector3 targetPosition = transform.position + newDirection * _waypointRange;
        float additiveAngle = 10f;
        while (_agent.enabled && !_agent.SetDestination(targetPosition))
        {
            float correctionAngle = additiveAngle * sign;
            // Debug.Log("[" + name + "] I GOT STUCK! Rotating " + correctionAngle + "º to try to get unstuck.");
            newDirection = Quaternion.Euler(0, correctionAngle, 0) * newDirection;
            targetPosition = transform.position + newDirection * _waypointRange;
            additiveAngle += 10f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Entered range: " + other.name + "; Enemy HQ: " + _enemyHq.name);
        if (other.gameObject == _enemyHq)
        {
            // Debug.Log("Enemy HQ Is in range Sir!!");
            _isEngagingHQ = true;
        }

        var otherParent = other.transform.parent == null ? null : other.transform.parent.gameObject;
        if (otherParent != null && !CompareTag(otherParent.tag) && otherParent.name.StartsWith("Soldier"))
        {
            enemiesInRange.Add(otherParent);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var otherParent = other.transform.parent == null ? null : other.transform.parent.gameObject;
        if (otherParent != null && enemiesInRange.Contains(otherParent))
        {
            enemiesInRange.Remove(otherParent);
            if (_isEngaging && ReferenceEquals(_engagedEnemy, otherParent.GetComponent<Soldier>()))
            {
                Disengage();
            }
        }
    }

    // Collision events
    private void OnCollisionEnter(Collision other)
    {
        // If collider is not a bullet, exits
        if (!other.gameObject.CompareTag("Bullet"))
        {
            // Debug.Log("Not a bullet");
            return;
        }

        Bullet bullet = other.transform.GetComponent<Bullet>();

        // Debug.Log(name + " got hit by a bullet");
        _currentHealth--;
        _healthbar.UpdateHealthbar(hitPoints, _currentHealth);

        if (_currentHealth <= 0) //Dies if hp reaches 0.
        {
            Die();
        }
        else // Still alive, plays hit animation.
        {
            StartCoroutine(HitAnimation());
        }
    }

    public void Die()
    {
        _isDead = true;
        // Stops any routines running (Eg.:Hit animations).
        StopAllCoroutines();
        if (_agent != null && _agent.enabled)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
            _agent.enabled = false;
        }

        _soldierRb.isKinematic = false;
        // Changes color to red.
        _soldierRenderer.material.SetColor(Color1, Color.red);
        // Turns off constraint on rotations.
        _soldierRb.constraints = RigidbodyConstraints.None;
        // Pushes Npc back onto the floor.
        _soldierRb.AddForce(-transform.forward * 2f);
        // Invokes the DestroyBody function after a set bodySpawn delay to remove the body.
        var child = transform.GetChild(0);
        GetComponent<Collider>().enabled = false;
        _parentHq.RemoveSoldier(this);
        Invoke(nameof(DestroyBody), bodyDespawnDelay);
    }

    // Method to remove body from scene.
    private void DestroyBody()
    {
        Destroy(gameObject);
    }

    // Method to play hit animation (flash red twice).
    private IEnumerator HitAnimation()
    {
        // Flashes red twice.
        for (var i = 0; i < 2; i++)
        {
            // Sets color to red.
            _soldierRenderer.material.color = Color.red;
            yield return new WaitForSeconds(.2f);

            // After 0.2 seconds, if character is dead, exits out the for loop and, consequently, the method.
            if (_isDead) break;

            // If character still alive, returns back to the initial color and waits 0.2 seconds again.
            _soldierRenderer.material.color = _initialColor;
            yield return new WaitForSeconds(.2f);
        }
    }

    private IEnumerator ShootBullet()
    {
        _agent.enabled = false;
        Vector3 relativePos;
        if (_isEngagingHQ)
        {
            relativePos = _enemyHq.transform.position - transform.position;
        }
        else
        {
            try
            {
                relativePos = _engagedEnemy.ObjectTransform.position - transform.position;
            }
            catch (Exception)
            {
                Disengage();
                yield break;
            }
        }

        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = rotation;
        _isShooting = true;
        _agent.enabled = true;
        yield return new WaitForSeconds(shootCooldownSeconds);
        while (_isEngaging)
        {
            if (_engagedEnemy == null || _engagedEnemy.IsDead())
            {
                break;
            }

            relativePos = _engagedEnemy.ObjectTransform.position - transform.position;
            rotation = Quaternion.LookRotation(relativePos);
            transform.rotation = rotation;

            // Sample random accuracy
            Vector3 randomAccuracy = new Vector3(accuracyGenerator.Get() * _accModifier,
                accuracyGenerator.Get() * _accModifier, 0f);

            // Calculates the offset to spawn outside (in front of) the collider of the player.
            Vector3 bulletDirection = Quaternion.Euler(randomAccuracy) * transform.forward;
            Vector3 offset = new Vector3(bulletDirection.x * .5f, 0.2f, bulletDirection.z * .5f);

            // Create a new sphere GameObject on the position of the player with the offset calculated above.
            GameObject bullet = Instantiate(bulletPrefab, transform.position + offset, transform.rotation);
            bullet.GetComponent<Bullet>().ParentSoldier = this;
            // Add a Rigidbody component to the sphere, sets the collision to continuous to avoid missing a collision.
            Rigidbody bulletRb = bullet.AddComponent<Rigidbody>();
            bulletRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            bulletRb.useGravity = false;
            // Add a force to the sphere to make it shoot forward.
            bullet.GetComponent<Bullet>().Shoot(_bulletTravelTime[_level - 1]);
            bulletRb.AddForce(bulletDirection * shootForce);

            yield return new WaitForSeconds(shootCooldownSeconds);
        }

        _isShooting = false;
        _isEngaging = false;
        Disengage();
    }

    private IEnumerator PerformEngageCooldown()
    {
        yield return new WaitForSeconds(engageCooldownSeconds);
        _isOnEngageCooldown = false;
    }

    public void BulletHit()
    {
        _currentHealth--;
        _healthbar.UpdateHealthbar(hitPoints, _currentHealth);

        if (_currentHealth <= 0) // Dies if hp reaches 0.
        {
            Die();
        }
        else // Still alive, plays hit animation.
        {
            StartCoroutine(HitAnimation());
        }
    }

    public bool IsDead()
    {
        return _currentHealth <= 0;
    }

    public void Init(int spawnSoldierLevel)
    {
        _level = spawnSoldierLevel;
        _accModifier = _accuracies[spawnSoldierLevel - 1];
        GetComponent<SphereCollider>().radius = _ranges[spawnSoldierLevel - 1];
        hitPoints += spawnSoldierLevel;
    }
}