using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    private Behaviour[] componentsToDisable;

    [SerializeField]
    private string remoteLayerName = "RemotePlayer";

    [SerializeField]
    private string dontDrawLayerName = "DontDraw";
    [SerializeField]
    private GameObject playerGraphics;

    private Camera sceneCamera;
    [SerializeField]
    private GameObject playerUIPrefab;
    private GameObject playerUIInstance;


	// Use this for initialization
	void Start () {
		if(!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();

        } else
        {
            sceneCamera = Camera.main;
            if(sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }

            //disable local player skin
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            //player UI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;
        }

        GetComponent<Player>().Setup();
	}

    public override void OnStartClient()
    {
        base.OnStartClient();
        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();
        GameManager.RegisterPlayer(netID, player);
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach(Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    private void OnDisable()
    {
        Destroy(playerUIInstance);

        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(false);
        }

        GameManager.UnregisterPlayer(transform.name);
    }
}
