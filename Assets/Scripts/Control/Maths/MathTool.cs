using UnityEngine;
using System.Collections.Generic;

public class MathTool
{

    /// <summary>
    /// 点是否在多边形范围内
    /// </summary>
    /// <param name="p">点</param>
    /// <param name="vertexs">多边形顶点列表</param>
    /// <returns></returns>
    public static bool IsPointInPolygon(Vector3 p, List<Vector3> vertexs)
    {
        return ContainsPoint(vertexs.ToArray(), p);

        int crossNum = 0;
        int vertexCount = vertexs.Count;
        List<Vector3> tempList = new List<Vector3>();
        for(int i = 0;i<vertexCount;i+=2)
        {
            tempList.Add(vertexs[i]);
        }
        for (int i = vertexCount - 1; i >= 0; i -= 2)
        {
            tempList.Add(vertexs[i]);
        }

        for (int i = 0; i < vertexCount; i++)
        {
            Vector3 v1 = tempList[i];
            Vector3 v2 = tempList[(i + 1) % vertexCount];

            if (((v1.z <= p.z) && (v2.z > p.z))
                || ((v1.z> p.z) && (v2.z <= p.z)))
            {
                if (p.x < v1.x + (p.z - v1.z) / (v2.z - v1.z) * (v2.x - v1.x))
                {
                    crossNum += 1;
                }
            }
        }

        if (crossNum % 2 == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static bool ContainsPoint(Vector3[] polyPoints, Vector3 p)
    {
        var j = polyPoints.Length - 1;
        var inside = false;
        for (int i = 0; i < polyPoints.Length; j = i++)
        {
            var pi = polyPoints[i];
            var pj = polyPoints[j];
            if (((pi.z <= p.z && p.z < pj.z) || (pj.z <= p.z && p.z < pi.z)) &&
                (p.x < (pj.x - pi.x) * (p.z - pi.z) / (pj.z - pi.z) + pi.x))
                inside = !inside;
        }
        return inside;
    }
}