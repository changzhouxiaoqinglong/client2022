/// <summary>
/// 配置
/// </summary>
public class AppConfig : ConfigBase<AppConfig>
{
    /// <summary>
    /// 机号
    /// </summary>
    public static int Client_ID;

    /// <summary>
    /// 席位号
    /// </summary>
    public static int Person_ID;

    /// <summary>
    /// 车型
    /// </summary>
    public static int Device_ID;


    /// <summary>
    /// 机号
    /// </summary>
    public static int MACHINE_ID;

    /// <summary>
    /// 席位号
    /// </summary>
    public static int SEAT_ID;

    /// <summary>
    /// 车型
    /// </summary>
    public static int CAR_ID;

    public static void InitConfig()
    {
        ParseConfigByReflection("AppConfig.cfg");
        MACHINE_ID = Client_ID;
        SEAT_ID = Person_ID;
        CAR_ID = Device_ID;
        
       // UnityEngine.Debug.Log(MACHINE_ID);
       // UnityEngine.Debug.Log(SEAT_ID);
       // UnityEngine.Debug.Log(CAR_ID);
    }
}
