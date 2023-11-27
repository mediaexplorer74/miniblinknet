using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Kyozy.MiniblinkNet;
using System.IO;
using System.Runtime.InteropServices;

namespace Demo
{
    public partial class Form_Transparent : Form
    {
        private MemoryFileSystem m_mfs;
        private WebView m_webView;

        [DllImport("user32.dll", EntryPoint = "SendMessageW")]
        private static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        [DllImport("user32.dll", EntryPoint = "PostMessageW")]
        private static extern void PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private static extern int ReleaseCapture();

        public Form_Transparent()
        {
            m_webView = new WebView();
            //m_mfs = new MemoryFileSystem();
            InitializeComponent();
        }

        private void Form_Transparent_Load(object sender, EventArgs e)
        {
            m_webView.Bind(this, true);

            m_webView.OnLoadUrlBegin += new EventHandler<LoadUrlBeginEventArgs>(m_webView_OnLoadUrlBegin);

            //不再使用这个老接口，采样上面的 OnLoadUrlBegin 事件
            //m_mfs.OnDataLoaded += new EventHandler<DataLoadedEventArgs>(m_mfs_OnDataLoaded);
            //WebView.SetFileSystem(m_mfs);

            

            JsValue.BindFunction("JsCallExit", new wkeJsNativeFunction(JsCallExit), 0);



            m_webView.LoadFile("main.htm");
        }


        void m_webView_OnLoadUrlBegin(object sender, LoadUrlBeginEventArgs e)
        {
            string name = Path.GetFileName(e.URL);
            switch (name)
            {
                case "main.htm":
                    {
                        WebView.NetSetData(e.Job, Properties.Resources.String_HtmlTransparent);
                        e.Cancel = true;
                    }
                    break;
                case "1.png":
                    {
                        WebView.NetSetData(e.Job, Properties.Resources._1);
                        e.Cancel = true;
                    }
                    break;
                default:
                    break;
            }
        }

        void m_webView_OnDocumentReady(object sender, DocumentReadyEventArgs e)
        {
            IntPtr es = m_webView.GlobalExec();
            long func1 = JsValue.FunctionValue(es, new jsCallAsFunction(JsCallTest1));
            JsValue.SetGlobal(es, "JsCallTest1", func1);

            long func2 = JsValue.FunctionValue(es, new jsCallAsFunction(JsCallTest2));
            JsValue.SetGlobal(es, "JsCallTest2", func2);

            //long funcExit = JsValue.FunctionValue(es, new jsCallAsFunction(JsCallExit));
            //JsValue.SetGlobal(es, "JsCallExit", funcExit);



            long funcMoveFrame = JsValue.FunctionValue(es, new jsCallAsFunction(JsCallMoveFrame));
            JsValue.SetGlobal(es, "JsCallMoveFrame", funcMoveFrame);
        }

        //void m_mfs_OnDataLoaded(object sender, DataLoadedEventArgs e)
        //{
        //    string name = Path.GetFileName(e.Path);
        //    System.Diagnostics.Debug.WriteLine(e.Path);
        //    switch (name)
        //    {
        //        case "main.htm":
        //            e.StringData = Properties.Resources.String_HtmlTransparent;
        //            break;
        //        case "1.png":
        //            e.ImageData = Properties.Resources._1;
        //            break;
        //        default:
        //            break;
        //    }

        //}

        long JsCallTest1(IntPtr es, long obj, JsValue[] args)
        {
            MessageBox.Show(JsValue.Arg(es, 0).ToString(es));
            return JsValue.UndefinedValue();
        }

        long JsCallTest2(IntPtr es, long obj, JsValue[] args)
        {
            if (args.Length != 2)
                return JsValue.UndefinedValue();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("参数1：" + args[0].ToString(es));
            sb.AppendLine("参数2：" + args[1].ToInt32(es));
           
            MessageBox.Show(sb.ToString());

            return JsValue.UndefinedValue();
        }

        long JsCallExit(IntPtr es, IntPtr userData)
        {
            PostMessage(m_webView.HostHandle, 16, 0, 0);
            return JsValue.UndefinedValue();
        }
        long JsCallMoveFrame(IntPtr es, long obj, JsValue[] args)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 161, 2, 0);
            return JsValue.UndefinedValue();
        }

        private void Form_Transparent_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_webView.Dispose();
        }

    }
}
