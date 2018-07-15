using UnityEngine;
using UnityEngine.Networking;

public class PlayerWeapon : NetworkBehaviour {

    private string PLAYER_TAG = "Player";

    [SerializeField]
    private Camera cam;

    public float range = 100f;
    public float cooldown = 2f;
    private float cooldownTimer = 2f;

    [SerializeField]
    private LayerMask mask;

	// Use this for initialization
	void Start () {
		if (cam == null)
        {
            Debug.LogError("PlayerWeapon : camera not found");
            this.enabled = false;
        }

        cooldownTimer = cooldown;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (cooldownTimer <= 0)
        {
            cooldownTimer = cooldown;
        }
        else
        {
            if (cooldownTimer < cooldown)
            {
                cooldownTimer -= Time.fixedDeltaTime;
            }
        }
	}

    [Client]
    public void Shoot()
    {
        if(cooldownTimer == cooldown)
        {
            cooldownTimer -= Time.fixedDeltaTime;
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, mask))
            {
                Debug.Log("hit : " + hit.collider.name + "(" + hit.point.ToString() + ")");
                if (hit.collider.tag == PLAYER_TAG)
                {
                    CmdPlayerShot(hit.collider.name, 1);
                }
            }
        }

    }

    [Command]
    void CmdPlayerShot(string id, int damage)
    {
        Debug.Log(id + " has been shot");
        GameManager.GetPlayer(id).Hit(damage);
    }
}
