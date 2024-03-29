﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete("Using UNET")]
public class Wall : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    [ServerCallback]
    private void OnTriggerEnter(Collider collision)
    {
        Projectile proj = collision.gameObject.GetComponentInParent<Projectile>();
        if (proj == null)
        {
            return;
        }
        Destroy(proj.gameObject);
    }
}
