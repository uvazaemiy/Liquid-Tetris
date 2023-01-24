using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBridge : MonoBehaviour
{
    LineDrawer _listener;
    private TouchDetector _secondListener;
    public void Initialize(LineDrawer l)
    {
        _listener = l;
    }

   

    void OnCollisionEnter2D(Collision2D collision)
    {
        _listener.OnCollisionEnter2D(collision);
    }
   
}
