#region 描述
//-----------------------------------------------------------------------------
// 文 件 名: DESHelper
// 作    者：d.w
// 创建时间：2015/3/23 11:40:40
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
using System.Security.Cryptography;
using System.Text;

/// <summary>
///C# DES 的摘要说明
/// </summary>
public class DESHelper
{
    public static String DES_KEY_STRING = "tk#Auth1";

    /// <summary>
    /// 使用缺省密钥加密
    /// </summary>
    /// <param name="s">明文</param>
    /// <returns>密文</returns>
    public static string encrypt(string s)
    {
        return encrypt(s, DES_KEY_STRING);
    }

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="s">明文</param>
    /// <param name="k">密钥</param>
    /// <returns>密文</returns>
    public static string encrypt(string s, string k)
    {
        //把字符串放到byte数组中   
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        byte[] inputByteArray = Encoding.Default.GetBytes(s);
        des.Key = Encoding.ASCII.GetBytes(k); //建立加密对象的密钥和偏移量   
        des.IV = Encoding.ASCII.GetBytes(k); //原文使用ASCIIEncoding.ASCII方法的GetBytes方法   
        MemoryStream ms = new MemoryStream(); //使得输入密码必须输入英文文本   
        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        StringBuilder ret = new StringBuilder();
        foreach (byte b in ms.ToArray())
            ret.AppendFormat("{0:X2}", b);
        ret.ToString();
        return ret.ToString();
    }

    /// <summary>
    /// 使用缺省密钥解密
    /// </summary>
    /// <param name="s">密文</param>
    /// <returns>明文</returns>
    public static string decrypt(string s)
    {
        return decrypt(s, DES_KEY_STRING);
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="s">密文</param>
    /// <param name="k">密钥</param>
    /// <returns>明文</returns>
    public static string decrypt(string s, string k)
    {
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        byte[] inputByteArray = new byte[s.Length / 2];
        for (int x = 0; x < s.Length / 2; x++)
        {
            int i = (Convert.ToInt32(s.Substring(x * 2, 2), 16));
            inputByteArray[x] = (byte)i;
        }
        des.Key = Encoding.ASCII.GetBytes(k); //建立加密对象的密钥和偏移量，此值重要，不能修改   
        des.IV = Encoding.ASCII.GetBytes(k);
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        StringBuilder ret = new StringBuilder(); //建立StringBuild对象，CreateDecrypt使用的是流对象，必须把解密后的文本变成流对象   
        return System.Text.Encoding.Default.GetString(ms.ToArray());
    }

    /// <summary>
    /// MD5加密
    /// </summary>
    /// <param name="s">明文</param>
    /// <returns>密文</returns>
    public static string md5(string s)
    {
        Byte[] clearBytes = Encoding.Default.GetBytes(s);
        Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
        return BitConverter.ToString(hashedBytes).ToLower().Replace("-", "");
    }

}
