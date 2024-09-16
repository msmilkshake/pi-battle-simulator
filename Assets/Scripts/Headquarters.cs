using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Headquarters : MonoBehaviour, IEngageable
{
    // IEngageable interface
    public Transform ObjectTransform => transform;

    public GameObject enemyPrefab;
    public GameObject enemyHq;
    public int maxNpcs;

    public int hitpoints = 50;
    private int _currentHitpoints;
    public Healthbar healthbar;
    public Healthbar UIhealthbar;
    public GameObject winnerPanel;
    public GameObject themeSongs;
    public GameObject confetis;
    public GameObject escapeMenu;
    

    [Range(0f, 90f)] public float spawnAngle = 60f;
    public float spawnDistance = 3f;

    public Material material;

    //Local variables.
    private static IDistribution _levelDistribution = new LevelDiscreteDistribution();
    private static float[] _levelDelays = { 3f, 4.5f, 6f, 7.5f, 9f };
    private static float[] _rateSpeedModifier = { 1.5f, 1.0f, 0.5f };
    
    private GaussianDistribution _spawnDelayGenerator = new();

    private int _npcCount;
    private float _respawnInterval;
    private string _team;
    private float _respawnRate;

    private int _spawnSoldierLevel;

    private bool _isFlashing = false;
    private bool _isInitialized = false;

    private List<Soldier> _hqSoldiers = new();


    private void Start()
    {
        _team = tag.Equals("BlueHQ") ? "Blue" : "Red";
        maxNpcs = PlayerPrefs.GetInt(_team + "soldiers");
        _respawnRate = _rateSpeedModifier[PlayerPrefs.GetInt(_team + "spawnrate")];
        _currentHitpoints = hitpoints;
        _spawnSoldierLevel = 0;
        _isInitialized = true;
        StartCoroutine(SpawnNpcs());
    }

    private void Update()
    {
        _npcCount = _hqSoldiers.Count;
    }

    private IEnumerator SpawnNpcs()
    {
        while (true)
        {
            while (_npcCount < maxNpcs)
            {
                if (IsDead())
                {
                    yield break;
                }

                _spawnSoldierLevel = _spawnSoldierLevel == 0 ? 1 : (int)_levelDistribution.Get();
                float delay = _levelDelays[_spawnSoldierLevel - 1] * _respawnRate;
                // Debug.Log("Delay: " + delay);
                _respawnInterval = _spawnDelayGenerator
                    .Get(delay, 0.25f, delay - 1f, delay + 1f);

                // Debug.Log(name + " Spawns: Level " + _spawnSoldierLevel + ", and waits: " + _respawnInterval + "s");
                StartCoroutine(SpawnSoldier());
                yield return new WaitForSeconds(_respawnInterval);
                if (enemyHq == null || (_isInitialized && enemyHq.GetComponent<Headquarters>().IsDead()))
                {
                    yield break;
                }
            }

            yield return null;
        }
    }

    private IEnumerator SpawnSoldier()
    {
        float angleInDegrees = Random.Range(-spawnAngle, spawnAngle);
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        var localTransform = transform;
        Vector3 localPosition = localTransform.position +
                                localTransform.forward * (spawnDistance * Mathf.Cos(angleInRadians)) +
                                transform.right * (spawnDistance * Mathf.Sin(angleInRadians));
        Quaternion rotationTowardsPosition = Quaternion.LookRotation(localPosition - transform.position);
        localPosition.y = 0f;
        GameObject npcObject = Instantiate(enemyPrefab, localPosition, rotationTowardsPosition);
        Soldier soldier = npcObject.GetComponent<Soldier>();
        _hqSoldiers.Add(soldier);
        soldier.Init(_spawnSoldierLevel);
        soldier.GetComponentInChildren<TextMeshProUGUI>().SetText("" + _spawnSoldierLevel);
        soldier.transform.Find("Body").transform.Find("hat")
            .GetComponent<MeshRenderer>().material = material;
        soldier.ParentHq = this;
        soldier.EnemyHq = enemyHq;
        if (transform.CompareTag("RedHQ"))
        {
            soldier.tag = "RedNpc";
        }
        else if (transform.CompareTag("BlueHQ"))
        {
            soldier.tag = "BlueNpc";
        }

        // _npcCount++;
        yield return null;
    }

    public void RemoveSoldier(Soldier soldier)
    {
        _hqSoldiers.Remove(soldier);
    }


    private void OnCollisionEnter(Collision other)
    {
        // Debug.Log(other.transform.name);
        //If collider is not a bullet, exits
        if (!other.gameObject.CompareTag("Bullet") || IsDead())
        {
            return;
        }

        _currentHitpoints--;
        healthbar.UpdateHealthbar(hitpoints, _currentHitpoints);
        UIhealthbar.UpdateHealthbar(hitpoints, _currentHitpoints);

        if (_currentHitpoints <= 0)
        {
            StartCoroutine(HqDestroySequence());
            // gameObject.transform.parent.GetComponent<Renderer>().enabled = false;
        }
        else //Still alive, plays hit animation.
        {
            StartCoroutine(HitAnimation());
        }
    }

    private IEnumerator HqDestroySequence()
    {
        escapeMenu.GetComponent<EscapeMenu>().SetGameOver();
        
        //winner panel and theme song
        string winner = tag.Equals("RedHQ") ? "BLUE" : "RED";
        winnerPanel.GetComponentInChildren<TMP_Text>().SetText(winner + " TEAM WINS!");
        winnerPanel.SetActive(true);
        
        foreach (Transform child in confetis.transform)
        {
            child.gameObject.SetActive(true);
            child.gameObject.GetComponentInChildren<ParticleSystem>().Play();
        }
        themeSongs.transform.GetChild(0).GetComponent<MusicController>().FadeOut(0.5f);
        yield return new WaitForSeconds(0.5f);
        themeSongs.transform.GetChild(0).gameObject.SetActive(false);
        themeSongs.transform.GetChild(1).gameObject.SetActive(true);
        
        GetComponent<MeshCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        while (_hqSoldiers.Count > 0)
        {
            Soldier soldier = _hqSoldiers[0];
            _hqSoldiers.Remove(soldier);
            soldier.Die();
        }

        StartCoroutine(RotateOverTime());
        yield return new WaitForSeconds(5);
        Destroy(transform.parent.gameObject);
    }

    IEnumerator RotateOverTime()
    {
        float startTime = Time.time; // get start time

        while (Time.time < startTime + 5)
        {
            float rotationAngle = 3f * Time.deltaTime; // calculate rotation angle based on speed and time
            transform.Rotate(Vector3.forward * rotationAngle); // rotate the entity around z-axis
            yield return null; // yield execution until next frame
        }
    }

    private IEnumerator HitAnimation()
    {
        if (_isFlashing)
        {
            yield break;
        }

        _isFlashing = true;
        //Flashes red.
        for (var i = 0; i < 1; i++)
        {
            var initialColor = GetComponent<Renderer>().material.color;
            //Sets color to red.
            GetComponent<Renderer>().material.color = Color.red;
            yield return new WaitForSeconds(.2f);

            //If character still alive, returns back to the initial color and waits 0.1 seconds again.
            GetComponent<Renderer>().material.color = initialColor;
            yield return new WaitForSeconds(.2f);
            _isFlashing = false;
        }
    }

    public bool IsDead()
    {
        return _currentHitpoints <= 0;
    }

    public Soldier GetRandomSoldier()
    {
        if (_hqSoldiers.Count != 0)
        {
            return _hqSoldiers[Random.Range(0, _hqSoldiers.Count)];
        }

        return null;
    }

    public int GetCurrentNumberOfSoldiers()
    {
        return _hqSoldiers.Count;
    }
}