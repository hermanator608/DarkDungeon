using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerLight : MonoBehaviour
{
    public float rotateRadius = .25f;

    // Update is called once per frame
    void Update()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Ensure the mouse position is at the same z-coordinate as the light

        // Calculate the direction from the light to the mouse position
        Vector3 direction = mousePosition - transform.position;

        // Calculate the angle in radians
        float angle = Mathf.Atan2(direction.y, direction.x);
        angle -= 90;

        // Calculate the new position based on the angle and radius
        float x = transform.position.x + Mathf.Cos(angle) * rotateRadius;
        float y = transform.position.y + Mathf.Sin(angle) * rotateRadius;

        Debug.Log(angle);
        Debug.Log(x + ", " + y);
        Debug.Log(transform);

        // Update the light's position
        transform.position = new Vector3(x, y, 0f);
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}