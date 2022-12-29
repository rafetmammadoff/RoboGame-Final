using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using RootMotion.Demos;
using DG.Tweening;
using System;
using static UnityEditor.Progress;

public class HoldControl : MonoBehaviour
{
    public static HoldControl Instance;
    public bool isPicked = false;
    [SerializeField] Transform PickTransform;
    [SerializeField] float Radius;
    Collider[] Cols;
    [SerializeField] LayerMask LayerMask;
    GameObject _PickedItem;
    HingeJoint hj;
    Rigidbody itemRb;
    bool inrope = false;
    bool isHoldIdleAnim=true;
    GameObject rope;
    void Start()
    {
        hj = PickTransform.GetComponent<HingeJoint>();
    }

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(PickTransform.position, Radius);
        Gizmos.color = new Color(0.4f, 0, 0, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HoldObj();
        }

        if (Input.GetMouseButtonDown(1))
        {
            BrakeObj();
        }

        if (inrope)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.up * 3f * Time.deltaTime;
                PlayerMovement.Instance.anim.SetBool("ropeMove", true);
                
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                PlayerMovement.Instance.anim.SetBool("ropeMove", false);
            }

        }


        if (PlayerMovement.Instance.hit.transform!=null && PlayerMovement.Instance.hit.transform.CompareTag("rope"))
        {
            //hj.connectedBody = PlayerMovement.Instance.hit.transform.gameObject.GetComponent<Rigidbody>();
            inrope = true;
            PlayerMovement.Instance.anim.SetBool("rope", true);
            PlayerMovement.Instance.MovementSpeed = 0;
            isPicked = true;
            rope = PlayerMovement.Instance.hit.transform.gameObject;
        }
        if (PlayerMovement.Instance.hit.transform != null && PlayerMovement.Instance.hit.transform.CompareTag("ropeEnd"))
        {
            //hj.connectedBody = PlayerMovement.Instance.hit.transform.gameObject.GetComponent<Rigidbody>();
            Debug.Log("cixdim");
            PlayerMovement.Instance.Rigidbody.AddForce(transform.up * .2f, ForceMode.Impulse);
            PlayerMovement.Instance.anim.SetBool("ropeMove", false);
            PlayerMovement.Instance.anim.SetBool("rope", false);
           
           // hj.connectedBody = null;
            inrope = false;
            
            isPicked = false;
            GetComponent<Rigidbody>().isKinematic = false;
            PlayerMovement.Instance.MovementSpeed = 5;
            //rope.transform.tag = "ropeActive";
        }




    }

    private void BrakeObj()
    {
        if (isPicked)
        {
            EventHolder.Instance.PlayerHoldToIdle(gameObject);
            //if (PlayerMovement.Instance.direction.magnitude < 0.01f)
            //{
            //    EventHolder.Instance.PlayerHoldToIdle(gameObject);

            //}
            //else
            //{

            //}
            hj.connectedBody = null;
            isPicked = false;
            itemRb.gameObject.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }

    }

    public void HoldObj()
    {
        Cols = Physics.OverlapSphere(PickTransform.position, Radius, LayerMask);
        foreach (Collider item in Cols)
        {

            if (!isPicked)
            {
                
                isPicked = true;
                _PickedItem = item.gameObject;
                itemRb = item.GetComponent<Rigidbody>();
                hj.connectedBody = itemRb;
                item.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                item.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
               
                    EventHolder.Instance.PlayerHoldIdleStart(gameObject);
                    Debug.Log("tuttum;");
                    isHoldIdleAnim= false;  
                

            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        //if (collision.transform.CompareTag("rope"))
        //{
        //    hj.connectedBody = collision.gameObject.GetComponent<Rigidbody>();
        //    inrope = true;
        //    PlayerMovement.Instance.anim.SetBool("rope",true);
        //    PlayerMovement.Instance.MovementSpeed = 0;
        //    isPicked = true;


        //}




    }

    //private void OnTriggerExit(Collider other)
    //{

    //    rope.transform.tag = "rope";
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    Debug.Log("cixdim");
    //    hj.connectedBody = null;
    //    inrope = false;
    //    PlayerMovement.Instance.anim.SetBool("rope", false);
    //    GetComponent<Rigidbody>().isKinematic = false;
    //}
}
