using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    int movementSpeed;
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = -14;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z < -4)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            transform.position += new Vector3(0, 0, movementSpeed) * Time.deltaTime;
        }


    }

    public void StopMovement()
    {
        movementSpeed = 0;
    }

    public void ResumeMovement()
    {
        movementSpeed = -14;
    }

}
