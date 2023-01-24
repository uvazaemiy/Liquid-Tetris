using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuTweener : MonoBehaviour
{
    [SerializeField] private float endValue;
    // Start is called before the first frame update
    void Start()
    {

        
            transform.DOLocalMoveY(endValue, 2f);
        
        
    }

}
