#region 描述
//-----------------------------------------------------------------------------
// 文 件 名: Utilities
// 作    者：d.w
// 创建时间：2015/11/11 8:55:13
// 描    述：
// 版    本：1.0.0.0
//-----------------------------------------------------------------------------
// 历史更新纪录
//-----------------------------------------------------------------------------
// 版    本：           修改时间：           修改人：           
// 修改内容：
//-----------------------------------------------------------------------------
// Copyright (C) 2009-2015 TechQuick . All Rights Reserved.
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
public static class Utilities
{
    public const string JSESSIONID = "JSESSIONID";

    /// <summary>
    /// 创建唯一ID
    /// </summary>
    /// <returns></returns>
    public static string NewId()
    {
        return Guid.NewGuid().ToString("N");
    }
    public class DownloadContent
    {
        /// <summary>
        /// 
        /// </summary>
        public string CommandKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TargetFileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int DelaySeconds { get; set; }

    }

    public class PingResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int FailureCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double AverageResponseTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double MinimumResponseTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double MaximumResponseTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PingLog { get; set; }

    }



    public class TraceResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int ResponseTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int FailureCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int HopsNumberOfEntries { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TraceLog { get; set; }

    }



    public class DiagResults
    {
        /// <summary>
        /// 
        /// </summary>
        public PingResult PingResult { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TraceResult TraceResult { get; set; }

    }

    public class UploadContent
    {
        /// <summary>
        /// 
        /// </summary>
        public string CommandKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string URL { get; set; }

    }

    public class ReqPara
    {
        /// <summary>
        /// 
        /// </summary>
        public string IMEI { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int UsrOperationType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RequestUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DownloadContent DownloadContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UploadContent UploadContent { get; set; }
    }
    public static class JSON
    {

    }
    public static Dictionary<string, string> D_Auth=new Dictionary<string, string> ();
    public static Dictionary<string, List<string>> D_GetParameterValues = new Dictionary<string, List<string>>();
    public static Dictionary<string, long> D_CpeUpStreams = new Dictionary<string, long>();
    public static Dictionary<string, long> D_CpeDownStreams = new Dictionary<string, long>();
    public static Dictionary<string, string> D_UserOps = new Dictionary<string, string>();
    public static Dictionary<string, string> D_DiagResults = new Dictionary<string, string>();
}
