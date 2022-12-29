using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        


        //if (PlayerMovement.Instance.hit.transform!=null && PlayerMovement.Instance.hit.transform.CompareTag("rope"))
        //{
        //    inrope = true;
        //    PlayerMovement.Instance.anim.SetBool("rope", true);
        //    PlayerMovement.Instance.MovementSpeed = 0;
        //    isPicked = true;
        //    rope = PlayerMovement.Instance.hit.transform.gameObject;
        ////}
        //if (PlayerMovement.Instance.hit.transform != null && PlayerMovement.Instance.hit.transform.CompareTag("ropeEnd"))
        //{
        //    Debug.Log("cixdim");
        //    PlayerMovement.Instance.Rigidbody.AddForce(transform.up * .2f, ForceMode.Impulse);
        //    PlayerMovement.Instance.anim.SetBool("ropeMove", false);
        //    PlayerMovement.Instance.anim.SetBool("rope", false);
           
        //    inrope = false;
            
        //    isPicked = false;
        //    GetComponent<Rigidbody>().isKinematic = false;
        //    PlayerMovement.Instance.MovementSpeed = 5;
        //}
    }

    private void BrakeObj()
    {
        if (isPicked)
        {
            EventHolder.Instance.PlayerHoldToIdle(gameObject);
            _PickedItem.AddComponent<Rigidbody>();
            //if (PlayerMovement.Instance.direction.magnitude < 0.01f)
            //{
            //    EventHolder.Instance.PlayerHoldToIdle(gameObject);
            _PickedItem.transform.parent =null;
            //}
            //else
            //{

            //}
           // hj.connectedBody = null;
            isPicked = false;
           //itemRb.gameObject.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
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


                _PickedItem.transform.parent = transform;
                Destroy(_PickedItem.GetComponent<Rigidbody>()); 
                //itemRb = item.GetComponent<Rigidbody>();
                //hj.connectedBody = itemRb;
                //item.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                //item.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
               
                    EventHolder.Instance.PlayerHoldIdleStart(gameObject);
                //    Debug.Log("tuttum;");
                //    isHoldIdleAnim= false;  
                

            }
        }
    }
}
