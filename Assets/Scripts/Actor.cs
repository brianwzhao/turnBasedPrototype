﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    moveInfo MoveStart;
    moveInfo MoveEnd;

    moveInfo delta;

    int bulletCount;

    public float Health;

    [SerializeField]
    private GameObject Projectile;

    // Start is called before the first frame update
    void Start()
    {
        MoveEnd.Position = transform.position;
        MoveEnd.View = transform.position;
        MoveEnd.View += transform.forward.normalized * 2f;
    }


    void FixedUpdate()
    {
        if (TurnManager.Instance.status != TurnStatus.Executing) return;
        //Only run if the turn is being executed

        MoveStart.Time += Time.fixedDeltaTime;
        transform.position = Vector3.Lerp(
            MoveStart.Position,
            MoveEnd.Position,
            MoveStart.Time/MoveEnd.Time
            );

        Vector3 view = Vector3.Lerp(
            MoveStart.View,
            MoveEnd.View,
            MoveStart.Time / MoveEnd.Time
            );

        transform.LookAt(view);

        if (bulletCount == 4)
        {
            bulletCount = 0;
            ShootForward();
        }
        bulletCount++;
    }

    public void doMove(moveInfo move)
    {
        MoveStart.Time = 0;
        MoveStart.Position = transform.position;
        MoveStart.View = MoveEnd.View;
        MoveEnd = move;

        delta.Position = MoveStart.Position - MoveEnd.Position;
        delta.View = MoveStart.View - MoveEnd.View;
        delta.Time = (0.1f + delta.Position.magnitude) * (10 + delta.View.magnitude);
    }

    private void ShootForward()
    {
        GameObject go = Instantiate(Projectile);
        Projectile proj = go.GetComponent<Projectile>();
        go.transform.rotation = transform.rotation;
        go.transform.position = transform.position;
        go.transform.position += transform.forward.normalized * 2f;

        proj.velocity = transform.forward.normalized * 10 * (10 + Random.Range(-1f, 1f)) * MoveEnd.Time;
        proj.velocity += transform.right * Random.Range(-1f, 1f) * delta.Time;

        proj.lifetime = 4;
    }

    private void OnTriggerEnter(Collider collision)
    {
        Projectile proj = collision.gameObject.GetComponentInParent<Projectile>();
        if(proj == null)
        {
            return;
        }
        Health -= 1;
        if (Health <= 0) Destroy(gameObject);
        Destroy(proj.gameObject);
    }
}
