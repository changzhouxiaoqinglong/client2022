using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiologyArea : HarmAreaBase
{
    /// <summary>
    /// 毒区路径
    /// </summary>
    private const string CREATE_Biology_PATH = "Prefabs/Sprite/MaxMapIcon/DrugArea";

    private const int TempRadio = 2;
    /// <summary>
    /// 毒数据
    /// </summary>
    public BiologyData biologydata
    {
        get; set;
    }

    protected override void Start()
    {
        base.Start();
      
        Vector3 pos = (SceneMgr.GetInstance().curScene as Train3DSceneCtrBase).terrainChangeMgr.GetTerrainPosByGis(biologydata.Pos.ToVector2());
        transform.position = new Vector3(pos.x, transform.position.y, pos.z);
        windDir = taskEnvVarData.Wearth.GetWindDir();
        windSp = taskEnvVarData.Wearth.WindSp;
        CreatePointRange(transform.position, windDir);
        StartCoroutine(ISetBiologyAreaPoison());
    }

    /// <summary>
    /// 是否在范围内
    /// </summary>
    public override bool IsInRange(Vector3 pos)
    {
        return MathTool.IsPointInPolygon(pos, pointList);
    }

    public float GetBiologyDentity(Vector3 pos)
    {
        if (MathTool.IsPointInPolygon(pos, pointList))
        {
            Vector2 p1 = new Vector2(center.position.x, center.position.z);
            Vector2 p2 = new Vector2(pos.x, pos.z);
            Vector2 p3 = new Vector2(cubelist[3].position.x, cubelist[3].position.z);
            float maxdis = Vector2.Distance(p3, p1);
            float dis = Vector2.Distance(p1, p2);
            //print("maxdis: "+ maxdis+ "dis: " + dis);
            return BiologyAreaConstanst.Biology_DENTITY * (1 - Mathf.Clamp01(dis / maxdis));
        }
        return 0;
    }

    /// <summary>
    /// 设置毒类型的图片和坐标
    /// </summary>
    public override string GetImageSpritePath()
    {
        return CREATE_Biology_PATH;
    }

    public override float GetHarmRange()
    {
        return (biologydata.BiologySpeed / (float)BiologyAreaConstanst.Biology_SIZE)  / 2;//DrugVarData.Speed/1000
    }

    /// <summary>
    /// 更新坐标
    /// </summary>
    public override void SetPosition()
    {      
        Vector3 pos = MathsMgr.PointDistance(windDir, windSp * HarmAreaBaseConstant.SPEED_RADIO, transform.position);
        transform.position = new Vector3(pos.x, transform.position.y, pos.z);
        CreatePointRange(transform.position, windDir);
    }

    IEnumerator ISetBiologyAreaPoison()
    {
        int count = 0;
        while (true)
        {
            yield return new WaitForSeconds(BiologyAreaConstanst.Biology_UPDATEPOS_TIME);
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

            Vector3 pos1 = MathsMgr.PointDistance(angle - 90, BiologyAreaConstanst.Biology_SIZE * GetHarmRange() * BiologyAreaConstanst.MIN_DISTANCE, startPos);
            Vector3 pos2 = MathsMgr.PointDistance(angle + 90, BiologyAreaConstanst.Biology_SIZE * GetHarmRange() * BiologyAreaConstanst.MIN_DISTANCE, startPos);
            pointList.Add(pos1);
            pointList.Add(pos2);
            Vector3 endPos = MathsMgr.PointDistance(angle, (BiologyAreaConstanst.Biology_SIZE * GetHarmRange()) * 0.91f, startPos);

            Vector3 pos3 = MathsMgr.PointDistance(angle - 90, BiologyAreaConstanst.Biology_SIZE * GetHarmRange() * BiologyAreaConstanst.MAX_DISTANCE, endPos);
            Vector3 pos4 = MathsMgr.PointDistance(angle + 90, BiologyAreaConstanst.Biology_SIZE * GetHarmRange() * BiologyAreaConstanst.MAX_DISTANCE, endPos);
            pointList.Add(pos4);//point要顺时针 不然有问题
            pointList.Add(pos3);

            if (cubelist.Count == 0)
            {
                cubelist = new List<Transform>();
                for (int i = 0; i < 5; i++)
                {
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.name = "毒区扇形区域点" + i;
                    go.transform.parent = transform;
                    cubelist.Add(go.transform);
                }
                GameObject centerobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                center = centerobj.transform;
            }
            else
            {
                cubelist[0].transform.position = pos1;
                cubelist[1].transform.position = pos2;
                cubelist[2].transform.position = pos4;
                cubelist[3].transform.position = pos3;
                cubelist[4].transform.position = endPos;
                Debug.DrawLine(pos1, pos2, Color.red, BiologyAreaConstanst.Biology_UPDATEPOS_TIME);
                Debug.DrawLine(pos2, pos4, Color.red, BiologyAreaConstanst.Biology_UPDATEPOS_TIME);
                Debug.DrawLine(pos4, pos3, Color.red, BiologyAreaConstanst.Biology_UPDATEPOS_TIME);
                Debug.DrawLine(pos3, pos1, Color.red, BiologyAreaConstanst.Biology_UPDATEPOS_TIME);
                center.position = (pos1 + pos2 + pos3 + pos4) / 4;
            }
        }
        



    }

}


public class BiologyAreaConstanst
{
    /// <summary>
    /// 生物区中心浓度
    /// </summary>
    public const float Biology_DENTITY = 1000;

    /// <summary>
    ///  生物区大小 默认500位基数1
    /// </summary>
    public const int Biology_SIZE = 500;

    /// <summary>
    /// 中心两端的延长距离
    /// </summary>
    public const float MIN_DISTANCE = 0.12f;

    /// <summary>
    /// 底部两端的延长距离
    /// </summary>
    public const float MAX_DISTANCE = 0.44f;

    /// <summary>
    ///  生物区坐标更新间隔时间
    /// </summary>
    public const int Biology_UPDATEPOS_TIME = 1;
}
