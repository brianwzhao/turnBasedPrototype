using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private Handle destination;

    [SerializeField]
    private Handle view;

    [SerializeField]
    private Actor character;

    LineRenderer moveLr;
    LineRenderer aimLr;

    private float timeElapsed;
    private Vector3 startPosition;
    private Vector3 endPosition;

    // Start is called before the first frame update
    void Start()
    {
        moveLr = destination.gameObject.AddComponent<LineRenderer>();
        aimLr = view.gameObject.AddComponent<LineRenderer>();

        moveLr.startWidth = 0.3f;
        moveLr.endWidth = 0.1f;
        moveLr.material = destination.GetComponent<MeshRenderer>().material;

        aimLr.startWidth = 0.3f;
        aimLr.endWidth = 0.1f;
        aimLr.material = view.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CheckObstacles();

        UpdateLines();

    }

    private void FixedUpdate()
    {
        switch (TurnManager.Instance.status)
        {
            case TurnStatus.StartExecution:
                moveInfo movement;
                movement.Position = destination.transform.position;
                movement.Position.y = 0;
                movement.View = view.transform.position;
                movement.View.y = 0;
                movement.Time = TurnManager.Instance.TimeInterval;
                character.doMove(movement);
                break;
            case TurnStatus.FinishExecution:
                Vector3 offset = character.transform.localPosition;
                offset.y = 0;
                transform.position += offset;
                character.transform.position -= offset;
                view.transform.position -= offset;
                break;
        }
    }

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

    private void UpdateLines()
    {
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
