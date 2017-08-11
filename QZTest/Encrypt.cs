using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Configuration;

/// <summary>
/// 密码工具类
/// </summary>
public class Encrypt
{
    /// <summary>
    /// 默认密钥
    /// </summary>
    private static string _3DESKEY = "dGVjaHF1aWNrNjUxNzEzOThmamNzeXl3";

    /// <summary>
    /// MD5
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string MD5(string str)
    {
        return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(str))).Replace("-", "");
    }

    /// <summary>
    /// CRC32
    /// </summary>
    /// <param name="str"></param>
    /// <param name="toIntFlag">保存为32位带符号整数</param>
    /// <returns></returns>
    public static string CRC32(string str)
    {

        ulong[] Crc32Table = new ulong[256];
        for (int i = 0; i < 256; i++)
        {
            ulong Crc = (ulong)i;
            for (int j = 8; j > 0; j--)
            {
                if ((Crc & 1) == 1)
                    Crc = (Crc >> 1) ^ 0xEDB88320;
                else
                    Crc >>= 1;
            }
            Crc32Table[i] = Crc;
        }

        byte[] buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(str);
        ulong value = 0xffffffff;
        int len = buffer.Length;
        for (int i = 0; i < len; i++)
        {
            value = (value >> 8) ^ Crc32Table[(value & 0xFF) ^ buffer[i]];
        }
        ulong r = value ^ 0xffffffff;

        return r.ToString();
    }

    /// <summary>
    /// CRC32
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string CRE32ToInt(string str)
    {
        long r = long.Parse(CRC32(str));

        string result = r.ToString();

        if (r > 2147483647)
        {
            result = (r - 4294967296).ToString();
        }

        return result;
    }

    ///  <summary > 
    /// 3DES加密 
    ///  </summary > 
    ///  <param name="Value" >待加密字符串 </param > 
    ///  <param name="sKey" >密钥 </param > 
    ///  <returns >加密后字符串 </returns > 
    public static string encrypt3DES(string Value, string sKey)
    {
        string result = "";
        //构造对称算法 
        SymmetricAlgorithm mCSP = new TripleDESCryptoServiceProvider();

        ICryptoTransform ct;
        MemoryStream ms;
        CryptoStream cs;
        byte[] byt;
        mCSP.Key = Convert.FromBase64String(sKey);
        //指定加密的运算模式 
        mCSP.Mode = System.Security.Cryptography.CipherMode.ECB;
        //获取或设置加密算法的填充模式 
        mCSP.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
        ct = mCSP.CreateEncryptor(mCSP.Key, mCSP.IV);
        byt = Encoding.UTF8.GetBytes(Value);
        ms = new MemoryStream();
        cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
        cs.Write(byt, 0, byt.Length);
        cs.FlushFinalBlock();
        cs.Close();

        byte[] _result = ms.ToArray();
        for (int i = 0; i < _result.Length; i++)
        {
            result += _result[i].ToString("X2").ToUpper();
        }
        return result;
    }

    /// <summary>
    /// 3DES加密，采用默认密钥
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static string encrypt3DES(string Value)
    {
        return encrypt3DES(Value, _3DESKEY);
    }

    ///  <summary > 
    /// 3DES解密 
    ///  </summary > 
    ///  <param name="Value" >待解密字符串(16进制)</param > 
    ///  <param name="sKey" >密钥 </param > 
    ///  <returns >解密后字符串</returns > 
    public static string decrypt3DES(string Value, string sKey)
    {
        //构造对称算法 
        SymmetricAlgorithm mCSP = new TripleDESCryptoServiceProvider();

        ICryptoTransform ct;
        MemoryStream ms;
        CryptoStream cs;
        byte[] byt;
        mCSP.Key = Convert.FromBase64String(sKey);
        mCSP.Mode = System.Security.Cryptography.CipherMode.ECB;
        mCSP.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
        ct = mCSP.CreateDecryptor(mCSP.Key, mCSP.IV);

        int len = Value.Length / 2;
        byt = new byte[len];
        for (int i = 0; i < len; i++)
        {
            byt[i] = Convert.ToByte(Value.Substring(i * 2, 2), 16);
        }
        ms = new MemoryStream();
        cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
        cs.Write(byt, 0, byt.Length);
        cs.FlushFinalBlock();
        cs.Close();
        return Encoding.UTF8.GetString(ms.ToArray());


    }

    /// <summary>
    /// 3DES解密，采用默认密钥
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static string decrypt3DES(string Value)
    {
        return decrypt3DES(Value, _3DESKEY);
    }

}