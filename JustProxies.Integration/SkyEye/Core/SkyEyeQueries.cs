using System.Text.Json.Serialization;

namespace JustProxies.Integration.SkyEye;

public class SkyEyeQueries : SkyEyeQueriesRoot
{
    public SkyEyeQueries()
    {
        this.BeginTime = DateTime.Now.AddHours(-12).ToString("yyyy-MM-dd HH:mm:ss.fff");
        this.EndTime = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss.fff");
    }

    /// <summary>
    /// 模块 列表
    /// </summary>
    public List<string> Modules { get; set; } = new();

    /// <summary>
    /// 分类 列表
    /// </summary>
    public List<string> Categories { get; set; } = new();

    /// <summary>
    /// 子分类 列表
    /// </summary>
    public List<string> SubCategories { get; set; } = new();

    /// <summary>
    /// 过滤1 列表
    /// </summary>
    [JsonPropertyName("filter1s")]
    public List<string> Filter1List { get; set; } = new();

    /// <summary>
    /// 过滤2 列表
    /// </summary>
    [JsonPropertyName("filter2s")]
    public List<string> Filter2List { get; set; } = new();

    /// <summary>
    /// 跟踪ID 列表
    /// </summary>
    [JsonPropertyName("contextIdes")]
    public List<string> ContextIdList { get; set; } = new();

    /// <summary>
    /// ip 地址列表
    /// </summary>
    [JsonPropertyName("ips")]
    public List<string> IpList { get; set; } = new();


    /// <summary>
    /// 模块
    /// </summary>
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// 分类
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 子分类
    /// </summary>
    public string SubCategory { get; set; } = string.Empty;

    /// <summary>
    /// 过滤1
    /// </summary>
    public string Filter1 { get; set; } = string.Empty;

    /// <summary>
    /// 过滤2
    /// </summary>
    public string Filter2 { get; set; } = string.Empty;

    /// <summary>
    /// 日志级别(0:FATAL,1:ERROR,2:WARN,3:INFO,4:DEBUG)
    /// </summary>
    public string Priority { get; set; } = string.Empty;

    /// <summary>
    /// 环境
    /// </summary>
    public string Env { get; set; } = string.Empty;

    /// <summary>
    /// msg 模糊查询
    /// </summary>
    public string IndexContext { get; set; } = string.Empty;

    /// <summary>
    /// 不超过500
    /// </summary>
    public int PageSize { get; set; } = 500;

    /// <summary>
    /// 日志开始时间，务必带毫秒数！！yyyy-MM-dd HH:mm:ss.SSS，小于该时间
    /// </summary>
    public string BeginTime { get; set; }

    /// <summary>
    /// 日志结束时间，务必带毫秒数！！yyyy-MM-dd HH:mm:ss.SSS，大于等于该时间
    /// </summary>
    public string EndTime { get; set; }

    /// <summary>
    /// 向下分页
    /// </summary>
    public string OperationType { get; set; } = "next";

    /// <summary>
    /// 首页为0，上一页的最后一条数据的timestamp字段
    /// </summary>
    public long LastRowTime { get; set; } = 0;

    /// <summary>
    /// 首页为"",上一页的最后一条数据的id字段
    /// </summary>
    public string LastRowData { get; set; } = string.Empty;
}