using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;



public class LineDrawer : MonoBehaviour
{
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Transform drawPanel;
    [SerializeField] private Transform gamePanel;
    [SerializeField] private TouchDetector touchDetector;
    [SerializeField] private GameObject root;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text addedScore;
    [SerializeField] private Canvas _canvas;
    
    
    public Material mat;
    public float sizeMult;

    private Color[] _colors = {Color.red, Color.green, Color.yellow, Color.blue};
    
    private Line activeLine;
    private GameObject lineGO;
    private Collider2D collider2D;
    

    
    private int score = 0;

    private float scale = 3.0f;
    public bool RecalculateNormals = false;

    private Color _color;
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;
    private Vector3[] _baseVertices;
    
    private float m_scale = 0.1f;
    private Vector2[] m_path;
    

    public bool isFalling;
    public bool DoNothing;
    void Update()
    {
        

        if (Input.GetMouseButtonDown(0) && !isFalling)
        {
                    if (IsPointerOverUIObject())
                    {
                        
                         lineGO = Instantiate(linePrefab, drawPanel);
                         
                         activeLine = lineGO.GetComponent<Line>();

                         
                         
                         _color = _colors[Random.Range(0, _colors.Length)];

                    }
                    
        }
        
        if (isFalling)
        {
            touchDetector.DetectTouch(lineGO);
        }

        


        
        collider2D = lineGO.GetComponent<PolygonCollider2D>();
        
        
        var lineRenderer = lineGO.GetComponent<Line>().lineRenderer;
                if (Input.GetMouseButtonUp(0) && Vector2.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(lineRenderer.positionCount - 1)) < 0.3f)
                {
                    
                    if (lineRenderer.positionCount > 20)
                    {
                        sizeMult = Random.Range(50, 150);
                        lineRenderer.loop = true;
                        lineRenderer.endColor = lineRenderer.startColor =
                            activeLine.color = _color;
                       
                       
                        BakeLineDebuger (lineGO, false);
                        CreateMesh();
                        lineGO.GetComponent<Line>().FindArea();
                        lineGO.GetComponent<Line>().CalculateScore(lineGO.transform.localScale.x * sizeMult, false);


                        activeLine = null;

                        lineGO.transform.SetParent(gamePanel);
                        

                        lineGO.transform.position = root.transform.position;
                        
                        
                        Debug.Log(sizeMult);
                        lineGO.transform.localScale = new Vector3(1 * sizeMult, 1f * sizeMult);
                        ColliderBridge cb = collider2D.gameObject.AddComponent<ColliderBridge>();
                        cb.Initialize(this);
                        isFalling = true;


                    }else{
                        activeLine = null;
                        Destroy(lineGO);
                    }


                }else if (Input.GetMouseButtonUp(0) && Vector2.Distance(lineRenderer.GetPosition(0),
                    lineRenderer.GetPosition(lineRenderer.positionCount - 1)) > 0.5f)
                {
                    
                    sizeMult = Random.Range(100, 200);
                    lineRenderer.endColor = lineRenderer.startColor =
                        activeLine.color = _color;
                    
                   
                    BakeLineDebuger(lineGO, true);
                    lineGO.GetComponent<Line>().FindArea();
                    lineGO.GetComponent<Line>().CalculateScore(lineGO.transform.localScale.x * sizeMult, true);
                    //CreateMesh();
                    //


                    activeLine = null;

                    
                  
                    lineGO.transform.SetParent(gamePanel);
                        
                        


                    lineGO.transform.position = root.transform.position;
                        
                        
                    Debug.Log(sizeMult);
                    lineGO.transform.localScale = new Vector3(1 * sizeMult, 1f * sizeMult);
                    
                    ColliderBridge cb = collider2D.gameObject.AddComponent<ColliderBridge>();
                    cb.Initialize(this);
                    isFalling = true;
                }else if(Input.GetMouseButtonUp(0))
                {
                    Destroy(lineGO);
                    
                }
                
 
                if (activeLine != null && IsPointerOverUIObject())
                {
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    activeLine.UpdateLine(mousePos);
                }

                
            
                
                

    }


    private bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    

    public void BakeLineDebuger(GameObject lineObj, bool line)
    {
        var lineRenderer = lineObj.GetComponent<LineRenderer>();
        var meshFilter = lineObj.AddComponent<MeshFilter>();
        var pollyCol = lineObj.GetComponent<PolygonCollider2D>();
        if (line)
        {
            lineRenderer.widthMultiplier = 2;
            m_path = pollyCol.GetPath(0);
            pollyCol.SetPath(0, ScalePath(m_scale));

        }
        Mesh mesh = new Mesh();
        
        lineRenderer.BakeMesh(mesh, true);
        meshFilter.sharedMesh = mesh;
        var meshRenderer = lineObj.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;
        meshRenderer.material.color = _color;
        pollyCol.bounds.Encapsulate(meshFilter.mesh.bounds);
        
        GameObject.Destroy(lineRenderer);
        
    }
    

    void CreateMesh()
    {
        var polygonCollider2D = lineGO.GetComponent<PolygonCollider2D>();
        var meshFilter = lineGO.GetComponent<MeshFilter>();
        var meshRenderer = lineGO.GetComponent<MeshRenderer>();
        
        
         
 
        //Render thing
        int pointCount = 0;
        pointCount = polygonCollider2D.GetTotalPointCount();
        Mesh mesh = new Mesh();
        Vector2[] points = polygonCollider2D.points;
        Vector3[] vertices = new Vector3[pointCount];
        Vector2[] uv = new Vector2[pointCount];
        for (int j = 0; j < pointCount; j++)
        {
            Vector2 actual = points[j];
            vertices[j] = new Vector3(actual.x, actual.y, 0);
            
            uv[j] = actual;
 
            
        }
        Triangulator tr = new Triangulator(points);
        int[] triangles = tr.Triangulate();
        
        meshRenderer.material = mat;
        meshRenderer.material.color = _color;
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        meshFilter.mesh = mesh;
        
    }

    public void OnCollisionEnter2D(Collision2D other)
    {


        if (!other.collider.CompareTag("Ground"))
        {
            isFalling = false;

            

                
                


                if (lineGO.GetComponent<MeshRenderer>().material.color ==
                    other.gameObject.GetComponent<MeshRenderer>().material.color)
                {
                    if (DoNothing) return;
                    other.gameObject.GetComponent<Line>().DoNothing = true;

                    if (lineGO.GetComponent<Line>().totalScore >= 30)
                    {
                        addedScore.text = lineGO.GetComponent<Line>().totalScore.ToString();
                        Debug.Log(addedScore.text);
                        addedScore.DOFade(255, 0.1f);
                        score += lineGO.GetComponent<Line>().totalScore / 2;
                        Debug.Log(score);
                        scoreText.text = score.ToString();
                        addedScore.DOFade(0, 1f);
                        Destroy(lineGO);
                        Destroy(other.gameObject);
                    }


                }
            
        }



    }
    
    private Vector2[] ScalePath(float scale)
    {
        var scaledPath = new List<Vector2>();
 
        for (int i = 0; i < m_path.Length - 1; i++) {
 
            var p0 = m_path[(i - 1 < 0 ? 1 : i - 1)];
            var p1 = m_path[i];
            var p2 = m_path[(i + 1) % m_path.Length];
 
            var point1 = (p0 + p1) / 2f;
            var point2 = (p1 + p2) / 2f;
 
            var dx = point2.x - point1.x;
            var dy = point2.y - point1.y;
            var n = new Vector2(dy, -dx).normalized; // normalize for uniform effect
 
            scaledPath.Add(p1 + n * scale);
        }
 
        return scaledPath.ToArray();
    }
    

}


