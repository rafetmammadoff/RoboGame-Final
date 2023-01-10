using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] GameObject PostcardPanel;
    [SerializeField] GameObject firstImage;
    [SerializeField] GameObject secondImage;
    [SerializeField] GameObject slice3Image;
    [SerializeField] RectTransform slice3ImageTransform;
    [SerializeField] GameObject slice4Image;
    [SerializeField] RectTransform slice4ImageTransform;
  
    public bool isFindSlice3 = false;
    public bool isFindSlice4 = false;
    PointerEventData eventData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
    }
  

    // Update is called once per frame
    void Update()
    {
        if (isFindSlice3)
        {
            PostcardPanel.SetActive(true);
            firstImage.SetActive(true);
            secondImage.SetActive(false);
            StartCoroutine(bigSlice3());
            isFindSlice3= false;
        }
        if (isFindSlice4)
        {
            PostcardPanel.SetActive(true);
            secondImage.SetActive(true);
            firstImage.SetActive(false);
            StartCoroutine(bigSlice4());
            isFindSlice4= false;
        }

    }
    public IEnumerator bigSlice3()
    {
        yield return new WaitForSeconds(1);
        slice3Image.transform.DOMove(slice3ImageTransform.position,0.4f);
        // slice2Image.transform.DOScale(0.9262591f, 0.2f);
    }
    public IEnumerator bigSlice4()
    {
        yield return new WaitForSeconds(1);
        slice4Image.transform.DOMove(slice4ImageTransform.position, 0.4f);
        // slice2Image.transform.DOScale(0.9262591f, 0.2f);
    }

    public void closePostcardPanel()
    {
        PostcardPanel.SetActive(false);
    }

   
}
