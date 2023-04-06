using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private float MovementSpeed = 100f;
    private Vector3 LastMovement = Vector3.zero;
    private float StopThreshold = 0.8f;
    
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

        bool did_move_y = false;
        // Move camera up and down
        if (Input.GetKey(KeyCode.Space))
        {
            tempTransform.Translate(Vector3.up * Time.deltaTime * MovementSpeed);
            LastMovement.y = Time.deltaTime * MovementSpeed;
            did_move_y = true;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            tempTransform.Translate(Vector3.down * Time.deltaTime * MovementSpeed);
            LastMovement.y = -Time.deltaTime * MovementSpeed;
            did_move_y = true;
        }
        if (!did_move_y)
        {
            // Continue moving in the last direction, but slower
            tempTransform.Translate(LastMovement * Time.deltaTime * MovementSpeed * StopThreshold);
            LastMovement.y *= StopThreshold;
        }

        bool did_move_z = false;
        // Move camera forward and back
        if (Input.GetKey(KeyCode.W))
        {
            tempTransform.Translate(Vector3.forward * Time.deltaTime * MovementSpeed);
            LastMovement.z = Time.deltaTime * MovementSpeed;
            did_move_z = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            tempTransform.Translate(Vector3.back * Time.deltaTime * MovementSpeed);
            LastMovement.z = -Time.deltaTime * MovementSpeed;
            did_move_z = true;
        }
        if (!did_move_z)
        {
            // Continue moving in the last direction, but slower
            tempTransform.Translate(LastMovement * Time.deltaTime * MovementSpeed * StopThreshold);
            LastMovement.z *= StopThreshold;
        }

        bool did_move_x = false;
        // Move camera left and right
        if (Input.GetKey(KeyCode.A))
        {
            tempTransform.Translate(Vector3.left * Time.deltaTime * MovementSpeed);
            LastMovement.x = -Time.deltaTime * MovementSpeed;
            did_move_x = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            tempTransform.Translate(Vector3.right * Time.deltaTime * MovementSpeed);
            LastMovement.x = Time.deltaTime * MovementSpeed;
            did_move_x = true;
        }
        if (!did_move_x)
        {
            // Continue moving in the last direction, but slower
            tempTransform.Translate(LastMovement * Time.deltaTime * MovementSpeed * StopThreshold);
            LastMovement.x *= StopThreshold;
        }

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
            x = 90f;
        }
        else if (x > 180f && x < 270f)
        {
            x = 270f;
        }

        transform.eulerAngles = new Vector3(x, y, 0);
    }
}
