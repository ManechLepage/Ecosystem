using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private float MovementSpeed = 100f;
    private Vector3 LastMovement = Vector3.zero;
    private float StopThreshold = 0.925f;
    private float StartTheshold = 0.075f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Set the mouse position to the middle of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (Input.mouseScrollDelta.y != 0)
        {
            MovementSpeed *= Input.mouseScrollDelta.y > 0 ? 1.1f : 0.9f;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        // Get inputs
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Create a temporaty transform to store the camera's position
        Transform tempTransform = new GameObject().transform;
        tempTransform.position = transform.position;
        tempTransform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);

        float movement = Time.deltaTime * MovementSpeed;

        bool did_move_y = false;
        bool dir1 = false;
        bool dir2 = false;
        // Move camera up and down
        if (Input.GetKey(KeyCode.Space))
        {
            LastMovement.y = Mathf.Min(LastMovement.y + movement * StartTheshold, movement);
            tempTransform.Translate(Vector3.up * LastMovement.y);
            did_move_y = true;
            dir1 = true;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            LastMovement.y = Mathf.Max(LastMovement.y - movement * StartTheshold, -movement);
            tempTransform.Translate(Vector3.down * Mathf.Abs(LastMovement.y));
            did_move_y = true;
            dir2 = true;
            
        }
        if (!did_move_y || (dir1 && dir2)) // && !(dir1 && dir2))
        {
            tempTransform.Translate(LastMovement * Time.deltaTime * MovementSpeed * StopThreshold);
            LastMovement.y *= StopThreshold;
        }
        //if (dir1 && dir2)
        //{
        //    LastMovement.y = 0;
        //}

        bool did_move_z = false;
        dir1 = false;
        dir2 = false;
        // Move camera forward and back
        if (Input.GetKey(KeyCode.W) || Input.GetMouseButtonDown(3))
        {
            LastMovement.z = Mathf.Min(LastMovement.z + movement * StartTheshold, movement);
            tempTransform.Translate(Vector3.forward * LastMovement.z);
            did_move_z = true;
            dir1 = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            LastMovement.z = Mathf.Max(LastMovement.z - movement * StartTheshold, -movement);
            tempTransform.Translate(Vector3.back * Mathf.Abs(LastMovement.z));
            did_move_z = true;
            dir2 = true;
        }
        if (!did_move_z || (dir1 && dir2))
        {
            // Continue moving in the last direction, but slower
            tempTransform.Translate(LastMovement * Time.deltaTime * MovementSpeed * StopThreshold);
            LastMovement.z *= StopThreshold;
        }
        //if (dir1 && dir2)
        //{
        //    LastMovement.z = 0;
        //}

        bool did_move_x = false;
        dir1 = false;
        dir2 = false;
        // Move camera left and right
        if (Input.GetKey(KeyCode.A))
        {
            LastMovement.x = Mathf.Max(LastMovement.x - movement * StartTheshold, -movement);
            tempTransform.Translate(Vector3.left * Mathf.Abs(LastMovement.x));
            did_move_x = true;
            dir1 = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            LastMovement.x = Mathf.Min(LastMovement.x + movement * StartTheshold, movement);
            tempTransform.Translate(Vector3.right * LastMovement.x);
            did_move_x = true;
            dir2 = true;
        }
        if (!did_move_x || (dir1 && dir2))// && !(dir1 && dir2))
        {
            // Continue moving in the last direction, but slower
            tempTransform.Translate(LastMovement * Time.deltaTime * MovementSpeed * StopThreshold);
            LastMovement.x *= StopThreshold;
        }
        //if (dir1 && dir2)
        //{
        //    LastMovement.x = 0;
        //}

        transform.position = tempTransform.position;

        // Rotate camera
        transform.Rotate(Vector3.up * mouseX * Time.deltaTime * 75f);
        transform.Rotate(Vector3.right * -mouseY * Time.deltaTime * 75f);

        // Clamp camera rotation
        float x = transform.eulerAngles.x;
        float y = transform.eulerAngles.y;
        float z = transform.eulerAngles.z;

        if (x > 90f && x < 180f)
        {
            x = 100f;
        }
        else if (x > 180f && x < 270f)
        {
            x = 280f;
        }

        transform.eulerAngles = new Vector3(x, y, 0);
    }
}
