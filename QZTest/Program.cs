using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QZTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"D:\\青海圈子\\20170509030000\\c-client-base_content_20170509030000_000.txt";
            //每次读取的字节数  
            int iBufferSize = 1024000;
            byte[] buffer = new byte[iBufferSize];
            FileStream fs = null;
            try
            {
                fs = new FileStream(filePath, FileMode.Open);
                //文件流的长度  
                long lFileSize = fs.Length;
                //文件需要读取次数  
                int iTotalCount = (int)Math.Ceiling((double)(lFileSize / iBufferSize));
                //当前读取次数  
                int iTempCount = 0;

                while (iTempCount < iTotalCount)
                {
                    //每次从最后读到的位置读取下一个[iBufferSize]的字节数  
                    fs.Read(buffer, 0, iBufferSize);
                    //将字节转换成字符  
                    string strRead = Encoding.UTF8.GetString(buffer);
                    string[] sss = strRead.Split('\n');
                    foreach(string s in sss)
                    {
                       string [] ss= s.Split('');
                    }
                    //此处加入你的处理逻辑  
                   // Console.Write(strRead);
                }
            }
            catch (Exception ex)
            {
                //异常处理  
            }
            finally
            {
                if (fs != null)
                {
                    fs.Dispose();
                }
            }
            long ttargetRowNum = 10000000;
            DateTime beginTime = DateTime.Now;
            string line = CreateMemoryMapFile(ttargetRowNum);
            double totalSeconds = DateTime.Now.Subtract(beginTime).TotalSeconds;
            Console.WriteLine(line);
            Console.WriteLine(string.Format("查找第{0}行,共耗时:{1}s", ttargetRowNum, totalSeconds));
            Console.ReadLine();
            string content = "";
            using (StreamReader sr = new StreamReader("D:\\青海圈子\\20170509030000\\c-client-base_content_20170509030000_000.txt", Encoding.Default))
            {
                content = sr.ReadToEnd();
            }
            string para = "clientversion=1.1";
            //para = Encrypt.encrypt3DES(para);

            //string method = "QueryAllElement";
            //string para = Encrypt.encrypt3DES("phoneNo=" + phoneNo);
            //string res = Utilities.GetHttpResult("http://qz.qh.chinamobile.com/BaseInterface/Boss/General.aspx?method=" + method + "&para=" + para, "");
            string phoneNo = "18809717474";
            string resc = GetHttpResult("http://112.4.19.152:8060/acms/GetTokenServlet", "","");
            string method = "Auth";
            para = Encrypt.encrypt3DES("phoneNo=" + phoneNo + "&ReqType=3&" + "&ContentID=50000000000005716198&chapterId=351795086&vt=1&pageOrder=0&offset=0&numberOfPages=0&UserAgent=" + phoneNo);//图书
            para = Encrypt.encrypt3DES("phoneNo=" + phoneNo + "&ReqType=1&" + "&ContentID=20000000060084400425&UsingType=0");//音乐
            para = Encrypt.encrypt3DES("phoneNo=" + phoneNo + "&ReqType=2&" + "&ContentID=10000000006061367000&serviceID=006061367000&serviceName=街机捕鱼狂想曲HD");//游戏
            string res =GetHttpResult("http://qz.qh.chinamobile.com/BaseInterface/Migu/General.aspx?method=" + method + "&para=" + para,null,phoneNo);
            if (!res.Contains("\"ELEMENT_ID\":\"20\",\"ELEMENT_NAME\":\"彩铃\""))
            {
                //未订购彩铃，则开通彩铃功能
                         if (res.Contains("\"respCode\":\"0\",\"respDesc\":\"ok\""))//开通成功
                {
                    //do your work

                }
            }

        }

        private const string TXT_FILE_PATH = "D:\\青海圈子\\20170509030000\\c-client-base_content_20170509030000_000.txt";
        private const string SPLIT_VARCHAR = "";
        private const char SPLIT_CHAR = '囧';
        private static long FILE_SIZE = 0;

        /// <summary>
        /// 创建内存映射文件
        /// </summary>
        private static string CreateMemoryMapFile(long ttargetRowNum)
        {
            string line = string.Empty;
            using (FileStream fs = new FileStream(TXT_FILE_PATH, FileMode.Open, FileAccess.ReadWrite))
            {
                long targetRowNum = ttargetRowNum + 1;//目标行
                long curRowNum = 1;//当前行
                FILE_SIZE = fs.Length;
                using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(fs, "test", fs.Length, MemoryMappedFileAccess.ReadWrite, null, HandleInheritability.None, false))
                {
                    long offset = 0;
                    //int limit = 250;
                    int limit = 200;
                    try
                    {
                        StringBuilder sbDefineRowLine = new StringBuilder();
                        do
                        {
                            long remaining = fs.Length - offset;
                            using (MemoryMappedViewStream mmStream = mmf.CreateViewStream(offset, remaining > limit ? limit : remaining))
                            //using (MemoryMappedViewStream mmStream = mmf.CreateViewStream(offset, remaining))
                            {
                                offset += limit;
                                using (StreamReader sr = new StreamReader(mmStream))
                                {
                                    //string ss = sr.ReadToEnd().ToString().Replace("\n", "囧").Replace(Environment.NewLine, "囧");
                                    string ss = sr.ReadToEnd().ToString().Replace("\n", SPLIT_VARCHAR).Replace(Environment.NewLine, SPLIT_VARCHAR);
                                    if (curRowNum <= targetRowNum)
                                    {
                                        if (curRowNum < targetRowNum)
                                        {
                                            string s = sbDefineRowLine.ToString();
                                            int pos = s.LastIndexOf(SPLIT_CHAR);
                                            if (pos > 0)
                                                sbDefineRowLine.Remove(0, pos);

                                        }
                                        else
                                        {
                                            line = sbDefineRowLine.ToString();
                                            return line;
                                        }
                                        if (ss.Contains(SPLIT_VARCHAR))
                                        {
                                            curRowNum += GetNewLineNumsOfStr(ss);
                                            sbDefineRowLine.Append(ss);
                                        }
                                        else
                                        {
                                            sbDefineRowLine.Append(ss);
                                        }
                                    }
                                    //sbDefineRowLine.Append(ss);
                                    //line = sbDefineRowLine.ToString();
                                    //if (ss.Contains(Environment.NewLine))
                                    //{
                                    //    ++curRowNum;
                                    //    //curRowNum++;
                                    //    //curRowNum += GetNewLineNumsOfStr(ss);
                                    //    //sbDefineRowLine.Append(ss);
                                    //}
                                    //if (curRowNum == targetRowNum)
                                    //{
                                    //    string s = "";
                                    //}

                                    sr.Dispose();
                                }

                                mmStream.Dispose();
                            }
                        } while (offset < fs.Length);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    return line;
                }
            }
        }

        private static long GetNewLineNumsOfStr(string s)
        {
            string[] _lst = s.Split(SPLIT_CHAR);
            return _lst.Length - 1;
        }
        #region MD5加密
        private static string MD5Encrypt(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();

            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }

            return ret.PadLeft(32, '0').ToUpper();
        }
        private static string MD5Encrypt_Migu(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();

            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }

            return ret.PadLeft(32, '0').ToLower();
        }
        #endregion
        #region HTTP模板
        public static CookieContainer m_Cookies = new CookieContainer();
        public enum Compression
        {
            GZip,
            Deflate,
            None,
        }

        /// <summary>
        /// 获取HttpWebRequest模板
        /// </summary>
        /// <param name="url">URL地址</param>
        /// <param name="postdata">POST</param>
        /// <param name="cookies">Cookies</param>
        /// <returns></returns>
        private static HttpWebRequest GetHttpRequest_Migu(string url, string postdata)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string signature = MD5Encrypt_Migu(url + timestamp);
            HttpWebRequest request = HttpWebRequest.Create(new Uri(url)) as HttpWebRequest;

            request.ContentType = "text/xml";
            request.ServicePoint.ConnectionLimit = 300;
            ServicePointManager.Expect100Continue = false;
            request.Headers.Add("MG-Timestamp", timestamp);//时间戳
            request.Headers.Add("MG-Signature", signature);//请求签名
            request.Referer = url;
            request.Accept = "*/*";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)";
            request.AllowAutoRedirect = true;
            if (postdata != null && postdata != "")
            {
                request.Method = "POST";
                byte[] byte_post = Encoding.Default.GetBytes(postdata);
                request.ContentLength = byte_post.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(byte_post, 0, byte_post.Length);
                }
            }
            else
            {
                request.Method = "GET";
            }
            return request;
        }

        /// <summary>
        /// 提取HttpWebResponse文本内容
        /// </summary>
        /// <param name="resp">HttpWebResponse响应包</param>
        /// <returns></returns>
        private static string GetResponseContent_Migu(HttpWebResponse resp)
        {

            if (resp.StatusCode != HttpStatusCode.OK)
                throw new Exception("远程服务器返回状态码: " + resp.StatusCode);

            Encoding enc = Encoding.UTF8;
            if (resp.CharacterSet != null && resp.CharacterSet != "")
                enc = Encoding.GetEncoding(resp.CharacterSet);

            Compression comp = Compression.None;
            if (resp.ContentEncoding != null && resp.ContentEncoding.Trim().ToUpper() == "GZIP")
                comp = Compression.GZip;
            else if (resp.ContentEncoding != null && resp.ContentEncoding.Trim().ToUpper() == "DEFLATE")
                comp = Compression.Deflate;

            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, enc))
            {
                StreamReader sr;
                switch (comp)
                {
                    case Compression.GZip:
                        sr = new StreamReader(new System.IO.Compression.GZipStream(resp.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress), enc);
                        break;
                    case Compression.Deflate:
                        sr = new StreamReader(new System.IO.Compression.DeflateStream(resp.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress), enc);
                        break;
                    default:
                        sr = new StreamReader(resp.GetResponseStream(), enc);
                        break;
                }

                while (!sr.EndOfStream)
                {
                    char[] buf = new char[16000];
                    int read = sr.ReadBlock(buf, 0, 16000);
                    StringBuilder sb = new StringBuilder();
                    sb.Append(buf, 0, read);
                    sw.Write(buf, 0, read);
                }
                sr.Close();
            }

            byte[] mbuf = ms.GetBuffer();
            string sbuf = enc.GetString(mbuf);
            return sbuf;
        }

        /// <summary>
        /// 获取HttpWebRequest返回值
        /// </summary>
        /// <param name="url">URL地址</param>
        /// <param name="postdata">PostData</param>
        /// <returns></returns>
        private static string GetHttpResult_Migu(string url, string postdata)
        {
            try
            {
                HttpWebRequest request = GetHttpRequest_Migu(url, postdata);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string res = GetResponseContent_Migu(response);
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 获取HttpWebRequest模板
        /// </summary>
        /// <param name="url">URL地址</param>
        /// <param name="postdata">POST</param>
        /// <param name="cookies">Cookies</param>
        /// <returns></returns>
        private static HttpWebRequest GetHttpRequest_Boss(string url, string postdata, CookieContainer cookies)
        {
            HttpWebRequest request = HttpWebRequest.Create(new Uri(url)) as HttpWebRequest;

            //request.CookieContainer = cookies;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ServicePoint.ConnectionLimit = 300;
            ServicePointManager.Expect100Continue = false;
            request.Referer = url;
            request.Accept = "*/*";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)";
            request.AllowAutoRedirect = true;
            if (postdata != null && postdata != "")
            {
                request.Method = "POST";
                byte[] byte_post = Encoding.Default.GetBytes(postdata);
                request.ContentLength = byte_post.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(byte_post, 0, byte_post.Length);
                }
            }
            else
            {
                request.Method = "GET";
            }
            return request;
        }

        /// <summary>
        /// 提取HttpWebResponse文本内容
        /// </summary>
        /// <param name="resp">HttpWebResponse响应包</param>
        /// <returns></returns>
        private static string GetResponseContent_Boss(HttpWebResponse resp)
        {

            if (resp.StatusCode != HttpStatusCode.OK)
                throw new Exception("远程服务器返回状态码: " + resp.StatusCode);

            Encoding enc = Encoding.UTF8;
            if (resp.CharacterSet != null && resp.CharacterSet != "")
                enc = Encoding.GetEncoding(resp.CharacterSet);

            Compression comp = Compression.None;
            if (resp.ContentEncoding != null && resp.ContentEncoding.Trim().ToUpper() == "GZIP")
                comp = Compression.GZip;
            else if (resp.ContentEncoding != null && resp.ContentEncoding.Trim().ToUpper() == "DEFLATE")
                comp = Compression.Deflate;

            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, enc))
            {
                StreamReader sr;
                switch (comp)
                {
                    case Compression.GZip:
                        sr = new StreamReader(new System.IO.Compression.GZipStream(resp.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress), enc);
                        break;
                    case Compression.Deflate:
                        sr = new StreamReader(new System.IO.Compression.DeflateStream(resp.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress), enc);
                        break;
                    default:
                        sr = new StreamReader(resp.GetResponseStream(), enc);
                        break;
                }

                while (!sr.EndOfStream)
                {
                    char[] buf = new char[16000];
                    int read = sr.ReadBlock(buf, 0, 16000);
                    StringBuilder sb = new StringBuilder();
                    sb.Append(buf, 0, read);
                    sw.Write(buf, 0, read);
                }
                sr.Close();
            }

            byte[] mbuf = ms.GetBuffer();
            string sbuf = enc.GetString(mbuf);
            return sbuf;
        }

        /// <summary>
        /// 获取HttpWebRequest返回值
        /// </summary>
        /// <param name="url">URL地址</param>
        /// <param name="postdata">PostData</param>
        /// <returns></returns>
        private static string GetHttpResult_Boss(string url, string postdata)
        {
            try
            {
                HttpWebRequest request = GetHttpRequest_Boss(url, postdata, m_Cookies);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //m_Cookies.Add(response.Cookies);

                string res = GetResponseContent_Boss(response);
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 获取HttpWebRequest模板
        /// </summary>
        /// <param name="url">URL地址</param>
        /// <param name="postdata">POST</param>
        /// <param name="cookies">Cookies</param>
        /// <returns></returns>
        public static HttpWebRequest GetHttpRequest(string url, string postdata, string phoneno)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            HttpWebRequest request = HttpWebRequest.Create(new Uri(url)) as HttpWebRequest;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ServicePoint.ConnectionLimit = 300;
            ServicePointManager.Expect100Continue = false;
            request.Headers.Add("X-Up-Calling-Line-ID", phoneno);//手机号
                                                                 // request.Headers.Add("User-Agent", "");//终端标识
            request.Headers.Add("X-Channel-Code", "300000100001");//渠道代码
            request.Headers.Add("X-Timestamp", timestamp);//时间戳
            request.Headers.Add("X-Signature", MD5Encrypt(url + phoneno + timestamp + "aspire"));//请求签名
            request.Headers.Add("x-up-bear-type", "CMNET");//网络接入类型
            request.Headers.Add("excode", "100000000000000001");//自定义流水号
            request.Referer = url;
            request.Accept = "*/*";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)";
            request.AllowAutoRedirect = true;
            if (postdata != null && postdata != "")
            {
                request.Method = "POST";
                byte[] byte_post = Encoding.Default.GetBytes(postdata);
                request.ContentLength = byte_post.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(byte_post, 0, byte_post.Length);
                }
            }
            else
            {
                request.Method = "GET";
            }
            return request;
        }

        /// <summary>
        /// 提取HttpWebResponse文本内容
        /// </summary>
        /// <param name="resp">HttpWebResponse响应包</param>
        /// <returns></returns>
        public static string GetResponseContent(HttpWebResponse resp)
        {

            if (resp.StatusCode != HttpStatusCode.OK)
                throw new Exception("远程服务器返回状态码: " + resp.StatusCode);

            Encoding enc = Encoding.UTF8;
            if (resp.CharacterSet != null && resp.CharacterSet != "")
                enc = Encoding.GetEncoding(resp.CharacterSet);

            Compression comp = Compression.None;
            if (resp.ContentEncoding != null && resp.ContentEncoding.Trim().ToUpper() == "GZIP")
                comp = Compression.GZip;
            else if (resp.ContentEncoding != null && resp.ContentEncoding.Trim().ToUpper() == "DEFLATE")
                comp = Compression.Deflate;

            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, enc))
            {
                StreamReader sr;
                switch (comp)
                {
                    case Compression.GZip:
                        sr = new StreamReader(new System.IO.Compression.GZipStream(resp.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress), enc);
                        break;
                    case Compression.Deflate:
                        sr = new StreamReader(new System.IO.Compression.DeflateStream(resp.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress), enc);
                        break;
                    default:
                        sr = new StreamReader(resp.GetResponseStream(), enc);
                        break;
                }

                while (!sr.EndOfStream)
                {
                    char[] buf = new char[16000];
                    int read = sr.ReadBlock(buf, 0, 16000);
                    StringBuilder sb = new StringBuilder();
                    sb.Append(buf, 0, read);
                    sw.Write(buf, 0, read);
                }
                sr.Close();
            }

            byte[] mbuf = ms.GetBuffer();
            string sbuf = enc.GetString(mbuf);
            return sbuf;
        }

        /// <summary>
        /// 获取HttpWebRequest返回值
        /// </summary>
        /// <param name="url">URL地址</param>
        /// <param name="postdata">PostData</param>
        /// <returns></returns>
        public static string GetHttpResult(string url, string postdata, string phoneno)
        {
            try
            {
                HttpWebRequest request = GetHttpRequest(url, postdata, phoneno);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                return GetResponseContent(response);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        ///   <summary>   
        ///   去除HTML标记   
        ///   </summary>   
        ///   <param   name="NoHTML">包括HTML的源码 </param>   
        ///   <returns>已经去除后的文字</returns>   
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
        #endregion
    }
}
