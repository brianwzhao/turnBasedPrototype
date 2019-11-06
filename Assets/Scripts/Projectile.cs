using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete("Using UNET")]
public class Projectile : NetworkBehaviour
{
    public Vector3 velocity;
    public float lifetime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    [ServerCallback]
    void FixedUpdate()
    {
        if (TurnManager.Instance.status != TurnStatus.Executing) return;
        //Only run if the turn is being executed

        transform.position += velocity * Time.fixedDeltaTime;
        lifetime -= Time.fixedDeltaTime;
        if (lifetime < 0) Destroy(gameObject);
    }
}
