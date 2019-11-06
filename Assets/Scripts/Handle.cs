using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : MonoBehaviour
{
    new protected Camera camera;

    [SerializeField]
    private float maxDistance;

    private Vector3 initialHandlePosition;
    private Vector3 initialMousePosition;

    private void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        clampPosition();
    }
    
    private void OnMouseDown()
    {
        initialMousePosition = getMousePosition();
        initialHandlePosition = transform.position;
    }
    
    private void OnMouseDrag()
    {
        if (true)
        {
            Vector3 tempMousePosition = getMousePosition();
            Vector3 delta = initialMousePosition - tempMousePosition;
            delta.y = 0;

            gameObject.transform.position = initialHandlePosition - delta;
        }
    }
    
    private Vector3 getMousePosition()
    {
        Vector3 screen = Input.mousePosition;
        screen.z = (camera.transform.position - transform.position).magnitude;
        Vector3 position = camera.ScreenToWorldPoint(screen);
        return position;
    }

    private void clampPosition()
    {
        Vector3 position = transform.localPosition;
        float y = position.y;
        position.y = 0;
        if (position.sqrMagnitude > maxDistance * maxDistance)
        {
            float mag = position.magnitude;
            position = position.normalized * maxDistance;
        }
        position.y = y;
        transform.localPosition = position;
    }
}
