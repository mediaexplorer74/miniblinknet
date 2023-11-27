using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Kyozy.MiniblinkNet;

namespace Demo
{
    public partial class Form_Main : Form
    {
        private WebView m_wke;
        private ToolStripSpringTextBox m_tsstbUrl;

        public Form_Main()
        {
            m_wke = new WebView(); //构造函数可以是无参，但是后面必须调用 Bind 方法
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //添加一个自动填满ToolStrip 的 TextBox
            m_tsstbUrl = new ToolStripSpringTextBox();
            m_tsstbUrl.KeyDown += new KeyEventHandler(m_tsstbUrl_KeyDown);
            toolStrip1.Items.Add(m_tsstbUrl);
            

            if (!m_wke.Bind(pictureBox1)) //随便绑定一个控件或窗口
            {
                return;
            }
            //wkeProxy proxy = new wkeProxy();
            //proxy.HostName = "121.31.103.6";
            //proxy.Port = 8123;
            //proxy.Type = wkeProxyType.HTTP;

            //m_wke.SetViewProxy(proxy);
            m_wke.SetDeviceParameter("screen.width", string.Empty, 1440);
            m_wke.NavigationToNewWindowEnable = false;

            //事件
            m_wke.OnTitleChange += new EventHandler<TitleChangeEventArgs>(m_wke_OnTitleChange);
            m_wke.OnURLChange += new EventHandler<UrlChangeEventArgs>(m_wke_OnURLChange);
            m_wke.OnLoadingFinish += new EventHandler<LoadingFinishEventArgs>(m_wke_OnLoadingFinish);
            m_wke.OnLoadUrlBegin += new EventHandler<LoadUrlBeginEventArgs>(m_wke_OnLoadUrlBegin);
            m_wke.OnDocumentReady += new EventHandler<DocumentReadyEventArgs>(m_wke_OnDocumentReady);
            m_wke.OnDownload += new EventHandler<DownloadEventArgs>(m_wke_OnDownload);

            //测试绑定JsFunction
            JsValue.BindFunction("OnBtnClick", new wkeJsNativeFunction(OnBtnClick), 0);

            m_wke.LoadHTML(Properties.Resources.String_Test);
            //m_wke.LoadURL(@"http://www.baidu.com");
        }

        void m_wke_OnDownload(object sender, DownloadEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("{0}:{1}", "OnDownload", e.URL);
            e.Cancel = true;
        }

        void m_wke_OnDocumentReady(object sender, DocumentReadyEventArgs e)
        {
            JsValue v = m_wke.RunJsByFrame(e.Frame, "document.getElementsByTagName('html')[0].outerHTML;");
            //System.Diagnostics.Debug.WriteLine(v.ToString(m_wke.GlobalExec()));
        }

        void m_wke_OnLoadUrlBegin(object sender, LoadUrlBeginEventArgs e)
        {
            statusStrip1.Items[0].Text = string.Format("OnLoadUrlBegin: {0}",e.URL);
            
        }

        void m_wke_OnLoadingFinish(object sender, LoadingFinishEventArgs e)
        {
            statusStrip1.Items[0].Text = string.Format("OnLoadingFinish:{0}, error: {1} ", e.LoadingResult, e.FailedReason);
        }

        //js中的 OnBtnClick() 会调用到此处
        long OnBtnClick(IntPtr es, IntPtr param)
        {
            JsValue v = m_wke.RunJS("return document.getElementsByTagName('html')[0].outerHTML;");
            System.Diagnostics.Debug.WriteLine(v.ToString(es)); // 带参数的 ToString 方法来取文本
            return JsValue.UndefinedValue();
        }

        void m_tsstbUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                m_wke.Load(m_tsstbUrl.Text);
            }
        }

        void m_wke_OnURLChange(object sender, UrlChangeEventArgs e)
        {
            m_tsstbUrl.Text = e.URL;

        }

        void m_wke_OnTitleChange(object sender, TitleChangeEventArgs e)
        {
            this.Text = e.Title;
        }
        private void tsbtnMain_Click(object sender, EventArgs e)
        {
            m_wke.LoadHTML(Properties.Resources.String_Test);
        }
        private void tsbtnBack_Click(object sender, EventArgs e)
        {
            m_wke.GoBack();

        }

        private void tsbtnForward_Click(object sender, EventArgs e)
        {
            m_wke.GoForward();
        }

        private void tsbtnReload_Click(object sender, EventArgs e)
        {
            m_wke.Reload();
        }

        private void tsbtnStop_Click(object sender, EventArgs e)
        {
            m_wke.StopLoading();
        }

        private void 截图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_ShowBmp fs = new Form_ShowBmp(m_wke.PrintToBitmap());
            fs.ShowDialog(this);
            fs.Dispose();

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form_Transparent ft = new Form_Transparent();
            ft.ShowDialog(this);
            ft.Dispose();
        }

        private void 访问所有CookieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wkeCookieVisitor visitor = new wkeCookieVisitor((IntPtr usetData, string name, string value, string domain, string path, int secure, int httpOnly, ref int expires) =>
            {
                System.Diagnostics.Debug.WriteLine("name={0},value={1},domain={2},path={3},secure={4},httpOnly={5},expires={6}", name, value, domain, path, secure, httpOnly, expires);
                return false;
            });
            m_wke.VisitAllCookie(visitor, IntPtr.Zero);
        }

        private void Form_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_wke.Dispose();
        }

        private void 临时测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        


    }
}
