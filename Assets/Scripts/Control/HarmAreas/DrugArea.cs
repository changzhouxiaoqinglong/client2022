
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 毒区
/// </summary>
public class DrugArea : HarmAreaBase
{
    /// <summary>
    /// 毒区路径
    /// </summary>
    private const string CREATE_DRUG_PATH = "Prefabs/Sprite/MaxMapIcon/DrugAreaNew";

    private const int TempRadio = 2;
    /// <summary>
    /// 毒数据
    /// </summary>
    public DrugVarData DrugVarData
    {
        get; set;
    }

    protected override void Start()
    {
        base.Start();
        //初始化大小 范围要*2
        //transform.localScale = Vector3.one * DrugVarData.Range * 2;
        //位置
        Vector3 pos = (SceneMgr.GetInstance().curScene as Train3DSceneCtrBase).terrainChangeMgr.GetTerrainPosByGis(DrugVarData.Pos.ToVector2());
        transform.position = new Vector3(pos.x, transform.position.y,pos.z);
        windDir = taskEnvVarData.Wearth.GetWindDir();
        windSp = taskEnvVarData.Wearth.WindSp;
        CreatePointRange(transform.position, windDir);
        StartCoroutine(ISetDrugAreaPoison());
    }

    /// <summary>
    /// 是否在范围内
    /// </summary>
    public override bool IsInRange(Vector3 pos)
    {
        return MathTool.IsPointInPolygon(pos, pointList);
    }

    /*    /// <summary>
        /// 获得对应位置的浓度
        /// </summary>
        public float GetDrugDentity(Vector3 pos)
        {
            //距离
            float dis = Vector3.Distance(transform.position, pos);
            if (dis > DrugVarData.Range)
            {
                //在范围之外
                return 0;
            }
            else
            {
                return (DrugVarData.Range - dis) / DrugVarData.Range * DrugVarData.Dentity;
            }
        }*/

    /// <summary>
    /// 获得对应位置的浓度
    /// </summary>
    //public float GetDrugDentity(Vector3 pos)
    //{
    //    if (MathTool.IsPointInPolygon(pos, pointList))
    //    {
    //        float dentity = DrugAreaConstanst.DRUG_DENTITY * GetHarmRange();//12000*500/1000
    //        float dis = Vector3.Distance(transform.position, pos);
    //        print(dis);
    //        float drugSize;
    //       // if (NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.Scene == SceneConstant.HILLS)
    //            drugSize = DrugAreaConstanst.DRUG_SIZE * GetHarmRange();
    //       // else
    //       //     drugSize = DrugAreaConstanst.DRUG_SIZE * GetHarmRange() / TempRadio;
    //        if (dis <= drugSize * 0.1)
    //        {
    //            return ((drugSize - dis) / drugSize) * dentity;
    //        }
    //        if(dis <= drugSize * 0.8)
    //        {
    //            return ((drugSize - dis) / drugSize) * 0.03f * dentity; 
    //        }
    //        return ((drugSize - dis) / drugSize) * 0.000001f * dentity;
    //    }
    //    return 0;
    //}

    /// <summary>
    /// 获得对应位置的浓度
    /// </summary>
    public float GetDrugDentity(Vector3 pos)
    {
        if (MathTool.IsPointInPolygon(pos, pointList))
        {                     
            Vector2 p1 = new Vector2(center.position.x, center.position.z);
            Vector2 p2 = new Vector2(pos.x, pos.z);
            Vector2 p3 = new Vector2(cubelist[3].position.x, cubelist[3].position.z);
            float maxdis = Vector2.Distance(p3,p1);
            float dis = Vector2.Distance(p1, p2);
            //print("maxdis: "+ maxdis+ "dis: " + dis);
            return DrugAreaConstanst.DRUG_DENTITY * (1- Mathf.Clamp01(dis / maxdis))+100000;
        }
        return 0;
    }


    /// <summary>
    /// 设置毒类型的图片和坐标
    /// </summary>
    public override string GetImageSpritePath()
    {
        return CREATE_DRUG_PATH;
    }

/*  /// <summary>
    /// 返回毒区的直径范围
    /// </summary>
    public override float GetHarmRange()
    {
        return DrugVarData.Range * 2;
    }*/


    /// <summary>
    /// 返回毒区的直径范围 释放速度越大 范围越大 
    /// </summary>
    public override float GetHarmRange() 
    {
        return 1;
       // return (DrugVarData.Speed / (float)DrugAreaConstanst.DRUG_SIZE) * HarmAreaBaseConstant.SIZE_RADIO / 2;//DrugVarData.Speed/1000
    }

    /// <summary>
    /// 更新坐标
    /// </summary>
    public override void SetPosition()
    {
        //transform.position = MathsMgr.PointDistance(windDir, 0,  transform.position);


        Vector3 pos = MathsMgr.PointDistance(windDir, windSp * HarmAreaBaseConstant.SPEED_RADIO ,  transform.position);        
        transform.position = new Vector3(pos.x, transform.position.y, pos.z);

        CreatePointRange(transform.position, windDir);
    }

    IEnumerator ISetDrugAreaPoison()
    {
        int count = 0;
        while(true)
        {
            yield return new WaitForSeconds(DrugAreaConstanst.DRUGAREA_UPDATEPOS_TIME);
            count++;
            SetPosition();
            if (count >= NetConfig.SITUATION_SYNC_POS_OFF_TIME)
            {
                SituationSyncLogic.SyncHarm(transform.position.ToCustVect3());
                count = 0;
            }

        }

    }


    [SerializeField]
    List<Transform> cubelist;
    public Transform center;
    /// <summary>
    /// 找到扇形所有区域点
    /// </summary>
    private void CreatePointRange(Vector3 startPos, float angle)
    {
        //print(DrugAreaConstanst.DRUG_SIZE * GetHarmRange());
        // if (NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.Scene == SceneConstant.HILLS)
        {
            pointList.Clear();

            Vector3 pos1 = MathsMgr.PointDistance(angle - 90, DrugAreaConstanst.DRUG_SIZE * GetHarmRange() * DrugAreaConstanst.MIN_DISTANCE/windSp, startPos);
            Vector3 pos2 = MathsMgr.PointDistance(angle + 90, DrugAreaConstanst.DRUG_SIZE * GetHarmRange() * DrugAreaConstanst.MIN_DISTANCE/windSp, startPos);
            pointList.Add(pos1);
            pointList.Add(pos2);
            Vector3 endPos = MathsMgr.PointDistance(angle, (DrugAreaConstanst.DRUG_SIZE * GetHarmRange()) * 0.91f, startPos);

            Vector3 pos3 = MathsMgr.PointDistance(angle - 90, DrugAreaConstanst.DRUG_SIZE * GetHarmRange() * DrugAreaConstanst.MAX_DISTANCE/windSp, endPos);
            Vector3 pos4 = MathsMgr.PointDistance(angle + 90, DrugAreaConstanst.DRUG_SIZE * GetHarmRange() * DrugAreaConstanst.MAX_DISTANCE/windSp, endPos);
            pointList.Add(pos4);//point要顺时针 不然有问题
            pointList.Add(pos3);

            if (cubelist.Count == 0)
            {
                cubelist = new List<Transform>();
                for (int i = 0; i < 5; i++)
                {
                    GameObject go = new GameObject();
                    go.name = "毒区扇形区域点" + i;
                    go.transform.parent = transform;
                    cubelist.Add(go.transform);
                }
                GameObject centerobj = new GameObject();
                center = centerobj.transform;
            }
            else
            {
                cubelist[0].transform.position = pos1;
                cubelist[1].transform.position = pos2;
                cubelist[2].transform.position = pos4;
                cubelist[3].transform.position = pos3;
                cubelist[4].transform.position = endPos;
                Debug.DrawLine(pos1, pos2, Color.red, DrugAreaConstanst.DRUGAREA_UPDATEPOS_TIME);
                Debug.DrawLine(pos2, pos4, Color.red, DrugAreaConstanst.DRUGAREA_UPDATEPOS_TIME);
                Debug.DrawLine(pos4, pos3, Color.red, DrugAreaConstanst.DRUGAREA_UPDATEPOS_TIME);
                Debug.DrawLine(pos3, pos1, Color.red, DrugAreaConstanst.DRUGAREA_UPDATEPOS_TIME);
                center.position=(pos1 + pos2 + pos3 + pos4 ) / 4;
            }
        }
        //else
        //{
        //    pointList.Clear();
        //    pointList.Add(MathsMgr.PointDistance(angle - 90, DrugAreaConstanst.DRUG_SIZE * GetHarmRange() * DrugAreaConstanst.MIN_DISTANCE / TempRadio, startPos));
        //    pointList.Add(MathsMgr.PointDistance(angle + 90, DrugAreaConstanst.DRUG_SIZE * GetHarmRange() * DrugAreaConstanst.MIN_DISTANCE / TempRadio, startPos));
        //    Vector3 endPos = MathsMgr.PointDistance(angle, (DrugAreaConstanst.DRUG_SIZE * GetHarmRange()) * 0.91f / TempRadio, startPos);
        //    pointList.Add(MathsMgr.PointDistance(angle - 90, DrugAreaConstanst.DRUG_SIZE * GetHarmRange() * DrugAreaConstanst.MAX_DISTANCE / TempRadio, endPos));
        //    pointList.Add(MathsMgr.PointDistance(angle + 90, DrugAreaConstanst.DRUG_SIZE * GetHarmRange() * DrugAreaConstanst.MAX_DISTANCE / TempRadio, endPos));
        //}
        
    
    
    }

}


public class DrugAreaConstanst
{
   /// <summary>
   /// 毒区中心浓度
   /// </summary>
    public const float DRUG_DENTITY = 1000;

    /// <summary>
    /// 毒区大小 默认500位基数1
    /// </summary>
    public const int DRUG_SIZE = 500;

    /// <summary>
    /// 中心两端的延长距离
    /// </summary>
    public const float MIN_DISTANCE = 0.12f;

    /// <summary>
    /// 底部两端的延长距离
    /// </summary>
    public const float MAX_DISTANCE = 0.44f;

    /// <summary>
    /// 毒区坐标更新间隔时间
    /// </summary>
    public const int DRUGAREA_UPDATEPOS_TIME = 1;

}