#region 描述
//-----------------------------------------------------------------------------
// 文 件 名: TemplateHelper
// 作    者：d.w
// 创建时间：2015/4/29 13:13:30
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
using System.Text;
using System.Xml;
using System.Data;
using System.Text.RegularExpressions;
using System.Net;
using System.Security.Cryptography;
using System.Globalization;
using System.IO;
using Microsoft.ApplicationBlocks.Data;

public static class TemplateHelper
{
    /// <summary>
    /// Envelope模板 消息主模板
    /// </summary>
    /// <param name="id"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public static string CreateEnvelopeTemplate(string id, string body)
    {
        StringBuilder soap = new StringBuilder();
        soap.Append(Environment.NewLine + "<soap:Envelope soap:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:soapenc=\"http://schemas.xmlsoap.org/soap/encoding/\" xmlns:cwmp=\"urn:dslforum-org:cwmp-1-0\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
        soap.Append(Environment.NewLine + "  <soap:Header>");
        soap.Append(Environment.NewLine + "    <cwmp:ID soap:mustUnderstand=\"1\">" + id + "</cwmp:ID>");
        soap.Append(Environment.NewLine + "  </soap:Header>");
        soap.Append(Environment.NewLine + "  <soap:Body>");
        soap.Append(body);
        soap.Append(Environment.NewLine + "  </soap:Body>");
        soap.Append(Environment.NewLine + "</soap:Envelope>");
        return soap.ToString();
    }

    /// <summary>
    /// InformResponse模板
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string CreateInformResponseTemplate(string id)
    {
        //ACS接受设备端(cpe)端的inform请求(即创建cwmp连接的请求)
        StringBuilder soap = new StringBuilder();
        soap.Append(Environment.NewLine + "    <cwmp:InformResponse>");
        soap.Append(Environment.NewLine + "      <MaxEnvelopes>1</MaxEnvelopes>");
        soap.Append(Environment.NewLine + "    </cwmp:InformResponse>");
        return CreateEnvelopeTemplate(id, soap.ToString());
    }

    /// <summary>
    /// 用于响应CPE汇报上传下载完成消息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string CreateTransferCompleteResponseTemplate(string id)
    {
        StringBuilder soap = new StringBuilder();
        soap.Append(Environment.NewLine + "    <TransferCompleteResponse></TransferCompleteResponse>");
        return CreateEnvelopeTemplate(id, soap.ToString());
    }

    //在设备发起POST Inform 6 CONNECTION REQUEST ，同时ACS Response后
    //1.设备发起空POST
    //2.Response 请求内容 //是否需要设置StatusCode为200，目的是对先前一个消息的确认，序列号合法
    //3.设备发起回应POST
    //4.Response 空内容，结束。//是否需要设置StatusCode为200，目的是对上个流程进行确认

    #region GetRPCMethods
    /// <summary>
    /// 参数模型查询 用于发现所支持的方法集
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string CreateGetRPCMethodsTemplate(string id)
    {
        StringBuilder soap = new StringBuilder();
        soap.Append(Environment.NewLine + "    <cwmp:GetRPCMethods />");
        return CreateEnvelopeTemplate(id, soap.ToString());
        #region 响应内容
        //cwmp:GetRPCMethodsResponse MethodList
        #endregion
    }
    #endregion

    #region GetParameterNames
    /// <summary>
    /// 查询家庭网关设备参数模型
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ParameterPath"></param>
    /// <param name="NextLevel"></param>
    /// <returns></returns>
    public static string CreateGetParameterNames(string id, string ParameterPath, bool NextLevel)
    {
        StringBuilder soap = new StringBuilder();
        soap.Append(Environment.NewLine + "    <cwmp:GetParameterNames>");
        soap.Append(Environment.NewLine + "      <ParameterPath>" + ParameterPath + "</ParameterPath>");
        soap.Append(Environment.NewLine + "      <NextLevel>" + (NextLevel ? "0" : "1") + "</NextLevel>");
        soap.Append(Environment.NewLine + "    </cwmp:GetParameterNames>");
        return CreateEnvelopeTemplate(id, soap.ToString());
        #region 响应内容
        //cwmp:GetParameterNamesResponse ParameterList
        #endregion
    }
    #endregion

    #region Set/GetParameterValues
    /// <summary>
    /// 修改家庭网关设备的一个或多个参数
    /// </summary>
    /// <param name="id"></param>
    /// <param name="parameterKey">可以用于acs，来识别参数更新，或者让其为空</param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string CreateSetParameterValuesTemplate(string id, string parameterKey, List<ParameterValueStruct> list)
    {
        StringBuilder soap = new StringBuilder();
        soap.Append(Environment.NewLine + "    <cwmp:SetParameterValues>");
        soap.Append(Environment.NewLine + "      <ParameterList soapenc:arrayType=\"cwmp:ParameterValueStruct[" + list.Count + "]\">");
        foreach (ParameterValueStruct pvs in list)
        {
            soap.Append(Environment.NewLine + "        <ParameterValueStruct>");
            //soap.Append(Environment.NewLine + "          <Name xsi:type=\"" + pvs.type + "\">" + pvs.name + "</Name>");
            //soap.Append(Environment.NewLine + "          <Value xsi:type=\"" + pvs.type + "\">" + pvs.value + "</Value>");
            soap.Append(Environment.NewLine + "          <Name>" + pvs.name + "</Name>");
            soap.Append(Environment.NewLine + "          <Value>" + pvs.value + "</Value>");
            soap.Append(Environment.NewLine + "        </ParameterValueStruct>");
        }
        soap.Append(Environment.NewLine + "      </ParameterList>");
        soap.Append(Environment.NewLine + "      <ParameterKey xsi:type=\"xsd:string\">" + parameterKey + "</ParameterKey>");
        soap.Append(Environment.NewLine + "    </cwmp:SetParameterValues>");
        return CreateEnvelopeTemplate(id, soap.ToString());
        #region 响应内容
        //cwmp:SetParameterValuesResponse
        #endregion
    }

    /// <summary>
    /// 获取一个或多个家庭网关设备参数的值
    /// </summary>
    /// <param name="id"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string CreateGetParameterValuesTemplate(string id, List<string> list)
    {
        StringBuilder soap = new StringBuilder();
        soap.Append(Environment.NewLine + "    <cwmp:GetParameterValues>");
        soap.Append(Environment.NewLine + "      <ParameterNames soapenc:arrayType=\"xsd:string[" + list.Count + "]\">");
        foreach (string s in list)
        {
            soap.Append(Environment.NewLine + "        <string>" + s + "</string>");
        }
        soap.Append(Environment.NewLine + "      </ParameterNames>");
        soap.Append(Environment.NewLine + "    </cwmp:GetParameterValues>");
        return CreateEnvelopeTemplate(id, soap.ToString());
        #region 响应内容
        //cwmp:GetParameterValuesResponse
        #endregion
    }
    #endregion

    #region Set/GetParameterAttributes
    /// <summary>
    /// 用于修改CPE上一个或者多个参数的属性
    /// </summary>
    /// <param name="id"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string CreateSetParameterAttributesTemplate(string id, List<SetParameterAttributesStruct> list)
    {
        StringBuilder soap = new StringBuilder();
        soap.Append(Environment.NewLine + "    <cwmp:SetParameterAttributes>");
        soap.Append(Environment.NewLine + "      <ParameterList>");
        foreach (SetParameterAttributesStruct sas in list)
        {
            soap.Append(Environment.NewLine + "        <SetParameterAttributesStruct>");
            soap.Append(Environment.NewLine + "          <Name>" + sas.Name + "</Name>");
            soap.Append(Environment.NewLine + "          <NotificationChange>" + (sas.NotificationChange ? 1 : 0) + "</NotificationChange>");
            soap.Append(Environment.NewLine + "          <Notification>" + sas.Notification + "</Notification>");
            soap.Append(Environment.NewLine + "          <AccessListChange>" + (sas.AccessListChange ? 1 : 0) + "</AccessListChange>");
            soap.Append(Environment.NewLine + "          <AccessList>");
            foreach (string s in sas.AccessList)
                soap.Append(Environment.NewLine + "            <string>" + s + "</string>");
            soap.Append(Environment.NewLine + "          </AccessList>");
            soap.Append(Environment.NewLine + "        </SetParameterAttributesStruct>");
        }
        soap.Append(Environment.NewLine + "      </ParameterList>");
        soap.Append(Environment.NewLine + "    </cwmp:SetParameterAttributes>");
        return CreateEnvelopeTemplate(id, soap.ToString());
        #region 响应内容
        //cwmp:SetParameterAttributesResponse
        #endregion
    }

    /// <summary>
    /// 查询一个参数或多个参数的属性
    /// </summary>
    /// <param name="id"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string CreateGetParameterAttributesTemplate(string id, List<string> list)
    {
        StringBuilder soap = new StringBuilder();
        soap.Append(Environment.NewLine + "    <cwmp:GetParameterAttributes>");
        soap.Append(Environment.NewLine + "      <ParameterNames>");
        foreach (string s in list)
        {
            soap.Append(Environment.NewLine + "        <string>" + s + "</string>");
        }
        soap.Append(Environment.NewLine + "      </ParameterNames>");
        soap.Append(Environment.NewLine + "    </cwmp:GetParameterAttributes>");
        return CreateEnvelopeTemplate(id, soap.ToString());
        #region 响应内容
        //cwmp:GetParameterAttributesResponse
        #endregion
    }
    #endregion

    #region Add/DeleteObject
    /// <summary>
    /// 新增实例
    /// </summary>
    /// <param name="id"></param>
    /// <param name="parameterKey"></param>
    /// <param name="ObjectName"></param>
    /// <returns></returns>
    public static string CreateAddObjectTemplate(string id, string parameterKey, string ObjectName)
    {
        StringBuilder soap = new StringBuilder();
        soap.Append(Environment.NewLine + "    <cwmp:AddObject>");
        soap.Append(Environment.NewLine + "      <ObjectName>" + ObjectName + "</ObjectName>");
        soap.Append(Environment.NewLine + "      <ParameterKey xsi:type=\"xsd:string\">" + parameterKey + "</ParameterKey>");
        soap.Append(Environment.NewLine + "    </cwmp:AddObject>");
        return CreateEnvelopeTemplate(id, soap.ToString());
        #region 响应内容
        //cwmp:AddObjectResponse
        #endregion
    }

    /// <summary>
    /// 删除实例
    /// </summary>
    /// <param name="id"></param>
    /// <param name="parameterKey"></param>
    /// <param name="ObjectName"></param>
    /// <returns></returns>
    public static string CreateDeleteObjectTemplate(string id, string parameterKey, string ObjectName)
    {
        StringBuilder soap = new StringBuilder();
        soap.Append(Environment.NewLine + "    <cwmp:DeleteObject>");
        soap.Append(Environment.NewLine + "      <ObjectName>" + ObjectName + "</ObjectName>");
        soap.Append(Environment.NewLine + "      <ParameterKey xsi:type=\"xsd:string\">" + parameterKey + "</ParameterKey>");
        soap.Append(Environment.NewLine + "    </cwmp:DeleteObject>");
        return CreateEnvelopeTemplate(id, soap.ToString());
        #region 响应内容
        //cwmp:DeleteObjectResponse
        #endregion
    }
    #endregion

    #region Upload
    /// <summary>
    /// 用于要求CPE终端向指定位置上传某一特定文件
    /// </summary>
    /// <param name="id"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static string CreateUploadTemplate(string id, UploadContent entity)
    {
        StringBuilder soap = new StringBuilder();
        soap.Append(Environment.NewLine + "    <cwmp:Upload>");
        soap.Append(Environment.NewLine + "      <CommandKey>" + entity.CommandKey + "</CommandKey>");
        soap.Append(Environment.NewLine + "      <FileType>" + GetUploadFileType2String(entity.FileType) + "</FileType>");
        soap.Append(Environment.NewLine + "      <URL>" + entity.URL + "</URL>");
        soap.Append(Environment.NewLine + "      <Username>" + entity.Username + "</Username>");
        soap.Append(Environment.NewLine + "      <Password>" + entity.Password + "</Password>");
        soap.Append(Environment.NewLine + "      <DelaySeconds>" + entity.DelaySeconds + "</DelaySeconds>");
        soap.Append(Environment.NewLine + "    </cwmp:Upload>");
        return CreateEnvelopeTemplate(id, soap.ToString());
        #region 响应内容
        //cwmp:UploadResponse
        #endregion
    }
    #endregion

    #region Download
    /// <summary>
    /// CPE终端在指定的位置下载指定的文件
    /// </summary>
    /// <param name="id"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static string CreateDownloadTemplate(string id, DownloadContent entity)
    {
        StringBuilder soap = new StringBuilder();
        soap.Append(Environment.NewLine + "    <cwmp:Download>");
        soap.Append(Environment.NewLine + "      <CommandKey>" + entity.CommandKey + "</CommandKey>");
        soap.Append(Environment.NewLine + "      <FileType>" + GetDownloadFileType2String(entity.FileType) + "</FileType>");
        soap.Append(Environment.NewLine + "      <URL>" + entity.URL + "</URL>");
        soap.Append(Environment.NewLine + "      <Username>" + entity.Username + "</Username>");
        soap.Append(Environment.NewLine + "      <Password>" + entity.Password + "</Password>");
        soap.Append(Environment.NewLine + "      <FileSize>" + entity.FileSize + "</FileSize>");
        soap.Append(Environment.NewLine + "      <TargetFileName>" + entity.TargetFileName + "</TargetFileName>");
        soap.Append(Environment.NewLine + "      <DelaySeconds>" + entity.DelaySeconds + "</DelaySeconds>");
        soap.Append(Environment.NewLine + "      <SuccessURL />");
        soap.Append(Environment.NewLine + "      <FailureURL />");
        soap.Append(Environment.NewLine + "    </cwmp:Download>");
        return CreateEnvelopeTemplate(id, soap.ToString());
        #region 响应内容
        //cwmp:DownloadResponse
        #endregion
    }
    #endregion

    #region Reboot
    /// <summary>
    /// 家庭网关设备重新启动
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string CreateRebootTemplate(string id)
    {
        StringBuilder soap = new StringBuilder();
        soap.Append(Environment.NewLine + "    <cwmp:Reboot>");
        soap.Append(Environment.NewLine + "      <CommandKey>CommandKey</CommandKey>");
        soap.Append(Environment.NewLine + "    </cwmp:Reboot>");
        return CreateEnvelopeTemplate(id, soap.ToString());
        #region 响应内容
        //cwmp:RebootResponse
        #endregion
    }
    #endregion

    #region FactoryReset
    /// <summary>
    /// 设备恢复出厂设置
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string CreateFactoryResetTemplate(string id)
    {
        StringBuilder soap = new StringBuilder();
        //soap.Append(Environment.NewLine + "    <cwmp:FactoryReset xmlns:cwmp=\"urn:dslforum-org:cwmp-1-0\"/>");
        soap.Append(Environment.NewLine + "    <cwmp:FactoryReset></cwmp:FactoryReset>");
        return CreateEnvelopeTemplate(id, soap.ToString());
        #region 响应内容
        //cwmp:FactoryResetResponse
        #endregion
    }
    #endregion

    /// <summary>
    /// CPE上传文件类型
    /// </summary>
    /// <param name="filetype"></param>
    /// <returns></returns>
    public static string GetUploadFileType2String(UploadFileType filetype)
    {
        switch (filetype)
        {
            case UploadFileType.Configuration:
                return "1 Vendor Configuration File";
            case UploadFileType.Log:
                return "2 Vendor Log File";
            default:
                return "";
        }
    }

    /// <summary>
    /// CPE下载文件类型
    /// </summary>
    /// <param name="filetype"></param>
    /// <returns></returns>
    public static string GetDownloadFileType2String(DownloadFileType filetype)
    {
        switch (filetype)
        {
            case DownloadFileType.Upgrade:
                return "1 Firmware Upgrade Image";
            case DownloadFileType.WebContent:
                return "2 Web Content";
            case DownloadFileType.Configuration:
                return "3 Vendor Configuration File";
            default:
                return "";
        }
    }

    /// <summary>
    /// 解析Inform
    /// </summary>
    /// <param name="soap"></param>
    /// <returns></returns>
    public static Inform ParseInform(string soap)
    {
        #region
        Inform inform = new Inform();
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(soap);
        inform.CWMPId = doc.DocumentElement["SOAP-ENV:Header"]["cwmp:ID"].InnerXml;
        inform.Devide.Manufacturer = doc.DocumentElement["SOAP-ENV:Body"]["cwmp:Inform"]["DeviceId"]["Manufacturer"].InnerXml;
        inform.Devide.OUI = doc.DocumentElement["SOAP-ENV:Body"]["cwmp:Inform"]["DeviceId"]["OUI"].InnerXml;
        inform.Devide.ProductClass = doc.DocumentElement["SOAP-ENV:Body"]["cwmp:Inform"]["DeviceId"]["ProductClass"].InnerXml;
        inform.Devide.SerialNumber = doc.DocumentElement["SOAP-ENV:Body"]["cwmp:Inform"]["DeviceId"]["SerialNumber"].InnerXml;

        foreach (XmlNode xn in doc.DocumentElement["SOAP-ENV:Body"]["cwmp:Inform"]["Event"].ChildNodes)
        {
            EventStruct es = new EventStruct();
            es.EventCode = xn["EventCode"].InnerXml;
            es.CommandKey = xn["CommandKey"].InnerXml;
            inform.events.Add(es);
            inform.EventCodes += es.EventCode + ";";
        }

        inform.MaxEnvelopes = doc.DocumentElement["SOAP-ENV:Body"]["cwmp:Inform"]["MaxEnvelopes"].InnerXml;
        inform.CurrentTime = doc.DocumentElement["SOAP-ENV:Body"]["cwmp:Inform"]["CurrentTime"].InnerXml;
        inform.RetryCount = doc.DocumentElement["SOAP-ENV:Body"]["cwmp:Inform"]["RetryCount"].InnerXml;

        foreach (XmlNode xn in doc.DocumentElement["SOAP-ENV:Body"]["cwmp:Inform"]["ParameterList"].ChildNodes)
        {
            ParameterValueStruct pvs = new ParameterValueStruct();
            pvs.name = xn["Name"].InnerXml;
            pvs.value = xn["Value"].InnerXml;
            inform.ParameterList.Add(pvs);
            if (pvs.name.Equals("InternetGatewayDevice.DeviceInfo.SpecVersion"))
                inform.Devide.SpecVersion = pvs.value;
            if (pvs.name.Equals("InternetGatewayDevice.DeviceInfo.HardwareVersion"))
                inform.Devide.HardwareVersion = pvs.value;
            if (pvs.name.Equals("InternetGatewayDevice.DeviceInfo.SoftwareVersion"))
                inform.Devide.SoftwareVersion = pvs.value;
            if (pvs.name.Equals("InternetGatewayDevice.DeviceInfo.ProvisioningCode"))
                inform.Devide.ProvisioningCode = pvs.value;
            if (pvs.name.Equals("InternetGatewayDevice.WANDevice.1.WANConnectionDevice.1.WANIPConnection.1.ExternalIPAddress"))
                inform.Devide.ExternalIPAddress = pvs.value;
            if (pvs.name.Equals("InternetGatewayDevice.WANDevice.1.WANConnectionDevice.1.WANIPConnection.1.MACAddress"))
                inform.Devide.MACAddress = pvs.value;
            if (pvs.name.Equals("InternetGatewayDevice.ManagementServer.ConnectionRequestURL"))
                inform.Devide.ConnectionRequestURL = pvs.value;
            if (pvs.name.Equals("InternetGatewayDevice.ManagementServer.ParameterKey"))
                inform.Devide.ParameterKey = pvs.value;
            if (pvs.name.Equals("InternetGatewayDevice.X_CMCC_UserInfo.UserName"))
                inform.X_CMCC_UserInfo_UserName = pvs.value;
            if (pvs.name.Equals("InternetGatewayDevice.X_CMCC_UserInfo.Password"))
                inform.X_CMCC_UserInfo_Password = pvs.value;
            if (pvs.name.Equals("InternetGatewayDevice.DeviceInfo.X_CMCC_IMEI"))
                inform.Devide.IMEI = pvs.value;
            if (pvs.name.Equals("InternetGatewayDevice.DeviceInfo.X_CMCC_ConfigVersion"))
                inform.Devide.ConfigVersion = pvs.value;
            if (pvs.name.Equals("InternetGatewayDevice.DeviceInfo.X_CMCC_ModuleVersion"))
                inform.Devide.ModuleVersion = pvs.value;
        }

        return inform;
        #endregion
    }
    public static string GetCPEAlarm(string [] alarms)
    {
        string sql = @"INSERT INTO [HGU2-LTE].[dbo].[tb_lte_alarm_threshold]
           ([Id],[tag]
           ,[name]
           ,[remark]
           ,[value1]
           ,[value2]
           ,[beginTime]
           ,[status]
           ,[creater]
           ,[createTime])VALUES ";
        for (int i = 0; i < alarms.Length; i++)
        {
            string[] sum_alarm = ParseAlarm(alarms[i]).Split(',');
            string name = sum_alarm[0].ToString();
            string remark = sum_alarm[1].ToString();
            sql += "('" + Guid.NewGuid().ToString() + "','" + name + "','" + remark + "','','','date','status','acs',getDATETIME()),";
            if (i == alarms.Length - 1)
                sql += "('" + Guid.NewGuid().ToString() + "','" + name + "','" + remark + "','','','date','status','acs',getDATETIME());";
        }

        return sql;
    }
    private static string ParseAlarm(string alarmNo)
    {
        string tag = string.Empty, remark = string.Empty;
        string mainAlarm = alarmNo.Substring(0, 1);
        string secAlarm = alarmNo.Substring(2, 3);
        switch (mainAlarm)
        {
            case "1":
                tag = "设备告警";
                switch (secAlarm)
                {
                    case "00":
                        remark = "与主机、板卡有关的硬件故障";
                        break;
                    case "01":
                        remark = "与网络有关的硬件故障";
                        break;
                    case "02":
                        remark = "与存储有关的硬件故障";
                        break;
                    case "03":
                        remark = "与外设有关的硬件故障";
                        break;
                    case "04":
                        remark = "终端设备告警";
                        break;
                    default:break;
                }
                break;
            case "2":
                tag = "服务质量告警";
                switch (secAlarm)
                {
                    case "01":
                        remark = "越门限告警";
                        break;
                    case "02":
                        remark = "信号源引起的服务质量下降";
                        break;
                    default: break;
                }
                break;
            case "3":
                tag = "处理出错告警";
                switch (secAlarm)
                {
                    case "01":
                        remark = "软件错误引起的故障";
                        break;
                    case "02":
                        remark = "配置错误引起的故障";
                        break;
                    case "03":
                        remark = "安全相关的故障";
                        break;
                    default: break;
                }
                break;
            case "4":
                tag = "处理出错告警";
                switch (secAlarm)
                {
                    case "01":
                        remark = "连接终端";
                        break;
                    default: break;
                }
                break;
            case "5":
                tag = "环境告警";
                break;
            default:break;
        }

        return tag+","+remark;
    }
    public static int GetParametervaluesresponse(string soap,string sn)
    {
        string sql = "update [HGU2-LTE].[dbo].[tb_lte_cpe] SET  ";

        Regex reg = new Regex("<Name xsi[\\d\\D]+?</Value>");
        MatchCollection mac = reg.Matches(soap);
        foreach (Match ma in mac)
        {
            string name = string.Empty, value = string.Empty;
            name = RemoveHTMLFlag(ma.Value.Split('\n')[0]).Replace("\r", "");
            value = RemoveHTMLFlag(ma.Value.Split('\n')[1]);
            if (name.IndexOf(".") > 0)
            {
                name = name.Substring(name.LastIndexOf(".")).Replace(".","");
                if (name.Contains("X_CMCC_"))
                    name = name.Replace("X_CMCC_", "");
            }
            sql += name + "= " + value;
        }
        sql += "where imei="+sn;
        return SQLUtil.UpdateParameterValues(sql);
    }
    public static string RemoveHTMLFlag(string input)
    {
        input = Regex.Replace(input, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

        Regex regex = new Regex("<.+?>", RegexOptions.IgnoreCase);
        input = regex.Replace(input, "");
        input = Regex.Replace(input, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"-->", "", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"<!--.*", "", RegexOptions.IgnoreCase);

        input = Regex.Replace(input, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"&#(\d+);", "", RegexOptions.IgnoreCase);

        input.Replace("<", "");
        input.Replace(">", "");
        input.Replace("\r\n", "");

        return input;
    }
    public static void GetDiagnosticsValues(string soap, List<string> para, string imei)
    {
        string json = "{";
        if (soap.Contains("IPPingDiagnostics"))
        {
            json += "    \"PingResult\": { \r";
            Regex reg = new Regex("<Name[\\d\\D]+?</Value>");
            MatchCollection mac = reg.Matches(soap);
            foreach (Match ma in mac)
            {
                string name = string.Empty, value = string.Empty;
                name = RemoveHTMLFlag(ma.Value.Split('\n')[0]).Replace("\r", "");
                if (name.Contains("PingLog"))
                {
                    value = RemoveHTMLFlag(ma.Value).Replace("InternetGatewayDevice.IPPingDiagnostics.PingLog", "").Replace("\n","").Replace("\r", "");
                }
                else
                    value = RemoveHTMLFlag(ma.Value.Split('\n')[1]);
                if (name.IndexOf(".") > 0)
                {
                    name = name.Substring(name.LastIndexOf(".")).Replace(".", "");
                    if (name.Contains("X_CMCC_"))
                        name = name.Replace("X_CMCC_", "");
                }
                if (name == "SuccessCount")
                    json += "        \"SuccessCount\": " + value + ",\r\n";
                if (name == "FailureCount")
                    json += "        \"FailureCount\": " + value + ",\r\n";
                if (name == "AverageResponseTime")
                    json += "        \"AverageResponseTime\": " + value + ",\r\n";
                if (name == "MinimumResponseTime")
                    json += "        \"MinimumResponseTime\": " + value + ",\r\n";
                if (name == "MaximumResponseTime")
                    json += "        \"MaximumResponseTime\": " + value + ",\r\n";
                if (name == "PingLog")
                    json += "        \"PingLog\": \"" + value + "\",\r\n";
            }
            if (soap.Contains("TraceRouteDiagnostics"))
                json += "    },\r\n";
            else
                json += "    }\r\n}";
            Utilities.D_DiagResults.Add(imei, json);
        }
        else if (soap.Contains("TraceRouteDiagnostics"))
        {
            json += "    \"TraceResult\": {\r";
            Regex reg = new Regex("<Name[\\d\\D]+?</Value>");
            MatchCollection mac = reg.Matches(soap);
            foreach (Match ma in mac)
            {
                string name = string.Empty, value = string.Empty;
                name = RemoveHTMLFlag(ma.Value.Split('\n')[0]).Replace("\r", "");
                if (name.Contains("Log"))
                {
                    value = RemoveHTMLFlag(ma.Value).Replace("InternetGatewayDevice.TraceRouteDiagnostics.TraceLog", "");
                }
                else
                    value = RemoveHTMLFlag(ma.Value.Split('\n')[1]);
                if (name.IndexOf(".") > 0)
                {
                    name = name.Substring(name.LastIndexOf(".")).Replace(".", "");
                    if (name.Contains("X_CMCC_"))
                        name = name.Replace("X_CMCC_", "");
                }
                if (name == "ResponseTime")
                    json += "        \"ResponseTime\": " + value + ",\r\n";
                if (name == "HopsNumberOfEntries")
                    json += "        \"HopsNumberOfEntries\": " + value + ",\r\n";
                if (name == "TraceLog")
                    json += "        \"TraceLog\": \"" + value + "\",\r\n";
            }
            json += "    }\r\n \r\n}";
            Utilities.D_DiagResults.Add(imei, json);
        }
        else
        {

        }

    }
    public static string GetPERIODICValues(string imei, Inform info)
    {

        string sql = "select COUNT(*) from [HGU2-LTE].dbo.tb_lte_cpe where IMEI='" + imei+ "'";
        List<ParameterValueStruct> list = info.ParameterList;
        if (Convert.ToInt32(SqlHelper.ExecuteScalar(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql)) > 0)
        {
            try
            {
              sql=@"UPDATE [HGU2-LTE].[dbo].[tb_lte_cpe] SET[manufacturer] = '"+info.Devide.Manufacturer + "',[identification] = '"+FindListValue(list,"InternetGatewayDevice.X_CMCC_ICCID")+ "',[imei] = '"+imei+"',[userPortNumber] = 1,[deviceType] = '"+ info.Devide.ProductClass + "',[serialNumber] = '"+info.Devide.SerialNumber+"',[oui] = '"+info.Devide.OUI+"',[provisioningCode] = '"+info.Devide.ProvisioningCode+"',[hardwareVersion] = '"+FindListValue(list,"InternetGatewayDevice.DeviceInfo.HardwareVersion")+"',[softwareVersion] = '"+FindListValue(list,"InternetGatewayDevice.DeviceInfo.SoftwareVersion")+"',[moduleVersion] = '"+FindListValue(list,"InternetGatewayDevice.DeviceInfo.X_CMCC_ModuleVersion")+"',[moduleType] = '"+FindListValue(list,"InternetGatewayDevice.DeviceInfo.X_CMCC_ModuleType")+"',[configVersion] = '"+FindListValue(list,"InternetGatewayDevice.DeviceInfo.X_CMCC_ConfigVersion")+"',[deviceStatus] = 1,[eNodeBId] = '"+FindListValue(list,"InternetGatewayDevice.X_CMCC_EnodeBId")+"',[cellId] = '"+FindListValue(list,"InternetGatewayDevice.X_CMCC_CellId")+"',[netRegStatus] = "+Convert.ToInt32(FindListValue(list, "InternetGatewayDevice.X_CMCC_NetRegStatus"))+",[gprsRegStatus] = "+ Convert.ToInt32(FindListValue(list, "InternetGatewayDevice.X_CMCC_GprsRegStatus")) + ",[epsRegStatus] = "+ Convert.ToInt32(FindListValue(list, "InternetGatewayDevice.X_CMCC_EpsRegStatus")) + ",[currentNetwork] = '"+FindListValue(list,"InternetGatewayDevice.X_CMCC_CurrentNetwork")+"' ,[currentNetmode] = 2,[networkPriority] = '"+FindListValue(list,"InternetGatewayDevice.X_CMCC_NetworkPriority")+"',[singalLevel] ="+FindListValue(list,"InternetGatewayDevice.X_CMCC_SingalLevel")+",[txPower] ="+ FindListValue(list,"InternetGatewayDevice.X_CMCC_Txpower")+",[frequencyPoint] ="+ FindListValue(list,"InternetGatewayDevice.X_CMCC_FrequencyPoint")+",[band] = "+FindListValue(list,"InternetGatewayDevice.X_CMCC_Band")+",[bandWidth] ="+ FindListValue(list,"InternetGatewayDevice.X_CMCC_BandWidth") +",[globeCellId] = '"+FindListValue(list,"InternetGatewayDevice.X_CMCC_GlobeCellId")+"',[physicsCellId] = '"+FindListValue(list,"InternetGatewayDevice.X_CMCC_PhysicsCellId")+"',[iccid] = '"+FindListValue(list,"InternetGatewayDevice.X_CMCC_ICCID")+"',[apn] = '"+FindListValue(list,"InternetGatewayDevice.X_CMCC_APN")+"',[frequencyLockInfo] = '"+FindListValue(list,"InternetGatewayDevice.X_CMCC_FrequencyLockInfo")+"',[cellLockInfo] = '"+FindListValue(list,"InternetGatewayDevice.X_CMCC_CellLockInfo")+"',[imsi] = '',[startTime] = '"+ FindListValue(list, "InternetGatewayDevice.X_CMCC_StartTime") +"',[upTime] = '" + FindListValue(list, "InternetGatewayDevice.DeviceInfo.UpTime") + "',[loadAverage] = '"+FindListValue(list,"InternetGatewayDevice.X_CMCC_LoadAverage")+"',[deviceMemory] = '"+FindListValue(list,"InternetGatewayDevice.X_CMCC_DeviceMemory")+"',[externalIPAddress] = '"+FindListValue(list, "InternetGatewayDevice.WANDevice.1.WANConnectionDevice.1.WANIPConnection.1.ExternalIPAddress") +"',[lastMsgTime] = GETDATE(),[status] = 1 ,[updater] = 'updater',[updateTime] = GETDATE() WHERE IMEI = '"+imei+"'";

}
            catch (Exception ex) { }
        }
        else
        {
            sql = @"insert into dbo.tb_lte_cpe ([id]
      ,[manufacturer]
      ,[identification]
      ,[imei]
      ,[userPortNumber]
      ,[deviceType]
      ,[serialNumber]
      ,[oui]
      ,[provisioningCode]
      ,[hardwareVersion]
      ,[softwareVersion]
      ,[moduleVersion]
      ,[moduleType]
      ,[configVersion]
      ,[deviceStatus]
      ,[eNodeBId]
      ,[cellId]
      ,[netRegStatus]
      ,[gprsRegStatus]
      ,[epsRegStatus]
      ,[currentNetwork]
      ,[currentNetmode]
      ,[networkPriority]
      ,[singalLevel]
      ,[txPower]
      ,[frequencyPoint]
      ,[band]
      ,[bandWidth]
      ,[globeCellId]
      ,[physicsCellId]
      ,[iccid]
      ,[apn]
      ,[frequencyLockInfo]
      ,[cellLockInfo]
      ,[imsi]
      ,[startTime]
      ,[upTime]
      ,[loadAverage]
      ,[deviceMemory]
      ,[externalIPAddress]
      ,[lastMsgTime]
      ,[status]
      ,[creater]
      ,[createTime])values(";

            sql += "'" + Guid.NewGuid().ToString("N") + "','" + info.Devide.Manufacturer + "','"+ FindListValue(list, "InternetGatewayDevice.X_CMCC_ICCID") + "','" + imei + "',1,'"+ info.Devide.ProductClass + "','" + info.Devide.SerialNumber + "','"+info.Devide.OUI+"','"+info.Devide.ProvisioningCode+"','" + FindListValue(list, "InternetGatewayDevice.DeviceInfo.X_CMCC_ConfigVersion") + "','"+ FindListValue(list, "InternetGatewayDevice.DeviceInfo.SoftwareVersion") + "','" + FindListValue(list, "InternetGatewayDevice.DeviceInfo.X_CMCC_ModuleVersion") + "','" + FindListValue(list, "InternetGatewayDevice.DeviceInfo.X_CMCC_ModuleType") + "','" + FindListValue(list, "InternetGatewayDevice.DeviceInfo.X_CMCC_ConfigVersion") + "',1,'" + FindListValue(list, "InternetGatewayDevice.X_CMCC_EnodeBId") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_CellId") + "','"+FindListValue(list, "InternetGatewayDevice.X_CMCC_NetRegStatus") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_GprsRegStatus") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_EpsRegStatus") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_CurrentNetwork") + "',0,'" + FindListValue(list, "InternetGatewayDevice.X_CMCC_NetworkPriority") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_SingalLevel")+ "'," + Convert.ToInt32(FindListValue(list, "InternetGatewayDevice.X_CMCC_Txpower")) + "," + Convert.ToInt32(FindListValue(list, "InternetGatewayDevice.X_CMCC_FrequencyPoint")) + "," +
  FindListValue(list, "InternetGatewayDevice.X_CMCC_Band") + "," + Convert.ToInt32(FindListValue(list, "InternetGatewayDevice.X_CMCC_BandWidth")) + ",'" + FindListValue(list, "InternetGatewayDevice.X_CMCC_GlobeCellId") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_PhysicsCellId") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_ICCID") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_APN") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_FrequencyLockInfo") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_CellLockInfo") + "','imsi','"+ FindListValue(list, "InternetGatewayDevice.X_CMCC_StartTime") + "','"+ FindListValue(list, "InternetGatewayDevice.DeviceInfo.UpTime") +"','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_LoadAverage") + "','" +
  FindListValue(list, "InternetGatewayDevice.X_CMCC_DeviceMemory") + "','" + FindListValue(list, "InternetGatewayDevice.WANDevice.1.WANConnectionDevice.1.WANIPConnection.1.ExternalIPAddress") + "',GETDATE(),1,'creater',GETDATE())";

        }
        try
        {
            CreateAlarm_transboundary(imei, info.Devide.Manufacturer, FindListValue(list, "InternetGatewayDevice.X_CMCC_EnodeBId"), FindListValue(list, "InternetGatewayDevice.X_CMCC_CellId"));
        }
        catch (Exception ex){ Console.WriteLine(ex.ToString());  }
        try
        {
            InsertCpeConfig(imei,info);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        try
        {
            InsertPorformance(imei, info);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        try { UpdateUserInfo(imei, info, list); } catch (Exception ex){ Console.WriteLine(ex.ToString()); }
        try {

            CreateTrend();
        } catch (Exception ex){ Console.WriteLine(ex.ToString()); }
        return sql;
    }
    public static void CreateTrend()
    {
        string date = RetClock()+":00";
        if (Convert.ToDateTime(date).Minute == DateTime.Now.Minute)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            List<string> list_cpe = new List<string>();
            int count_snir = 0; int count_rsrp = 0;
            string sql = "select a.cpeId,a.SINR_count from (select cpeid,count(distinct cpeid) as SINR_count from [HGU2-LTE-Performance].dbo.tb_lte_performance_ where sinr<(SELECT value1 FROM [HGU2-LTE].dbo.tb_lte_alarm_threshold WHERE status=1 and tag='SINR') and tag2Time='" + date + ".000' group by cpeId) as a";
            sql = "select b.city+'/'+b.county  AS city,a.cpeId,a.SINR_count from (select cpeid,count(distinct cpeid) as SINR_count from [HGU2-LTE-Performance].dbo.tb_lte_performance_" + DateTime.Now.ToString("yyyyMMdd") + " where sinr<(SELECT value1 FROM [HGU2-LTE].dbo.tb_lte_alarm_threshold WHERE status=1 and tag='SINR') and tag2Time='" + date + ".000' group by cpeId) as a,dbo.tb_lte_cpe_user b where a.cpeId=b.cpeId";
            DataSet ds_SINR = SqlHelper.ExecuteDataset(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);

            if (ds_SINR.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds_SINR.Tables[0].Rows)
                {
                    string belong = dr["city"].ToString();
                    if (!dic.ContainsKey(belong))
                    {
                        count_snir++;
                        dic.Add(belong, 1);
                    }
                    else
                        dic[belong] = dic[belong] + 1;
                    if (!list_cpe.Contains(dr["cpeid"].ToString()))
                        list_cpe.Add(dr["cpeid"].ToString());
                }
            }
            //ds.Clear();
            sql = "select a.cpeId,a.RSRP_count from (select cpeid,count(distinct cpeid) as RSRP_count from [HGU2-LTE-Performance].dbo.tb_lte_performance_" + DateTime.Now.ToString("yyyyMMdd") + " where sinr<(SELECT value1 FROM [HGU2-LTE].dbo.tb_lte_alarm_threshold WHERE status=1 and tag='RSRP') and tag2Time='" + date + "' group by cpeId) as a";
            sql = "select b.city+'/'+b.county  AS city,a.cpeId,a.RSRP_count from (select cpeid,count(distinct cpeid) as RSRP_count from [HGU2-LTE-Performance].dbo.tb_lte_performance_" + DateTime.Now.ToString("yyyyMMdd") + " where sinr<(SELECT value1 FROM [HGU2-LTE].dbo.tb_lte_alarm_threshold WHERE status=1 and tag='RSRP') and tag2Time='" + date + ".000' group by cpeId) as a,dbo.tb_lte_cpe_user b where a.cpeId=b.cpeId";
            DataSet ds_RSRP = SqlHelper.ExecuteDataset(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
            if (ds_RSRP.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds_RSRP.Tables[0].Rows)
                {
                    string belong = dr["city"].ToString();
                    if (!dic.ContainsKey(belong))
                    {
                        count_rsrp++;
                        dic.Add(belong, 1);
                    }
                    else
                        dic[belong] = dic[belong] + 1;
                    if (!list_cpe.Contains(dr["cpeid"].ToString()))
                        list_cpe.Add(dr["cpeid"].ToString());
                }
            }
            foreach (string s in dic.Keys)
            {
                string city = s.Split('/')[0];
                string country = s.Split('/')[1];
                int count = 0;
                dic.TryGetValue(s, out count);
                sql = "select * from [HGU2-LTE-Performance].[dbo].[tb_lte_performance_trend_" + DateTime.Now.ToString("yyyyMMdd") + "] where tag2time='" + date + "' and city='" + city + "' and county='" + country + "'";
                DataSet ds = SqlHelper.ExecuteDataset(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    sql = "update [HGU2-LTE-Performance].[dbo].[tb_lte_performance_trend_" + DateTime.Now.ToString("yyyyMMdd") + "] set cpe=" + count + ",RSRP=" + ds_RSRP.Tables[0].Select("city ='" + s + "'").Length + ",SINR=" + ds_SINR.Tables[0].Select("city ='" + s + "'").Length + " where tag2Time='" + date + "' and city='" + city + "' and county='" + country + "'";
                    SqlHelper.ExecuteNonQuery(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
                }
                else
                {
                    try
                    {
                        sql = @"if not exists(select * from [HGU2-LTE-Performance].[dbo].[tb_lte_performance_trend_"+ DateTime.Now.ToString("yyyyMMdd") + "]  where city='"+city+"'and county='"+country+"'and tag2Time='"+date+"')INSERT INTO [HGU2-LTE-Performance].[dbo].[tb_lte_performance_trend_" + DateTime.Now.ToString("yyyyMMdd") + "]([city],[county],[cpe],[RSRP],[SINR],[tag1Time],[tag2Time])VALUES('" + city + "','" + country + "'," + count + "," + ds_RSRP.Tables[0].Select("city ='" + s + "'").Length + "," + ds_SINR.Tables[0].Select("city ='" + s + "'").Length + ",GETDATE(),'" + date + "')";
                        SqlHelper.ExecuteNonQuery(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
                    }
                    catch (Exception ex) {Console.WriteLine(ex.ToString()); }
                }
            }

            dic.Clear();
        }
    }
    public static void UpdateUserInfo(string imei,Inform info,List<ParameterValueStruct> list)
    {
        string cpeid = GetCPEID(imei);
        string sql = "select count(*) from [HGU2-LTE].dbo.tb_lte_cpe_user where cpeId='"+ cpeid + "' and userName='"+ FindListValue(list, "InternetGatewayDevice.WANDevice.1.WANConnectionDevice.2.WANPPPConnection.1.Username") + "'";
        int i = (int)SqlHelper.ExecuteScalar(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
        if (i > 0)
        {
            sql = @"UPDATE [HGU2-LTE].[dbo].[tb_lte_cpe_user]
   SET [cpePort] = '" + FindListValue(list, "InternetGatewayDevice.X_CMCC_WANDeviceNumberOfEntries") + "' ,[dialStatus] = '" + FindListValue(list, "InternetGatewayDevice.WANDevice.1.WANConnectionDevice.2.WANPPPConnection.1.Enable") + "',[upTime] ='" + FindListValue(list, "InternetGatewayDevice.WANDevice.1.WANConnectionDevice.2.WANPPPConnection.1.Uptime") + "',[realName] = '',[tel] = '',[address] = '',[status] = '" + FindListValue(list, "InternetGatewayDevice.WANDevice.1.WANConnectionDevice.2.WANPPPConnection.1.Enable") + "',[updater] = 'acs',[updateTime] = GETDATE() WHERE  cpeid='" + cpeid + "' and username='" + FindListValue(list, "InternetGatewayDevice.WANDevice.1.WANConnectionDevice.2.WANPPPConnection.1.Username") + "'";
            SqlHelper.ExecuteNonQuery(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
        }
        else
        {
            sql = "select * from [HGU2-LTE].[dbo].[tb_lte_cell] where enodebid='" + FindListValue(list, "InternetGatewayDevice.X_CMCC_EnodeBId") + "' and cellId='" + FindListValue(list, "InternetGatewayDevice.X_CMCC_CellId") + "'";
            DataTable dt = SqlHelper.ExecuteDataset(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql).Tables[0];
            string city = string.Empty; string country = string.Empty;
            if (dt.Rows.Count > 0)
            {
                city = dt.Rows[0]["city"].ToString();
                country = dt.Rows[0]["county"].ToString();
            }
            sql = "INSERT INTO [HGU2-LTE].[dbo].[tb_lte_cpe_user]([ID],[city],[county],[cpeId],[cpePort],[userName],[userPassword],[dialStatus],[upTime],[realName],[tel],[address],[status],[creater],[createTime])VALUES('"+ Guid.NewGuid().ToString() + "','" + city + "','" + country + "','" + cpeid + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_WANDeviceNumberOfEntries") + "','" + FindListValue(list, "InternetGatewayDevice.WANDevice.1.WANConnectionDevice.2.WANPPPConnection.1.Username") + "','','" + FindListValue(list, "InternetGatewayDevice.WANDevice.1.WANConnectionDevice.2.WANPPPConnection.1.Enable") + "','" + FindListValue(list, "InternetGatewayDevice.WANDevice.1.WANConnectionDevice.2.WANPPPConnection.1.Uptime") + "','','','','" + FindListValue(list, "InternetGatewayDevice.WANDevice.1.WANConnectionDevice.2.WANPPPConnection.1.Enable") + "','acs',GETDATE())";
          i= SqlHelper.ExecuteNonQuery(SQLUtil.m_ConnString_CPE_PON,CommandType.Text,sql);
        }

    }
    public static void CreateFile(string IMEI,string content) {
        try
        {
            string MyPath = AppDomain.CurrentDomain.BaseDirectory + "files//" + IMEI;
            using (StreamWriter txt = new StreamWriter(MyPath, false, Encoding.Default))
            {
                txt.Flush();
                txt.WriteLine(content);
                txt.Close();
            }
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
    }
    public static void CreateOpDoneFile(string IMEI)
    {
        try
        {
            string MyPath = AppDomain.CurrentDomain.BaseDirectory + "OPDevs";
            using (StreamWriter txt = new StreamWriter(MyPath, true, Encoding.Default))
            {
                txt.Flush();
                txt.WriteLine(IMEI+"------"+DateTime.Now);
                txt.Close();
            }
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
    }
    public static string GetCpePwd()
    {
        return SQLUtil.SelectCpePasswd();
    }
    public static string GetMantanAccount()
    {
        return SQLUtil.SelectMantanAccount();
    }
    public static void RequstCPE(string imei)
    {
        string requrl=SQLUtil.GetCPERequstUrl(imei);
        DigestHttpWebRequest digreq = new DigestHttpWebRequest("cpe", "cwmp");
        HttpWebResponse result = digreq.GetResponse(new Uri(requrl));
        
    }
    public static void RequstCPE_Url(string url)
    {
        DigestHttpWebRequest digreq = new DigestHttpWebRequest("cpe", "cwmp");
        HttpWebResponse result = digreq.GetResponse(new Uri(url));

    }
    public static string RetClock()
    {
        string date = DateTime.Now.ToString();
        int min = DateTime.Now.Minute;
        if (min == 0)
        {
            date = DateTime.Now.Date.AddHours(DateTime.Now.Hour).ToString("yyyy-MM-dd HH:mm");
        }
        else if (min - 15 <= 0)
        {
            date = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(15).ToString("yyyy-MM-dd HH:mm");
        }
        else if (min - 15 > 0 && min - 15 <= 15)
            date = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(30).ToString("yyyy-MM-dd HH:mm");
        else if (min - 15 > 15 && min - 15 <= 30)
            date = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(45).ToString("yyyy-MM-dd HH:mm");
        else if (min - 15 > 30 && min - 15 < 45)
            date = DateTime.Now.Date.AddHours(DateTime.Now.Hour + 1).AddMinutes(0).ToString("yyyy-MM-dd HH:mm");
        return date;
    }
    public static void CreateAlarm_transboundary(string imei,string manufacturer, string eNodeBId,string cellid)
    {

        string sql = "select * from dbo.tb_lte_cell where eNodeBId='"+ eNodeBId + "' and cellId='"+ cellid + "';";
        sql = "select * from dbo.tb_lte_cpe where imei='"+imei+"';";
        string id = SqlHelper.ExecuteScalar(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql).ToString();

        string city=SQLUtil.GetBelongCity(id);
        sql = "select (city+'/'+county) as city from dbo.tb_lte_cell where eNodeBId='" + eNodeBId + "' and cellId='"+cellid+"';";
        string result = SqlHelper.ExecuteScalar(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql).ToString();
        if (!city.Equals(result))
        {
            sql = "select count(*) from dbo.tb_lte_alarm_transboundary where cpeId='41fe0cafe4f847d7ad62ccae7faa2892' and status=1";
            if (Convert.ToInt32(SqlHelper.ExecuteScalar(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql)) <= 0)
            {
                sql = "insert [HGU2-LTE].dbo.tb_lte_alarm_transboundary (alarmName,[cpeId],[manufacturer],[city],[county] ,[currentCity] ,[currentCounty] ,[happenTime] ,[status],[creater] ,[createTime]) values('跨地市告警跨地市告警','" + id + "','" + manufacturer + "','" + city.Split('/')[0] + "','" + city.Split('/')[1] + "','" + result.Split('/')[0] + "','" + result.Split('/')[1] + "',GETDATE(),1,'ACS',GETDATE())";
                SqlHelper.ExecuteNonQuery(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
            }
            else
            {
                sql = "update [HGU2-LTE].dbo.tb_lte_alarm_transboundary set updater='ACS',updateTime=GETDATE(),[recoverTime]=GETDATE() where cpeId='"+GetCPEID(imei)+ "'";
                SqlHelper.ExecuteNonQuery(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
            }
        }
        else
        {
             sql = "select count(*) from dbo.tb_lte_alarm_transboundary where cpeId='"+ GetCPEID(imei) + "' and status=1";
            if (Convert.ToInt32(SqlHelper.ExecuteScalar(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql)) <= 0)
            {
                sql = "update [HGU2-LTE].dbo.tb_lte_alarm_transboundary set updater='ACS',updateTime=GETDATE(),clearTime=GETDATE(),status=0 where cpeId='"+ GetCPEID(imei) + "'";
                SqlHelper.ExecuteNonQuery(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
            }
        }
    }
    public static void InsertPorformance(string imei,Inform info)
    {
        string cpeid = GetCPEID(imei);
        List<ParameterValueStruct> list = info.ParameterList;
        string sql = "  select top 1 * from [HGU2-LTE-Performance].[dbo].[tb_lte_performance] order by id desc";
        DataTable dt = SqlHelper.ExecuteDataset(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql).Tables[0];
        long ups = 0, downs = 0, inO = 0, outO = 0;
        foreach(DataRow dr in dt.Rows)
        {
            ups=long.Parse(dr["upstreamBytes"].ToString());
            downs = long.Parse(dr["downstreamBytes"].ToString());
            inO = long.Parse(dr["inOctets"].ToString());
            outO = long.Parse(dr["outOctets"].ToString());
        }
        long X_Up = long.Parse(FindListValue(list, "InternetGatewayDevice.X_CMCC_UpstreamTotalByte"));
        long X_Down = long.Parse(FindListValue(list, "InternetGatewayDevice.X_CMCC_DownstreamTotalByte"));
        long x_up_temp = 0, x_down_temp = 0;
        Utilities.D_CpeDownStreams.TryGetValue(imei, out x_down_temp);
        Utilities.D_CpeUpStreams.TryGetValue(imei, out x_up_temp);
        ups = X_Up == x_up_temp ? 0 :X_Up-x_up_temp;
        downs = X_Down == x_down_temp ? 0 : X_Down- x_down_temp ;
        inO = X_Down;
        outO = X_Up;
        if (X_Up != 0)
        {
            if (!Utilities.D_CpeUpStreams.ContainsKey(imei))
                Utilities.D_CpeUpStreams.Add(imei, X_Up);
            else
                Utilities.D_CpeUpStreams[imei] = X_Up;
        }
        
        if (X_Down != 0)
        {
            if (!Utilities.D_CpeDownStreams.ContainsKey(imei))
                Utilities.D_CpeDownStreams.Add(imei, X_Down);
            else
                Utilities.D_CpeDownStreams[imei] = X_Down;
        }
        sql = "select *  from  [HGU2-LTE-Performance].[dbo].[tb_lte_performance] where imei='" + imei + "'";
        DataSet ds = SqlHelper.ExecuteDataset(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
        if (ds.Tables[0].Rows.Count > 0)
        {
            sql = "UPDATE [HGU2-LTE-Performance].[dbo].[tb_lte_performance]SET [rsrp] = " + FindListValue(list, "InternetGatewayDevice.X_CMCC_RSRP") + ",[sinr] =" + FindListValue(list, "InternetGatewayDevice.X_CMCC_SINR") + ",[upstreamBytes] = " + outO + ",[downstreamBytes] = " + inO + ",[inOctets] = " + downs + ",[outOctets] = " + ups + ",[tag1Time] = '" + DateTime.Now.ToString() + "',[tag2Time] = '" + RetClock() + "',[createTime] = getdate() WHERE IMEI='" + imei + "'";
            SqlHelper.ExecuteNonQuery(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
        }
        else
        {
            sql = @"INSERT INTO [HGU2-LTE-Performance].[dbo].[tb_lte_performance]
           ([cpeId]
           ,[imei]
           ,[rsrp]
           ,[sinr]
           ,[upstreamBytes]
           ,[downstreamBytes]
           ,[inOctets]
           ,[outOctets]
           ,[tag1Time]
           ,[tag2Time]
           ,[createTime])
     VALUES
           ('" + cpeid + "','" + imei + "' ," + FindListValue(list, "InternetGatewayDevice.X_CMCC_RSRP") + " ," + FindListValue(list, "InternetGatewayDevice.X_CMCC_SINR") + "," + outO + "," + inO + "," + downs + "," + ups + ",'" + DateTime.Now.ToString() + "','" + RetClock() + "',getdate())";
            SqlHelper.ExecuteNonQuery(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
        }
        sql = @"INSERT INTO [HGU2-LTE-Performance].[dbo].[tb_lte_performance_" + DateTime.Now.ToString("yyyyMMdd") + "]([cpeId],[imei],[rsrp] ,[sinr] ,[upstreamBytes] ,[downstreamBytes],[inOctets],[outOctets],[tag1Time],[tag2Time],[createTime])VALUES('" + cpeid + "','" + imei + "' ," + FindListValue(list, "InternetGatewayDevice.X_CMCC_RSRP") + " ," + FindListValue(list, "InternetGatewayDevice.X_CMCC_SINR") + "," + outO + "," + inO + "," + downs + "," + ups + ",'" + DateTime.Now.ToString() + "','" + RetClock() + "',getdate())";

        SqlHelper.ExecuteNonQuery(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
    }
    public static string GetCPEID(string imei)
    {
        string sql = "select * from dbo.tb_lte_cpe where imei='" + imei + "';";
        return SqlHelper.ExecuteScalar(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql).ToString();
    }
    private static void InsertCpeConfig(string imei, Inform info)
    {
        string sql = string.Empty;
        string cpeid = GetCPEID(imei);
        List<ParameterValueStruct> list = info.ParameterList;
        sql = "select count(*) from dbo.tb_lte_cpe_config where cpeId='" + cpeid + "';";
        string pppoeEnableInfo = string.Empty;
        string res= FindListValue(list, "InternetGatewayDevice.WANDevice.1.WANConnectionDevice.2.WANPPPConnection.1.Enable");
        pppoeEnableInfo = "P1:"+res+";";
        if (Convert.ToInt32(SqlHelper.ExecuteScalar(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql)) <= 0)
        {
            sql = @"INSERT INTO [HGU2-LTE].[dbo].[tb_lte_cpe_config]
           ([cpeId]
           ,[imei]
           ,[pppoeEnableInfo]
           ,[connectionRequestURL]
           ,[connectionRequestUsername]
           ,[connectionRequestPassword]
           ,[parameterKey]
           ,[frequencyLockEnable]
           ,[frequencyLocking]
           ,[cellLockEnable]
           ,[cellLockType]
           ,[lockPinEnable]
           ,[lockPinType]
           ,[firstPin]
           ,[fixedPin]
           ,[encryptCardEnable]
           ,[encryptCardKey]
           ,[creater]
           ,[createTime])
     VALUES('" + cpeid + "','" + imei + "','"+ pppoeEnableInfo + "','" + FindListValue(list, "InternetGatewayDevice.ManagementServer.ConnectionRequestURL") + "','" + FindListValue(list, "InternetGatewayDevice.ManagementServer.ConnectionRequestUsername") + "','" + FindListValue(list, "InternetGatewayDevice.ManagementServer.ConnectionRequestPassword") + "','" + FindListValue(list, "InternetGatewayDevice.ManagementServer.ParameterKey") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_IsFrequencyLock") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_FrequencyLocking") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_IsCellLock") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_CellLockType") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_LockPin") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_LockPinType") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_FirstPin") + "','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_FixedPin") + "','1','" + FindListValue(list, "InternetGatewayDevice.X_CMCC_EncryptCardKey") + "','ACS',GETDATE())";
        }
        else
        {
            sql = @"UPDATE [HGU2-LTE].[dbo].[tb_lte_cpe_config] SET [connectionRequestURL] ='" + FindListValue(list, "InternetGatewayDevice.ManagementServer.ConnectionRequestURL") + "',[connectionRequestUsername] = '" + FindListValue(list, "InternetGatewayDevice.ManagementServer.ConnectionRequestUsername") + "',[connectionRequestPassword] = '" + FindListValue(list, "InternetGatewayDevice.ManagementServer.ConnectionRequestPassword") + "',[parameterKey] = '" + FindListValue(list, "InternetGatewayDevice.ManagementServer.ParameterKey") + "',[frequencyLockEnable] ='" + FindListValue(list, "InternetGatewayDevice.X_CMCC_IsFrequencyLock") + "' ,[frequencyLocking] = '" + FindListValue(list, "InternetGatewayDevice.X_CMCC_FrequencyLocking") + "',[cellLockEnable] = '" + FindListValue(list, "InternetGatewayDevice.X_CMCC_IsCellLock") + "',[cellLockType] = '" + FindListValue(list, "InternetGatewayDevice.X_CMCC_CellLockType") + "',[lockPinEnable] = '" + FindListValue(list, "InternetGatewayDevice.X_CMCC_LockPin") + "',[lockPinType] ='" + FindListValue(list, "InternetGatewayDevice.X_CMCC_LockPinType") + "',[firstPin] ='" + FindListValue(list, "InternetGatewayDevice.X_CMCC_FirstPin") + "' ,[fixedPin] = '" + FindListValue(list, "InternetGatewayDevice.X_CMCC_FixedPin") + "',[encryptCardEnable] ='1',[encryptCardKey] ='" + FindListValue(list, "InternetGatewayDevice.X_CMCC_EncryptCardKey") + "',[updater] ='ACS',[updateTime] = getdate(),[pppoeEnableInfo]='"+pppoeEnableInfo+"' WHERE cpeid='" + cpeid + "'";
        }
       int i=SqlHelper.ExecuteNonQuery(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
    }
    private static string FindListValue(List<ParameterValueStruct> list,string keyname)
    {
        string value = string.Empty;
        try
        {
            if(list.Find(delegate (ParameterValueStruct p) { return p.name == keyname; })!=null)
                  value = list.Find(delegate (ParameterValueStruct p) { return p.name == keyname; }).value;
            if (value == "N/A")
                value = string.Empty;
        }
        catch (Exception ex){ Console.WriteLine(ex.ToString()); }
        return value;
    }
    public static string GetCPEConfigValues(JsessionIdModel jsim, List<ParameterValueStruct> list, string id)
    {
        List<string> list_para = new List<string>();
        list_para.AddRange(new string[] { "InternetGatewayDevice.ManagementServer.Username", "InternetGatewayDevice.ManagementServer.Password", "InternetGatewayDevice.parameterKey", "InternetGatewayDevice.X_CMCC_IsFrequencyLock", "InternetGatewayDevice.X_CMCC_FrequencyLocking", "InternetGatewayDevice.ConnectionRequestURL", });
        TemplateHelper.CreateGetParameterValuesTemplate(id,list_para);
        string sql = @"insert into dbo.tb_lte_cpe (,[imei]
      ,[connectionRequestURL]
      ,[connectionRequestUsername]
      ,[connectionRequestPassword]
      ,[parameterKey]
      ,[frequencyLockEnable]
      ,[frequencyLocking]
      ,[cellLockEnable]
      ,[cellLockType]
      ,[lockPinEnable]
      ,[lockPinType]
      ,[firstPin]
      ,[fixedPin]
      ,[encryptCardEnable]
      ,[encryptCardKey]";
        sql += "";
        
        return sql;
    }
}
    public class DigestHttpWebRequest
{
    private string _user;
    private string _password;
    private string _realm;
    private string _nonce;
    private string _qop;
    private string _cnonce;
    private Algorithm _md5;
    private DateTime _cnonceDate;
    private int _nc;

    private string _requestMethod = WebRequestMethods.Http.Get;
    private string _contentType;
    private byte[] _postData;

    public DigestHttpWebRequest(string user, string password)
    {
        _user = user;
        _password = password;
    }

    public string Method
    {
        get { return _requestMethod; }
        set { _requestMethod = value; }
    }

    public string ContentType
    {
        get { return _contentType; }
        set { _contentType = value; }
    }

    public byte[] PostData
    {
        get { return _postData; }
        set { _postData = value; }
    }

    public HttpWebResponse GetResponse(Uri uri)
    {
        HttpWebResponse response = null;
        int infiniteLoopCounter = 0;
        int maxNumberAttempts = 2;

        while ((response == null ||
            response.StatusCode != HttpStatusCode.Accepted) &&
            infiniteLoopCounter < maxNumberAttempts)
        {
            try
            {
                var request = CreateHttpWebRequestObject(uri);

                // If we've got a recent Auth header, re-use it!
                if (!string.IsNullOrEmpty(_cnonce) &&
                    DateTime.Now.Subtract(_cnonceDate).TotalHours < 1.0)
                {
                    request.Headers.Add("Authorization", ComputeDigestHeader(uri));
                }

                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException webException)
                {
                    // Try to fix a 401 exception by adding a Authorization header
                    if (webException.Response != null &&
                        ((HttpWebResponse)webException.Response).StatusCode == HttpStatusCode.Unauthorized)
                    {

                        var wwwAuthenticateHeader = webException.Response.Headers["WWW-Authenticate"];
                        _realm = GetDigestHeaderAttribute("realm", wwwAuthenticateHeader);
                        _nonce = GetDigestHeaderAttribute("nonce", wwwAuthenticateHeader);
                        _qop = GetDigestHeaderAttribute("qop", wwwAuthenticateHeader);
                        _md5 = GetMD5Algorithm(wwwAuthenticateHeader);

                        _nc = 0;
                        _cnonce = new Random().Next(123400, 9999999).ToString();
                        _cnonceDate = DateTime.Now;

                        request = CreateHttpWebRequestObject(uri, true);

                        infiniteLoopCounter++;
                        response = (HttpWebResponse)request.GetResponse();
                    }
                    else
                    {
                        throw webException;
                    }
                }

                if (request != null)
                    request.Abort();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.Accepted:
                        return response;
                    case HttpStatusCode.Redirect:
                    case HttpStatusCode.Moved:
                        uri = new Uri(response.Headers["Location"]);

                        // We decrement the loop counter, as there might be a variable number of redirections which we should follow
                        infiniteLoopCounter--;
                        break;
                }

            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        throw new Exception("Error: Either authentication failed, authorization failed or the resource doesn't exist");
    }

    private HttpWebRequest CreateHttpWebRequestObject(Uri uri, bool addAuthenticationHeader)
    {
        var request = (HttpWebRequest)WebRequest.Create(uri);
        request.AllowAutoRedirect = false;
        request.Method = this.Method;

        if (!String.IsNullOrEmpty(this.ContentType))
        {
            request.ContentType = this.ContentType;
        }

        if (addAuthenticationHeader)
        {
            request.Headers.Add("Authorization", ComputeDigestHeader(uri));
        }

        request.Timeout = 10000;
        //request.ContentType = "application/x-www-form-urlencoded";
        //request.ServicePoint.ConnectionLimit = 300;
        //request.Referer = url;
        //request.Accept = "*/*";
        //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)";
        //request.AllowAutoRedirect = true;
        //request.KeepAlive = true;

        if (this.PostData != null && this.PostData.Length > 0)
        {
            request.ContentLength = this.PostData.Length;
            Stream postDataStream = request.GetRequestStream(); //open connection
            postDataStream.Write(this.PostData, 0, this.PostData.Length); // Send the data.
            postDataStream.Close();
        }
        else if (
            this.Method == WebRequestMethods.Http.Post &&
            (this.PostData == null || this.PostData.Length == 0))
        {
            request.ContentLength = 0;
        }

        return request;
    }

    private HttpWebRequest CreateHttpWebRequestObject(Uri uri)
    {
        return CreateHttpWebRequestObject(uri, false);
    }

    private string ComputeDigestHeader(Uri uri)
    {
        _nc = _nc + 1;

        string ha1, ha2;

        switch (_md5)
        {
            case Algorithm.MD5sess:

                var secret = ComputeMd5Hash(string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}", _user, _realm, _password));
                ha1 = ComputeMd5Hash(string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}", secret, _nonce, _cnonce));
                ha2 = ComputeMd5Hash(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", this.Method, uri.PathAndQuery));

                var data = string.Format(CultureInfo.InvariantCulture, "{0}:{1:00000000}:{2}:{3}:{4}",
                    _nonce,
                    _nc,
                    _cnonce,
                    _qop,
                    ha2);

                var kd = ComputeMd5Hash(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ha1, data));

                return string.Format("Digest username=\"{0}\", realm=\"{1}\", nonce=\"{2}\", uri=\"{3}\", " +
                    "algorithm=MD5-sess, response=\"{4}\", qop={5}, nc={6:00000000}, cnonce=\"{7}\"",
                    _user, _realm, _nonce, uri.PathAndQuery, kd, _qop, _nc, _cnonce);

            case Algorithm.MD5:

                ha1 = ComputeMd5Hash(string.Format("{0}:{1}:{2}", _user, _realm, _password));
                ha2 = ComputeMd5Hash(string.Format("{0}:{1}", this.Method, uri.PathAndQuery));
                var digestResponse =
                    ComputeMd5Hash(string.Format("{0}:{1}:{2:00000000}:{3}:{4}:{5}", ha1, _nonce, _nc, _cnonce, _qop, ha2));

                return string.Format("Digest username=\"{0}\", realm=\"{1}\", nonce=\"{2}\", uri=\"{3}\", " +
                    "algorithm=MD5, response=\"{4}\", qop={5}, nc={6:00000000}, cnonce=\"{7}\"",
                    _user, _realm, _nonce, uri.PathAndQuery, digestResponse, _qop, _nc, _cnonce);

        }

        throw new Exception("The digest header could not be generated");
    }

    private string GetDigestHeaderAttribute(string attributeName, string digestAuthHeader)
    {
        var regHeader = new Regex(string.Format(@"{0}=""([^""]*)""", attributeName));
        var matchHeader = regHeader.Match(digestAuthHeader);
        if (matchHeader.Success)
            return matchHeader.Groups[1].Value;
        throw new ApplicationException(string.Format("Header {0} not found", attributeName));
    }

    private Algorithm GetMD5Algorithm(string digestAuthHeader)
    {
        var md5Regex = new Regex(@"algorithm=(?<algo>.*)[,]", RegexOptions.IgnoreCase);
        var md5Attribute = md5Regex.Match(digestAuthHeader);
        if (md5Attribute.Success)
        {
            char[] charSeparator = new char[] { ',' };
            string algorithm = md5Attribute.Result("${algo}").ToLower().Split(charSeparator)[0];

            switch (algorithm)
            {
                case "md5-sess":
                case "\"md5-sess\"":
                    return Algorithm.MD5sess;

                case "md5":
                case "\"md5\"":
                default:
                    return Algorithm.MD5;
            }
        }
        else//如果没有指定，默认MD5
        {
            return Algorithm.MD5;
        }

        throw new ApplicationException("Could not determine Digest algorithm to be used from the server response.");
    }

    private string ComputeMd5Hash(string input)
    {
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var hash = MD5.Create().ComputeHash(inputBytes);
        var sb = new StringBuilder();
        foreach (var b in hash)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    public enum Algorithm
    {
        MD5 = 0, // Apache Default
        MD5sess = 1 //IIS Default
    }

}
public class Inform
{
    public string CWMPId = string.Empty;
    private DeviceInfo devide = new DeviceInfo();
    public List<EventStruct> events = new List<EventStruct>();
    public string EventCodes = string.Empty;
    public string MaxEnvelopes = "1";
    public string CurrentTime = string.Empty;
    public string RetryCount = "0";
    public List<ParameterValueStruct> ParameterList = new List<ParameterValueStruct>();
    public string X_CMCC_UserInfo_UserName = string.Empty;
    public string X_CMCC_UserInfo_Password = string.Empty;

    public DeviceInfo Devide
    {
        get
        {
            return devide;
        }

        set
        {
            devide = value;
        }
    }
}

public class DeviceInfo
{
    public string Manufacturer = string.Empty;
    public string OUI = string.Empty;
    public string ProductClass = string.Empty;
    public string SerialNumber = string.Empty;
    public string SpecVersion = string.Empty;
    public string HardwareVersion = string.Empty;
    public string SoftwareVersion = string.Empty;
    public string ProvisioningCode = string.Empty;
    public string ExternalIPAddress = string.Empty;
    public string MACAddress = string.Empty;
    public string ConnectionRequestURL = string.Empty;
    public string ParameterKey = string.Empty;
    public string IMEI = string.Empty;
    public string ConfigVersion = string.Empty;
    public string ModuleVersion = string.Empty;
}

public class EventStruct
{
    public string EventCode = string.Empty;
    public string CommandKey = string.Empty;
}

public class ParameterValueStruct
{
    public string name = string.Empty;
    public string value = string.Empty;
}

/// <summary>
/// 用于参数属性值改动
/// </summary>
public class SetParameterAttributesStruct
{
    public string Name = string.Empty;
    public bool NotificationChange;
    public int Notification;
    public bool AccessListChange;
    public string[] AccessList;
}

//public enum FileType
//{
//    //"1 Firmware Upgrade Image" (download only)
//    //"2 Web Content" (download only)
//    //“3 Vendor Configuration File” (download or upload) [DEPRECATED for upload]
//    //“4 Vendor Log File” (upload only) [DEPRECATED]

//    /// <summary>
//    /// download only
//    /// </summary>
//    Upgrade = 1,            //升级包
//    /// <summary>
//    /// download only
//    /// </summary>
//    WebContent = 2,         //
//    /// <summary>
//    /// download or upload
//    /// </summary>
//    Configuration = 3,      //配置文件
//    /// <summary>
//    /// upload only
//    /// </summary>
//    Log = 4                 //日志文件
//}

public enum DownloadFileType
{
    Upgrade = 1,            //升级包
    WebContent = 2,
    Configuration = 3,      //配置文件
}

public enum UploadFileType
{
    Configuration = 1,      //配置文件
    Log = 2,            //日志
}

public class DownloadContent
{
    public string CommandKey = string.Empty;
    /// <summary>
    /// 文件类型
    /// </summary>
    public DownloadFileType FileType;
    public string URL = string.Empty;
    public string Username = string.Empty;
    public string Password = string.Empty;
    /// <summary>
    /// 以字节为单位的要传输文件的大小
    /// </summary>
    public string FileSize = string.Empty;
    /// <summary>
    /// 在目标文件系统中应使用的文件名
    /// </summary>
    public string TargetFileName = string.Empty;
    /// <summary>
    /// 开始下载之间的时间间隔
    /// </summary>
    public string DelaySeconds = string.Empty;
}
public class UploadContent
{
    public string CommandKey = string.Empty;
    /// <summary>
    /// 文件类型
    /// </summary>
    public UploadFileType FileType;
    public string URL = string.Empty;
    public string Username = string.Empty;
    public string Password = string.Empty;
    /// <summary>
    /// 开始下载之间的时间间隔
    /// </summary>
    public string DelaySeconds = string.Empty;
}

/// <summary>
/// CPE请求帮助类
/// </summary>
public static class RequestHelper
{

}

/// <summary>
/// 响应CPE帮助类
/// </summary>

public static class SQLUtil
{
    public static string m_ConnString_CPE_PON = "Data Source=10.138.20.131;Initial Catalog=HGU2-LTE;User ID=sa;Password=admin!123";
    public static void UpdateCPE(DeviceInfo cpe)
    {
        string sql = string.Empty;
        try
        {
            sql = @"IF EXISTS(SELECT * FROM dbo.HGUInfo WHERE SerialNumber='{1}')
UPDATE dbo.HGUInfo SET Manufacturer='{2}',OUI='{3}',ProductClass='{4}',SpecVersion='{5}',HardwareVersion='{6}',SoftwareVersion='{7}',ProvisioningCode='{8}',ExternalIPAddress='{9}',MACAddress='{10}',ConnectionRequestURL='{11}',ParameterKey='{12}',UpdateTime=GETDATE() WHERE SerialNumber='{1}'
ELSE
INSERT INTO dbo.HGUInfo
        ( ID ,SerialNumber ,Manufacturer ,OUI ,ProductClass ,SpecVersion ,HardwareVersion ,SoftwareVersion ,ProvisioningCode ,ExternalIPAddress ,MACAddress ,ConnectionRequestURL ,ParameterKey ,UpdateTime)
VALUES  ( '{0}' ,'{1}' ,'{2}' ,'{3}' ,'{4}' ,'{5}' ,'{6}' ,'{7}' ,'{8}' ,'{9}' ,'{10}' , '{11}' , '{12}' ,GETDATE()
        )";
            sql = string.Format(sql, Utilities.NewId(), cpe.SerialNumber, cpe.Manufacturer, cpe.OUI, cpe.ProductClass, cpe.SpecVersion, cpe.HardwareVersion, cpe.SoftwareVersion, cpe.ProvisioningCode, cpe.ExternalIPAddress, cpe.MACAddress, cpe.ConnectionRequestURL, cpe.ParameterKey);
            SqlHelper.ExecuteNonQuery(m_ConnString_CPE_PON, System.Data.CommandType.Text, sql);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message + Environment.NewLine + sql);
        }
    }

    /// <summary>
    /// 更新任务请求时间
    /// </summary>
    /// <param name="id"></param>
    public static void UpdateTaskRequestTime(string id)
    {
        string sql = "update [CPE-PON].dbo.Task set TaskStatus='已发送',RequestTime=getdate() where ID='{0}'";
        sql = string.Format(sql, id);
        SqlHelper.ExecuteNonQuery("Persist Security Info=false;server=localhost;database=CPE-PON;user id=sa;password=123456", CommandType.Text, sql);
    }
    public static int UpdateParameterValues(string sql)
    {
        int i = 0;
        i = SqlHelper.ExecuteNonQuery(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
        return i;
    }
    public static string SelectCpePasswd()
    {
        string sql = "SELECT code,value FROM [HGU2-LTE].[dbo].[tb_lte_tr069_parameter] where code='InternetGatewayDevice.ManagementServer.Username' or code='InternetGatewayDevice.ManagementServer.Password' or code='InternetGatewayDevice.ManagementServer.ConnectionRequestUsername' or code='InternetGatewayDevice.ManagementServer.ConnectionRequestPassword'";
        DataTable dt = SqlHelper.ExecuteDataset(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql).Tables[0];
        string user_acs = string.Empty, passwd_acs = string.Empty, user_cpe = string.Empty, passwd_cpe = string.Empty;
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["code"].ToString().Equals("InternetGatewayDevice.ManagementServer.Username"))
                    user_acs = dr["value"].ToString();
                if (dr["code"].ToString().Equals("InternetGatewayDevice.ManagementServer.Password"))
                    passwd_acs = dr["value"].ToString();
                if (dr["code"].ToString().Equals("InternetGatewayDevice.ManagementServer.ConnectionRequestUsername"))
                    user_cpe = dr["value"].ToString();
                if (dr["code"].ToString().Equals("InternetGatewayDevice.ManagementServer.ConnectionRequestPassword"))
                    passwd_cpe = dr["value"].ToString();
            }
        }
        return user_acs + "," + passwd_acs + "," + user_cpe + "," + passwd_cpe;
    }
    public static string GetBelongCity(string cpeid)
    {
        string sql = "select city+'/'+county from dbo.tb_lte_cpe_user where cpeId='" + cpeid + "';";
        string result = SqlHelper.ExecuteScalar(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql).ToString();
        return result;
    }
    public static string SelectMantanAccount()
    {
        string sql = "SELECT code,value FROM [HGU2-LTE].[dbo].[tb_lte_tr069_parameter] where code='InternetGatewayDevice.DeviceInfo.X_CMCC_TeleComAccount.Username' or code='InternetGatewayDevice.DeviceInfo.X_CMCC_TeleComAccount.Password'";
        DataTable dt = SqlHelper.ExecuteDataset(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql).Tables[0];
        string usr = string.Empty, pwd = string.Empty;
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["code"].ToString().Equals("InternetGatewayDevice.ManagementServer.Username"))
                    usr = dr["value"].ToString();
                if (dr["code"].ToString().Equals("InternetGatewayDevice.ManagementServer.Password"))
                    pwd = dr["value"].ToString();
            }
        }
        return usr + "," + pwd;
    }
    public static int InsertCPEConfig(string sql)
    {
        int i = SqlHelper.ExecuteNonQuery(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
        return i;
    }

    public static string GetCPERequstUrl(string imei)
    {
        string sql = "SELECT [connectionRequestURL] FROM [HGU2-LTE].[dbo].[tb_lte_cpe_config] where imei='"+imei+"';";

        return SqlHelper.ExecuteScalar(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql).ToString();
    }
    public static void InsertPERIODICValues(string sql)
    {
        int i = SqlHelper.ExecuteNonQuery(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
    }
    /// <summary>
    /// 更新任务响应时间
    /// </summary>
    /// <param name="id"></param>
    public static void UpdateTaskResponseTime(string id)
    {
        string sql = "update task set TaskStatus='已结束',ResponseTime=getdate() where ID='{0}'";
        sql = string.Format(sql, id);
        SqlHelper.ExecuteNonQuery(SQLUtil.m_ConnString_CPE_PON, CommandType.Text, sql);
    }
}
#region JSESSIONID 模型
/// <summary>
/// JSESSIONID 模型
/// </summary>
public class JsessionIdModel
{
    public string Manufacturer { get; set; }
    public string SerialNumber { get; set; }
    public string IMEI { get; set; }
    public string Method { get; set; }

    public JsessionIdModel(string manufacturer, string sn, string imei)
    {
        Manufacturer = manufacturer;
        SerialNumber = sn;
        IMEI = imei;
    }

    public JsessionIdModel(string jsessionIdEncrypt)
    {
        if (string.IsNullOrEmpty(jsessionIdEncrypt))
            return;
        string model = DESHelper.decrypt(jsessionIdEncrypt);
        string[] s = model.Split('#');
        if (s.Length != 3)
            throw new Exception(string.Format("{0} 位数不对", Utilities.JSESSIONID));
        Manufacturer = s[0];
        SerialNumber = s[1];
        IMEI = s[2];
    }

    public override string ToString()
    {
        return string.Format("{0}#{1}#{2}", Manufacturer, SerialNumber, IMEI);
    }
    public string ToEncryptString()
    {
        return DESHelper.encrypt(string.Format("{0}#{1}#{2}", Manufacturer, SerialNumber, IMEI));
    }
}
#endregion