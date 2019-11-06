using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Wall trigger");
        Projectile proj = collision.gameObject.GetComponentInParent<Projectile>();
        if (proj == null)
        {
            return;
        }
        Destroy(proj.gameObject);
    }
}
