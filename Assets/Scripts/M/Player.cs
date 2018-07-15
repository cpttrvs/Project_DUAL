using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    [SerializeField]
    private int team = 0;

    [SerializeField]
    private int startingHealth = 2;

    [SyncVar]
    private int health = 0;

    [SyncVar]
    private bool isMarked = false;

    private bool dead = false;
    public bool isDead
    {
        get { return dead; }
        protected set { dead = value; }
    }

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;
    
    [SerializeField]
    private float markCD = 2f;
    private float markCD_timer = 0f;


	// Use this for initialization
	public void Setup () {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }

        SetDefaults();
	}
    
	
	// Update is called once per frame
	void Update () {
		if(isMarked)
        {
            markCD_timer -= Time.deltaTime;
            if (markCD_timer <= 0)
            {
                isMarked = false;
                markCD_timer = markCD;
            }
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(2);
        }
	}

    public void Hit(int damage)
    {
        if(isMarked)
        {
            isMarked = false;
            RpcTakeDamage(damage);
        } else
        {
            isMarked = true;
        }
    }

    [ClientRpc]
    private void RpcTakeDamage(int damage)
    {
        health -= damage;

        Debug.Log(transform.name + " : " + health + " health");
        if (health < 0)
        {
            health = 0;
        }

        if (health == 0)
        {
            Die();
        }

    }

    private void Die()
    {
        isDead = true;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null) { col.enabled = false; }

        Debug.Log(transform.name + " is dead");

        //respawn
        StartCoroutine(Respawn());
    }

    public void SetDefaults()
    {
        health = startingHealth;
        markCD_timer = markCD;
        isDead = false;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider col = GetComponent<Collider>();
        if (col != null) { col.enabled = true; }

    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnDelay);
        SetDefaults();
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        Debug.Log(transform.name + " respawned");
    }

    public int GetHealth() { return health; }
    public void SetTeam(int id) { team = id; }
    
}
