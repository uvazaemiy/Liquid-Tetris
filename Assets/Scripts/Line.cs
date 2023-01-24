using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Line : MonoBehaviour
{

    [SerializeField] private Text scoreText;

    public FixedJoint2D Joint2D;
    public LineRenderer lineRenderer;
    public PolygonCollider2D polyCol;
    public Rigidbody2D rigidbody;
    public Color color;
    public float area;
    public int lineScore;
    public int totalScore;
    

    public Text txt;
    public bool DoNothing;

    List<Vector2> points;
    
    
    
    
    

    public void UpdateLine(Vector2 mousePos)
    {
        if (points == null)
        {
            points = new List<Vector2>();
            SetPoint(mousePos);
            return;
        }
 
        if (Vector2.Distance(points.Last(), mousePos) > .1f)
            SetPoint(mousePos);
    }
 
    void SetPoint(Vector2 point)
    {
        points.Add(point);
 
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPosition(points.Count - 1, point);
 
        if (points.Count > 1)
            polyCol.points = points.ToArray();
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        
        if (!other.collider.CompareTag("Ground") )
        {
            
                var meshRenderer = GetComponent<MeshRenderer>();
                if (meshRenderer.material.color == other.gameObject.GetComponent<MeshRenderer>().material.color)
                {
                    if(DoNothing) return;
                    other.gameObject.GetComponent<Line>().DoNothing = true;
                    totalScore = lineScore + other.gameObject.GetComponent<Line>().totalScore;
                    //other.gameObject.GetComponent<Line>().totalScore = totalScore;
                    Debug.Log(totalScore);
                    if (totalScore >= 30)
                    {

                       
                    }
                    else
                    {

                        
                        other.gameObject.transform.SetParent(gameObject.transform);
                        other.gameObject.GetComponent<Line>().txt.text = "";
                        txt.rectTransform.localScale =
                            Abs(new Vector3(100, 100, 0) - other.gameObject.transform.localScale) / 1000;
                        txt.text = totalScore.ToString();
                        txt.transform.position = Vector3.Lerp(polyCol.bounds.center,
                            other.gameObject.GetComponent<PolygonCollider2D>().bounds.center, 0.5f);
                        
                    }
                }

                
            
        }

    }

    public void CalculateScore(float size, bool line)
    {
        
        // int scale = (int) SizeMult;
        // int areA = (int) area;
        // if (areA < 1)
        // {
        //     areA = 1;
        // }

        int o_size = (int) size;
        
        lineScore = o_size / 10;
        totalScore = lineScore;
        txt.transform.position = polyCol.bounds.center;
        if (line)
        {
            txt.rectTransform.localScale = txt.rectTransform.localScale * new Vector2(0.5f, 0.5f);
        }
        txt.text = lineScore.ToString();
        Debug.Log(lineScore);
        Debug.Log(area);
    }
    
    public void FindArea()
    {
        
        float firstAns = 0, secondAns = 0;
        
        for (int i = 0; i < polyCol.points.Length; i++)
        {
            if (i == polyCol.points.Length - 1)
            {
                firstAns += polyCol.points[i].x * polyCol.points[0].y;
                secondAns += polyCol.points[i].y * polyCol.points[0].x;
            }
            else
            {
                firstAns += polyCol.points[i].x * polyCol.points[i + 1].y;
                secondAns += polyCol.points[i].y * polyCol.points[i + 1].x;
            }
        }

        area = (firstAns - secondAns) / 2;
        Debug.Log(area);
    }

    public Vector3 Abs (Vector3 v2) {
        return new Vector3(Mathf.Abs(v2.x), Mathf.Abs(v2.y), Mathf.Abs(v2.z));
    }
}
