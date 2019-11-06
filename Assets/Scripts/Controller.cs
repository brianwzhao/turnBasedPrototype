using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete("Using UNET")]
public class Controller : NetworkBehaviour
{
    [SerializeField]
    private Handle destination;

    [SerializeField]
    private Handle view;

    [SerializeField]
    [SyncVar]
    private GameObject character;

    [SerializeField]
    private GameObject playerCharacter;
    [SerializeField]
    private GameObject Handle;
    [SerializeField]
    private Material DestinationMaterial;
    [SerializeField]
    private Material AimMaterial;

    LineRenderer moveLr;
    LineRenderer aimLr;

    private float timeElapsed;
    private Vector3 startPosition;
    private Vector3 endPosition;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        character = null;
        if (isServer)
        {
            serverStart();
        }

        if (isLocalPlayer)
        {
            Debug.Log("local");

            //Instantiate objects
            GameObject aim = Instantiate(Handle, transform);
            aim.gameObject.GetComponent<MeshRenderer>().material = AimMaterial;
            GameObject dest = Instantiate(Handle, transform);
            dest.gameObject.GetComponent<MeshRenderer>().material = DestinationMaterial;

            view = aim.GetComponent<Handle>();
            destination = dest.GetComponent<Handle>();



            //Define Draw Lines
            moveLr = destination.gameObject.AddComponent<LineRenderer>();
            aimLr = view.gameObject.AddComponent<LineRenderer>();

            moveLr.startWidth = 0.3f;
            moveLr.endWidth = 0.1f;
            moveLr.material = destination.GetComponent<MeshRenderer>().material;

            aimLr.startWidth = 0.3f;
            aimLr.endWidth = 0.1f;
            aimLr.material = view.GetComponent<MeshRenderer>().material;

            GameObject buttonObj = GameObject.Find("Canvas/Button");
            buttonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(sendMovement);

            CmdRegisterPlayer();
        }
    }

    [ServerCallback]
    private void OnDestroy()
    {
        Destroy(character);
    }

    [Server]
    private void serverStart()
    {
        Debug.Log("server");
        GameObject player = Instantiate(playerCharacter);
        player.transform.position = transform.position;
        NetworkServer.Spawn(player);
        StartCoroutine(setCharacter(player));

    }

    private IEnumerator setCharacter(GameObject pl)
    {
        while(character == null)
        {
            Debug.Log("try again");
            RpcSetCharacter(pl);
            yield return new WaitForFixedUpdate();
        }
    }

    [ClientRpc]
    private void RpcSetCharacter(GameObject character)
    {
        this.character = character;
    }

    // Update is called once per frame
    [ClientCallback]
    void LateUpdate()
    {
        if (isLocalPlayer)
        {
            if (!character) return;
            CheckObstacles();

            UpdateLines();
        }
    }

    [ClientCallback]
    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            switch (TurnManager.Instance.status)
            {
                case TurnStatus.StartExecution:
                    break;
                case TurnStatus.FinishExecution:
                    Vector3 offset = character.transform.position - transform.position;
                    offset.y = 0;
                    transform.position += offset;
                    view.transform.position -= offset;
                    destination.transform.position -= offset;
                    break;
            }
        }
    }

    [Client]
    private void CheckObstacles()
    {
        Ray ray = new Ray();
        Vector3 destinationPos = destination.transform.localPosition;
        float y = destinationPos.y;
        destinationPos.y = 0;
        float distanceSquare = destinationPos.magnitude + 1;
        ray.direction = destinationPos;
        ray.origin = transform.position;
        
        LayerMask mask = 1 << 10;

        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo, distanceSquare, mask);
        if (hit)
        {   
            float distance = hitInfo.distance;
            if(distance < distanceSquare)
            {
                //clamp the locatioin
                destinationPos = ray.GetPoint(distance - 1);
                destinationPos.y = y;
                destination.transform.position = destinationPos;
            }
        }
    }

    [Client]
    private void sendMovement()
    {
        moveInfo movement;
        movement.Position = destination.transform.position;
        movement.Position.y = 0;
        movement.View = view.transform.position;
        movement.View.y = 0;
        movement.Time = TurnManager.Instance.TimeInterval;
        Debug.Log("Send Command" + movement);
        CmdSendMove(movement);
    }

    [Command]
    private void CmdSendMove(moveInfo movement)
    {
        //TODO validate movement
        character.GetComponent<Actor>().CmdDoMove(movement);
        TurnManager.Instance.setReady(connectionToClient);
        Debug.Log("recieved move" + playerControllerId);
    }

    [Command]
    private void CmdRegisterPlayer()
    {
        TurnManager.Instance.registerPlayer(connectionToClient);
    }

    [Client]
    private void UpdateLines()
    {
        if (!character) return;
        Vector3 point2 = destination.transform.position;
        point2.y = 0;
        moveLr.SetPosition(0, character.transform.position);
        moveLr.SetPosition(1, point2);

        point2 = view.transform.position;
        point2.y = 0;
        aimLr.SetPosition(0, character.transform.position);
        aimLr.SetPosition(1, point2);
    }

}

public struct moveInfo
{
    public Vector3 Position;
    public Vector3 View;
    public float Time;
}
