using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 5f;
    public Rigidbody rb;
    public bool isFirstPerson = false;

    private float initialHeight;
    
    // Start is called before the first frame update
    void Start()
    {
        SetHeight(transform.position.y);
    }

    void SetHeight(float height)
    {
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
        initialHeight = height;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFirstPerson)
            return;
        
        float overrideMoveSpeed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftControl))
            overrideMoveSpeed *= 3f;

        // if WASD, addforce to camera in that direction (addrelativeforce)
        Vector3 force = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
            force += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.S))
            force += new Vector3(0, 0, -1);
        if (Input.GetKey(KeyCode.A))
            force += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.D))
            force += new Vector3(1, 0, 0);
        
        rb.AddRelativeForce(force * overrideMoveSpeed * Time.deltaTime * 60f);

        force = new Vector3(0, 0, 0);
        bool didMoveVertical = false;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            force += new Vector3(0, -1, 0);
            didMoveVertical = true;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            force += new Vector3(0, 1, 0);
            didMoveVertical = true;
        }

        if (didMoveVertical)
        {
            rb.AddForce(force * overrideMoveSpeed * Time.deltaTime * 60f);
            SetHeight(transform.position.y);
        }


        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            Vector3 mouseAngles = new Vector3(-mouseY * turnSpeed, mouseX * turnSpeed, 0);
            Vector3 currentAngles = transform.eulerAngles;

            if (currentAngles.x > 180)
                currentAngles.x -= 360;
            
            currentAngles.x = Mathf.Clamp(
                currentAngles.x + mouseAngles.x,
                -90,
                90
            );

            transform.eulerAngles = new Vector3(
                currentAngles.x,
                currentAngles.y + mouseAngles.y,
                currentAngles.z
            );
        }

        // change the camera position y to initialHeight
        transform.position = new Vector3(
            transform.position.x,
            initialHeight,
            transform.position.z
        );
    }
}
