using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line3dControl : MonoBehaviour
{
    public static Line3dControl Instance;

    private GameObject lineModel;

    private List<Vector3> listPoint = new List<Vector3>();

    private const float lineDistance = 40f;

    private void Awake()
    {
        Instance = this;
        lineModel = transform.GetChild(0).gameObject;
    }

    GameObject linerender;
    /// <summary>
    /// 创建线
    /// </summary>
    /// <param name="lines"></param>
    /// <param name="radio"></param>
    public void CreateLine(Transform[] lines,Vector2 radio, Transform[] point, Vector3 carPos)
    {
        Debug.Log("TerrainY" + (SceneMgr.GetInstance().curScene as Train3DSceneCtrBase).terrainChangeMgr.curTerrain.transform.position.y);
        Debug.Log(radio);
        float posY = (SceneMgr.GetInstance().curScene as Train3DSceneCtrBase).terrainChangeMgr.curTerrain.transform.position.y + 380;
        for (int i = 1; i < lines.Length; i++)
        {
            RectTransform lineRect = lines[i].GetComponent<RectTransform>();
            GameObject obj = Instantiate(lineModel,transform);
            Vector2 lineSize = lineRect.sizeDelta;
            Vector2 linePos = lineRect.anchoredPosition;
            Vector3 lineAngle = new Vector3(0, -lineRect.localEulerAngles.z, 0);
            float x=Mathf.Sqrt(Mathf.Pow(radio.x, 2) + Mathf.Pow(radio.y, 2));
           // Debug.Log(x);
             Vector3 lineScale = new Vector3(lineSize.x / 10 * radio.x, lineRect.localScale.y, lineRect.localScale.z);
           // Vector3 lineScale = new Vector3(lineSize.x*x / 10 , lineRect.localScale.y, lineRect.localScale.z);
            obj.transform.localPosition = new Vector3(radio.x * linePos.x - 5, posY, radio.y * linePos.y);
            obj.transform.localEulerAngles = lineAngle;
            obj.transform.localScale = lineScale;
            if (i == 1)
                CreateFristPath(new Vector3(obj.transform.localPosition.x,obj.transform.localPosition.y,obj.transform.localPosition.z-10), obj.transform.localScale.x * 10, lineDistance, obj.transform.localEulerAngles.y);
            else
                CreateAreaPath(obj.transform.localPosition, obj.transform.localScale.x * 10, lineDistance, obj.transform.localEulerAngles.y);
        }
         linerender = new GameObject("Linerender");
        linerender.layer =5;
        LineRenderer lineRenderer = linerender.AddComponent<LineRenderer>();
        lineRenderer.positionCount = point.Length;
        lineRenderer.startWidth = 10f;
        lineRenderer.endWidth = 10f;
        lineRenderer.SetPosition(0, new Vector3(radio.x * carPos.x, posY, radio.y * carPos.y));

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        // lineRenderer.startColor = Color.red;
        // lineRenderer.endColor = Color.blue;
        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.blue;
        for (int i = 1; i < point.Length; i++)
        {
           
           Vector2 pointPos = point[i].GetComponent<RectTransform>().anchoredPosition;
            Vector3 temp = new Vector3(radio.x * pointPos.x, posY, radio.y * pointPos.y);

           // lineRenderer.positionCount+=1;
            lineRenderer.SetPosition(i, temp);

            continue;
            Vector2 pointPospre;
            Vector3 pre;
            if(i==1)
			{
                pre = new Vector3(radio.x * carPos.x, posY, radio.y * carPos.y);
            }
            else
			{
                pointPospre = point[i - 1].GetComponent<RectTransform>().anchoredPosition;
                pre = new Vector3(radio.x * pointPospre.x, posY, radio.y * pointPospre.y);
            }
            



            float  dot=Vector3.Dot(pre.normalized,temp.normalized);
            float angle = Mathf.Acos(dot)*Mathf.Rad2Deg;
            Debug.Log("夹角为 "+angle);

            RectTransform lineRect = lines[i].GetComponent<RectTransform>();
            Vector3 lineAngle = new Vector3(0, -lineRect.localEulerAngles.z, 0);
            transform.GetChild(i).transform.localEulerAngles = lineAngle;

            transform.GetChild(i).transform.localPosition = pre;
            //  transform.GetChild(i).transform.localPosition = new Vector3((pre.x + temp.x) / 2, posY, (pre.z + temp.z) / 2);
            transform.GetChild(i).localScale = new Vector3(Vector3.Distance(pre, temp) / 10, 1, 1);
            //Vector3.Angle(pre,temp);
            //transform.GetChild(i).localEulerAngles = new Vector3(0, Vector3.Angle(pre, temp), 0); ;
        }
    }


   


    /// <summary>
    /// 销毁线
    /// </summary>
    public void DestroyLine()
    {
        for(int i = 1; i<transform.childCount;i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        listPoint.Clear();
        Destroy(linerender);
    }


    /// <summary>
    /// 创建路径上的点
    /// </summary>
    /// <param name="startPos">开始的位置</param>
    /// <param name="size">线的长度</param>
    /// <param name="distance">偏航距离</param>
    /// <param name="angle">偏航角度</param>
    private void CreateAreaPath(Vector3 startPos,float size,float distance, float angle)
    {
        listPoint.Add(MathsMgr.PointDistance(startPos, distance, angle - 90));
        listPoint.Add(MathsMgr.PointDistance(startPos, distance, angle + 90));
        Vector3 endPos = MathsMgr.PointDistance(startPos, size, angle);
        listPoint.Add(MathsMgr.PointDistance(endPos, distance, angle - 90));
        listPoint.Add(MathsMgr.PointDistance(endPos, distance, angle + 90));
    }

    private void CreateFristPath(Vector3 startPos,float size,float distance,float angle)
    {
        listPoint.Add(MathsMgr.PointDistance(startPos, distance, angle - 90));
        listPoint.Add(MathsMgr.PointDistance(startPos, distance, angle + 90));
        Vector3 endPos = MathsMgr.PointDistance(startPos, size, angle);
        listPoint.Add(MathsMgr.PointDistance(endPos, distance, angle - 90));
        listPoint.Add(MathsMgr.PointDistance(endPos, distance, angle + 90));
    }

    /// <summary>
    /// 获取路径数组
    /// </summary>
    /// <returns></returns>
    public List<Vector3> GetListPoint()
    {
        return listPoint;
    }
}
