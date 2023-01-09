using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ToonyColorsPro.ShaderGenerator.Enums;

public class detectBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("box") && HoldControl.Instance.isPicked)
        {
            Debug.Log("===========");
           PlayerMovement.Instance.isMoveable = false;
           PlayerMovement.Instance.MovementSpeed = 0;
        }
    }
}
