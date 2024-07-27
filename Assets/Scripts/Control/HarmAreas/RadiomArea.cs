

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 辐射区域
/// </summary>
public class RadiomArea : HarmAreaBase
{
    private const string CREATE_RADIOM_PATH = "Prefabs/Sprite/MaxMapIcon/RadiomArea";

    private List<Vector3> radiomList = new List<Vector3>();

    private Train3DSceneCtrBase scene3D;


    /// <summary>
    /// 当前3d场景
    /// </summary>
    private Train3DSceneCtrBase Scene3D
    {
        get
        {
            if (scene3D == null)
            {
                if (SceneMgr.GetInstance().curScene is Train3DSceneCtrBase curScene)
                {
                    scene3D = curScene;
                }
            }
            return scene3D;
        }
    }

    /// <summary>
    /// 自己所在的车
    /// </summary>
    private CarBase selfCar;

    private CarBase SelfCar
    {
        get
        {
            if (selfCar == null)
            {
                if (Scene3D != null)
                {
                    selfCar = Scene3D.carMgr.GetCarByMachineId(AppConfig.MACHINE_ID);
                }
            }
            return selfCar;
        }
    }

    /// <summary>
    /// 辐射数据
    /// </summary>
    public RadiatVarData RadiatVarData
    {
        get; set;
    }

    /// <summary>
    /// 是否上报了接近测量点时的车速
    /// </summary>
    private bool HaveReportCheckSpeed = false;

    protected override void Start()
    {
        base.Start();
        //初始化大小 范围要*2
        //transform.localScale = Vector3.one * RadiatVarData.Range * 2;
        //位置
        transform.position = (SceneMgr.GetInstance().curScene as Train3DSceneCtrBase).terrainChangeMgr.GetTerrainPosByGis(RadiatVarData.Pos.ToVector3());
        windDir = taskEnvVarData.Wearth.GetWindDir();
        windSp = taskEnvVarData.Wearth.GetWindSp();
        CreatePointRange(transform.position, windDir);
        //StartCoroutine(ISetRadiomAreaPoison());
    }

    private void Update()
    {
        CheckSpeedReport();
    }

    /// <summary>
    /// 是否在范围内
    /// </summary>
    public override bool IsInRange(Vector3 pos)
    {
        return MathTool.IsPointInPolygon(pos, pointList);
    }

    /*    /// <summary>
        /// 获得对应位置的剂量率
        /// </summary>
        public float GetRadiomRate(Vector3 pos)
        {
            //距离
            float dis = Vector3.Distance(transform.position, pos);
            if (dis > RadiatVarData.Range)
            {
                //在范围之外
                return 0;
            }
            else
            {
                return (RadiatVarData.Range - dis) / RadiatVarData.Range * RadiatVarData.DoseRate;
            }
        }*/


    /// <summary>
    /// 获得对应位置的剂量率
    /// </summary>
    public float GetRadiomRate(Vector3 pos)
    {
        if (MathTool.IsPointInPolygon(pos, pointList))
        {
            Vector2 p1 = new Vector2(center.position.x, center.position.z);
            Vector2 p2 = new Vector2(pos.x, pos.z);
            Vector2 p3 = new Vector2(cubelist[3].position.x, cubelist[3].position.z);
            float maxdis = Vector2.Distance(p3, p1);
            float dis = Vector2.Distance(p1, p2);
            //print("maxdis: "+ maxdis+ "dis: " + dis);
            return RadiomAreaConstanst.RADIOM_DENTITY * (1 - Mathf.Clamp01(dis / maxdis))+100000;
        }
        return 0;
    }


    ///// <summary>
    ///// 获得对应位置的剂量率
    ///// </summary>
    //public float GetRadiomRate(Vector3 pos)
    //{
    //    if (MathTool.IsPointInPolygon(pos, pointList))
    //    {
    //        float dentity = RadiomAreaConstanst.RADIOM_DENTITY * GetHarmRange();
    //        if (dentity > RadiomAreaConstanst.RADIOM_MAX_DENTITY)
    //            dentity = RadiomAreaConstanst.RADIOM_MAX_DENTITY;
    //        float dis = Vector3.Distance(transform.position, pos);
    //        float radiomSize;
    //      //  if (NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.Scene == SceneConstant.HILLS)
    //            radiomSize = RadiomAreaConstanst.RADIOM_SIZE * GetHarmRange();
    //       // else
    //       //     radiomSize = RadiomAreaConstanst.RADIOM_SIZE * GetHarmRange() / RadiomAreaConstanst.TempRadio;
    //        if (dis <= radiomSize * 0.4)
    //        {
    //            return ((radiomSize - dis) / radiomSize) * dentity;
    //        }
    //        if (dis <= radiomSize * 0.8)
    //        {
    //            return ((radiomSize - dis) / radiomSize) * 0.01f * dentity;
    //        }
    //        return ((radiomSize - dis) / radiomSize) * 0.00001f * dentity;
    //    }
    //    return 0;
    //}

    /// <summary>
    /// 接近测量点 车速上报逻辑
    /// </summary>
    private void CheckSpeedReport()
    {
        if (HaveReportCheckSpeed || AppConfig.SEAT_ID != SeatType.DRIVE)
        {
            return;
        }
        if (SelfCar != null)
        {
            //车子在范围内 延时上报速度
            if (IsInRange(SelfCar.transform.position))
            {
                //延时上报
                this.InvokeByYield(() =>
                {
                    CheckSpeedModel model = new CheckSpeedModel()
                    {
                        CurSpeed = selfCar.vehicleCtr.GetCurSpeed(),
                    };
                    NetManager.GetInstance().SendMsg(ServerType.GuideServer, JsonTool.ToJson(model), NetProtocolCode.CHECK_POINT_SPEED);
                }, new WaitForSeconds(AppConstant.CHECK_SPEED_DELAY));
                HaveReportCheckSpeed = true;
            }
        }
    }


    /// <summary>
    /// 获取辐射类型图片路径
    /// </summary>

    public override string GetImageSpritePath()
    {
        return CREATE_RADIOM_PATH;
    }

    /*    /// <summary>
        /// 返回毒区的直径范围
        /// </summary>
        public override float GetHarmRange()
        {
            return RadiatVarData.Range * 2;
        }*/

    public override float GetHarmRange()
    {
        return 2;
        return RadiatVarData.DangLiang / 1000.0f;
        return (RadiatVarData.DangLiang * RadiomAreaConstanst.RADIOM_RADIO / RadiomAreaConstanst.RADIOM_SIZE) * HarmAreaBaseConstant.SIZE_RADIO;
    }

    public override void SetPosition()
    {
        transform.position = MathsMgr.PointDistance(windDir, 0, transform.position);
        //transform.position = MathsMgr.PointDistance(windDir, windSp * HarmAreaBaseConstant.SPEED_RADIO, transform.position);
        CreatePointRange(transform.position, windDir);
    }

    IEnumerator ISetRadiomAreaPoison()
    {
        int count = 0;
        while (true)
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
        float radiomDis = RadiomAreaConstanst.RADIOM_SIZE * GetHarmRange();
        
        print(radiomDis);
       // if (NetVarDataMgr.GetInstance()._NetVarData._TaskEnvVarData.Scene == SceneConstant.HILLS)
        {
            pointList.Clear();
            Vector3 pos1 = MathsMgr.PointDistance(angle - 90, radiomDis * RadiomAreaConstanst.MIN_DISTANCE / GetCurvePosition(windSp), startPos);
            Vector3 pos2 = MathsMgr.PointDistance(angle + 90, radiomDis * RadiomAreaConstanst.MIN_DISTANCE / GetCurvePosition(windSp), startPos);

            pointList.Add(pos1);
            pointList.Add(pos2);

            Vector3 endPos = MathsMgr.PointDistance(angle, radiomDis * 0.91f, startPos);

            Vector3 pos3 = MathsMgr.PointDistance(angle - 90, radiomDis * RadiomAreaConstanst.MAX_DISTANCE / GetCurvePosition(windSp), endPos);
            Vector3 pos4 = MathsMgr.PointDistance(angle + 90, radiomDis * RadiomAreaConstanst.MAX_DISTANCE / GetCurvePosition(windSp), endPos);

            pointList.Add(pos4);
            pointList.Add(pos3);


          //  if (cubelist.Count == 0)
            {
                cubelist = new List<Transform>();
                for (int i = 0; i < 5; i++)
                {
                    //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    GameObject go = new GameObject();
                    go.name = "辐射区扇形区域点" + i;
                    go.transform.parent = transform;
                    cubelist.Add(go.transform);
                }
                GameObject centerobj = new GameObject(); 
                center = centerobj.transform;
            }
          //  else
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
                center.position = (pos1 + pos2 + pos3 + pos4) / 4;
            }

        }
        //else
        //{
        //    pointList.Clear();
        //    pointList.Add(MathsMgr.PointDistance(angle - 90, radiomDis * RadiomAreaConstanst.MIN_DISTANCE / RadiomAreaConstanst.TempRadio, startPos));
        //    pointList.Add(MathsMgr.PointDistance(angle + 90, radiomDis * RadiomAreaConstanst.MIN_DISTANCE / RadiomAreaConstanst.TempRadio, startPos));
        //    Vector3 endPos = MathsMgr.PointDistance(angle, radiomDis * 0.91f / RadiomAreaConstanst.TempRadio, startPos);
        //    pointList.Add(MathsMgr.PointDistance(angle - 90, radiomDis * RadiomAreaConstanst.MAX_DISTANCE / RadiomAreaConstanst.TempRadio, endPos));
        //    pointList.Add(MathsMgr.PointDistance(angle + 90, radiomDis * RadiomAreaConstanst.MAX_DISTANCE / RadiomAreaConstanst.TempRadio, endPos));

        //    radiomList.Clear();
        //    Vector3 startPosIn = MathsMgr.PointDistance(angle, radiomDis * 0.1f / RadiomAreaConstanst.TempRadio, startPos);
        //    radiomList.Add(MathsMgr.PointDistance(angle - 90, radiomDis * RadiomAreaConstanst.MIN_DISTANCE_IN / RadiomAreaConstanst.TempRadio, startPosIn));
        //    radiomList.Add(MathsMgr.PointDistance(angle + 90, radiomDis * RadiomAreaConstanst.MIN_DISTANCE_IN / RadiomAreaConstanst.TempRadio, startPosIn));
        //    Vector3 endPosIn = MathsMgr.PointDistance(angle, radiomDis * 0.6f / RadiomAreaConstanst.TempRadio, startPosIn);
        //    radiomList.Add(MathsMgr.PointDistance(angle - 90, radiomDis * RadiomAreaConstanst.MAX_DISTANCE_IN / RadiomAreaConstanst.TempRadio,  endPosIn));
        //    radiomList.Add(MathsMgr.PointDistance(angle + 90, radiomDis * RadiomAreaConstanst.MAX_DISTANCE_IN / RadiomAreaConstanst.TempRadio, endPosIn));

        //}
    }

    public class RadiomAreaConstanst
    {
        /// <summary>
        /// 辐射比值
        /// </summary>
        public const float RADIOM_RADIO = 150f;


        /// <summary>
        /// 辐射大小比值
        /// </summary>
        public const float RADIOM_SIZE = 500;


        /// <summary>
        /// 辐射浓度
        /// </summary>
        public const float RADIOM_DENTITY = 1000;


        /// <summary>
        /// 辐射缩放比值
        /// </summary>
        public const int TempRadio = 2;

        /// <summary>
        /// 辐射最大浓度
        /// </summary>
       // public const int RADIOM_MAX_DENTITY = 20000;

        public const float MIN_DISTANCE = 0.15f;

        public const float MAX_DISTANCE = 0.44f;

        public const float MIN_DISTANCE_IN = 0.06f;

        public const float MAX_DISTANCE_IN = 0.32f;

    }


}
