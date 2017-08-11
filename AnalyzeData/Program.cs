using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace AnalyzeData
{
    class Program
    {
        static System.Timers.Timer timer = new System.Timers.Timer();
        static string path = @"C:\wwwroot\files";
        //static string path = AppDomain.CurrentDomain.BaseDirectory+"files";
        static DataTable dt_insert = new DataTable();
        static DataTable dt_update = new DataTable();
        static List<string> list_imei = new List<string>();
        static void Main(string[] args)
        {
            StringBuilder xml = new StringBuilder();
            xml.Append(Environment.NewLine + "<?xml version=\"1.0\" encoding=\"utf - 8\" ?>");
            xml.Append(Environment.NewLine + "<opDetail>");
            xml.Append(Environment.NewLine + "  <recordInfo>");
            xml.Append(Environment.NewLine + "    <fieldInfo>");
            xml.Append(Environment.NewLine + "  <fieldChName>开通工单流水号</fieldChName>");
            xml.Append(Environment.NewLine + "  <fieldEnName> OrderNo </fieldEnName>");
            xml.Append(Environment.NewLine + "<fieldContent>FJ-327-170718-30565</fieldContent>");
            xml.Append(Environment.NewLine + "  </fieldInfo>");
            xml.Append(Environment.NewLine + "</recordInfo>");
            xml.Append(Environment.NewLine + "</opDetail>");
           string x= xml.ToString();

            dt_insert.Columns.AddRange(new DataColumn[] {
                new DataColumn("",typeof(string)),
            });
            //getAllFiles();
            timer.Interval = 5 * 60 * 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            Thread.Sleep(Timeout.Infinite);
        }
        static object o_o = new object();
        static int In_timer = 0;
        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (o_o)
            {
                try
                {
                    In_timer = 1;
                    getAllFiles();
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                In_timer = 0;
            }
        }
        private static void getAllFiles()
        {
           string [] files= Directory.GetFiles(path);
            List<string> list = new List<string>();
            foreach(string file in files)
            {
                if (Math.Abs((new FileInfo(file).CreationTime - DateTime.Now).Days) > 1)
                    continue;
                list.Add(file);
                if (list.Count >= 1000)
                {
                   List<string> list_t= list.ToList();
                    DispatchWork(list_t);
                    list.Clear();
                }
            }
            if (list.Count > 0)
            {
                List<string> list_t = list.ToList();
                DispatchWork(list_t);
                list.Clear();
            }
        }
        private static void DispatchWork(List<string> list)
        {
            Thread th = new Thread(DataAnaly);
            th.Start(list);
        }
        private static void DataAnaly(object o)
        {
            List<string> list=(List<string>)o;
            foreach(string file in list)
            {
                try
                {
                    string req = File.ReadAllText(file);
                    string imei = Path.GetFileName(file);
                    list_imei.Add(imei);
                    TemplateHelper.GetPERIODICValues(imei, TemplateHelper.ParseInform(req));
                    Console.WriteLine(DateTime.Now+"   "+imei+" Done!");
                }
                catch (Exception ex){ Console.WriteLine(ex.ToString()); }

            }
        }
    }
}
