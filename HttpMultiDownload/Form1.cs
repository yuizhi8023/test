using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HttpMultiDownload
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "http://sw.bos.baidu.com/sw-search-sp/software/952c9d6e73f50/QQ_8.9.20029.0_setup.exe";
            textBox2.Text = "test.exe";
        }
        private const int threadNumber = 3;
        private void button1_Click(object sender, EventArgs e)
        {
            MultiDownload md = new MultiDownload(threadNumber, textBox1.Text, AppDomain.CurrentDomain.BaseDirectory);
            md.Start();
            //HttpDownLoadFile(textBox1.Text, textBox2.Text);
        }
        private void HttpDownLoadFile(string sourceUrl, string targetFilename)
        {
            if (IsWebResourceAvailble(sourceUrl) == false)
            {
                MessageBox.Show("指定的资源无效!");
                return;
            }
            listBox1.Items.Add("同时接收线程数:" + threadNumber);
            HttpWebRequest request;
            long fileSize = 0;
            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(sourceUrl);
                request.Method = WebRequestMethods.Http.Head;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                fileSize = response.ContentLength;
                listBox1.Items.Add("文件大小:" + Math.Ceiling(fileSize / 1024.0f) + "KB");
                response.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            int downloadFileSize = (int)(fileSize / threadNumber);
            HttpDownload[] d = new HttpDownload[threadNumber];
            for (int i = 0; i < threadNumber; i++)
            {
                d[i] = new HttpDownload(listBox1, i);
                d[i].StartPosition = downloadFileSize * i;
                if (i < threadNumber - 1)
                {
                    d[i].FileSize = downloadFileSize;
                }
                else
                {
                    d[i].FileSize = (int)(fileSize - downloadFileSize * (i - 1));
                }
                d[i].IsFinish = false;
                d[i].TargetFileName = Path.GetFileNameWithoutExtension(targetFilename) + ".$$" + i;
                d[i].SourceUrl = textBox1.Text;
            }
            Thread[] threads = new Thread[threadNumber];
            for (int i = 0; i < threadNumber; i++)
            {
                threads[i] = new Thread(d[i].receive);
                threads[i].Start();
            }
            CombineFiles c = new CombineFiles(listBox1, d, textBox2.Text);
            Thread t = new Thread(c.Combine);
            t.Start();
        }
        private static bool IsWebResourceAvailble(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Head;
                request.Timeout = 2000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch (Exception ex) { System.Diagnostics.Trace.Write(ex.ToString()); return false; }
        }

    }
    public class HttpDownload
    {

        public bool IsFinish { get; set; }
        public string TargetFileName { get; set; }
        public int StartPosition { get; set; }
        public int FileSize { get; set; }
        public string SourceUrl{ get; set; }
        private int threadIndex;
        private ListBox listBox;
        private Stopwatch stopWatch = new Stopwatch();
        public HttpDownload(ListBox listBox,int threadIndex) {

            this.listBox = listBox;
            this.threadIndex = threadIndex;
        }
        public void receive()
        {
            stopWatch.Reset();
            stopWatch.Start();
            AddStatus("线程"+threadIndex+"开始接收");
            int totalBytes = 0;
            using (FileStream fs=new FileStream (TargetFileName,FileMode.Create))
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(SourceUrl);
                    request.AddRange(StartPosition, StartPosition + FileSize - 1);
                    AddStatus("线程" + threadIndex + "接收区间：" + StartPosition + "--" + (StartPosition + FileSize - 1));
                    Stream stream = request.GetResponse().GetResponseStream();
                    byte[] receiveBytes = new byte[512];
                    int readBytes = stream.Read(receiveBytes, 0, receiveBytes.Length);
                    while (readBytes > 0)
                    {
                        fs.Write(receiveBytes, 0, readBytes);
                        totalBytes += readBytes;
                        readBytes = stream.Read(receiveBytes, 0, receiveBytes.Length);
                    }
                    stream.Close();
                }
                catch (Exception ex)
                {
                    AddStatus("线程" + threadIndex + "接收出错：" + ex.Message);
                }
            }
            ChangeStatus("线程"+threadIndex+"开始接收,","接收完毕！",totalBytes);
            stopWatch.Stop();
            this.IsFinish = true;
        }
        public delegate void AddStatusDelegate(string message);
        public void AddStatus(string message) {
            if (listBox.InvokeRequired)
            {
                AddStatusDelegate d = AddStatus;
                listBox.Invoke(d,message);
            }
            else
            {
                listBox.Items.Add(message);
            }
        }
        public delegate void ChangeStatusDelegate(string oldMessage,string newMessage,int number);
        public void ChangeStatus(string oldMessage, string newMessage, int number) {

            if (listBox.InvokeRequired)
            {
                ChangeStatusDelegate d = ChangeStatus;
                listBox.Invoke(d,oldMessage,newMessage,number);
            }
            else
            {

                int i = listBox.FindString(oldMessage);
                if(i != -1)
                {
                    string[] items = new string[listBox.Items.Count];
                    listBox.Items.CopyTo(items, 0);
                    items[i] = oldMessage + " " + newMessage + " 接受字节数: " + Math.Ceiling(number / 1024.0f) + "KB,用时: " + stopWatch.ElapsedMilliseconds / 1000.0f;
                    listBox.Items.Clear();
                    listBox.Items.AddRange(items);
                    listBox.SelectedIndex = i;
                }
            }
        }
    }
    public class CombineFiles {
        private bool downloadFinish;
        private HttpDownload[] down;
        private ListBox listbox;
        string targetFileName;
        public CombineFiles(ListBox listbox,HttpDownload[] down,string targetFileName)
        {
            this.listbox = listbox;
            this.down = down;
            this.targetFileName = targetFileName;
        }
        public delegate void AddStatusDelegate(string message);
        public void AddStatus(string message)
        {
            if (listbox.InvokeRequired)
            {
                AddStatusDelegate d = AddStatus;
                listbox.Invoke(d, message);
            }
            else
            {
                listbox.Items.Add(message);
            }
        }
        public void Combine()
        {
            while (true)
            {
                downloadFinish = true;
                for (int i=0;i<down.Length;i++) {

                    if (down[i].IsFinish == false)
                    {
                        downloadFinish = false;
                        Thread.Sleep(100);
                        break;
                    }
                }
                if (downloadFinish == true) break;
            }
            AddStatus("下载完毕,开始合并临时文件!");
            FileStream targetFileStream;
            FileStream sourceFileStream;
            int readfile;
            byte[] bytes = new byte[8192];
            targetFileStream = new FileStream(targetFileName,FileMode.Create);
            for(int k = 0; k < down.Length; k++)
            {
                sourceFileStream = new FileStream(down[k].TargetFileName,FileMode.Open);
                while (true)
                {
                    readfile = sourceFileStream.Read(bytes,0,bytes.Length);
                    if (readfile > 0)
                    {
                        targetFileStream.Write(bytes, 0, readfile);
                    }
                    else
                        break;
                }
                sourceFileStream.Close();
            }
            targetFileStream.Close();
           for(int i = 0; i < down.Length; i++)
            {
                File.Delete(down[i].TargetFileName);
            }
            DateTime dt = DateTime.Now;
            AddStatus("合并完毕!");
        }

    }
    public class MultiDownload
    {
        #region 变量
        private int _threadNum;    //线程数量
        private long _fileSize;    //文件大小
        private string _fileUrl;   //文件地址
        private string _fileName;   //文件名
        private string _savePath;   //保存路径
        private short _threadCompleteNum; //线程完成数量
        private bool _isComplete;   //是否完成
        private volatile int _downloadSize; //当前下载大小(实时的)
        private Thread[] _thread;   //线程数组
        private List<string> _tempFiles = new List<string>();
        private object locker = new object();
        #endregion
        #region 属性
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize
        {
            get
            {
                return _fileSize;
            }
        }
        /// <summary>
        /// 当前下载大小(实时的)
        /// </summary>
        public int DownloadSize
        {
            get
            {
                return _downloadSize;
            }
        }
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsComplete
        {
            get
            {
                return _isComplete;
            }
        }
        /// <summary>
        /// 线程数量
        /// </summary>
        public int ThreadNum
        {
            get
            {
                return _threadNum;
            }
        }
        /// <summary>
        /// 保存路径
        /// </summary>
        public string SavePath
        {
            get
            {
                return _savePath;
            }
            set
            {
                _savePath = value;
            }
        }
        #endregion
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="threahNum">线程数量</param>
        /// <param name="fileUrl">文件Url路径</param>
        /// <param name="savePath">本地保存路径</param>
        public MultiDownload(int threahNum, string fileUrl, string savePath)
        {
            this._threadNum = threahNum;
            this._thread = new Thread[threahNum];
            this._fileUrl = fileUrl;
            this._savePath = savePath;
        }
        public void Start()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_fileUrl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            _fileSize = response.ContentLength;
            int singelNum = (int)(_fileSize / _threadNum);  //平均分配
            int remainder = (int)(_fileSize % _threadNum);  //获取剩余的
            request.Abort();
            response.Close();
            for (int i = 0; i < _threadNum; i++)
            {
                List<int> range = new List<int>();
                range.Add(i * singelNum);
                if (remainder != 0 && (_threadNum - 1) == i) //剩余的交给最后一个线程
                    range.Add(i * singelNum + singelNum + remainder - 1);
                else
                    range.Add(i * singelNum + singelNum - 1);
                //下载指定位置的数据
                int[] ran = new int[] { range[0], range[1] };
                _thread[i] = new Thread(new ParameterizedThreadStart(Download));
                _thread[i].Name = System.IO.Path.GetFileNameWithoutExtension(_fileUrl) + "_{0}".Replace("{0}", Convert.ToString(i + 1));
                _thread[i].Start(ran);
            }
        }
        private void Download(object obj)
        {
            Stream httpFileStream = null, localFileStram = null;
            try
            {
                int[] ran = obj as int[];
                string tmpFileBlock = System.IO.Path.GetTempPath() + Thread.CurrentThread.Name + ".tmp";
                _tempFiles.Add(tmpFileBlock);
                HttpWebRequest httprequest = (HttpWebRequest)WebRequest.Create(_fileUrl);
                httprequest.AddRange(ran[0], ran[1]);
                HttpWebResponse httpresponse = (HttpWebResponse)httprequest.GetResponse();
                httpFileStream = httpresponse.GetResponseStream();
                localFileStram = new FileStream(tmpFileBlock, FileMode.Create);
                byte[] by = new byte[5000];
                int getByteSize = httpFileStream.Read(by, 0, (int)by.Length); //Read方法将返回读入by变量中的总字节数
                while (getByteSize > 0)
                {
                    Thread.Sleep(20);
                    lock (locker) _downloadSize += getByteSize;
                    localFileStram.Write(by, 0, getByteSize);
                    getByteSize = httpFileStream.Read(by, 0, (int)by.Length);
                }
                lock (locker) _threadCompleteNum++;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
                if (httpFileStream != null) httpFileStream.Dispose();
                if (localFileStram != null) localFileStram.Dispose();
            }
            if (_threadCompleteNum == _threadNum)
            {
                Complete();
                _isComplete = true;
            }
        }
        /// <summary>
        /// 下载完成后合并文件块
        /// </summary>
        private void Complete()
        {
            Stream mergeFile = new FileStream(@_savePath, FileMode.Create);
            BinaryWriter AddWriter = new BinaryWriter(mergeFile);
            foreach (string file in _tempFiles)
            {
                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                    BinaryReader TempReader = new BinaryReader(fs);
                    AddWriter.Write(TempReader.ReadBytes((int)fs.Length));
                    TempReader.Close();
                }
                File.Delete(file);
            }
            AddWriter.Close();
        }
    }

}
