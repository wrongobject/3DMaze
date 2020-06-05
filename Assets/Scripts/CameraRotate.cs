using UnityEngine;
using System.Collections;

public class CameraRotate : MonoBehaviour
{
    public float speed = 1;
    bool draging = false;
    Vector3 start;
    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (draging)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 delta = mousePos - start;
            if (delta != Vector3.zero)
            {
                transform.RotateAround(transform.position, transform.rotation * Vector3.up,delta.x);
                transform.RotateAround(transform.position, transform.rotation *  Vector3.left, delta.y);
             //transform.Rotate(delta * speed);
                start = mousePos;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            draging = true;
            start = Input.mousePosition;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            draging = false;
        }
        
    }
}
