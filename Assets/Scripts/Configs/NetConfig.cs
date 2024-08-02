/// <summary>
/// 网络配置
/// </summary>
public class NetConfig : ConfigBase<NetConfig>
{
    /// <summary>
    /// 导控服务器 ip地址
    /// </summary>
    public static string SERVER_IP;

    /// <summary>
    /// 导控服务器 端口号
    /// </summary>
    public static int SERVER_PORT;

    /// <summary>
    /// TCP连接 消息包头的大小 包头保存包体大小 int型 占4字节
    /// </summary>
    public static int TCP_MESSAGE_HEAD_LEN=4;

    /// <summary>
    /// 心跳上报间隔 ms
    /// </summary>
    public static int HEART_BEAT_OFFTIME=3000;

    /// <summary>
    /// 断开连接 心跳超时时间 s
    /// </summary>
    public static int HEART_BEAT_TIME_OUT=15;

    /// <summary>
    /// 同步驾驶画面Server端IP
    /// </summary>
    public static string SYS_SCREEN_IP;

    /// <summary>
    /// 同步驾驶画面Server端监听端口
    /// </summary>
    public static int FM_SERVER_PORT=3333;

    /// <summary>
    /// 同步驾驶画面Client端监听端口
    /// </summary>
    public static int FM_CLIENT_PORT = 3334;

    /// <summary>
    /// 同步102遥测画面Server端监听端口
    /// </summary>
    public static int FM_102yaoce_SERVER_PORT = 3337;

    /// <summary>
    /// 同步102遥测画面Client端监听端口
    /// </summary>
    public static int FM_102yaoce_CLIENT_PORT = 3338;


    /// <summary>
    /// 同步全屏画面Server端监听端口
    /// </summary>
    public static int FM_SERVER_FULL_PORT= 3335;

    /// <summary>
    /// 同步全屏画面Client端监听端口
    /// </summary>
    public static int FM_CLIENT_FULL_PORT=3336;

    /// <summary>
    /// 状态同步每秒次数
    /// </summary>
    public static int SYNC_SECOND_TIMES=10;

    /// <summary>
    /// 位置态势同步上报间隔s
    /// </summary>
    public static int SITUATION_SYNC_POS_OFF_TIME=2;

    /// <summary>
    /// FTP文件保留时间
    /// </summary>
    public static int FTP_SAVE_TIME=7;


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

    public static void InitConfig()
    {
        ParseConfigByReflection("NetConfig.cfg");
    }
}
