using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject slice2Image;
    void Start()
    {
        slice2Image.transform.localScale = Vector3.zero;
        StartCoroutine(bigSlice2());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator bigSlice2()
    {
        yield return new WaitForSeconds(1);
        slice2Image.transform.DOScale(1.277099f, 0.2f);
    }
}
