using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TouchDetector : MonoBehaviour
{
    private float endValue;
    private bool CR_Running;
    private Vector3 playerPosition;


    

    public void DetectTouch(GameObject gameObject)
    {

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        var rigidbody2D = gameObject.GetComponent<Rigidbody2D>();


        if (Input.GetMouseButtonDown(0))
        {
            if (mousePos.x < 0)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x - 3f, rigidbody2D.velocity.y);

            }
            else
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x + 3f, rigidbody2D.velocity.y);

            }
        }
        else if (Input.GetMouseButtonUp(0))
        {

            rigidbody2D.velocity = new Vector2(0f, rigidbody2D.velocity.y);

        }




    }
}

    
