using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    public Animator PlaneAnim;
    public Animator PerAnim;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            PlaneAnim.SetTrigger("active");
            PerAnim.SetTrigger("active");
            StartCoroutine(WaitFly());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            PlaneAnim.SetTrigger("deactive");
        }
    }


    public IEnumerator WaitFly()
    {
        yield return new WaitForSeconds(0.5f);
        transform.DOMoveY(transform.position.y + 6, 4);
        yield return new WaitForSeconds(2f);
        transform.DOMoveY(transform.position.y - 6, 4);
    }
}
