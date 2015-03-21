using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Xml;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Management;
using System.Net;
using System.Reflection;
using System.Data.SqlClient;
using Microsoft.Win32;

namespace testdm
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(
        IntPtr hWnd, // handle to window   
        int id, // hot key identifier   
        KeyModifiers fsModifiers, // key-modifier options   
        Keys vk // virtual-key code   
        );
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(
        IntPtr hWnd, // handle to window   
        int id // hot key identifier   
        );
        [Flags()]
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8
        }
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd,int Msg,int wParam,int lParam);
        /// <summary>
        /// 3个测试用全局大漠对象
        /// </summary>
        CDmSoft[] testDm = new CDmSoft[10];
        CDmSoft[] megaphoneTestDm = new CDmSoft[10];
        CDmSoft[] startupTestDm = new CDmSoft[10];
        CDmSoft[] startupCloudTestDm = new CDmSoft[10];
        /// <summary>
        /// 一个全局大漠对象
        /// </summary>
        CDmSoft hole_Dm = new CDmSoft();
        /// <summary>
        /// 线程集合
        /// </summary>
        List<Thread> Thread_List = new List<Thread>();
        List<Thread> megaphoneThread_List = new List<Thread>();
        /// <summary>
        /// 句柄数组(动态)
        /// </summary>
        ArrayList hwndArrary = new ArrayList();
        ArrayList megaphoneHwndArrary = new ArrayList();
        /// <summary>
        ///   string数组
        /// </summary>
        string [] memberId      = new string[10] ;
        string [] eqmExt        = new string[9] ;
        string [] eqm           = new string[9] ;
        string[] chooseRoute    = new string[10];
        string [] hero          = new string[7] ;
        public static string[] account        = new string[10];
        /// <summary>
        /// string数据
        /// </summary>
        string system, allRoute, matchMode, killWay, chooseMode, tempStr, stopTime, megaphoneText,skillSequence;
        /// <summary>
        /// 整形数据和布尔数据
        /// </summary>
        /// 
        Thread threadGameCrash,threadStartup;
        private int [] loginState = new int[10];
        private int escapeMode,gameNeedUpdate, gatTime, inviteTeammate, waitMemberRevive, leadIsMe, haveLead, disRoute, timeFun, reconnectNum, autoLogin, bindIndex, megaphoneIsRunning, mainIsRunning, banSkillBeforeGather, downCpuValue, downCpu, megaphoneDelay, shutdown, inningStopNum, inningStopFun;
        private int[,] heroCoordinate = new int[7,2] ;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;

        #region 程序验证控制
        const int threadNum = 1;
        const int productCode   = 1; // 1  --  永久版    2  --  散客双开 3  --  散客五开  4  --  工作室
        #endregion

        #region "公共变量"
        public static Verify a = new Verify(productCode);
        public static string key = "";
        public static bool loginSuccess ;
        public static string strFilePath = System.Windows.Forms.Application.StartupPath + "\\config.ini";//获取INI文件路径
        public static string strSec      = ""; //setction
        public static string startupPath, startupWay;
        #endregion
        #region "类"
        public class Verify
        {
            private const string KEY2 = "wWnXh&\\|3>l)K\"fdIJEG8L?#O:@;.01Y']}7u_-p9b$tBHNc*q!/jP6y4CkzFog%iVS,=xQTa(+vAUMsemR^2[5<DZ{r";
            private const string KEY3 = "0123456789!@Q#WA$ESZ%RDX^TFC&YGV*UHB(IJN)OKM_PL<+{:>}\"?|/']\\.;[=,lp-mkonjibhuvgycftxdrzsewaq";
            private const string KEY = "YfYDpf@fD@WsfVv3K}_W:K%%";
            /// <summary>
            /// 验证地址
            /// </summary>
            private const string BINDURL = "2V-E]sJwp1vALV2AYwG0L=&A-aL0:?Lx]sMwf]L+$1W5+a0%";
            private const string BINDURL_PERSON = "2V-E]sJwp1vALV2AYwG0L=&A-aL0:?Lx]sMwf]L+$1W5pM%%";
            private const string UNBINDURL = "2V-E]sJwp1vALV2AYwG0L=&A-aL0:?Lx]sMw#YK[$@Ks+a0%";
            private string _verifyCode;
            private string _errInfo;
            private int _productID;


            /// <summary>
            /// 获取或设置使用的机器码
            /// </summary>
            public string MachineCode;

            /// <summary>
            /// 返回上一次错误发生的原因
            /// </summary>
            public string LastError
            {
                get
                {
                    return _errInfo;
                }
            }
            /// <summary>
            /// 获取CPU序列号
            /// </summary>
            /// <returns>CPU序列号字符串</returns>
            private static string GetCpuInfo()
            {
                try
                {
                    string cpuInfo = "";
                    ManagementClass cimobject = new ManagementClass("Win32_Processor");
                    ManagementObjectCollection moc = cimobject.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        cpuInfo += mo.Properties["ProcessorId"].Value.ToString();
                    }
                    return cpuInfo.ToString();
                }
                catch
                {
                    return "unknown";
                }
            }

            /// <summary>
            /// 获取硬盘ID  
            /// </summary>
            /// <returns>string</returns>
            private static string GetHardDiskID()
            {
                try
                {
                    ManagementClass mcHD = new ManagementClass("win32_logicaldisk");
                    ManagementObjectCollection mocHD = mcHD.GetInstances();
                    foreach (ManagementObject m in mocHD)
                    {
                        if (m["DeviceID"].ToString() == "C:")
                        {
                            return m["VolumeSerialNumber"].ToString();
                        }
                    }
                    return "";
                }
                catch
                {
                    return "unknown";
                }
            }

            /// <summary>
            /// 获取网卡的物理地址
            /// </summary>
            /// <returns></returns>
            private static string GetMacAddress()
            {
                try
                {
                    string mac = "";
                    ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                    ManagementObjectCollection moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        if ((bool)mo["IPEnabled"] == true)
                        {
                            mac += mo["MacAddress"].ToString();
                        }
                    }
                    moc = null;
                    mc = null;
                    return mac;
                }
                catch
                {
                    return "unknown";
                }
            }

            /// <summary>
            /// 获取网卡的IP地址
            /// </summary>
            /// <returns></returns>
            private static string GetIPAddress()
            {
                try
                {
                    //获取IP地址 
                    string st = "";
                    ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                    ManagementObjectCollection moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        if ((bool)mo["IPEnabled"] == true)
                        {
                            System.Array ar;
                            ar = (System.Array)(mo.Properties["IpAddress"].Value);
                            st += ar.GetValue(0).ToString();
                        }
                    }
                    moc = null;
                    mc = null;
                    return st;
                }
                catch
                {
                    return "unknow";
                }
                finally
                {
                }
            }
            /// <summary>
            /// 对字符串进行MD5并生成16进制编码的哈希值
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            private static string MD5(string input)
            {
                MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
                md5Hasher.Initialize();
                byte[] data = Encoding.UTF8.GetBytes(input);
                byte[] hash = md5Hasher.ComputeHash(data, 0, data.Length);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X").PadLeft(2, '0'));
                }
                return sb.ToString();
            }

            /// <summary>
            /// 获取机器码
            /// </summary>
            /// <returns>当前机器的机器码</returns>
            public static string GetMachinCode()
            {
                return MD5(GetCpuInfo() + GetHardDiskID() + GetMacAddress() /*+ GetIPAddress()*/);
            }
            /// <summary>
            /// 创建平台认证类
            /// </summary>
            /// <param name="verifyCode">卡号</param>
            /// <param name="machineCode">指定的机器码</param>
            public Verify(int productID, string machineCode)
            {
                _productID = productID;
                MachineCode = machineCode;
            }
            /// <summary>
            /// 创建平台认证类，使用该类自动获取的机器码
            /// </summary>
            /// <param name="verifyCode"></param>
            public Verify(int productID)
            {
                _productID = productID;
                MachineCode = GetMachinCode();
            }
            /// <summary>  
            /// 通过HTTP的GET方式获取目标地址所返回的字符串  
            /// </summary>  
            /// <param name="url">请求的URL</param>  
            /// <param name="timeout">请求的超时时间</param>  
            /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
            /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
            /// <returns></returns>  
            public static string HttpGet(string url, int? timeout = 10000, string userAgent = null, CookieCollection cookies = null)
            {
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentNullException("url");
                }
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                if (!string.IsNullOrEmpty(userAgent))
                {
                    request.UserAgent = userAgent;
                }
                if (timeout.HasValue)
                {
                    request.Timeout = timeout.Value;
                }
                if (cookies != null)
                {
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(cookies);
                }
                StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream());
                return sr.ReadToEnd();
            }
            /// <summary>
            /// 将c# DateTime时间格式转换为Unix时间戳格式
            /// </summary>
            /// <param name="time">时间</param>
            /// <returns>double</returns>
            public static int ConvertDateTimeInt(System.DateTime time)
            {
                int intResult = 0;
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                intResult = (int)(time - startTime).TotalSeconds;
                return intResult;
            }
            /// <summary>
            /// 将该机器绑定到所使用的验证码上，用于工作室版
            /// 如果绑定成功，则返回true，失败则返回false，并在该对象的ErrInfo中返回绑定失败的原因
            /// </summary>
            /// <param name="verifyCode">注册码</param>
            /// <returns></returns>
            public bool Bind(string verifyCode)
            {
                _verifyCode = verifyCode;
                int timeStamp = ConvertDateTimeInt(DateTime.Now);
                string result = "NETWORK_ERR";
                for (int i = 0; i < 10 && result == "NETWORK_ERR"; i++)
                {
                    try
                    {
                        result = HttpGet(DecodeStr(BINDURL) + "v=2&c=" + _verifyCode + "&mc=" + MachineCode + "&p=" + _productID.ToString() + "&t=" + timeStamp.ToString(), 1000);
                    }
                    catch { }
                }
                switch (result)
                {
                    case "NETWORK_ERR":
                        _errInfo = "无法连接到认证服务器,详情请查看说明书处理具体问题";
                        break;
                    case "TIME_ERROR":
                        _errInfo = "时间错误，请检查你的系统时间";
                        break;
                    case "CODE_NOT_FOUND":
                        _errInfo = "卡号错误";
                        break;
                    case "PRODUCT_UNMATCH":
                        _errInfo = "产品号错误,请确保没有下错程序,例如单开下成多开";
                        break;
                    case "MACHINE_CODE_UNMATCH":
                        _errInfo = "已达到可同时运行程序上限，请返回原机器解锁后再使用";
                        break;
                    case "CARD_FREEZED":
                        _errInfo = "卡已过期或被冻结，请联系客服进行续费";
                        break;
                    default:
                        if (result == MD5(_verifyCode + DecodeStr(KEY) + MachineCode + timeStamp.ToString() + _productID.ToString()))
                            return true;
                        break;
                }
                return false;
            }
            /// <summary>
            /// 将该机器绑定到所使用的验证码上，用于个人版
            /// 如果绑定成功，则返回true，失败则返回false，并在该对象的ErrInfo中返回绑定失败的原因
            /// </summary>
            /// <param name="verifyCode">注册码</param>
            /// <returns></returns>
            public bool BindPerson(string verifyCode)
            {
                _verifyCode = verifyCode;
                int timeStamp = ConvertDateTimeInt(DateTime.Now);
                string result = "NETWORK_ERR";
                for (int i = 0; i < 10 && result == "NETWORK_ERR"; i++)
                {
                    try
                    {
                        result = HttpGet(DecodeStr(BINDURL) + "v=2&c=" + _verifyCode + "&mc=" + MachineCode + "&p=" + _productID.ToString() + "&t=" + timeStamp.ToString(), 1000);
                    }
                    catch { }
                }
                switch (result)
                {
                    case "NETWORK_ERR":
                        _errInfo = "无法连接到认证服务器,详情请查看说明书处理具体问题";
                        break;
                    case "TIME_ERROR":
                        _errInfo = "时间错误，请检查你的系统时间";
                        break;
                    case "CODE_NOT_FOUND":
                        _errInfo = "卡号错误";
                        break;
                    case "PRODUCT_UNMATCH":
                        _errInfo = "产品号错误";
                        break;
                    case "MACHINE_CODE_UNMATCH":
                        _errInfo = "已达到可同时运行程序上限，请联系客服";
                        break;
                    case "CARD_FREEZED":
                        _errInfo = "卡已被冻结，请联系客服";
                        break;
                    case "OUT_OF_EXPIRE_TIME":
                        _errInfo = "卡已过期，请联系客服";
                        break;
                    default:
                        if (result == MD5(_verifyCode + DecodeStr(KEY) + MachineCode + timeStamp.ToString() + _productID.ToString()))
                            return true;
                        break;
                }
                return false;
            }
            /// <summary>
            /// 加密字符串
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public static string EncodeStr(string input)
            {
                string tmp = "";
                for (int i = 0; i < input.Length; i++)
                    tmp += KEY2[KEY3.IndexOf(input[i])];
                string tmp2 = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(tmp));
                tmp = "";
                for (int i = 0; i < tmp2.Length; i++)
                    tmp += KEY2[KEY3.IndexOf(tmp2[i])];
                return tmp;
            }
            /// <summary>
            /// 解密字符串
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public static string DecodeStr(string input)
            {
                string tmp = "";
                for (int i = 0; i < input.Length; i++)
                    tmp += KEY3[KEY2.IndexOf(input[i])];
                string tmp2 = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(tmp));
                tmp = "";
                for (int i = 0; i < tmp2.Length; i++)
                    tmp += KEY3[KEY2.IndexOf(tmp2[i])];
                return tmp;
            }
            /// <summary>
            /// 解除对该机器的绑定,最好每次在退出程序前执行该操作
            /// </summary>
            /// <returns></returns>
            public void Unbind()
            {
                int timeStamp = ConvertDateTimeInt(DateTime.Now);
                try
                {
                    HttpGet(DecodeStr(UNBINDURL) + "c=" + _verifyCode + "&mc=" + MachineCode + "&p=" + _productID.ToString() + "&t=" + timeStamp.ToString());
                }
                catch { }
            }
        }
        class APP
        {
            private string documentName;
            private string documentPath;
            private string nativeClientExeName;
            private string steamExeName;
            public APP()
            {
                documentName = "Start.exe";
                documentPath = "C:\\Program Files\\Sandboxie\\";
                nativeClientExeName = "dota2launcher.exe";
                steamExeName = "Steam.exe";
            }
            public void runappInSandbox(string tempArguments)
            {
                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                info.FileName           = documentName;
                info.Arguments          = tempArguments;
                info.WorkingDirectory   = documentPath;
                System.Diagnostics.Process pro;
                try
                {
                    pro = System.Diagnostics.Process.Start(info);
                }
                catch (System.ComponentModel.Win32Exception ex)
                {
                    return;
                }
            }
            public void runapp(string tempArguments)
            {
                string docPath;
                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                info.Arguments        = tempArguments;
                
                if ("steam" == startupWay)
                {
                    info.FileName = steamExeName; 
                    docPath = startupPath.Replace(steamExeName, "");
                }
                else
                {
                    info.FileName = nativeClientExeName;
                    docPath = startupPath.Replace(nativeClientExeName, "");
                }

                info.WorkingDirectory = docPath;
                System.Diagnostics.Process.Start(info);
            }
        }
        class preventMulOpen
        {
            public void guidWay()
            {
                Guid ownGUID = new Guid(((GuidAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(GuidAttribute))).Value);
                Guid proGUID;
                int ownPID = Process.GetCurrentProcess().Id;
                int proPID;

                foreach (Process p in Process.GetProcesses())
                {
                    try
                    {
                        proGUID = new Guid(((GuidAttribute)Attribute.GetCustomAttribute(Assembly.LoadFile(p.MainModule.FileName), typeof(GuidAttribute))).Value);
                        proPID = p.Id;
                        if (proGUID.Equals(ownGUID) && proPID != ownPID)
                            Process.GetCurrentProcess().Kill();
                    }
                    catch
                    {
                        continue;//遇上进程访问异常就跳过该进程
                    }
                }
            }
            public void mutexWay()
            {
                bool createdNew = false;//互斥体是否创建成功的标志
                Mutex mutex = new Mutex(true, "winformdoc", out createdNew);//MutexName为互斥体名称
                if (!createdNew)
                    Process.GetCurrentProcess().Kill();
            }
            public void titleWay()
            {
                string hwndList;
                CDmSoft tempDm = new CDmSoft();
                hwndList = tempDm.EnumWindow(0, "3r", "", 1 + 4 + 8);
                int i;
                i = 0;
                if (hwndList.Length > 0)
                {
                    foreach (string hwnds in hwndList.Split(','))
                        i++;
                }
                if (i > 2)
                    Process.GetCurrentProcess().Kill();
            }
            public void processWay()
            {
                Process[] ps = Process.GetProcesses();
                int i;
                i = 0;
                foreach (Process item in ps)
                {
                    if (item.ProcessName == "3r")
                        i++;
                }
                if (i > 2)
                    Process.GetCurrentProcess().Kill();
            }
        }
        public void automaticLogin()
        {
            APP application = new APP();
            int i;
            object x, y;
            string hwndList, hwndTemp, winClass, strTemp;
            i = 0;
            if ("perfect" == startupWay)
            {
                if (1 == threadNum)
                {
                    hwndList = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
                    if (hwndList.Length > 0)
                    {
                        foreach (string hwnds in hwndList.Split(','))
                        {
                            winClass = hole_Dm.GetWindowClass(int.Parse(hwnds));
                            try
                            {
                                winClass = winClass.Remove(7);
                                if (winClass == "Sandbox")
                                    hole_Dm.SetWindowState(int.Parse(hwnds), 13);
                            }
                            catch (System.Exception ex)
                            {

                            }

                        }
                    }
                    hwndList = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
                    while (hwndList.Length == 0)
                    {
                        hwndTemp = hole_Dm.EnumWindowByProcess("dota2launcher.exe", "", "", 0);
                        if (hwndTemp != "")
                        {
                            foreach (string strs in hwndTemp.Split(','))
                                hole_Dm.SetWindowState(int.Parse(strs), 13);
                        }

                        strTemp = " -login account password ";
                        try
                        {
                            strTemp = strTemp.Replace("account", account[0].Split('-')[0]);
                            strTemp = strTemp.Replace("password", account[0].Split('-')[1]);
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("账号-密码为空");
                        }
                        application.runapp(strTemp);
                        Thread.Sleep(15000);

                        hwndTemp = hole_Dm.EnumWindow(0, "Dota 2启动器", "", 1 + 2);
                        if (hwndTemp != "")
                        {
                            startupTestDm[i] = new CDmSoft();
                            Thread.Sleep(200);
                            startupTestDm[i].SetPath(".\\game_script");
                            Thread.Sleep(200);
                            startupTestDm[i].BindWindow(int.Parse(hwndTemp), "gdi", "dx", "dx", 2);
                            Thread.Sleep(200);
                            startupTestDm[i].MoveTo(890, 80);
                            Thread.Sleep(200);
                            startupTestDm[i].LeftClick();
                            Thread.Sleep(200);
                            startupTestDm[i].LeftClick();
                            Thread.Sleep(200);
                            startupTestDm[i].UnBindWindow();
                        }
                        hwndTemp = hole_Dm.EnumWindow(0, "Dota 2 - 云同步冲突", "", 1 + 2);
                        if (hwndTemp != "")
                        {
                            startupTestDm[i] = new CDmSoft();
                            Thread.Sleep(200);
                            startupTestDm[i].SetPath(".\\game_script");
                            Thread.Sleep(200);
                            startupTestDm[i].BindWindow(int.Parse(hwndTemp), "gdi", "dx", "dx", 0);
                            Thread.Sleep(200);
                            startupTestDm[i].MoveTo(280,170);
                            Thread.Sleep(200);
                            startupTestDm[i].LeftClick();
                            Thread.Sleep(200);
                            startupTestDm[i].LeftClick();
                            Thread.Sleep(200);
                            startupTestDm[i].UnBindWindow();
                        }
                        hwndTemp = hole_Dm.EnumWindow(0, "Dota 2 - 警告", tempStr, 1 + 2);
                        if (hwndTemp != "")
                        {
                            startupTestDm[i] = new CDmSoft();
                            Thread.Sleep(200);
                            startupTestDm[i].SetPath(".\\game_script");
                            Thread.Sleep(200);
                            startupTestDm[i].BindWindow(int.Parse(hwndTemp), "gdi", "dx", "dx", 0);
                            Thread.Sleep(200);
                            startupTestDm[i].MoveTo(890, 80);
                            Thread.Sleep(200);
                            startupTestDm[i].LeftClick();
                            Thread.Sleep(200);
                            startupTestDm[i].LeftClick();
                            Thread.Sleep(200);
                            startupTestDm[i].UnBindWindow();
                        }
                        Thread.Sleep(10000);
                        hwndList = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
                    }
                }
                else
                {
                    for (i = 0; i < 10; i++)
                        loginState[i] = 0;
                    hwndList = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
                    if (hwndList.Length > 0)
                    {
                        foreach (string hwnds in hwndList.Split(','))
                        {
                            tempStr = hole_Dm.GetWindowClass(int.Parse(hwnds));
                            switch (tempStr)
                            {
                                case "Sandbox:D1:Valve001":
                                    loginState[0] = 1;
                                    break;
                                case "Sandbox:D2:Valve001":
                                    loginState[1] = 1;
                                    break;
                                case "Sandbox:D3:Valve001":
                                    loginState[2] = 1;
                                    break;
                                case "Sandbox:D4:Valve001":
                                    loginState[3] = 1;
                                    break;
                                case "Sandbox:D5:Valve001":
                                    loginState[4] = 1;
                                    break;
                                case "Sandbox:D6:Valve001":
                                    loginState[5] = 1;
                                    break;
                                case "Sandbox:D7:Valve001":
                                    loginState[6] = 1;
                                    break;
                                case "Sandbox:D8:Valve001":
                                    loginState[7] = 1;
                                    break;
                                case "Sandbox:D9:Valve001":
                                    loginState[8] = 1;
                                    break;
                                case "Sandbox:D10:Valve001":
                                    loginState[9] = 1;
                                    break;
                            }
                        }
                    }
                    while (threadNum != loginState.Sum())
                    {
                        for (i = 0; i < threadNum; i++)
                        {
                            if (0 == loginState[i])
                            {
                                try
                                {
                                    tempStr = "Sandbox:Dnum";
                                    tempStr = tempStr.Replace("num", (i + 1).ToString());
                                    hwndTemp = hole_Dm.EnumWindowByProcess("dota2launcher.exe", "", tempStr, 2);
                                    if (hwndTemp != "")
                                    {
                                        foreach (string strs in hwndTemp.Split(','))
                                            hole_Dm.SetWindowState(int.Parse(strs), 13);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    
                                }
                                tempStr = "/box:Dnum path -login account password";
                                tempStr = tempStr.Replace("path", startupPath);
                                tempStr = tempStr.Replace("num", (i + 1).ToString());
                                try
                                {
                                    tempStr = tempStr.Replace("account", account[i].Split('-')[0]);
                                    tempStr = tempStr.Replace("password", account[i].Split('-')[1]);
                                    application.runappInSandbox(tempStr);
                                }
                                catch (System.Exception ex)
                                {
                                    MessageBox.Show("第" + (i + 1).ToString() + "个账号 - 密码为空");
                                }

                                Thread.Sleep(15000);

                                tempStr = "Sandbox:Dnum";
                                tempStr = tempStr.Replace("num", (i + 1).ToString());
                                hwndTemp = hole_Dm.EnumWindow(0, "Dota 2启动器", tempStr, 1 + 2 + 16);
                                //hwndTemp = EnumWindowByProcess("dota2launcher.exe", "Dota 2启动器", tempStr, 1+2);
                                if (hwndTemp != "")
                                {
                                    startupTestDm[i] = new CDmSoft();
                                    Thread.Sleep(200);
                                    startupTestDm[i].SetPath(".\\game_script");
                                    Thread.Sleep(200);
                                    startupTestDm[i].BindWindow(int.Parse(hwndTemp), "gdi", "dx", "dx", 0);
                                    Thread.Sleep(200);
                                    startupTestDm[i].MoveTo(890, 80);
                                    Thread.Sleep(200);
                                    startupTestDm[i].LeftClick();
                                    Thread.Sleep(200);
                                    startupTestDm[i].LeftClick();
                                    Thread.Sleep(200);
                                    startupTestDm[i].UnBindWindow();
                                }
                                Thread.Sleep(5000);
                                //hwndTemp = EnumWindowByProcess("dota2launcher.exe", "Dota 2启动器", tempStr, 1 + 2);
                                hwndTemp = hole_Dm.EnumWindow(0, "Dota 2 - 云同步冲突", tempStr, 1 + 2 + 16);
                                if (hwndTemp != "")
                                {
                                    startupCloudTestDm[i] = new CDmSoft();
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].SetPath(".\\game_script");
                                    startupCloudTestDm[i].BindWindow(int.Parse(hwndTemp), "gdi", "dx", "dx", 0);
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].MoveTo(280, 170);
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].LeftClick();
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].LeftClick();
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].UnBindWindow();
                                }
                                hwndTemp = hole_Dm.EnumWindow(0, "Dota 2 - 警告", tempStr, 1 + 2 + 16);
                                if (hwndTemp != "")
                                {
                                    startupCloudTestDm[i] = new CDmSoft();
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].SetPath(".\\game_script");
                                    startupCloudTestDm[i].BindWindow(int.Parse(hwndTemp), "gdi", "dx", "dx", 0);
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].MoveTo(270, 165);
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].LeftClick();
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].LeftClick();
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].UnBindWindow();
                                }
                                Thread.Sleep(8000);
                                for (i = 0; i < 10; i++)
                                   loginState[i] = 0;
                                hwndList = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
                                if (hwndList.Length > 0)
                                {
                                    foreach (string hwnds in hwndList.Split(','))
                                    {
                                        tempStr = hole_Dm.GetWindowClass(int.Parse(hwnds));
                                        switch (tempStr)
                                        {
                                            case "Sandbox:D1:Valve001":
                                                loginState[0] = 1;
                                                break;
                                            case "Sandbox:D2:Valve001":
                                                loginState[1] = 1;
                                                break;
                                            case "Sandbox:D3:Valve001":
                                                loginState[2] = 1;
                                                break;
                                            case "Sandbox:D4:Valve001":
                                                loginState[3] = 1;
                                                break;
                                            case "Sandbox:D5:Valve001":
                                                loginState[4] = 1;
                                                break;
                                            case "Sandbox:D6:Valve001":
                                                loginState[5] = 1;
                                                break;
                                            case "Sandbox:D7:Valve001":
                                                loginState[6] = 1;
                                                break;
                                            case "Sandbox:D8:Valve001":
                                                loginState[7] = 1;
                                                break;
                                            case "Sandbox:D9:Valve001":
                                                loginState[8] = 1;
                                                break;
                                            case "Sandbox:D10:Valve001":
                                                loginState[9] = 1;
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //STEAM
                if (1 == threadNum)
                {
                    hwndList = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
                    if (hwndList.Length > 0)
                    {
                        foreach (string hwnds in hwndList.Split(','))
                        {
                            winClass = hole_Dm.GetWindowClass(int.Parse(hwnds));
                            try
                            {
                                winClass = winClass.Remove(7);
                                if (winClass == "Sandbox")
                                    hole_Dm.SetWindowState(int.Parse(hwnds), 13);
                            }
                            catch (System.Exception ex)
                            {

                            }

                        }
                    }
                    hwndList = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
                    while (hwndList.Length == 0)
                    {
                        hwndTemp = hole_Dm.EnumWindowByProcess("steam.exe", "", "", 0);
                        if (hwndTemp != "")
                        {
                            foreach (string strs in hwndTemp.Split(','))
                                hole_Dm.SetWindowState(int.Parse(strs), 13);
                        }
                        strTemp = " -login account password ";
                        try
                        {
                            strTemp = strTemp.Replace("account", account[0].Split('-')[0]);
                            strTemp = strTemp.Replace("password", account[0].Split('-')[1]);
                            application.runapp(strTemp);
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("第" + (i + 1).ToString() + "个账号 - 密码为空");
                        }
                        Thread.Sleep(10000);
                        Process.Start("steam://rungameid/570");
                        Thread.Sleep(20000);
                        hwndTemp = hole_Dm.EnumWindow(0, "Steam - 云同步冲突", "", 1 + 2);
                        if (hwndTemp != "")
                        {
                            startupTestDm[i] = new CDmSoft();
                            Thread.Sleep(200);
                            startupTestDm[i].SetPath(".\\game_script");
                            Thread.Sleep(200);
                            startupTestDm[i].BindWindow(int.Parse(hwndTemp), "gdi", "dx", "dx", 0);
                            Thread.Sleep(200);
                            startupTestDm[i].MoveTo(280, 170);
                            Thread.Sleep(200);
                            startupTestDm[i].LeftClick();
                            Thread.Sleep(200);
                            startupTestDm[i].LeftClick();
                            Thread.Sleep(200);
                            startupTestDm[i].UnBindWindow();
                        }
                        hwndTemp = hole_Dm.EnumWindow(0, "Steam - 警告", tempStr, 1 + 2);
                        if (hwndTemp != "")
                        {
                            startupTestDm[i] = new CDmSoft();
                            Thread.Sleep(200);
                            startupTestDm[i].SetPath(".\\game_script");
                            Thread.Sleep(200);
                            startupTestDm[i].BindWindow(int.Parse(hwndTemp), "gdi", "dx", "dx", 0);
                            Thread.Sleep(200);
                            startupTestDm[i].MoveTo(100,225);
                            Thread.Sleep(200);
                            startupTestDm[i].LeftClick();
                            Thread.Sleep(200);
                            startupTestDm[i].LeftClick();
                            Thread.Sleep(200);
                            startupTestDm[i].UnBindWindow();
                        }
                        Thread.Sleep(10000);
                        hwndList = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
                    }
                }
                else
                {
                    for (i = 0; i < 10; i++)
                        loginState[i] = 0;
                    hwndList = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
                    if (hwndList.Length > 0)
                    {
                        foreach (string hwnds in hwndList.Split(','))
                        {
                            tempStr = hole_Dm.GetWindowClass(int.Parse(hwnds));
                            switch (tempStr)
                            {
                                case "Sandbox:D1:Valve001":
                                    loginState[0] = 1;
                                    break;
                                case "Sandbox:D2:Valve001":
                                    loginState[1] = 1;
                                    break;
                                case "Sandbox:D3:Valve001":
                                    loginState[2] = 1;
                                    break;
                                case "Sandbox:D4:Valve001":
                                    loginState[3] = 1;
                                    break;
                                case "Sandbox:D5:Valve001":
                                    loginState[4] = 1;
                                    break;
                                case "Sandbox:D6:Valve001":
                                    loginState[5] = 1;
                                    break;
                                case "Sandbox:D7:Valve001":
                                    loginState[6] = 1;
                                    break;
                                case "Sandbox:D8:Valve001":
                                    loginState[7] = 1;
                                    break;
                                case "Sandbox:D9:Valve001":
                                    loginState[8] = 1;
                                    break;
                                case "Sandbox:D10:Valve001":
                                    loginState[9] = 1;
                                    break;
                            }
                        }
                    }
                    while (threadNum != loginState.Sum())
                    {
                        for (i = 0; i < threadNum; i++)
                        {
                            if (0 == loginState[i])
                            {
                                try
                                {
                                    tempStr = "Sandbox:Dnum";
                                    tempStr = tempStr.Replace("num", (i + 1).ToString());
                                    hwndTemp = hole_Dm.EnumWindowByProcess("steam.exe", "", tempStr, 2);
                                    if (hwndTemp != "")
                                    {
                                        foreach (string strs in hwndTemp.Split(','))
                                            hole_Dm.SetWindowState(int.Parse(strs), 13);
                                    }
                                }
                                catch (System.Exception ex)
                                {

                                }
                                tempStr = "/box:Dnum path -login account password";
                                tempStr = tempStr.Replace("path", startupPath);
                                tempStr = tempStr.Replace("num", (i + 1).ToString());
                                tempStr = tempStr.Replace("account", account[i].Split('-')[0]);
                                tempStr = tempStr.Replace("password", account[i].Split('-')[1]);
                                application.runappInSandbox(tempStr);
                                Thread.Sleep(10000);
                                tempStr = @"/box:Dnum steam://rungameid/570";
                                tempStr = tempStr.Replace("num", (i + 1).ToString());
                                application.runappInSandbox(tempStr);
                                Thread.Sleep(5000);
                                tempStr = "Sandbox:Dnum";
                                tempStr = tempStr.Replace("num", (i + 1).ToString());
                                hwndTemp = hole_Dm.EnumWindow(0, "Steam - 云同步冲突", tempStr, 1 + 2 + 16);
                                if (hwndTemp != "")
                                {
                                    startupCloudTestDm[i] = new CDmSoft();
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].SetPath(".\\game_script");
                                    startupCloudTestDm[i].BindWindow(int.Parse(hwndTemp), "gdi", "dx", "dx", 0);
                                    Thread.Sleep(200);
                                     startupCloudTestDm[i].FindPic(215,145,340,200, "steamdowntodevicde.bmp", "000000", 0.9, 0, out x, out y);
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].MoveTo((int)x,(int)y);
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].LeftClick();
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].LeftClick();
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].UnBindWindow();
                                }
                                hwndTemp = hole_Dm.EnumWindow(0, "Steam - 警告", tempStr, 1 + 2 + 16);
                                if (hwndTemp != "")
                                {
                                    startupCloudTestDm[i] = new CDmSoft();
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].SetPath(".\\game_script");
                                    startupCloudTestDm[i].BindWindow(int.Parse(hwndTemp), "gdi", "dx", "dx", 0);
                                    Thread.Sleep(200);
                                    startupTestDm[i].MoveTo(100, 225);
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].LeftClick();
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].LeftClick();
                                    Thread.Sleep(200);
                                    startupCloudTestDm[i].UnBindWindow();
                                }
                                Thread.Sleep(10000);
                                for (i = 0; i < 10; i++)
                                    loginState[i] = 0;
                                hwndList = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
                                if (hwndList.Length > 0)
                                {
                                    foreach (string hwnds in hwndList.Split(','))
                                    {
                                        tempStr = hole_Dm.GetWindowClass(int.Parse(hwnds));
                                        switch (tempStr)
                                        {
                                            case "Sandbox:D1:Valve001":
                                                loginState[0] = 1;
                                                break;
                                            case "Sandbox:D2:Valve001":
                                                loginState[1] = 1;
                                                break;
                                            case "Sandbox:D3:Valve001":
                                                loginState[2] = 1;
                                                break;
                                            case "Sandbox:D4:Valve001":
                                                loginState[3] = 1;
                                                break;
                                            case "Sandbox:D5:Valve001":
                                                loginState[4] = 1;
                                                break;
                                            case "Sandbox:D6:Valve001":
                                                loginState[5] = 1;
                                                break;
                                            case "Sandbox:D7:Valve001":
                                                loginState[6] = 1;
                                                break;
                                            case "Sandbox:D8:Valve001":
                                                loginState[7] = 1;
                                                break;
                                            case "Sandbox:D9:Valve001":
                                                loginState[8] = 1;
                                                break;
                                            case "Sandbox:D10:Valve001":
                                                loginState[9] = 1;
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
               }
                hwndTemp = hole_Dm.EnumWindowByProcess("steam.exe", "", "", 0);
                if (hwndTemp != "")
                {
                    foreach (string strs in hwndTemp.Split(','))
                        hole_Dm.SetWindowState(int.Parse(strs),6);
                }
            }
        }
        #endregion
        #region "INI"

        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name="section">节点名称[如[TypeName]]</param>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">键</param>
        /// <param name="def">值</param>
        /// <param name="retval">stringbulider对象</param>
        /// <param name="size">字节大小</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        #endregion
        #region 按键消息

        public Form1()
        {
            InitializeComponent();
            Microsoft.Win32.SystemEvents.SessionEnding += LanjieEvent;
        }
        void LanjieEvent(object sender, SessionEndingEventArgs e)
        {
            SessionEndReasons ganshenme = e.Reason;

            switch (ganshenme)
            {
                case SessionEndReasons.Logoff:
                    a.Unbind();
                    break;

                case SessionEndReasons.SystemShutdown:
                    a.Unbind();
                    break;

                default:
                    break;
            }
        }
        private void textBox_MegaphoneDelay_TextChanged(object sender, EventArgs e)
        {
            megaphoneDelay = int.Parse(textBox_MegaphoneDelay.Text);
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.ShowInTaskbar == false)
                notifyIcon1.Visible = true;

            this.ShowInTaskbar = true;
            this.Show();
            this.Activate();
            this.WindowState = FormWindowState.Normal;
        }
        
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
                this.notifyIcon1.ShowBalloonTip(1000, "3R", "程序已经最小化,双击打开图标", ToolTipIcon.Info);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            RegisterHotKey(Handle, 100, 0, Keys.F10);
            RegisterHotKey(Handle, 101, 0, Keys.F11);
            preventMulOpen prevantWays = new preventMulOpen();
            prevantWays.guidWay();
            prevantWays.mutexWay();
            prevantWays.processWay();
            prevantWays.titleWay();

            StringBuilder temp = new StringBuilder(255);
            //第一页
            GetPrivateProfileString(strSec,"system","无法读取对应数值！", temp, 255, strFilePath);
            comboBox_System.Text = temp.ToString();
            GetPrivateProfileString(strSec,"matchMode","无法读取对应数值！", temp, 255, strFilePath);
            comboBox_matchMode.Text = temp.ToString();
            GetPrivateProfileString(strSec, "randomhero", "无法读取对应数值！", temp, 255, strFilePath);
            radioButton_RandomHero.Checked = Convert.ToBoolean(temp.ToString());
            GetPrivateProfileString(strSec, "customhero", "无法读取对应数值！", temp, 255, strFilePath);
            radioButton_CustomHero.Checked = Convert.ToBoolean(temp.ToString());
            GetPrivateProfileString(strSec, "route", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_Route.Text = temp.ToString();
            GetPrivateProfileString(strSec, "killway", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_KillWay.Text = temp.ToString();
            GetPrivateProfileString(strSec, "gattime", "无法读取对应数值！", temp, 255, strFilePath);
            textBox_GatTime.Text = temp.ToString();
            GetPrivateProfileString(strSec, "waitMemberRevive", "无法读取对应数值！", temp, 255, strFilePath);
            checkBox_waitMember.Checked = Convert.ToBoolean(temp.ToString());
            GetPrivateProfileString(strSec, "banSkillBeforeGather", "无法读取对应数值！", temp, 255, strFilePath);
            checkBox_banSkillBeforeGather.Checked = Convert.ToBoolean(temp.ToString());

            GetPrivateProfileString(strSec, "firsthero", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_FirstHero.SelectedIndex = Convert.ToInt32(temp.ToString());
            GetPrivateProfileString(strSec, "secondhero", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_SecondHero.SelectedIndex = Convert.ToInt32(temp.ToString());
            GetPrivateProfileString(strSec, "thirdhero", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_ThirdHero.SelectedIndex = Convert.ToInt32(temp.ToString());
            GetPrivateProfileString(strSec, "fourthhero", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_FourthHero.SelectedIndex = Convert.ToInt32(temp.ToString());
            GetPrivateProfileString(strSec, "fifthhero", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_FifthHero.SelectedIndex = Convert.ToInt32(temp.ToString());
            GetPrivateProfileString(strSec, "sixthhero", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_SixthHero.SelectedIndex = Convert.ToInt32(temp.ToString());
            GetPrivateProfileString(strSec, "seventhhero", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_SeventhHero.SelectedIndex = Convert.ToInt32(temp.ToString());

            GetPrivateProfileString(strSec, "firsteqm", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_FirstEqm.SelectedIndex = Convert.ToInt32(temp.ToString());
            GetPrivateProfileString(strSec, "secondeqm", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_SecondEqm.SelectedIndex = Convert.ToInt32(temp.ToString());
            GetPrivateProfileString(strSec, "thirdeqm", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_ThirdEqm.SelectedIndex = Convert.ToInt32(temp.ToString());
            GetPrivateProfileString(strSec, "fourtheqm", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_FourthEqm.SelectedIndex = Convert.ToInt32(temp.ToString());
            GetPrivateProfileString(strSec, "fiftheqm", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_FifthEqm.SelectedIndex = Convert.ToInt32(temp.ToString());
            GetPrivateProfileString(strSec, "sixtheqm", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_SixthEqm.SelectedIndex = Convert.ToInt32(temp.ToString());
            GetPrivateProfileString(strSec, "seventheqm", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_SeventhEqm.SelectedIndex = Convert.ToInt32(temp.ToString());
            GetPrivateProfileString(strSec, "eightheqm", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_EighthEqm.SelectedIndex = Convert.ToInt32(temp.ToString());
            GetPrivateProfileString(strSec, "ninetheqm", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_NinthEqm.SelectedIndex = Convert.ToInt32(temp.ToString());
            //第二页
            GetPrivateProfileString(strSec, "inviteteammate", "无法读取对应数值！", temp, 255, strFilePath);
            checkBox_InviteTeammate.Checked = Convert.ToBoolean(temp.ToString());
            GetPrivateProfileString(strSec, "isLead", "无法读取对应数值！", temp, 255, strFilePath);
            radioButton_isLead.Checked = Convert.ToBoolean(temp.ToString());
            GetPrivateProfileString(strSec, "isMember", "无法读取对应数值！", temp, 255, strFilePath);
            radioButton_isMember.Checked = Convert.ToBoolean(temp.ToString());
            GetPrivateProfileString(strSec, "idOne", "无法读取对应数值！", temp, 255, strFilePath);
            textBox_IdOne.Text = temp.ToString();
            GetPrivateProfileString(strSec, "idTwo", "无法读取对应数值！", temp, 255, strFilePath);
            textBox_IdTwo.Text = temp.ToString();
            GetPrivateProfileString(strSec, "idThree", "无法读取对应数值！", temp, 255, strFilePath);
            textBox_IdThree.Text = temp.ToString();
            GetPrivateProfileString(strSec, "idFour", "无法读取对应数值！", temp, 255, strFilePath);
            textBox_IdFour.Text = temp.ToString();
            GetPrivateProfileString(strSec, "idFive", "无法读取对应数值！", temp, 255, strFilePath);
            textBox_IdFive.Text = temp.ToString();
            GetPrivateProfileString(strSec, "idSix", "无法读取对应数值！", temp, 255, strFilePath);
            textBox_IdSix.Text = temp.ToString();
            GetPrivateProfileString(strSec, "idSeven", "无法读取对应数值！", temp, 255, strFilePath);
            textBox_IdSeven.Text = temp.ToString();
            GetPrivateProfileString(strSec, "idEight", "无法读取对应数值！", temp, 255, strFilePath);
            textBox_IdEight.Text = temp.ToString();
            GetPrivateProfileString(strSec, "idNine", "无法读取对应数值！", temp, 255, strFilePath);
            textBox_IdNine.Text = temp.ToString();
            //第三页
            GetPrivateProfileString(strSec, "disRoute", "无法读取对应数值！", temp, 255, strFilePath);
            checkBox_DisRoute.Checked = Convert.ToBoolean(temp.ToString());
            GetPrivateProfileString(strSec, "oneRoute", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_RouteOne.Text = temp.ToString();
            GetPrivateProfileString(strSec, "twoRoute", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_RouteTwo.Text = temp.ToString();
            GetPrivateProfileString(strSec, "threeRoute", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_RouteThree.Text = temp.ToString();
            GetPrivateProfileString(strSec, "fourRoute", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_RouteFour.Text = temp.ToString();
            GetPrivateProfileString(strSec, "fiveRoute", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_RouteFive.Text = temp.ToString();
            GetPrivateProfileString(strSec, "sixRoute", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_RouteSix.Text = temp.ToString();
            GetPrivateProfileString(strSec, "sevenRoute", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_RouteSeven.Text = temp.ToString();
            GetPrivateProfileString(strSec, "eightRoute", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_RouteEight.Text = temp.ToString();
            GetPrivateProfileString(strSec, "nineRoute", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_RouteNine.Text = temp.ToString();
            GetPrivateProfileString(strSec, "tenRoute", "无法读取对应数值！", temp, 255, strFilePath);
            comboBox_RouteTen.Text = temp.ToString();
            //第五页
            GetPrivateProfileString(strSec, "autoLogin", "无法读取对应数值！", temp, 255, strFilePath);
            checkBox_autoLogin.Checked = Convert.ToBoolean(temp.ToString());
            GetPrivateProfileString(strSec, "steam", "无法读取对应数值！", temp, 255, strFilePath);
            radioButton_steam.Checked = Convert.ToBoolean(temp.ToString());
            GetPrivateProfileString(strSec, "perfect", "无法读取对应数值！", temp, 255, strFilePath);
            radioButton_perfect.Checked = Convert.ToBoolean(temp.ToString());
            GetPrivateProfileString(strSec, "perfectStartupPath", "无法读取对应数值！", temp, 255, strFilePath);
            textBox_perfectStartupPath.Text = temp.ToString();
            GetPrivateProfileString(strSec, "steamStartupPath", "无法读取对应数值！", temp, 255, strFilePath);
            textBox_steamStartupPath.Text = temp.ToString();
            GetPrivateProfileString(strSec, "account", "无法读取对应数值！", temp,500, strFilePath);
            tempStr = temp.ToString();
            tempStr = tempStr.Replace("****", "\r\n");
            textBox_account.Text = tempStr;
            //第六页
            GetPrivateProfileString(strSec, "stoptime", "无法读取对应数值！", temp, 255, strFilePath);
            textBox_stopTime.Text = temp.ToString();
            GetPrivateProfileString(strSec, "shutdown", "无法读取对应数值！", temp, 255, strFilePath);
            checkBox_shutdown.Checked = Convert.ToBoolean(temp.ToString());

            GetPrivateProfileString(Form1.strSec, "loginpassword", "", temp, 255, Form1.strFilePath);
            key = temp.ToString();

            GetPrivateProfileString(Form1.strSec, "autoMegaphone", "", temp, 255, Form1.strFilePath);
            textBox_megaphone.Text = temp.ToString();
            GetPrivateProfileString(Form1.strSec, "megaphoneDelay", "", temp, 255, Form1.strFilePath);
            textBox_MegaphoneDelay.Text = temp.ToString();


            GetPrivateProfileString(Form1.strSec, "skillSequence", "", temp, 255, Form1.strFilePath);
            textBox_skillSequence.Text = temp.ToString();
            GetPrivateProfileString(Form1.strSec, "escapeMode", "", temp, 255, Form1.strFilePath);
            checkBox_escapeMode.Checked = Convert.ToBoolean(temp.ToString());
            
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("你确定要关闭吗！", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                try
                {
                    WritePrivateProfileString(strSec, "system", comboBox_System.Text,strFilePath);
                    WritePrivateProfileString(strSec, "matchMode", comboBox_matchMode.Text, strFilePath);
                    WritePrivateProfileString(strSec, "randomhero", Convert.ToString(radioButton_RandomHero.Checked), strFilePath);
                    WritePrivateProfileString(strSec, "customhero", Convert.ToString(radioButton_CustomHero.Checked), strFilePath);
                    WritePrivateProfileString(strSec, "route", comboBox_Route.Text, strFilePath);
                    WritePrivateProfileString(strSec, "killway", comboBox_KillWay.Text, strFilePath);
                    WritePrivateProfileString(strSec, "gattime", textBox_GatTime.Text, strFilePath);
                    WritePrivateProfileString(strSec, "waitMemberRevive", Convert.ToString(checkBox_waitMember.Checked), strFilePath);
                    WritePrivateProfileString(strSec, "banSkillBeforeGather", Convert.ToString(checkBox_banSkillBeforeGather.Checked), strFilePath);

                    WritePrivateProfileString(strSec, "firsthero", Convert.ToString(comboBox_FirstHero.SelectedIndex), strFilePath);
                    WritePrivateProfileString(strSec, "secondhero", Convert.ToString(comboBox_SecondHero.SelectedIndex), strFilePath);
                    WritePrivateProfileString(strSec, "thirdhero", Convert.ToString(comboBox_ThirdHero.SelectedIndex), strFilePath);
                    WritePrivateProfileString(strSec, "fourthhero", Convert.ToString(comboBox_FourthHero.SelectedIndex), strFilePath);
                    WritePrivateProfileString(strSec, "fifthhero", Convert.ToString(comboBox_FifthHero.SelectedIndex), strFilePath);
                    WritePrivateProfileString(strSec, "sixthhero", Convert.ToString(comboBox_SixthHero.SelectedIndex), strFilePath);
                    WritePrivateProfileString(strSec, "seventhhero", Convert.ToString(comboBox_SeventhHero.SelectedIndex), strFilePath);

                    WritePrivateProfileString(strSec, "firsteqm", Convert.ToString(comboBox_FirstEqm.SelectedIndex), strFilePath);
                    WritePrivateProfileString(strSec, "secondeqm", Convert.ToString(comboBox_SecondEqm.SelectedIndex), strFilePath);
                    WritePrivateProfileString(strSec, "thirdeqm", Convert.ToString(comboBox_ThirdEqm.SelectedIndex), strFilePath);
                    WritePrivateProfileString(strSec, "fourtheqm", Convert.ToString(comboBox_FourthEqm.SelectedIndex), strFilePath);
                    WritePrivateProfileString(strSec, "fiftheqm", Convert.ToString(comboBox_FifthEqm.SelectedIndex), strFilePath);
                    WritePrivateProfileString(strSec, "sixtheqm", Convert.ToString(comboBox_SixthEqm.SelectedIndex), strFilePath);
                    WritePrivateProfileString(strSec, "seventheqm", Convert.ToString(comboBox_SeventhEqm.SelectedIndex), strFilePath);
                    WritePrivateProfileString(strSec, "eightheqm", Convert.ToString(comboBox_EighthEqm.SelectedIndex), strFilePath);
                    WritePrivateProfileString(strSec, "ninetheqm", Convert.ToString(comboBox_NinthEqm.SelectedIndex), strFilePath);

                    WritePrivateProfileString(strSec, "inviteteammate", Convert.ToString(checkBox_InviteTeammate.Checked), strFilePath);
                    WritePrivateProfileString(strSec, "isLead", Convert.ToString(radioButton_isLead.Checked), strFilePath);
                    WritePrivateProfileString(strSec, "isMember", Convert.ToString(radioButton_isMember.Checked), strFilePath);
                    WritePrivateProfileString(strSec, "idOne", Convert.ToString(textBox_IdOne.Text), strFilePath);
                    WritePrivateProfileString(strSec, "idTwo", Convert.ToString(textBox_IdTwo.Text), strFilePath);
                    WritePrivateProfileString(strSec, "idThree", Convert.ToString(textBox_IdThree.Text), strFilePath);
                    WritePrivateProfileString(strSec, "idFour", Convert.ToString(textBox_IdFour.Text), strFilePath);
                    WritePrivateProfileString(strSec, "idFive", Convert.ToString(textBox_IdFive.Text), strFilePath);
                    WritePrivateProfileString(strSec, "idSix", Convert.ToString(textBox_IdSix.Text), strFilePath);
                    WritePrivateProfileString(strSec, "idSeven", Convert.ToString(textBox_IdSeven.Text), strFilePath);
                    WritePrivateProfileString(strSec, "idEight", Convert.ToString(textBox_IdEight.Text), strFilePath);
                    WritePrivateProfileString(strSec, "idNine", Convert.ToString(textBox_IdNine.Text), strFilePath);

                    WritePrivateProfileString(strSec, "disRoute", Convert.ToString(checkBox_DisRoute.Checked), strFilePath);
                    WritePrivateProfileString(strSec, "oneRoute", Convert.ToString(comboBox_RouteOne.Text), strFilePath);
                    WritePrivateProfileString(strSec, "twoRoute", Convert.ToString(comboBox_RouteTwo.Text), strFilePath);
                    WritePrivateProfileString(strSec, "threeRoute", Convert.ToString(comboBox_RouteThree.Text), strFilePath);
                    WritePrivateProfileString(strSec, "fourRoute", Convert.ToString(comboBox_RouteFour.Text), strFilePath);
                    WritePrivateProfileString(strSec, "fiveRoute", Convert.ToString(comboBox_RouteFive.Text), strFilePath);
                    WritePrivateProfileString(strSec, "sixRoute", Convert.ToString(comboBox_RouteSix.Text), strFilePath);
                    WritePrivateProfileString(strSec, "sevenRoute", Convert.ToString(comboBox_RouteSeven.Text), strFilePath);
                    WritePrivateProfileString(strSec, "eightRoute", Convert.ToString(comboBox_RouteEight.Text), strFilePath);
                    WritePrivateProfileString(strSec, "nineRoute", Convert.ToString(comboBox_RouteNine.Text), strFilePath);
                    WritePrivateProfileString(strSec, "tenRoute", Convert.ToString(comboBox_RouteTen.Text), strFilePath); 

                    WritePrivateProfileString(strSec, "autoLogin", Convert.ToString(checkBox_autoLogin.Checked), strFilePath);
                    WritePrivateProfileString(strSec, "steam", Convert.ToString(radioButton_steam.Checked), strFilePath);
                    WritePrivateProfileString(strSec, "perfect", Convert.ToString(radioButton_perfect.Checked), strFilePath);
                    WritePrivateProfileString(strSec, "perfectStartupPath", Convert.ToString(textBox_perfectStartupPath.Text), strFilePath);
                    WritePrivateProfileString(strSec, "steamStartupPath", Convert.ToString(textBox_steamStartupPath.Text), strFilePath);
                    tempStr = textBox_account.Text;
                    tempStr = tempStr.Replace("\r\n", "****");
                    WritePrivateProfileString(strSec, "account",tempStr, strFilePath);

                    WritePrivateProfileString(strSec, "stoptime", Convert.ToString(textBox_stopTime.Text), strFilePath);
                    WritePrivateProfileString(strSec, "shutdown", Convert.ToString(checkBox_shutdown.Checked), strFilePath);

                    WritePrivateProfileString(strSec, "loginpassword", Convert.ToString(key), strFilePath);

                    WritePrivateProfileString(strSec, "autoMegaphone", Convert.ToString(textBox_megaphone.Text), strFilePath);
                    WritePrivateProfileString(strSec, "megaphoneDelay", Convert.ToString(textBox_MegaphoneDelay.Text), strFilePath);


                    WritePrivateProfileString(strSec, "skillSequence", Convert.ToString(textBox_skillSequence.Text), strFilePath);

                    WritePrivateProfileString(strSec, "escapeMode", Convert.ToString(checkBox_escapeMode.Checked), strFilePath);

                    UnregisterHotKey(Handle, 100);//卸载快捷键  
                    UnregisterHotKey(Handle, 101);
                }
                catch (System.Exception ex)
                {
                	
                }
                e.Cancel = false;  //点击OK  
                a.Unbind();
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                e.Cancel = true;
            }
        }
        private void button_saveSetting_Click(object sender, EventArgs e)
        {
            try
            {
                WritePrivateProfileString(strSec, "system", comboBox_System.Text, strFilePath);
                WritePrivateProfileString(strSec, "matchMode", comboBox_matchMode.Text, strFilePath);
                WritePrivateProfileString(strSec, "randomhero", Convert.ToString(radioButton_RandomHero.Checked), strFilePath);
                WritePrivateProfileString(strSec, "customhero", Convert.ToString(radioButton_CustomHero.Checked), strFilePath);
                WritePrivateProfileString(strSec, "route", comboBox_Route.Text, strFilePath);
                WritePrivateProfileString(strSec, "killway", comboBox_KillWay.Text, strFilePath);
                WritePrivateProfileString(strSec, "gattime", textBox_GatTime.Text, strFilePath);
                WritePrivateProfileString(strSec, "waitMemberRevive", Convert.ToString(checkBox_waitMember.Checked), strFilePath);
                WritePrivateProfileString(strSec, "banSkillBeforeGather", Convert.ToString(checkBox_banSkillBeforeGather.Checked), strFilePath);
                
                WritePrivateProfileString(strSec, "firsthero", Convert.ToString(comboBox_FirstHero.SelectedIndex), strFilePath);
                WritePrivateProfileString(strSec, "secondhero", Convert.ToString(comboBox_SecondHero.SelectedIndex), strFilePath);
                WritePrivateProfileString(strSec, "thirdhero", Convert.ToString(comboBox_ThirdHero.SelectedIndex), strFilePath);
                WritePrivateProfileString(strSec, "fourthhero", Convert.ToString(comboBox_FourthHero.SelectedIndex), strFilePath);
                WritePrivateProfileString(strSec, "fifthhero", Convert.ToString(comboBox_FifthHero.SelectedIndex), strFilePath);
                WritePrivateProfileString(strSec, "sixthhero", Convert.ToString(comboBox_SixthHero.SelectedIndex), strFilePath);
                WritePrivateProfileString(strSec, "seventhhero", Convert.ToString(comboBox_SeventhHero.SelectedIndex), strFilePath);

                WritePrivateProfileString(strSec, "firsteqm", Convert.ToString(comboBox_FirstEqm.SelectedIndex), strFilePath);
                WritePrivateProfileString(strSec, "secondeqm", Convert.ToString(comboBox_SecondEqm.SelectedIndex), strFilePath);
                WritePrivateProfileString(strSec, "thirdeqm", Convert.ToString(comboBox_ThirdEqm.SelectedIndex), strFilePath);
                WritePrivateProfileString(strSec, "fourtheqm", Convert.ToString(comboBox_FourthEqm.SelectedIndex), strFilePath);
                WritePrivateProfileString(strSec, "fiftheqm", Convert.ToString(comboBox_FifthEqm.SelectedIndex), strFilePath);
                WritePrivateProfileString(strSec, "sixtheqm", Convert.ToString(comboBox_SixthEqm.SelectedIndex), strFilePath);
                WritePrivateProfileString(strSec, "seventheqm", Convert.ToString(comboBox_SeventhEqm.SelectedIndex), strFilePath);
                WritePrivateProfileString(strSec, "eightheqm", Convert.ToString(comboBox_EighthEqm.SelectedIndex), strFilePath);
                WritePrivateProfileString(strSec, "ninetheqm", Convert.ToString(comboBox_NinthEqm.SelectedIndex), strFilePath);

                WritePrivateProfileString(strSec, "inviteteammate", Convert.ToString(checkBox_InviteTeammate.Checked), strFilePath);
                WritePrivateProfileString(strSec, "isLead", Convert.ToString(radioButton_isLead.Checked), strFilePath);
                WritePrivateProfileString(strSec, "isMember", Convert.ToString(radioButton_isMember.Checked), strFilePath);
                WritePrivateProfileString(strSec, "idOne", Convert.ToString(textBox_IdOne.Text), strFilePath);
                WritePrivateProfileString(strSec, "idTwo", Convert.ToString(textBox_IdTwo.Text), strFilePath);
                WritePrivateProfileString(strSec, "idThree", Convert.ToString(textBox_IdThree.Text), strFilePath);
                WritePrivateProfileString(strSec, "idFour", Convert.ToString(textBox_IdFour.Text), strFilePath);
                WritePrivateProfileString(strSec, "idFive", Convert.ToString(textBox_IdFive.Text), strFilePath);
                WritePrivateProfileString(strSec, "idSix", Convert.ToString(textBox_IdSix.Text), strFilePath);
                WritePrivateProfileString(strSec, "idSeven", Convert.ToString(textBox_IdSeven.Text), strFilePath);
                WritePrivateProfileString(strSec, "idEight", Convert.ToString(textBox_IdEight.Text), strFilePath);
                WritePrivateProfileString(strSec, "idNine", Convert.ToString(textBox_IdNine.Text), strFilePath);

                WritePrivateProfileString(strSec, "disRoute", Convert.ToString(checkBox_DisRoute.Checked), strFilePath);
                WritePrivateProfileString(strSec, "oneRoute", Convert.ToString(comboBox_RouteOne.Text), strFilePath);
                WritePrivateProfileString(strSec, "twoRoute", Convert.ToString(comboBox_RouteTwo.Text), strFilePath);
                WritePrivateProfileString(strSec, "threeRoute", Convert.ToString(comboBox_RouteThree.Text), strFilePath);
                WritePrivateProfileString(strSec, "fourRoute", Convert.ToString(comboBox_RouteFour.Text), strFilePath);
                WritePrivateProfileString(strSec, "fiveRoute", Convert.ToString(comboBox_RouteFive.Text), strFilePath);
                WritePrivateProfileString(strSec, "sixRoute", Convert.ToString(comboBox_RouteSix.Text), strFilePath);
                WritePrivateProfileString(strSec, "sevenRoute", Convert.ToString(comboBox_RouteSeven.Text), strFilePath);
                WritePrivateProfileString(strSec, "eightRoute", Convert.ToString(comboBox_RouteEight.Text), strFilePath);
                WritePrivateProfileString(strSec, "nineRoute", Convert.ToString(comboBox_RouteNine.Text), strFilePath);
                WritePrivateProfileString(strSec, "tenRoute", Convert.ToString(comboBox_RouteTen.Text), strFilePath);

                WritePrivateProfileString(strSec, "autoLogin", Convert.ToString(checkBox_autoLogin.Checked), strFilePath);
                WritePrivateProfileString(strSec, "steam", Convert.ToString(radioButton_steam.Checked), strFilePath);
                WritePrivateProfileString(strSec, "perfect", Convert.ToString(radioButton_perfect.Checked), strFilePath);
                WritePrivateProfileString(strSec, "perfectStartupPath", Convert.ToString(textBox_perfectStartupPath.Text), strFilePath);
                WritePrivateProfileString(strSec, "steamStartupPath", Convert.ToString(textBox_steamStartupPath.Text), strFilePath);
                tempStr = textBox_account.Text;
                tempStr = tempStr.Replace("\r\n", "****");
                WritePrivateProfileString(strSec, "account", tempStr, strFilePath);

                WritePrivateProfileString(strSec, "stoptime", Convert.ToString(textBox_stopTime.Text), strFilePath);
                WritePrivateProfileString(strSec, "shutdown", Convert.ToString(checkBox_shutdown.Checked), strFilePath);
                

                WritePrivateProfileString(strSec, "loginpassword", Convert.ToString(key), strFilePath);

                WritePrivateProfileString(strSec, "autoMegaphone", Convert.ToString(textBox_megaphone.Text), strFilePath);
                WritePrivateProfileString(strSec, "megaphoneDelay", Convert.ToString(textBox_MegaphoneDelay.Text), strFilePath);

                WritePrivateProfileString(strSec, "skillSequence", Convert.ToString(textBox_skillSequence.Text), strFilePath);
                WritePrivateProfileString(strSec, "escapeMode", Convert.ToString(checkBox_escapeMode.Checked), strFilePath);
                
            }
            catch (System.Exception ex)
            {

            }
        }
        private void checkBox_banSkillBeforeGather_CheckedChanged(object sender, EventArgs e)
        {
            if (true == checkBox_banSkillBeforeGather.Checked)
                banSkillBeforeGather = 1;
            else
                banSkillBeforeGather = 0;
            
        }
        private void textBox_megaphone_TextChanged(object sender, EventArgs e)
        {
            megaphoneText = textBox_megaphone.Text;
        }
        private void button_megaphone_Click(object sender, EventArgs e)
        {
            int dm_ret, i, megaphoneBindIndex,index;
            string megaphoneHwndList;
            megaphoneBindIndex = 0;
            index = 0;
            if (key != "")
            {
                if (a.Bind(key))
                {
                    Form1.loginSuccess = true;
                }
                else
                {
                    MessageBox.Show(a.LastError);
                    Form1.loginSuccess = false;
                }
            }
            if (false == loginSuccess)
            {
                form2 f2 = new form2();
                f2.StartPosition = FormStartPosition.CenterParent;
                f2.ShowDialog();
                if (false == loginSuccess)
                    return;
            }
            if (1 == mainIsRunning)
            {
                MessageBox.Show("已经开启自动挂机功能请关闭后在开启喊话功能");
                return;
            }
            if (1 == megaphoneIsRunning)
            {
                MessageBox.Show("已经开启喊话功能");
                return;
            }
            megaphoneHwndList = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
            if (megaphoneHwndList.Length <= 0)
            {
                MessageBox.Show("没有找到DOTA2窗口,请开启DOTA2后再启动");
                return;
            }
            //开始遍历并且绑定
            try
            {
                //养成好习惯，清空一下数组
                megaphoneHwndArrary.Clear();
                foreach (string hwnds in megaphoneHwndList.Split(','))
                {
                    //将每个大漠对象实例化【新手很容易忽略这一句】
                    megaphoneTestDm[megaphoneBindIndex] = new CDmSoft();
                    //设置大漠路径，这里大漠路径为大漠dll目录下的attachment文件夹，这文件夹里面存放图片和字库，在这里就是Debug目录，或者Release目录
                    megaphoneTestDm[megaphoneBindIndex].SetPath(".\\game_script");
                    //开始绑定，这里绑定的是后台
                    dm_ret = megaphoneTestDm[megaphoneBindIndex].BindWindow(int.Parse(hwnds), "dx.graphic.2d| dx.graphic.3d", "dx", "dx", 0);
                    if (1 == dm_ret)
                    {
                        megaphoneHwndArrary.Add(int.Parse(hwnds));//将句柄添加进句柄数组
                        megaphoneTestDm[megaphoneBindIndex].LockInput(1);
                        //控制多开
                    }
                    megaphoneBindIndex++;
                    if (threadNum == megaphoneBindIndex)
                        break;
                }

            }
            catch (System.Exception ex)
            {

            }
            try
            {
                megaphoneThread_List.Clear();
                for (index = 0; index < megaphoneHwndArrary.Count; index++)
                {
                    //执行带参数的线程，这里i为什么要赋值给index，不解释，大家照做，必须这样，特别是循环内启动线程一定要注意这点
                    Thread newThd = new Thread(() => thread_megaphone(megaphoneTestDm[index], (int)megaphoneHwndArrary[index]));
                    newThd.IsBackground = true;//设置为后台线程 有人说这就是设置线程模式为MTA模式，不知道是不是
                    //将这个线程添加进线程集合，方便停止
                    megaphoneThread_List.Add(newThd);
                    //启动
                    newThd.Start();
                    Thread.Sleep(1000);
                }
            }
            catch (System.Exception ex)
            {

            }
            megaphoneIsRunning = 1;

        }
        private void button_stopMegaphone_Click(object sender, EventArgs e)
        {
            int i;
            bool allIsKilled = true;
            megaphoneIsRunning = 0;
            for (i = 0; i < megaphoneTestDm.Length; i++)
            {
                try
                {
                    megaphoneTestDm[i].UnBindWindow();
                    megaphoneTestDm[bindIndex].LockInput(0);

                }
                catch (System.Exception ex)
                {

                }
            }
            while (true)
            {
                allIsKilled = true;
                try
                {
                    foreach (Thread Thd in megaphoneThread_List)
                    {
                        if (Thd.IsAlive)
                        {
                            Thd.Abort();
                            allIsKilled = false;
                            Thread.Sleep(50);
                        }
                    }
                }
                catch (System.Exception ex)
                {

                }
                if (allIsKilled)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }
        private void checkBox_waitMember_CheckedChanged(object sender, EventArgs e)
        {
            if (true == checkBox_waitMember.Checked)
                waitMemberRevive = 1;
            else
                waitMemberRevive = 0;

        }
        private void textBox_perfectStartupPath_TextChanged(object sender, EventArgs e)
        {
            if ("perfect" == startupWay)
            {
                startupPath = textBox_perfectStartupPath.Text;
            }
        }
        private void textBox_steamStartupPath_TextChanged(object sender, EventArgs e)
        {
            if ("steam" == startupWay)
            {
                startupPath = textBox_steamStartupPath.Text;
            }
            
        }
        private void radioButton_steam_CheckedChanged(object sender, EventArgs e)
        {
            startupWay = "steam";
        }

        private void radioButton_perfect_CheckedChanged(object sender, EventArgs e)
        {
            startupWay = "perfect";
        }

        private void comboBox_System_SelectedIndexChanged(object sender, EventArgs e)
        {
            system = comboBox_System.Text;
        }
        private void comboBox_Route_SelectedIndexChanged(object sender, EventArgs e)
        {
            allRoute = comboBox_Route.Text;
        }

        private void comboBox_KillWay_SelectedIndexChanged(object sender, EventArgs e)
        {
            killWay = comboBox_KillWay.Text;
        }

        private void textBox_GatTime_TextChanged(object sender, EventArgs e)
        {
            gatTime = int.Parse(textBox_GatTime.Text);
        }
        private void checkBox_InviteTeammate_CheckedChanged(object sender, EventArgs e)
        {
            if (true == checkBox_InviteTeammate.Checked)
            {
                inviteTeammate          = 1 ;
                textBox_IdOne.Enabled   = true ;
                textBox_IdTwo.Enabled   = true ;
                textBox_IdThree.Enabled = true ;
                textBox_IdFour.Enabled  = true ;
                textBox_IdFive.Enabled  = true ;
                textBox_IdSix.Enabled   = true ;
                textBox_IdSeven.Enabled = true ;
                textBox_IdEight.Enabled = true ;
                textBox_IdNine.Enabled  = true ;
            }
            else
            {
                inviteTeammate          = 0 ;
                textBox_IdOne.Enabled   = false ;
                textBox_IdTwo.Enabled   = false ;
                textBox_IdThree.Enabled = false ;
                textBox_IdFour.Enabled  = false ;
                textBox_IdFive.Enabled  = false ;
                textBox_IdSix.Enabled   = false ;
                textBox_IdSeven.Enabled = false ;
                textBox_IdEight.Enabled = false ;
                textBox_IdNine.Enabled  = false ;
            }

        }

        private void checkBox_DisRoute_CheckedChanged(object sender, EventArgs e)
        {
            if (true == checkBox_DisRoute.Checked)
            {
                disRoute                    = 1;
                comboBox_RouteOne.Enabled   = true;
                comboBox_RouteTwo.Enabled   = true;
                comboBox_RouteThree.Enabled = true;
                comboBox_RouteFour.Enabled  = true;
                comboBox_RouteFive.Enabled  = true;
                comboBox_RouteSix.Enabled   = true;
                comboBox_RouteSeven.Enabled = true;
                comboBox_RouteEight.Enabled = true;
                comboBox_RouteNine.Enabled  = true;
                comboBox_RouteTen.Enabled   = true;
            }
            else
            {
                disRoute                        = 0;
                comboBox_RouteOne.Enabled = false;
                comboBox_RouteTwo.Enabled = false;
                comboBox_RouteThree.Enabled = false;
                comboBox_RouteFour.Enabled = false;
                comboBox_RouteFive.Enabled = false;
                comboBox_RouteSix.Enabled = false;
                comboBox_RouteSeven.Enabled = false;
                comboBox_RouteEight.Enabled = false;
                comboBox_RouteNine.Enabled = false;
                comboBox_RouteTen.Enabled = false;
            }
        }

        private void radioButton_RandomHero_CheckedChanged(object sender, EventArgs e)
        {
            chooseMode                  = "随机英雄";
            comboBox_FirstHero.Enabled = false;
            comboBox_SecondHero.Enabled = false;
            comboBox_ThirdHero.Enabled = false;
            comboBox_FourthHero.Enabled = false;
            comboBox_FifthHero.Enabled = false;
            comboBox_SixthHero.Enabled = false;
            comboBox_SeventhHero.Enabled = false;
        }

        private void radioButton_CustomHero_CheckedChanged(object sender, EventArgs e)
        {
            chooseMode                      = "自定义英雄" ;
            comboBox_FirstHero.Enabled      = true;
            comboBox_SecondHero.Enabled     = true;
            comboBox_ThirdHero.Enabled      = true;
            comboBox_FourthHero.Enabled     = true;
            comboBox_FifthHero.Enabled       = true;
            comboBox_SixthHero.Enabled      = true;
            comboBox_SeventhHero.Enabled    = true;
        }
        private void comboBox_FirstHero_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x,y;
            tempStr = Convert.ToString(comboBox_FirstHero.SelectedIndex);
            tempStr = tempStr.Insert(tempStr.Length, ".bmp");
            tempStr = tempStr.Insert(0, "h");
            hero[0] = tempStr;


            calculateHeroCoordinate(comboBox_FirstHero.SelectedIndex,out x,out y);
            heroCoordinate[0,0] = x ;
            heroCoordinate[0,1] = y ;
        }

        private void comboBox_SecondHero_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x,y;
            tempStr = Convert.ToString(comboBox_SecondHero.SelectedIndex);
            tempStr = tempStr.Insert(tempStr.Length, ".bmp");
            tempStr = tempStr.Insert(0, "h");
            hero[1] = tempStr;

            calculateHeroCoordinate(comboBox_SecondHero.SelectedIndex, out x, out y);
            heroCoordinate[1, 0] = x;
            heroCoordinate[1, 1] = y;
        }

        private void comboBox_ThirdHero_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x, y;
            tempStr = Convert.ToString(comboBox_ThirdHero.SelectedIndex);
            tempStr = tempStr.Insert(tempStr.Length, ".bmp");
            tempStr = tempStr.Insert(0, "h");
            hero[2] = tempStr;

            calculateHeroCoordinate(comboBox_ThirdHero.SelectedIndex, out x, out y);
            heroCoordinate[2, 0] = x;
            heroCoordinate[2, 1] = y;
        }

        private void comboBox_FourthHero_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x, y;
            tempStr = Convert.ToString(comboBox_FourthHero.SelectedIndex);
            tempStr = tempStr.Insert(tempStr.Length, ".bmp");
            tempStr = tempStr.Insert(0, "h");
            hero[3] = tempStr;

            calculateHeroCoordinate(comboBox_FourthHero.SelectedIndex, out x, out y);
            heroCoordinate[3, 0] = x;
            heroCoordinate[3, 1] = y;
        }

        private void comboBox_FifthHero_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x, y;
            tempStr = Convert.ToString(comboBox_FifthHero.SelectedIndex);
            tempStr = tempStr.Insert(tempStr.Length, ".bmp");
            tempStr = tempStr.Insert(0, "h");
            hero[4] = tempStr;

            calculateHeroCoordinate(comboBox_FifthHero.SelectedIndex, out x, out y);
            heroCoordinate[4, 0] = x;
            heroCoordinate[4, 1] = y;
        }

        private void comboBox_SixthHero_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x, y;
            tempStr = Convert.ToString(comboBox_SixthHero.SelectedIndex);
            tempStr = tempStr.Insert(tempStr.Length, ".bmp");
            tempStr = tempStr.Insert(0, "h");
            hero[5] = tempStr;

            calculateHeroCoordinate(comboBox_SixthHero.SelectedIndex, out x, out y);
            heroCoordinate[5, 0] = x;
            heroCoordinate[5, 1] = y;
        }

        private void comboBox_SeventhHero_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x, y;
            tempStr = Convert.ToString(comboBox_SeventhHero.SelectedIndex);
            tempStr = tempStr.Insert(tempStr.Length, ".bmp");
            tempStr = tempStr.Insert(0, "h");
            hero[6] = tempStr;

            calculateHeroCoordinate(comboBox_SeventhHero.SelectedIndex, out x, out y);
            heroCoordinate[6, 0] = x;
            heroCoordinate[6, 1] = y;
        }
        private void radioButton_isLead_CheckedChanged(object sender, EventArgs e)
        {
            leadIsMe = 1;
        }

        private void radioButton_isMember_CheckedChanged(object sender, EventArgs e)
        {
            leadIsMe = 0;
        }
        private void textBox_IdOne_TextChanged(object sender, EventArgs e)
        {
            memberId[0] = textBox_IdOne.Text ;
        }

        private void textBox_IdTwo_TextChanged(object sender, EventArgs e)
        {
            memberId[1] = textBox_IdTwo.Text;
        }

        private void textBox_IdThree_TextChanged(object sender, EventArgs e)
        {
            memberId[2] = textBox_IdThree.Text;
        }

        private void textBox_IdFour_TextChanged(object sender, EventArgs e)
        {
            memberId[3] = textBox_IdFour.Text;
        }

        private void textBox_IdFive_TextChanged(object sender, EventArgs e)
        {
            memberId[4] = textBox_IdFive.Text;
        }

        private void textBox_IdSix_TextChanged(object sender, EventArgs e)
        {
            memberId[5] = textBox_IdSix.Text;
        }

        private void textBox_IdSeven_TextChanged(object sender, EventArgs e)
        {
            memberId[6] = textBox_IdSeven.Text;
        }

        private void textBox_IdEight_TextChanged(object sender, EventArgs e)
        {
            memberId[7] = textBox_IdEight.Text;
        }

        private void textBox_IdNine_TextChanged(object sender, EventArgs e)
        {
            memberId[8] = textBox_IdNine.Text;
        }

        private void comboBox_FirstEqm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_FirstEqm.SelectedIndex != -1)
            {
                tempStr     = Convert.ToString(comboBox_FirstEqm.SelectedIndex);
                tempStr     = tempStr.Insert(tempStr.Length, ".bmp");
                eqmExt[0]   = tempStr;
                tempStr = Convert.ToString(comboBox_FirstEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                tempStr = tempStr.Insert(0, "e");
                eqm[0] = tempStr;
            } 
        }
        private void comboBox_SecondEqm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_SecondEqm.SelectedIndex != -1)
            {
                tempStr = Convert.ToString(comboBox_SecondEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                eqmExt[1] = tempStr;
                tempStr = Convert.ToString(comboBox_SecondEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                tempStr = tempStr.Insert(0, "e");
                eqm[1] = tempStr;
            }
        }
        private void comboBox_ThirdEqm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_ThirdEqm.SelectedIndex != -1)
            {
                tempStr = Convert.ToString(comboBox_ThirdEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                eqmExt[2] = tempStr;
                tempStr = Convert.ToString(comboBox_ThirdEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                tempStr = tempStr.Insert(0, "e");
                eqm[2] = tempStr;
            }
        }
        private void comboBox_FourthEqm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_FourthEqm.SelectedIndex != -1)
            {
                tempStr = Convert.ToString(comboBox_FourthEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                eqmExt[3] = tempStr;
                tempStr = Convert.ToString(comboBox_FourthEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                tempStr = tempStr.Insert(0, "e");
                eqm[3] = tempStr;
            }
        }
        private void comboBox_FifthEqm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_FifthEqm.SelectedIndex != -1)
            {
                tempStr = Convert.ToString(comboBox_FifthEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                eqmExt[4] = tempStr;
                tempStr = Convert.ToString(comboBox_FifthEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                tempStr = tempStr.Insert(0, "e");
                eqm[4] = tempStr;
            }
        }
        private void comboBox_SixthEqm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_SixthEqm.SelectedIndex != -1)
            {
                tempStr = Convert.ToString(comboBox_SixthEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                eqmExt[5] = tempStr;
                tempStr = Convert.ToString(comboBox_SixthEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                tempStr = tempStr.Insert(0, "e");
                eqm[5] = tempStr;
            }
        }
        private void comboBox_SeventhEqm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_SixthEqm.SelectedIndex != -1)
            {
                tempStr = Convert.ToString(comboBox_SeventhEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                eqmExt[6] = tempStr;
                tempStr = Convert.ToString(comboBox_SeventhEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                tempStr = tempStr.Insert(0, "e");
                eqm[6] = tempStr;
            }
        }

        private void comboBox_eighthEqm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_SixthEqm.SelectedIndex != -1)
            {
                tempStr = Convert.ToString(comboBox_EighthEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                eqmExt[7] = tempStr;
                tempStr = Convert.ToString(comboBox_EighthEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                tempStr = tempStr.Insert(0, "e");
                eqm[7] = tempStr;
            }
        }

        private void comboBox_ninthEqm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_SixthEqm.SelectedIndex != -1)
            {
                tempStr = Convert.ToString(comboBox_NinthEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                eqmExt[8] = tempStr;
                tempStr = Convert.ToString(comboBox_NinthEqm.SelectedIndex);
                tempStr = tempStr.Insert(tempStr.Length, ".bmp");
                tempStr = tempStr.Insert(0, "e");
                eqm[8] = tempStr;
            }
        }
        private void comboBox_RouteOne_SelectedIndexChanged(object sender, EventArgs e)
        {
            chooseRoute[0] = comboBox_RouteOne.Text;
        }

        private void comboBox_RouteTwo_SelectedIndexChanged(object sender, EventArgs e)
        {
            chooseRoute[1] = comboBox_RouteTwo.Text;
        }

        private void comboBox_RouteThree_SelectedIndexChanged(object sender, EventArgs e)
        {
            chooseRoute[2] = comboBox_RouteThree.Text;
        }

        private void comboBox_RouteFour_SelectedIndexChanged(object sender, EventArgs e)
        {
            chooseRoute[3] = comboBox_RouteFour.Text;
        }

        private void comboBox_RouteFive_SelectedIndexChanged(object sender, EventArgs e)
        {
            chooseRoute[4] = comboBox_RouteFive.Text;
        }

        private void comboBox_RouteSix_SelectedIndexChanged(object sender, EventArgs e)
        {
            chooseRoute[5] = comboBox_RouteSix.Text;
        }

        private void comboBox_RouteSeven_SelectedIndexChanged(object sender, EventArgs e)
        {
            chooseRoute[6] = comboBox_RouteSeven.Text;
        }

        private void comboBox_RouteEight_SelectedIndexChanged(object sender, EventArgs e)
        {
            chooseRoute[7] = comboBox_RouteEight.Text;
        }

        private void comboBox_RouteNine_SelectedIndexChanged(object sender, EventArgs e)
        {
            chooseRoute[8] = comboBox_RouteNine.Text;
        }

        private void comboBox_RouteTen_SelectedIndexChanged(object sender, EventArgs e)
        {
            chooseRoute[9] = comboBox_RouteTen.Text;

        }
        private void checkBox_stopMatch_CheckedChanged(object sender, EventArgs e)
        {
            if (true == checkBox_stopMatch.Checked)
            {
                textBox_stopTime.Enabled    = true;
                timeFun                     = 1;
            }
            else
            {
                textBox_stopTime.Enabled = false;
                timeFun                  = 0;
            }
        }
        private void textBox_stopTime_TextChanged(object sender, EventArgs e)
        {
            stopTime = textBox_stopTime.Text;
        }
        private void button_stop_Click(object sender, EventArgs e)
        {
            int i;
            foreach (Thread Thd in Thread_List)
            {
                try
                {
                    Thd.Suspend();
                }
                catch (System.Exception ex)
                {

                }
                
            }
            for (i = 0; i < testDm.Length; i++)
            {
                try
                {
                    testDm[i].UnBindWindow();
                    Thread.Sleep(500);
                }
                catch (System.Exception ex)
                {

                }
            }
            foreach (Thread Thd in Thread_List)
            {
                try
                {
                    Thd.Abort();
                }
                catch (System.Exception ex)
                {

                }
            }
            try
            {
                threadStartup.Abort();
                threadGameCrash.Abort();
            }
            catch
            {
            }
            
//             while (true)
//             {
//                 allIsKilled = true;
//                 try
//                 {
//                     foreach (Thread Thd in Thread_List)
//                     {
//                         if (Thd.IsAlive)
//                         {
//                             Thd.Abort();
//                             allIsKilled = false;
//                             Thread.Sleep(50);
//                         }
//                     }
//                 }
//                 catch (System.Exception ex)
//                 {
// 
//                 }
//                 if (allIsKilled)
//                 {
//                     break;
//                 }
//                 else
//                 {
//                     Thread.Sleep(50);
//                 }
//             }

            mainIsRunning = 0;
            this.notifyIcon1.ShowBalloonTip(1000, "3R", "已关闭", ToolTipIcon.Info);
        }
        private void checkBox_autoLogin_CheckedChanged(object sender, EventArgs e)
        {
            if (true == checkBox_autoLogin.Checked)
            {
                autoLogin = 1;
                textBox_steamStartupPath.Enabled = true;
                textBox_perfectStartupPath.Enabled = true;
                textBox_account.Enabled = true;
            }
            else
            {
                autoLogin = 0;
                textBox_steamStartupPath.Enabled = false;
                textBox_perfectStartupPath.Enabled = false;
                textBox_account.Enabled = false;
            }
            
        }

        private void textBox_account_TextChanged(object sender, EventArgs e)
        {
            int i;
            tempStr = textBox_account.Text;
            tempStr = tempStr.Replace("\r\n", "*");
            i = 0 ;
            foreach (string strs in tempStr.Split('*'))
            {
                account[i] = strs;
                i++;
            }
            i = 0 ;
        }
        private void comboBox_matchMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            matchMode = comboBox_matchMode.Text;
        }
        private void button_statr_Click(object sender, EventArgs e)
        {
            if (1 == megaphoneIsRunning)
            {
                MessageBox.Show("请关闭喊话功能后再启动");
                return;
            }
            if (1 == mainIsRunning)
            {
                MessageBox.Show("请终止后再启动");
                return;
            }
            mainIsRunning = 1;        //监测函数运行防止开启喊话和多次启动
            this.notifyIcon1.ShowBalloonTip(1000, "3R", "已启动,请注意游戏不能最小化", ToolTipIcon.Info);
            Thread threadStartup = new Thread(() => thread_startup());
            threadStartup.Start();

            
        }

        protected override void WndProc(ref Message m)//监视Windows消息  
        {
            const int WM_HOTKEY = 0x0312;//按快捷键  
            const int WM_QUERYENDSESSION = 0x11;
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 100:
                            SendMessage(this.button_statr.Handle, WM_LBUTTONDOWN, 0, 0);
                            SendMessage(this.button_statr.Handle, WM_LBUTTONUP, 0, 0);
                            break;
                        case 101:
                            SendMessage(this.button_stop.Handle, WM_LBUTTONDOWN, 0, 0);
                            SendMessage(this.button_stop.Handle, WM_LBUTTONUP, 0, 0);
                            break;
                    }
                    break;
                case WM_QUERYENDSESSION:
                    a.Unbind();
                    Thread.Sleep(1000);
                    m.Result = (IntPtr)1;//０不关闭程序；１关闭程序
                    break;
            }
            base.WndProc(ref m);
        }
        #endregion


        #region 游戏状态监测

        private void thread_main(CDmSoft dm, int hwnd)
        {
            object x, y, x1;
            int lead, i;
            int stopMatch,noMember, threadIsRunning, chooseHeroNum, allInning;
            string hwndTemp;
            string winClass = dm.GetWindowClass(hwnd);
            List<Thread> threadList = new List<Thread>();
            APP application = new APP();
            x1 = 0;
            i = 0;
            stopMatch = 0;
            lead = 0;
            threadIsRunning = 0;
            chooseHeroNum   = 0;
            allInning       = 0;

            //游戏内数据
            if (1 == inviteTeammate)
            {
                if (threadNum < 5)
                {
                    if (1 == threadNum)
                    {
                        if (1 == leadIsMe && 0 == haveLead)
                        {
                            lead = 1;
                            haveLead = 1;
                        }
                    }
                    else
                    {
                        if(0 == haveLead)
                        {
                            lead = 1;
                            haveLead = 1;
                        }
                    }

                }
                else
                {
                    if ("Sandbox:D1:Valve001" == winClass)
                        lead = 1;
                    if ("Sandbox:D6:Valve001" == winClass)
                        lead = 2;
                }
            }
            //避免有的客户黑屏，并且能够直接进入游戏
            dm.KeyPress(27);
            /*游戏状态判断功能*/
            while (true)
            {
                if (1 == autoLogin)
                {
                    dm.FindPic(475, 110, 540, 145, "warn.bmp", "000000", 0.8, 0, out x, out y);
                    if ((int)x > 0)
                    {
                        dm.FindPic(250, 155, 750, 250, "gameneedupdate.bmp", "303030", 0.9, 0, out x, out y);
                        if ((int)x > 0)
                            gameNeedUpdate++;
                    }

                }
                dm.FindStr(90, 0, 135, 35, "help", "cbcbcb-999999", 0.9, out x, out y);
                    if ((int)x < 0)
                    {
                    if (1 == threadIsRunning)
                    {
                        dm.FindPic(945, 460, 995, 550, "add.bmp", "101010", 0.9, 0, out x, out y);
                        if ((int)x > 0)
                        {
                            dm.FindPic(0, 0, 35, 35, "setting.bmp", "101010", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                try
                                {
                                    foreach (Thread Thd in threadList)
                                    {
                                        if (Thd.IsAlive)
                                        {
                                            Thd.Abort();
                                            Thread.Sleep(50);
                                            threadList.Remove(Thd);
                                            Thread_List.Remove(Thd);  //之所以加两个是应为之后终止游戏的时候可以把这个线程关了
                                            threadIsRunning = 0;
                                            chooseHeroNum = 0;
                                        }
                                    }
                                }
                                catch (System.Exception ex)
                                {

                                }
                            }
                        }
                    }
                    dm.FindPic(440, 310, 565, 385, "beginGame.bmp", "303030", 0.8, 0, out x, out y);
                    if ((int)x > 0)
                    {
                        dm.MoveTo((int)x, (int)y);
                        Thread.Sleep(50);
                        dm.LeftClick();
                    }
                    //每次都点到游戏防止错误
                    dm.FindPic(405, 0, 485, 35, "game.bmp|ingame.bmp ", "000000", 0.9, 0, out x, out y);
                    if ((int)x > 0)
                    {
                        dm.MoveTo((int)x, (int)y);
                        Thread.Sleep(500);
                        dm.LeftDown();
                        Thread.Sleep(500);
                        dm.LeftUp();
                        Thread.Sleep(500);
                    }
                    if ("天梯" == matchMode)
                    {
                        dm.FindStr(45, 153, 130, 255, "天梯匹配", "686969-303030", 0.9, out x, out y);
                        if ((int)x > 0)
                        {
                            dm.MoveTo((int)x, (int)y);
                            Thread.Sleep(50);
                            dm.LeftClick();
                        }
                    }
                    else if ("人人" == matchMode)
                    {
                        dm.FindStr(45, 153, 130, 255, "普通匹配", "686969-303030", 0.9, out x, out y);
                        if ((int)x > 0)
                        {
                            dm.MoveTo((int)x + 3, (int)y + 3);
                            Thread.Sleep(50);
                            dm.LeftClick();
                        }
                    }
                    else if ("人机" == matchMode)
                    {
                        dm.FindStr(45, 153, 130, 255, "机器人比赛", "686969-303030", 0.9, out x, out y);
                        if ((int)x > 0)
                        {
                            dm.MoveTo((int)x, (int)y);
                            Thread.Sleep(50);
                            dm.LeftClick();
                        }
                    }
                    //到时间则停止搜索
                    if (1 == timeFun)
                    {
                        DateTime dt1 = Convert.ToDateTime(stopTime);
                        DateTime dt2 = DateTime.Now;
                        if (DateTime.Compare(dt2, dt1) > 0)
                            stopMatch = 1;
                    }
                    if (1 == inningStopFun && inningStopNum == allInning)
                        stopMatch = 1;
                    if ("天梯" == matchMode)
                        dm.FindStr(400, 200, 485, 235, "寻找比赛", "a6a7a3-454545", 0.9, out x, out y);
                    else if ("人人" == matchMode || "人机" == matchMode)
                        dm.FindStr(400,200,485,235, "寻找比赛", "a6a7a3-454545", 0.9, out x, out y);
                    if ((int)x > 0)
                    {
                        if (0 == stopMatch)
                        {
                            if (1 == inviteTeammate)
                            {
                                noMember = 0;
                                if (1 == lead || 2 == lead)
                                {
                                    dm.FindPic(910, 75, 975, 95, "quiteteam.bmp", "000000", 0.9, 0, out x, out y);
                                    if ((int)x < 0)
                                        noMember = 1;
                                    else
                                    {
                                        for (i = 0; i < 2; i++)
                                        {
                                            dm.FindPic(705, 95, 1000, 160, "nomember.bmp", "101010", 0.9, 0, out x, out y);
                                            if ((int)x > 0)
                                                noMember = 1;
                                        }
                                    }
                                    //队长等待账号开启
                                    if (threadNum == 5 && 1 == autoLogin)
                                    {
                                        do
                                        {
                                            i = 0;
                                            hwndTemp = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
                                            foreach (string strs in hwndTemp.Split(','))
                                                i++;
                                            Thread.Sleep(1000);
                                        } while (i < 5);
                                    }
                                    else if (threadNum == 10 && 1 == autoLogin)
                                    {
                                        do
                                        {
                                            i = 0;
                                            hwndTemp = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
                                            foreach (string strs in hwndTemp.Split(','))
                                                i++;
                                            Thread.Sleep(1000);
                                        } while (i < 10);
                                    }
                                    if (1 == noMember)
                                    {
                                        for (i = 0; i < 3; i++)
                                        {
                                            Thread.Sleep(1000);
                                            if (1 == lead)
                                            {
                                                if (memberId[0] != "")
                                                {
                                                    dm.FindPic(945, 460, 995, 550, "add.bmp", "101010", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1000);
                                                    dm.SendString2(hwnd, memberId[0]);
                                                    Thread.Sleep(2000);
                                                    dm.FindPic(760, 600, 815, 625, "searchmember.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x + 2, (int)y + 2);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1500);
                                                    dm.MoveTo(155, 145);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(50);
                                                    dm.FindPic(140, 195, 255, 265, "invitetoteam.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(2000);
                                                }
                                                if (memberId[1] != "")
                                                {
                                                    dm.FindPic(945, 460, 995, 550, "add.bmp", "101010", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1000);
                                                    dm.SendString2(hwnd, memberId[1]);
                                                    Thread.Sleep(2000);
                                                    dm.FindPic(760, 600, 815, 625, "searchmember.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x + 2, (int)y + 2);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1500);
                                                    dm.MoveTo(155, 145);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(50);
                                                    dm.FindPic(140, 195, 255, 265, "invitetoteam.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(2000);
                                                }
                                                if (memberId[2] != "")
                                                {
                                                    dm.FindPic(945, 460, 995, 550, "add.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1000);
                                                    dm.SendString2(hwnd, memberId[2]);
                                                    Thread.Sleep(2000);
                                                    dm.FindPic(760, 600, 815, 625, "searchmember.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x + 2, (int)y + 2);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1500);
                                                    dm.MoveTo(155, 145);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(50);
                                                    dm.FindPic(140, 195, 255, 265, "invitetoteam.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(2000);
                                                }
                                                if (memberId[3] != "")
                                                {
                                                    dm.FindPic(945, 460, 995, 550, "add.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1000);
                                                    dm.SendString2(hwnd, memberId[3]);
                                                    Thread.Sleep(2000);
                                                    dm.FindPic(760, 600, 815, 625, "searchmember.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x + 2, (int)y + 2);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1500);
                                                    dm.MoveTo(155, 145);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(50);
                                                    dm.FindPic(140, 195, 255, 265, "invitetoteam.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(2000);
                                                }
                                                if (memberId[4] != "")
                                                {
                                                    dm.FindPic(945, 460, 995, 550, "add.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1000);
                                                    dm.SendString2(hwnd, memberId[4]);
                                                    Thread.Sleep(2000);
                                                    dm.FindPic(760, 600, 815, 625, "searchmember.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x + 2, (int)y + 2);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1500);
                                                    dm.MoveTo(155, 145);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(50);
                                                    dm.FindPic(140, 195, 255, 265, "invitetoteam.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(2000);
                                                }
                                            }
                                            if (2 == lead)
                                            {
                                                if (memberId[5] != "")
                                                {
                                                    dm.FindPic(945, 460, 995, 550, "add.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1000);
                                                    dm.SendString2(hwnd, memberId[5]);
                                                    Thread.Sleep(2000);
                                                    dm.FindPic(760, 600, 815, 625, "searchmember.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x + 2, (int)y + 2);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1500);
                                                    dm.MoveTo(155, 145);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(50);
                                                    dm.FindPic(140, 195, 255, 265, "invitetoteam.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(2000);
                                                }
                                                if (memberId[6] != "")
                                                {
                                                    dm.FindPic(945, 460, 995, 550, "add.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1000);
                                                    dm.SendString2(hwnd, memberId[6]);
                                                    Thread.Sleep(2000);
                                                    dm.FindPic(760, 600, 815, 625, "searchmember.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x + 2, (int)y + 2);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1500);
                                                    dm.MoveTo(155, 145);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(50);
                                                    dm.FindPic(140, 195, 255, 265, "invitetoteam.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(2000);
                                                }
                                                if (memberId[7] != "")
                                                {
                                                    dm.FindPic(945, 460, 995, 550, "add.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1000);
                                                    dm.SendString2(hwnd, memberId[7]);
                                                    Thread.Sleep(2000);
                                                    dm.FindPic(760, 600, 815, 625, "searchmember.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x + 2, (int)y + 2);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1500);
                                                    dm.MoveTo(155, 145);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(50);
                                                    dm.FindPic(140, 195, 255, 265, "invitetoteam.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(2000);
                                                }
                                                if (memberId[8] != "")
                                                {
                                                    dm.FindPic(945, 460, 995, 550, "add.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1000);
                                                    dm.SendString2(hwnd, memberId[8]);
                                                    Thread.Sleep(2000);
                                                    dm.FindPic(760, 600, 815, 625, "searchmember.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x + 2, (int)y + 2);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(1500);
                                                    dm.MoveTo(155, 145);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(50);
                                                    dm.FindPic(140, 195, 255, 265, "invitetoteam.bmp", "000000", 0.9, 0, out x, out y);
                                                    Thread.Sleep(50);
                                                    dm.MoveTo((int)x, (int)y);
                                                    Thread.Sleep(50);
                                                    dm.LeftDown();
                                                    Thread.Sleep(50);
                                                    dm.LeftUp();
                                                    Thread.Sleep(2000);
                                                }
                                            }
                                            dm.FindPic(705, 95, 1000, 160, "nomember.bmp", "101010", 0.9, 0, out x, out y);
                                            if ((int)x < 0)
                                                break;
                                        }
                                    }
                                    //返回主界面
                                    dm.MoveTo(445, 20);
                                    Thread.Sleep(50);
                                    dm.LeftDown();
                                    Thread.Sleep(50);
                                    dm.LeftUp();
                                    if (0 == stopMatch)
                                    {
                                        if ("天梯" == matchMode)
                                        {
                                            dm.FindStr(400, 200, 485, 235, "寻找比赛", "a6a7a3-454545", 0.9, out x, out y);
                                            Thread.Sleep(50);
                                            dm.MoveTo((int)x, (int)y);
                                            Thread.Sleep(50);
                                            dm.LeftDown();
                                            Thread.Sleep(50);
                                            dm.LeftUp();
                                            Thread.Sleep(50);
                                        }
                                        else if ("人机" == matchMode)
                                        {
                                            dm.FindStr(400, 200, 485, 235, "寻找比赛", "a6a7a3-454545", 0.9, out x, out y);
                                            Thread.Sleep(50);
                                            dm.MoveTo((int)x, (int)y);
                                            Thread.Sleep(50);
                                            dm.LeftDown();
                                            Thread.Sleep(50);
                                            dm.LeftUp();
                                            Thread.Sleep(5000);
                                        }
                                        else if ("人人" == matchMode)
                                        {
                                            dm.FindStr(400, 200, 485, 235, "寻找比赛", "a6a7a3-454545", 0.9, out x, out y);
                                            Thread.Sleep(50);
                                            dm.MoveTo((int)x, (int)y);
                                            Thread.Sleep(50);
                                            dm.LeftDown();
                                            Thread.Sleep(50);
                                            dm.LeftUp();
                                            Thread.Sleep(5000);
                                        }
                                        else if ("混乱" == matchMode)
                                        {
                                            dm.FindStr(400, 200, 485, 235, "寻找比赛", "a6a7a3-454545", 0.9, out x, out y);
                                            Thread.Sleep(50);
                                            dm.MoveTo((int)x, (int)y);
                                            Thread.Sleep(50);
                                            dm.LeftDown();
                                            Thread.Sleep(50);
                                            dm.LeftUp();
                                            Thread.Sleep(5000);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //单人模式拒绝邀请
                                dm.FindStr(570, 360, 645,430, "拒", "7c7c7c-303030", 0.9, out x, out y);
                                if ((int)x > 0)
                                {
                                    dm.MoveTo((int)x + 1, (int)y + 1);
                                    Thread.Sleep(50);
                                    dm.LeftClick();
                                    Thread.Sleep(50);
                                    dm.LeftClick();
                                }
                                if (0 == stopMatch)
                                {
                                    if ("天梯" == matchMode)
                                    {
                                        dm.FindStr(400, 200, 485, 235, "寻找比赛", "a6a7a3-454545", 0.9, out x, out y);
                                        Thread.Sleep(50);
                                        dm.MoveTo((int)x, (int)y);
                                        Thread.Sleep(50);
                                        dm.LeftDown();
                                        Thread.Sleep(50);
                                        dm.LeftUp();
                                        Thread.Sleep(50);
                                    }
                                    else if ("人机" == matchMode)
                                    {
                                        dm.FindStr(400, 200, 485, 235, "寻找比赛", "a6a7a3-454545", 0.9, out x, out y);
                                        Thread.Sleep(50);
                                        dm.MoveTo((int)x, (int)y);
                                        Thread.Sleep(50);
                                        dm.LeftDown();
                                        Thread.Sleep(50);
                                        dm.LeftUp();
                                        Thread.Sleep(5000);
                                    }
                                    else if ("人人" == matchMode)
                                    {
                                        dm.FindStr(400, 200, 485, 235, "寻找比赛", "a6a7a3-454545", 0.9, out x, out y);
                                        Thread.Sleep(50);
                                        dm.MoveTo((int)x, (int)y);
                                        Thread.Sleep(50);
                                        dm.LeftDown();
                                        Thread.Sleep(50);
                                        dm.LeftUp();
                                        Thread.Sleep(5000);
                                    }
                                    else if("混乱" == matchMode)
                                    {
                                        dm.FindStr(400, 200, 485, 235, "寻找比赛", "a6a7a3-454545", 0.9, out x, out y);
                                        Thread.Sleep(50);
                                        dm.MoveTo((int)x, (int)y);
                                        Thread.Sleep(50);
                                        dm.LeftDown();
                                        Thread.Sleep(50);
                                        dm.LeftUp();
                                        Thread.Sleep(5000);
                                    }
                                }
                            }
                            //游戏更新则退出后等待20分钟在开启
                        }
                        else
                        {
                            if (1 == shutdown)
                            {
                                SendMessage(this.button_stop.Handle, WM_LBUTTONDOWN, 0, 0);
                                SendMessage(this.button_stop.Handle, WM_LBUTTONUP, 0, 0);
                                a.Unbind();
                                Thread.Sleep(5000);
                                System.Diagnostics.Process.Start("shutdown", "-s -t 10");
                            }
                        }
                    }
                    //拒绝交易
                    dm.FindPic(440, 160, 590, 195, "trade.bmp", "000000", 0.9, 0, out x, out y);
                    if ((int)x > 0)
                    {
                        dm.FindPic(570, 290, 640, 340, "refusetrade.bmp", "000000", 0.9, 0, out x, out y);
                        if ((int)x > 0)
                        {
                            dm.MoveTo((int)x + 2, (int)y + 2);
                            Thread.Sleep(50);
                            dm.LeftDown();
                            Thread.Sleep(50);
                            dm.LeftUp();
                        }
                    }
                    //开启组队模式成员自动进队伍
                    if (1 == inviteTeammate && lead != 1 && lead != 1)
                    {
                        dm.FindPic(415, 380, 460, 400, "inacceptteaminvite.bmp", "000000", 0.9, 0, out x, out y);
                        if ((int)x > 0)
                        {
                            dm.MoveTo((int)x + 2, (int)y + 2);
                            Thread.Sleep(50);
                            dm.LeftClick();
                            Thread.Sleep(3000);
                        }
                        dm.FindPic(415, 380, 460, 400, "acceptteaminvite.bmp", "000000", 0.9, 0, out x, out y);
                        if ((int)x > 0)
                        {
                            dm.MoveTo((int)x + 2, (int)y + 2);
                            Thread.Sleep(50);
                            dm.LeftClick();
                            Thread.Sleep(3000);
                        }
                    }
                    dm.FindPic(270, 255, 675, 340, "nothank.bmp", "000000", 0.9, 0, out x, out y);
                    if ((int)x > 0)
                    {
                        dm.MoveTo((int)x + 2, (int)y + 2);
                        Thread.Sleep(50);
                        dm.LeftClick();
                        Thread.Sleep(3000);
                    }
                    //接受游戏
                    //                     dm.FindPic(390, 275, 630, 340, "gameready.bmp", "000000", 0.9, 0, out x, out y);
                    //                     if ((int)x > 0)
                    //                     {
                    /*dm.FindPic(350, 340, 460, 400, "accept.bmp", "000000", 0.9, 0, out x, out y);*/
                    dm.FindStr(350, 340, 460, 400, "接", "7c7c7c-303030", 0.9, out x, out y);
                    if ((int)x > 0)
                    {
                        dm.MoveTo((int)x + 2, (int)y + 2);
                        Thread.Sleep(50);
                        dm.LeftDown();
                        Thread.Sleep(1200);
                        dm.LeftUp();
                        Thread.Sleep(50);
                        dm.MoveTo((int)x + 3, (int)y + 1);
                        Thread.Sleep(50);
                        dm.LeftDown();
                        Thread.Sleep(2000);
                        dm.LeftUp();
                        Thread.Sleep(50);



                    }
                    dm.FindPic(315, 335, 465, 395, "acceptlowrace.bmp", "000000", 0.9, 0, out x, out y);
                    if ((int)x > 0)
                    {
                        dm.MoveTo((int)x + 3, (int)y + 3);
                        Thread.Sleep(50);
                        dm.LeftDown();
                        Thread.Sleep(500);
                        dm.LeftUp();
                        Thread.Sleep(50);
                        dm.MoveTo((int)x + 5, (int)y + 3);
                        Thread.Sleep(50);
                        dm.LeftDown();
                        Thread.Sleep(800);
                        dm.LeftUp();
                        Thread.Sleep(50);
                        dm.MoveTo((int)x + 1, (int)y + 2);
                        Thread.Sleep(50);
                        dm.LeftDown();
                        Thread.Sleep(1000);
                        dm.LeftUp();
                        Thread.Sleep(50);
                    }
                    // }
                    //关闭换人
                    dm.FindStr(560, 250, 610, 280, "取", "7c7c7c-404040", 0.9, out x, out y);
                    if ((int)x > 0)
                    {
                         dm.MoveTo(950, 525);
                         Thread.Sleep(50);
                         dm.LeftClick();
                         Thread.Sleep(4000);
                    }
                    //选人
                    dm.FindStr(25, 40, 75, 70, "天辉", "eaf1ea-808080", 0.8, out x, out y);  //代替allpick
                    if ((int)x > 0)
                    {
                        chooseHeroNum++;
                        if ("随机英雄" == chooseMode)
                        {

                            dm.FindStr(50, 535, 130, 575, "随机英雄", "f0ebeb-606060", 0.9, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x, (int)y);
                                Thread.Sleep(50);
                                dm.LeftDown();
                                Thread.Sleep(200);
                                dm.LeftUp();
                                Thread.Sleep(2000);
                            }
                        }
                        else
                        {
                            dm.FindStr(990, 510, 1024, 540, "网格视图切换", "777777-101010", 0.9, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.KeyPress(162);
                            }
                            for (i = 0; i < 7; i++)
                            {
                                dm.MoveTo(heroCoordinate[i,0], heroCoordinate[i,1]);
                                Thread.Sleep(500);
                                dm.LeftDown();
                                Thread.Sleep(200);
                                dm.LeftUp();
                                Thread.Sleep(200);
                                dm.MoveTo(515, 560);
                                Thread.Sleep(500);
                                dm.LeftClick();
                                Thread.Sleep(500);
                                chooseHeroNum++;
                            }
                            if (chooseHeroNum >= 28)
                            {
                                dm.FindStr(50, 535, 130, 575, "随机英雄", "f0ebeb-606060", 0.9, out x, out y);
                                if ((int)x > 0)
                                {
                                    dm.MoveTo((int)x, (int)y);
                                    Thread.Sleep(50);
                                    dm.LeftClick();
                                    Thread.Sleep(2000);
                                }
                                chooseHeroNum = 0;
                            }
                        }
                        //选人完后确定
                        Thread.Sleep(200);
                        dm.MoveTo(515, 560);
                        Thread.Sleep(500);
                        dm.LeftClick();
                        Thread.Sleep(500);
                        dm.MoveTo(510, 500);
                        Thread.Sleep(500);
                        dm.LeftClick();
                    }
                    //重连
                    dm.FindStr(385, 420, 500, 550, "重新连接", "7c7c7c-303030", 0.9, out x, out y);
                    if ((int)x > 0)
                    {
                        dm.MoveTo((int)x, (int)y);
                        Thread.Sleep(50);
                        dm.LeftClick();
                        Thread.Sleep(50);
                        if (12 == reconnectNum)
                        {
                            dm.MoveTo(450, 560);
                            Thread.Sleep(50);
                            dm.LeftClick();
                            Thread.Sleep(1500);
                            dm.FindPic(375, 265, 505, 295, "disconnect.bmp", "000000", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x, (int)y);
                                Thread.Sleep(50);
                                dm.LeftClick();
                                Thread.Sleep(1500);
                            }
                            reconnectNum = 0;
                        }
                        reconnectNum++;
                    }
                    //在重连按钮上的重连
                    dm.FindPic(400, 490, 500, 550, "inreconnect.bmp", "000000", 0.9, 0, out x, out y);
                    if ((int)x > 0)
                    {
                        dm.MoveTo((int)x, (int)y);
                        Thread.Sleep(50);
                        dm.LeftClick();
                        Thread.Sleep(50);
                    }
                    //关闭警告
                    dm.FindPic(475, 335, 545, 400, "warnclose.bmp", "000000", 0.9, 0, out x, out y);
                    if ((int)x > 0)
                    {
                        dm.MoveTo((int)x, (int)y);
                        Thread.Sleep(50);
                        dm.LeftClick();
                        Thread.Sleep(50);
                    }
                    dm.FindPic(545, 350, 610, 385, "cancel.bmp", "000000", 0.9, 0, out x, out y);
                    if ((int)x > 0)
                    {
                        dm.MoveTo((int)x, (int)y);
                        Thread.Sleep(50);
                        dm.LeftClick();
                        Thread.Sleep(50);
                    }
                }
                //在游戏外执行的操作
                //开始游戏内线程
                if (0 == threadIsRunning)
                {
                    dm.KeyPress(112);
                    Thread.Sleep(500);
                    dm.KeyPress(112);
                    Thread.Sleep(500);
                    dm.KeyPress(112);
                    dm.FindStr(80, 0, 145, 35, "help", "cbcbcb-999999", 0.9, out x, out y);
                    if ((int)x > 0)
                    {
//                     dm.FindPic(80, 0, 145, 35, "help.bmp", "000000", 0.9, 0, out x, out y);
//                     if ((int)x > 0)
//                     {
                        dm.FindPic(220, 630, 330, 760, "wait.bmp", "000000", 0.8, 0, out x, out y);
                        if ((int)x < 0)
                        {
                            dm.FindStr(905, 0, 970, 30, "KDA", "7c7c7c-303030", 0.8, out x, out y);
                            if ((int)x > 0)
                            {
                                //                             dm.FindPic(905, 0, 970, 30, "waitfive.bmp", "101010", 0.9, 0, out x, out y);
                                //                             if ((int)x > 0)
                                //                             {
                                //                                 dm.FindPic(905,10, 970, 35, "waitsix.bmp", "101010", 0.9, 0, out x, out y);
                                //                                 if ((int)x > 0)
                                //                                 {


                                //                             dm.FindPic(345, 700, 445, 765, "waittwo.bmp", "303030", 0.8, 0, out x, out y);
                                //                             if ((int)x > 0)
                                //                             {
                                //                                 dm.FindPic(345, 700, 445, 765, "waitthree.bmp", "303030", 0.8, 0, out x, out y);
                                //                                 if ((int)x > 0)
                                //                                 {
                                dm.FindStr(45, 0, 80, 30, "infrm", "cbcacb-353535", 0.9, out x, out y);
                                if ((int)x > 0)
                                {
//                                 dm.FindPic(45, 0, 80, 30, "information.bmp", "000000", 0.9, 0, out x, out y);
//                                 if ((int)x > 0)
//                                 {
                                    Thread thread = new Thread(() => thread_ingame(dm, hwnd));
                                    threadList.Add(thread);
                                    Thread_List.Add(thread);
                                    thread.Start();
                                    threadIsRunning = 1;
                                    allInning++;
                                }
                                //                                 }
                                //                             }
                                //}
                            }
                        }
                    }
                }
                string pictrueList = dm.FindPicEx(370, 220, 685, 395, "sure.bmp", "202020", 0.9, 0);
                if (pictrueList.Length > 0)
                {
                    string[] pictrueList_Split = pictrueList.Split('|');
                    foreach (string pic in pictrueList_Split)
                    {
                        x = int.Parse(pic.Split(',')[1]);
                        y = int.Parse(pic.Split(',')[2]);
                        dm.MoveTo((int)x + 3, (int)y + 3);
                        Thread.Sleep(50);
                        dm.LeftDown();
                        Thread.Sleep(500);
                        dm.LeftUp();
                        Thread.Sleep(50);
                        dm.MoveTo((int)x + 5, (int)y + 3);
                        Thread.Sleep(50);
                        dm.LeftDown();
                        Thread.Sleep(800);
                        dm.LeftUp();
                        Thread.Sleep(50);
                        dm.MoveTo((int)x + 1, (int)y + 2);
                        Thread.Sleep(50);
                        dm.LeftDown();
                        Thread.Sleep(1000);
                        dm.LeftUp();
                        Thread.Sleep(50);
                    }
                }
                dm.FindStr(740, 470, 805, 505, "关", "7c7c7c-303030", 0.9, out x, out y);
                if ((int)x > 0)
                {
                    dm.MoveTo((int)x + 3, (int)y + 3);
                    Thread.Sleep(50);
                    dm.LeftDown();
                    Thread.Sleep(500);
                    dm.LeftUp();
                    Thread.Sleep(50);
                    dm.MoveTo((int)x + 5, (int)y + 3);
                    Thread.Sleep(50);
                    dm.LeftDown();
                    Thread.Sleep(1000);
                    dm.LeftUp();
                    Thread.Sleep(50);
                }
                dm.FindStr(735, 120, 790, 190, "X", "e3e4e3-454545", 0.9, out x, out y);
                if((int)x > 0)
                {
                    dm.MoveTo((int)x + 1, (int)y + 1);
                    Thread.Sleep(50);
                    dm.LeftDown();
                    Thread.Sleep(500);
                    dm.LeftUp();
                    Thread.Sleep(50);
                }
            }
        }
        #endregion

        #region 游戏内操作

        private void thread_ingame(CDmSoft dm, int hwnd)
        {
            object x, x1,y;
            int i;
            int isRediantNum,isDirtNum;
            int isRediant,isRediantTwo,isRediantThree,isRediantFour,isRediantFive,isRediantSix ;
            int isDirt,isDirtTwo,isDirtThree,isDirtFour,isDirtFive,isDirtSix ;
            int gather, hasGather,hasGatherTwo, banEscape, back, gameDet, adjust, hpJudge, camp, escape, haveFstEqm, towerNoExt, noDangureHp, hpNoFull,noSkill;
            double timeDbRet,waitTimeDbRet;
            string route,winClass;
            int [] haveEqm = new int[9] ;
            DateTime now ,tpTimeOne,tpTimeTwo,changeRouteTime,waitTimeOne,waitTimeTwo;
            TimeSpan timeRet, waitTimeRet;
            changeRouteTime = System.DateTime.Now;
            tpTimeOne       = System.DateTime.Now;
            i               = 0; 
            camp            = 0;
            gameDet         = 1;
            gather          = 0;
            hasGather       = 0;
            hasGatherTwo    = 0;
            banEscape       = 0;
            back            = 0;
            haveFstEqm      = 0;
            adjust          = 0;
            escape          = 0;
            route           = "";
            winClass        = dm.GetWindowClass(hwnd);

            dm.Capture(250,752,293,763, "exp.bmp");

            if (1 == banSkillBeforeGather)
                noSkill = 1;
            else
                noSkill = 0;
            if (1 == disRoute)
            {
                if ("Sandbox:D1:Valve001" == winClass)
                    route = chooseRoute[0];
                if ("Sandbox:D2:Valve001" == winClass)
                    route = chooseRoute[1];
                if ("Sandbox:D3:Valve001" == winClass)
                    route = chooseRoute[2];
                if ("Sandbox:D4:Valve001" == winClass)
                    route = chooseRoute[3];
                if ("Sandbox:D5:Valve001" == winClass)
                    route = chooseRoute[4];
                if ("Sandbox:D6:Valve001" == winClass)
                    route = chooseRoute[5];
                if ("Sandbox:D7:Valve001" == winClass)
                    route = chooseRoute[6];
                if ("Sandbox:D8:Valve001" == winClass)
                    route = chooseRoute[7];
                if ("Sandbox:D9:Valve001" == winClass)
                    route = chooseRoute[8];
                if ("Sandbox:D10:Valve001" == winClass)
                    route = chooseRoute[9];
            }
            else
                route = allRoute;
            while (true)
            {
                if(1 == gameDet)
                {
                    changeRouteTime = System.DateTime.Now ; 
                    isRediantNum    = 0 ;
                    isDirtNum       = 0 ;
                    
                    isRediant       = dm.CmpColor(175,605,"CC0000-000000",0.9) ;
                    isRediantTwo    = dm.CmpColor(170,605,"510200-000000",0.9) ;
                    isRediantThree  = dm.CmpColor(172,610,"FF0000-000000",0.9) ;
                    isRediantFour   = dm.CmpColor(35,735,"00A100-000000",0.9) ;
                    isRediantFive   = dm.CmpColor(40,735,"00F600-000000",0.9) ;
                    isRediantSix    = dm.CmpColor(35,730,"00FE00-000000",0.9) ;

                    isDirt      = dm.CmpColor(175,605,"00CA00-000000",0.9) ;
                    isDirtTwo   = dm.CmpColor(170,605,"01FB01-000000",0.9) ;
                    isDirtThree = dm.CmpColor(172,610,"00FE00-000000",0.9) ;
                    isDirtFour  = dm.CmpColor(35,735,"A10000-000000",0.9) ;
                    isDirtFive  = dm.CmpColor(40,735,"F60000-000000",0.9) ;
                    isDirtSix   = dm.CmpColor(35,730,"FE0000-000000",0.9) ;
                    if (0 == isRediant)
                        isRediantNum++ ;
                    if (0 == isRediantTwo)
                        isRediantNum++ ;
                    if (0 == isRediantThree)
                        isRediantNum++ ;
                    if (0 == isRediantFour)
                        isRediantNum++ ;
                    if (0 == isRediantFive)
                        isRediantNum++ ;
                    if (0 == isRediantSix)
                        isRediantNum++ ;
                    if (0 == isDirt)
                        isDirtNum++ ;
                    if (0 == isDirtTwo)
                        isDirtNum++ ;
                    if (0 == isDirtThree)
                        isDirtNum++ ;
                    if (0 == isDirtFour)
                        isDirtNum++ ;
                    if (0 == isDirtFive)
                        isDirtNum++ ;
                    if (0 == isDirtSix)
                        isDirtNum++ ;
                    if (isRediantNum > isDirtNum)
                    {
                        camp    = 1 ;
                        gameDet = 0 ;
                    }
                    else
                    {
                        camp    = 2 ;
                        gameDet = 0 ;
                    }
                    dm.FindPic( 250, 750, 290, 765,"lvone.bmp","000000",0.9,0,out x ,out y ) ;
                    if ((int)x > 0)
                        Thread.Sleep(50000) ;
                    //禁用帮助
                    dm.MoveTo(110,15);
                    Thread.Sleep(200);
                    dm.LeftClick();
                    Thread.Sleep(1000);

                    dm.MoveTo(373,100);
                    Thread.Sleep(200);
                    dm.LeftClick();
                    Thread.Sleep(200);
                    dm.MoveTo(373,133);
                    Thread.Sleep(200);
                    dm.LeftClick();
                    Thread.Sleep(200);
                    dm.MoveTo(373,166);
                    Thread.Sleep(200);
                    dm.LeftClick();
                    Thread.Sleep(200);
                    dm.MoveTo(373,199);
                    Thread.Sleep(200);
                    dm.LeftClick();
                    Thread.Sleep(200);

                    dm.MoveTo(110, 15);
                    Thread.Sleep(200);
                    dm.LeftClick();
                    Thread.Sleep(1000);
                    //无经验时间
                }
                //更变路线
                now = System.DateTime.Now;

                timeRet = now - changeRouteTime;
                timeDbRet = timeRet.TotalMinutes;
                if (timeDbRet > gatTime && 0 == hasGather)
                {
                    route = "中路";
                    gather = 1;
                    hasGather = 1;
                    noSkill = 0;
                    if(0 == escapeMode)
                    {
                        banEscape = 1;
                    }
                }
                //TP功能
                tpTimeTwo = System.DateTime.Now;
                timeRet = tpTimeTwo - tpTimeOne;
                timeDbRet = timeRet.TotalMinutes;
                if(timeDbRet > 3)
                {
                    dm.FindPic(245, 745, 300, 770, "exp.bmp", "303030", 0.9, 0, out x, out y);
                    if((int)x > 0)
                    {
                        dm.FindPic(775, 650, 965, 770, "tpinkna.bmp", "303030", 0.9, 0, out x, out y);
                        if ((int)x > 0)
                        {
                            dm.MoveTo((int)x + 2,(int)y + 2);
                            Thread.Sleep(500);
                            dm.LeftClick();
                            Thread.Sleep(500);
                            if(1 == camp)
                            {
                                dm.MoveTo(15, 750);
                                Thread.Sleep(500);
                                dm.LeftClick();
                                Thread.Sleep(6000);
                            }
                            else
                            {
                                dm.MoveTo(195, 590);
                                Thread.Sleep(500);
                                dm.LeftClick();
                                Thread.Sleep(6000);
                            }

                        }
                        
                    }
                    tpTimeOne = System.DateTime.Now;
                    dm.Capture(250, 752, 293, 763, "exp.bmp");
                }
//                 if (timeDbRet > gatTime * 2 && 0 == hasGatherTwo)
//                 {
//                     route = "中路";
//                     gather = 1;
//                     hasGatherTwo = 1;
//                 }
                //更变路线
                dm.KeyPress(112) ;
                Thread.Sleep(50);
                dm.KeyPress(112);
                Thread.Sleep(50) ;
                dm.KeyDown(112);
                if (timeDbRet < gatTime)
                    hpJudge = dm.CmpColor(630, 650, "101210-000000", 0.9);
                else
                    hpJudge = dm.CmpColor(530, 650, "101210-000000", 0.9);
                if (0 == hpJudge) //假如血不满0代表找到该点颜色
                {
                    if (1 == camp)
                    {
                        dm.MoveTo(20, 755);
                        Thread.Sleep(20);
                        dm.RightClick();
                        Thread.Sleep(20);
                        hpJudge = dm.CmpColor(765, 649, "236A23-000000", 0.9);
                        while (1 == hpJudge)
                        {
                            if (timeDbRet > gatTime && 0 == hasGather)
                                break;
                            dm.MoveTo(20, 755);
                            Thread.Sleep(20);
                            dm.RightClick();
                            Thread.Sleep(20);
                            hpJudge = dm.CmpColor(765, 649, "236A23-000000", 0.9);
                        }
                    }
                    else
                    {
                        dm.MoveTo(195, 590);
                        Thread.Sleep(20);
                        dm.RightClick();
                        Thread.Sleep(20);
                        hpJudge = dm.CmpColor(765, 649, "236A23-000000", 0.9);
                        while (1 == hpJudge)
                        {
                            if (timeDbRet > gatTime && 0 == hasGather)
                                break;
                            dm.MoveTo(195, 590);
                            Thread.Sleep(20);
                            dm.RightClick();
                            Thread.Sleep(20);
                            hpJudge = dm.CmpColor(765, 649, "236A23-000000", 0.9);
                        }
                    }
                }
                //防御塔砸的后侧撤
                hpNoFull = dm.CmpColor(765, 649, "236A23-000000", 0.9);
                if (1 == hpNoFull)
                {
                    noDangureHp = dm.CmpColor(665, 650, "101210-000000", 0.9);
                    if (0 == noDangureHp && 1 == escape)
                    {
                        if (1 == camp)
                        {
                            dm.MoveTo(15, 750);
                            Thread.Sleep(20);
                            dm.RightClick();
                            Thread.Sleep(3000);
                            dm.KeyPress(83);
                        }
                        else
                        {
                            dm.MoveTo(193, 590);
                            Thread.Sleep(20);
                            dm.RightClick();
                            Thread.Sleep(3000);
                            dm.KeyPress(83);
                        }
                        escape = 0;
                    }
                }
                //避免被击杀的后撤
                if (0 == banEscape)
                {
                    dm.FindPic(10, 45, 990, 600, "bothp.bmp", "000000", 1, 0, out x, out y);
                    dm.FindPic(10, 45, 990, 600, "minionshp.bmp", "000000", 1, 0, out x1, out y);
                    if ((int)x > 0 || (int)x1 > 0)
                    {
                        if (1 == camp)
                        {
                            dm.MoveTo(15, 750);
                            Thread.Sleep(20);
                            dm.RightClick();
                            Thread.Sleep(2500);
                            dm.KeyPress(83);
                        }
                        else
                        {
                            dm.MoveTo(193, 590);
                            Thread.Sleep(20);
                            dm.RightClick();
                            Thread.Sleep(2500);
                            dm.KeyPress(83);
                        }
                    }
                }
                //买装备
                dm.FindPic(820,630,900,665,"inshop.bmp","303030",0.9,0,out x ,out y ) ;
                if ((int)x > 0)
                {
                    dm.KeyPress(83);
                    Thread.Sleep(50);
                    if (1 == waitMemberRevive && 1 == hasGather)
                    {
                        if (1 == camp)
                        {
                            waitTimeOne = now;
                            dm.FindPic(195, 35, 450, 55, "die.bmp", "151515", 0.9, 0, out x, out y);
                            while ((int)x > 0)
                            {
                                dm.FindPic(195, 35, 450, 55, "die.bmp", "151515", 0.9, 0, out x, out y);
                                Thread.Sleep(200);
                                waitTimeTwo     = now;
                                waitTimeRet     = waitTimeTwo - waitTimeTwo;
                                waitTimeDbRet   = waitTimeRet.TotalSeconds;
                                if (waitTimeDbRet > 120)
                                    break;
                            }
                        }
                        else
                        {
                            waitTimeOne = now;
                            dm.FindPic(570, 35, 830, 55, "die.bmp", "151515", 0.9, 0, out x, out y);
                            while ((int)x > 0)
                            {
                                dm.FindPic(570, 35, 830, 55, "die.bmp", "151515", 0.9, 0, out x, out y);
                                Thread.Sleep(200);
                                waitTimeTwo = now;
                                waitTimeRet = waitTimeTwo - waitTimeTwo;
                                waitTimeDbRet = waitTimeRet.TotalSeconds;
                                if (waitTimeDbRet > 120)
                                    break;
                            }
                        }
                    }
                    if (1 == inviteTeammate && 0 == haveFstEqm)
                    {
                        dm.FindPic(775, 650, 965, 770, "shoesinkna.bmp", "303030", 0.8, 0, out x, out y);
                        if ((int)x < 0)
                        {
                            dm.FindPic(820, 630, 900, 665, "inshop.bmp", "000000", 0.8, 0, out x, out y);
                            Thread.Sleep(50);
                            dm.MoveTo((int)x, (int)y);
                            Thread.Sleep(50);
                            dm.LeftClick();
                            Thread.Sleep(1500);
                            dm.MoveTo(810, 95);
                            Thread.Sleep(50);
                            dm.LeftClick();
                            Thread.Sleep(500);
                            dm.MoveTo(988, 258);
                            Thread.Sleep(50);
                            dm.RightClick();
                            Thread.Sleep(50);

                        }
                        haveFstEqm = 1;
                    }
                    dm.FindPic(820, 630, 900, 665, "inshop.bmp", "000000", 0.8, 0, out x, out y);
                    Thread.Sleep(50);
                    dm.MoveTo((int)x, (int)y);
                    Thread.Sleep(50);
                    dm.LeftClick();
                    Thread.Sleep(2000);
                    dm.MoveTo(940, 95);
                    Thread.Sleep(50);
                    dm.LeftClick();
                    Thread.Sleep(2000);


                    dm.MoveTo(1002,48);   //选对正确位置才能购买
                    Thread.Sleep(50);
                    dm.LeftClick();
                    Thread.Sleep(1000);
                    dm.FindPic(775, 650, 965, 770, "tpinkna.bmp", "303030", 0.9, 0, out x, out y);
                    if ((int)x < 0)
                    {
                        dm.MoveTo(999,626);
                        Thread.Sleep(50);
                        dm.RightClick();
                    }
                    if (0 == haveEqm[0])
                    {
                        dm.FindPic(775, 650, 965, 770, eqmExt[0], "303030", 0.7, 0, out x, out y);
                        if ((int)x < 0)
                        {
                            dm.FindPic(780, 150, 1024, 510, eqm[0], "454545", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x, (int)y);
                                Thread.Sleep(50);
                                dm.RightClick();
                            }
                            Thread.Sleep(1000);
                            dm.FindPic(775, 650, 965, 770, eqmExt[0], "303030", 0.7, 0, out x, out y);
                            if ((int)x > 0)
                                haveEqm[0] = 1;

                        }
                    }
                    if (0 == haveEqm[1] && 1 == haveEqm[0])
                    {
                        dm.FindPic(775, 650, 980, 770, eqmExt[1], "303030", 0.7, 0, out x, out y);
                        if ((int)x < 0)
                        {
                            dm.FindPic(775, 150, 1024, 510, eqm[1], "454545", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x, (int)y);
                                Thread.Sleep(50);
                                dm.RightClick();
                            }
                            Thread.Sleep(1000);
                            dm.FindPic(775, 650, 965, 770, eqmExt[1], "303030", 0.7, 0, out x, out y);
                            if ((int)x > 0)
                                haveEqm[1] = 1;
                        }
                    }
                    if (0 == haveEqm[2] && 1 == haveEqm[1])
                    {
                        dm.FindPic(775, 650, 980, 770, eqmExt[2], "303030", 0.7, 0, out x, out y);
                        if ((int)x < 0)
                        {
                            dm.FindPic(775, 150, 1024, 510, eqm[2], "454545", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x, (int)y);
                                Thread.Sleep(50);
                                dm.RightClick();
                            }
                            Thread.Sleep(1000);
                            dm.FindPic(775, 650, 965, 770, eqmExt[2], "303030", 0.7, 0, out x, out y);
                            if ((int)x > 0)
                                haveEqm[2] = 1;
                        }
                    }
                    if (0 == haveEqm[3] && 1 == haveEqm[2])
                    {
                        dm.FindPic(775, 650, 980, 770, eqmExt[3], "303030", 0.7, 0, out x, out y);
                        if ((int)x < 0)
                        {
                            dm.FindPic(775, 150, 1024, 510, eqm[3], "454545", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x, (int)y);
                                Thread.Sleep(50);
                                dm.RightClick();
                            }
                            Thread.Sleep(1000);
                            dm.FindPic(775, 650, 965, 770, eqmExt[3], "303030", 0.7, 0, out x, out y);
                            if ((int)x > 0)
                                haveEqm[3] = 1;
                        }
                    }
                    if (0 == haveEqm[4] && 1 == haveEqm[3])
                    {
                        dm.FindPic(775, 650, 980, 770, eqmExt[4], "303030", 0.7, 0, out x, out y);
                        if ((int)x < 0)
                        {
                            dm.FindPic(775, 150, 1024, 510, eqm[4], "454545", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x, (int)y);
                                Thread.Sleep(50);
                                dm.RightClick();
                            }
                            Thread.Sleep(1000);
                            dm.FindPic(775, 650, 965, 770, eqmExt[4], "303030", 0.7, 0, out x, out y);
                            if ((int)x > 0)
                                haveEqm[4] = 1;
                        }
                    }
                    if (0 == haveEqm[5] && 1 == haveEqm[4])
                    {
                        dm.FindPic(780, 650, 980, 770, eqmExt[5], "303030", 0.7, 0, out x, out y);
                        if ((int)x < 0)
                        {
                            dm.FindPic(780, 150, 1024, 510, eqm[5], "454545", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x, (int)y);
                                Thread.Sleep(50);
                                dm.RightClick();
                            }
                            Thread.Sleep(1000);
                            dm.FindPic(775, 650, 965, 770, eqmExt[5], "303030", 0.7, 0, out x, out y);
                            if ((int)x > 0)
                                haveEqm[5] = 1;
                        }
                    }
                    if (0 == haveEqm[6] && 1 == haveEqm[5])
                    {
                        dm.FindPic(780, 650, 980, 770, eqmExt[6], "303030", 0.7, 0, out x, out y);
                        if ((int)x < 0)
                        {
                            dm.FindPic(780, 150, 1024, 510, eqm[6], "454545", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x, (int)y);
                                Thread.Sleep(50);
                                dm.RightClick();
                            }
                            Thread.Sleep(1000);
                            dm.FindPic(775, 650, 965, 770, eqmExt[6], "303030", 0.7, 0, out x, out y);
                            if ((int)x > 0)
                                haveEqm[6] = 1;
                        }
                    }
                    if (0 == haveEqm[7] && 1 == haveEqm[6])
                    {
                        dm.FindPic(780, 650, 980, 770, eqmExt[7], "303030", 0.7, 0, out x, out y);
                        if ((int)x < 0)
                        {
                            dm.FindPic(780, 150, 1024, 510, eqm[7], "454545", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x, (int)y);
                                Thread.Sleep(50);
                                dm.RightClick();
                            }
                            Thread.Sleep(1000);
                            dm.FindPic(775, 650, 965, 770, eqmExt[7], "303030", 0.7, 0, out x, out y);
                            if ((int)x > 0)
                                haveEqm[7] = 1;
                        }
                    }
                    if (0 == haveEqm[8] && 1 == haveEqm[7])
                    {
                        dm.FindPic(780, 650, 980, 770, eqmExt[8], "303030", 0.7, 0, out x, out y);
                        if ((int)x < 0)
                        {
                            dm.FindPic(780, 150, 1024, 510, eqm[8], "454545", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x, (int)y);
                                Thread.Sleep(50);
                                dm.RightClick();
                            }
                            Thread.Sleep(1000);
                            dm.FindPic(775, 650, 965, 770, eqmExt[8], "303030", 0.7, 0, out x, out y);
                            if ((int)x > 0)
                                haveEqm[8] = 1;
                        }
                    }

                    //买完东西出门
                    dm.MoveTo(860, 640);
                    Thread.Sleep(50);
                    dm.LeftClick();
                    if (1 == camp)
                    {
                        if ("上路" == route)
                        {
                            dm.MoveTo(25, 690);
                            Thread.Sleep(200);
                            dm.KeyPress(65);
                            Thread.Sleep(200);
                            dm.LeftClick();
                            Thread.Sleep(10000);
                        }
                        else if ("中路" == route)
                        {
                            dm.MoveTo(58, 715);
                            Thread.Sleep(200);
                            dm.KeyPress(65);
                            Thread.Sleep(200);
                            dm.LeftClick();
                            Thread.Sleep(10000);
                        }
                        else if ("下路" == route)
                        {
                            dm.MoveTo(70, 750);
                            Thread.Sleep(200);
                            dm.KeyPress(65);
                            Thread.Sleep(200);
                            dm.LeftClick();
                            Thread.Sleep(10000);
                        }
                        else
                        {
                            dm.MoveTo(70, 750);
                            Thread.Sleep(200);
                            dm.KeyPress(65);
                            Thread.Sleep(200);
                            dm.LeftClick();
                            Thread.Sleep(10000);
                        }
                    }
                    else
                    {
                        if ("上路" == route)
                        {
                            dm.MoveTo(138, 595);
                            Thread.Sleep(200);
                            dm.KeyPress(65);
                            Thread.Sleep(200);
                            dm.LeftClick();
                            Thread.Sleep(10000);
                        }
                        else if ("中路" == route)
                        {
                            dm.MoveTo(155, 620);
                            Thread.Sleep(200);
                            dm.KeyPress(65);
                            Thread.Sleep(200);
                            dm.LeftClick();
                            Thread.Sleep(10000);
                        }
                        else if ("下路" == route)
                        {
                            dm.MoveTo(185, 645);
                            Thread.Sleep(200);
                            dm.KeyPress(65);
                            Thread.Sleep(200);
                            dm.LeftClick();
                            Thread.Sleep(10000);
                        }
                        else
                        {
                            dm.MoveTo(138, 595);
                            Thread.Sleep(200);
                            dm.KeyPress(65);
                            Thread.Sleep(200);
                            dm.LeftClick();
                            Thread.Sleep(10000);
                        }
                    }
                }
                //升级
                dm.FindPic(235,575,295,605,"lvup.bmp","000000",0.8,0,out x ,out y ) ;
                if((int)x > 0)
                {
                    dm.KeyDown(162) ;
                    for(i = 0; i < skillSequence.Length;i++)
                    {
                        switch(skillSequence[i])
                        {
                            case 'r':
                                dm.KeyPress(82);
                                break;
                            case 'q':
                                dm.KeyPress(81);
                                break;
                            case 'w': ;
                                dm.KeyPress(87);
                                break;
                            case 'e': ;
                                dm.KeyPress(69);
                                break;
                            case 'R':
                                dm.KeyPress(82);
                                break;
                            case 'Q':
                                dm.KeyPress(81);
                                break;
                            case 'W': ;
                                dm.KeyPress(87);
                                break;
                            case 'E': ;
                                dm.KeyPress(69);
                                break;
                        }   
                    }
                    dm.KeyUp(162) ;
                    //黄点
                    Thread.Sleep(20);
                    dm.KeyPress(27);
                    Thread.Sleep(20);
                    dm.KeyPress(79);
                    Thread.Sleep(20);
                    dm.KeyPress(85);                    
                }
                //判断攻击
                if (1 == camp)
                {
                    if("上路" == route)
                    {
                        //一塔
                        towerNoExt = dm.CmpColor(49,590,"FF0000-641010",0.9) ;
                        if (0 == towerNoExt)
                        {
                            dm.MoveTo(30,600) ;
                            Thread.Sleep(20);
                            dm.KeyPress(65) ;
                            Thread.Sleep(20);
                            dm.LeftClick();
                            Thread.Sleep(20);
                        }
                        else
                        {
                            //二塔
                            towerNoExt = dm.CmpColor(106,590,"FF0000-641010",0.9) ;
                            if (0 == towerNoExt)
                            {
                                dm.KeyDown(16) ;
                                Thread.Sleep(20);
                                dm.MoveTo(35,595) ;
                                Thread.Sleep(20);
                                dm.KeyPress(65) ;
                                Thread.Sleep(20);
                                dm.LeftClick() ;
                                Thread.Sleep(20);
                                dm.MoveTo(93,590) ;
                                Thread.Sleep(20);
                                dm.KeyPress(65) ;
                                Thread.Sleep(20);
                                dm.LeftClick() ;
                                Thread.Sleep(20);
                                dm.KeyUp(16) ;
                            }
                            else
                            {
                                //三塔
                                towerNoExt = dm.CmpColor(160,620,"FF0000-641010",0.9) ;
                                if (0 == towerNoExt)
                                {
                                    dm.KeyDown(16) ;
                                    Thread.Sleep(20);
                                    dm.MoveTo(35,595) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.MoveTo(138,595) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                    Thread.Sleep(20);
                                    dm.KeyUp(16) ;
                                }
                                else
                                {
                                    //四塔
                                       dm.KeyDown(16) ;
                                        Thread.Sleep(20);
                                        dm.MoveTo(35,595) ;
                                        Thread.Sleep(20);
                                        dm.KeyPress(65) ;
                                        Thread.Sleep(20);
                                        dm.LeftClick() ;
                                        Thread.Sleep(20);
                                        dm.MoveTo(138,595) ;
                                        Thread.Sleep(20);
                                        dm.KeyPress(65) ;
                                        Thread.Sleep(20);
                                        dm.LeftClick() ;
                                        Thread.Sleep(20);
                                        dm.MoveTo(175,605) ;
                                        Thread.Sleep(20);
                                        dm.KeyPress(65) ;
                                        Thread.Sleep(20);
                                        dm.LeftClick() ;
                                        Thread.Sleep(20);
                                        dm.KeyUp(16) ;
                                }
                            }
                        }
                    }
                    else if("中路" == route)
                    {
                        //一塔
                        towerNoExt = dm.CmpColor(119,663,"FF0000-641010",0.9) ;
                        if(0 == towerNoExt)
                        {
                            dm.MoveTo(125,660) ;
                            Thread.Sleep(20);
                            dm.KeyPress(65) ;
                            Thread.Sleep(20);
                            dm.LeftClick() ;
                            Thread.Sleep(20);
                        }
                        else
                        {
                            //二塔
                            towerNoExt = dm.CmpColor(138,640,"FF0000-641010",0.9) ;
                            if(0 == towerNoExt)
                            {
                                dm.MoveTo(135,640) ;
                                Thread.Sleep(20);
                                dm.KeyPress(65) ;
                                Thread.Sleep(20);
                                dm.LeftClick() ;
                                
                            }
                            else
                            {
                                //三塔
                                towerNoExt = dm.CmpColor(160, 620, "FF0000-641010", 0.9);
                                if(0 == towerNoExt)
                                {
                                    dm.MoveTo(155,625) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                }
                                else
                                {
                                    dm.MoveTo(175,610) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                }
                            }
                        }
                    }
                    else if("下路" == route)
                    {
                        //一塔
                        towerNoExt = dm.CmpColor(184,688,"FF0000-641010",0.9) ;
                         if (0 == towerNoExt)
                         {
                             dm.KeyDown(16) ;
                             Thread.Sleep(20);
                             dm.MoveTo(180,740) ;
                             Thread.Sleep(20);
                             dm.KeyPress(65) ;
                             Thread.Sleep(20);
                             dm.LeftClick() ;
                             Thread.Sleep(20);
                             dm.MoveTo(185,705) ;
                             Thread.Sleep(20);
                             dm.KeyPress(65) ;
                             Thread.Sleep(20);
                             dm.LeftClick() ;
                             Thread.Sleep(20);
                             dm.KeyUp(16) ;
                         }
                         else
                         {
                             //二塔
                             towerNoExt = dm.CmpColor(185,664,"FF0000-641010",0.9) ;
                             if (0 == towerNoExt)
                             {
                                dm.KeyDown(16) ;
                                Thread.Sleep(20);
                                dm.MoveTo(180,740) ;
                                Thread.Sleep(20);
                                dm.KeyPress(65) ;
                                Thread.Sleep(20);
                                dm.LeftClick() ;
                                Thread.Sleep(20);
                                dm.MoveTo(185,680) ;
                                Thread.Sleep(20);
                                dm.KeyPress(65) ;
                                Thread.Sleep(20);
                                dm.LeftClick() ;
                                Thread.Sleep(20);
                                dm.KeyUp(16) ;
                               } 
                             else
                             {
                                //三塔
                                towerNoExt = dm.CmpColor(186,629,"FF0000-641010",0.9) ;
                                if (0 == towerNoExt)
                                {
                                    dm.KeyDown(16) ;
                                    Thread.Sleep(20);
                                    dm.MoveTo(180,740) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                    Thread.Sleep(20);
                                    dm.MoveTo(185,645) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                    Thread.Sleep(20);
                                    dm.KeyUp(16) ;
                                }
                                else
                                {
                                    //四塔
                                    dm.KeyDown(16) ;
                                    Thread.Sleep(20);
                                    dm.MoveTo(180,740) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                    Thread.Sleep(20);
                                    dm.MoveTo(185,645) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                    Thread.Sleep(20);
                                    dm.MoveTo(175,605) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                    Thread.Sleep(20);
                                    dm.KeyUp(16) ;
                                }
                             }
                             
                         }
                    }
                    else if ("打野" == route)
                    {
                        dm.KeyDown(16) ;
                        Thread.Sleep(20);
                        dm.MoveTo(145,727) ;
                        Thread.Sleep(20);
                        dm.KeyPress(65) ;
                        Thread.Sleep(20);
                        dm.LeftClick() ;
                        Thread.Sleep(20);
                        dm.MoveTo(103,707) ;
                        Thread.Sleep(20);
                        dm.KeyPress(65) ;
                        Thread.Sleep(20);
                        dm.LeftClick() ;
                        Thread.Sleep(20);
                        dm.KeyUp(16) ;
                    }
                }
                else
                {
                    //dirt阵营
                    if("上路" == route)
                    {
                        //一塔
                        towerNoExt = dm.CmpColor(31,643,"FF0000-641010",0.9) ;
                        if (0 == towerNoExt)
                        {
                            dm.KeyDown(16) ;
                            Thread.Sleep(20);
                            dm.MoveTo(35,595) ;
                            Thread.Sleep(20);
                            dm.KeyPress(65) ;
                            Thread.Sleep(20);
                            dm.LeftClick() ;
                            Thread.Sleep(20);
                            dm.MoveTo(30,630) ;
                            Thread.Sleep(20);
                            dm.KeyPress(65) ;
                            Thread.Sleep(20);
                            dm.LeftClick() ;
                            Thread.Sleep(20);
                            dm.KeyUp(16) ;
                        }
                        else
                        {
                            //二塔
                            towerNoExt = dm.CmpColor(30,678,"FF0000-641010",0.9) ;
                            if (0 == towerNoExt)
                            {
                                dm.KeyDown(16) ;
                                Thread.Sleep(20);
                                dm.MoveTo(35,595) ;
                                Thread.Sleep(20);
                                dm.KeyPress(65) ;
                                Thread.Sleep(20);
                                dm.LeftClick() ;
                                Thread.Sleep(20);
                                dm.MoveTo(30,666) ;
                                Thread.Sleep(20);
                                dm.KeyPress(65) ;
                                Thread.Sleep(20);
                                dm.LeftClick() ;
                                Thread.Sleep(20);
                                dm.KeyUp(16) ;
                            }
                            else
                            {
                                //三塔
                                towerNoExt = dm.CmpColor(23,710,"FF0000-641010",0.9) ;
                                if (0 == towerNoExt)
                                {
                                    dm.KeyDown(16) ;
                                    Thread.Sleep(20);
                                    dm.MoveTo(35,595) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                    Thread.Sleep(20);
                                    dm.MoveTo(25,690) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                    Thread.Sleep(20);
                                    dm.KeyUp(16) ;
                                }
                                else
                                {
                                    //四塔
                                       dm.KeyDown(16) ;
                                        Thread.Sleep(20);
                                        dm.MoveTo(35,595) ;
                                        Thread.Sleep(20);
                                        dm.KeyPress(65) ;
                                        Thread.Sleep(20);
                                        dm.LeftClick() ;
                                        Thread.Sleep(20);
                                        dm.MoveTo(25,690) ;
                                        Thread.Sleep(20);
                                        dm.KeyPress(65) ;
                                        Thread.Sleep(20);
                                        dm.LeftClick() ;
                                        Thread.Sleep(20);
                                        dm.MoveTo(30,735) ;
                                        Thread.Sleep(20);
                                        dm.KeyPress(65) ;
                                        Thread.Sleep(20);
                                        dm.LeftClick() ;
                                        Thread.Sleep(20);
                                        dm.KeyUp(16) ;
                                }
                            }
                        }
                    }
                    else if("中路" == route)
                    {
                        //一塔
                        towerNoExt = dm.CmpColor(88, 685, "FF0000-641010", 0.9);
                        if (0 == towerNoExt)
                        {
                            dm.MoveTo(90, 685) ;
                            Thread.Sleep(20);
                            dm.KeyPress(65) ;
                            Thread.Sleep(20);
                            dm.LeftClick() ;
                        }
                        else
                        {
                            //二塔
                            towerNoExt = dm.CmpColor(63, 703, "FF0000-641010", 0.9);
                            if (0 == towerNoExt)
                            {
                                dm.MoveTo(65,700) ;
                                Thread.Sleep(20);
                                dm.KeyPress(65) ;
                                Thread.Sleep(20);
                                dm.LeftClick() ;
                            }
                            else
                            {
                                //三塔
                                towerNoExt = dm.CmpColor(50,720,"FF0000-641010",0.9) ;
                                if(0 == towerNoExt)
                                {
                                    dm.MoveTo(50,725) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                    Thread.Sleep(20);
                                }
                                else
                                {
                                    dm.MoveTo(35,735) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                }
                            }
                        }
                    }
                    else if("下路" == route)
                    {
                        //一塔
                        towerNoExt = dm.CmpColor(168,745,"FF0000-641010",0.9) ;
                         if (0 == towerNoExt)
                         {
                             dm.MoveTo(185,735) ;
                             Thread.Sleep(20);
                             dm.KeyPress(65) ;
                             Thread.Sleep(20);
                             dm.LeftClick() ;
                         }
                         else
                         {
                             //二塔
                             towerNoExt = dm.CmpColor(100,745,"FF0000-641010",0.9) ;
                             if (0 == towerNoExt)
                             {
                                dm.KeyDown(16) ;
                                Thread.Sleep(20);
                                dm.MoveTo(180,740) ;
                                Thread.Sleep(20);
                                dm.KeyPress(65) ;
                                Thread.Sleep(20);
                                dm.LeftClick() ;
                                Thread.Sleep(20);
                                dm.MoveTo(115,750) ;
                                Thread.Sleep(20);
                                dm.KeyPress(65) ;
                                Thread.Sleep(20);
                                dm.LeftClick() ;
                                Thread.Sleep(20);
                                dm.KeyUp(16) ;
                               } 
                             else
                             {
                                //三塔
                                towerNoExt = dm.CmpColor(60,746,"FF0000-641010",0.9) ;
                                if (0 == towerNoExt)
                                {
                                    dm.KeyDown(16) ;
                                    Thread.Sleep(20);
                                    dm.MoveTo(180,740) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                    Thread.Sleep(20);
                                    dm.MoveTo(70,750) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                    Thread.Sleep(20);
                                    dm.KeyUp(16) ;
                                }
                                else
                                {
                                    //四塔
                                    dm.KeyDown(16) ;
                                    Thread.Sleep(20);
                                    dm.MoveTo(180,740) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                    Thread.Sleep(20);
                                    dm.MoveTo(70,750) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                    Thread.Sleep(20);
                                    dm.MoveTo(30,735) ;
                                    Thread.Sleep(20);
                                    dm.KeyPress(65) ;
                                    Thread.Sleep(20);
                                    dm.LeftClick() ;
                                    Thread.Sleep(20);
                                    dm.KeyUp(16) ;
                                }
                             }
                             
                         }
                    }
                    else if ("打野" == route)
                    {
                        dm.KeyDown(16) ;
                        Thread.Sleep(20);
                        dm.MoveTo(68,610) ;
                        Thread.Sleep(20);
                        dm.KeyPress(65) ;
                        Thread.Sleep(20);
                        dm.MoveTo(101,621) ;
                        Thread.Sleep(20);
                        dm.KeyPress(65) ;
                        Thread.Sleep(20);
                        dm.LeftClick() ;
                        Thread.Sleep(20);
                        dm.KeyUp(16) ;
                    }
                }
                //释放技能
                if (0 == noSkill)
                {
                    if ("清兵推线" == killWay)
                    {
                        if(skillSequence.Contains("R") || skillSequence.Contains("r"))
                        {
                            dm.FindPic(95, 55, 935, 585, "minionshp.bmp", "000000", 1, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x + 10, (int)y + 30);
                                Thread.Sleep(20);
                                dm.KeyPress(82);
                                Thread.Sleep(20);
                                dm.LeftClick();
                            }
                            
                        }
                        if (skillSequence.Contains("Q") || skillSequence.Contains("q"))
                        {
                            dm.FindPic(95, 55, 935, 585, "minionshp.bmp", "000000", 1, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x + 10, (int)y + 30);
                                Thread.Sleep(20);
                                dm.KeyPress(81);
                                Thread.Sleep(20);
                                dm.LeftClick();
                            }
                        }
                        if (skillSequence.Contains("W") || skillSequence.Contains("w"))
                        {
                            dm.FindPic(95, 55, 935, 585, "minionshp.bmp", "000000", 1, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x + 10, (int)y + 30);
                                Thread.Sleep(20);
                                dm.KeyPress(87);
                                Thread.Sleep(20);
                                dm.LeftClick();
                            }
                        }
                        if (skillSequence.Contains("E") || skillSequence.Contains("e"))
                        {
                            dm.FindPic(95, 55, 935, 585, "minionshp.bmp", "000000", 1, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x + 10, (int)y + 30);
                                Thread.Sleep(20);
                                dm.KeyPress(69);
                                Thread.Sleep(20);
                                dm.LeftClick();
                            }
                        }
                        if (skillSequence.Contains("R") || skillSequence.Contains("r"))
                        {
                            dm.FindPic(95, 55, 935, 585, "bothp.bmp", "000000", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x + 10, (int)y + 30);
                                Thread.Sleep(20);
                                dm.KeyPress(82);
                                Thread.Sleep(20);
                                dm.LeftClick();
                                Thread.Sleep(20);
                                dm.KeyPress(88);
                                Thread.Sleep(20);
                                dm.KeyPress(67);
                                Thread.Sleep(20);
                                dm.KeyPress(86);
                                Thread.Sleep(20);
                                dm.KeyPress(66);
                                Thread.Sleep(20);
                                dm.KeyPress(78);
                            }
                        }
                        if (skillSequence.Contains("Q") || skillSequence.Contains("q"))
                        {
                            dm.FindPic(95, 55, 935, 585, "bothp.bmp", "000000", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x + 10, (int)y + 30);
                                Thread.Sleep(20);
                                dm.KeyPress(81);
                                Thread.Sleep(20);
                                dm.LeftClick();
                            }
                        }
                        if (skillSequence.Contains("W") || skillSequence.Contains("w"))
                        {
                            dm.FindPic(95, 55, 935, 585, "bothp.bmp", "000000", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x + 10, (int)y + 30);
                                Thread.Sleep(20);
                                dm.KeyPress(87);
                                Thread.Sleep(20);
                                dm.LeftClick();
                            }
                        }
                        if (skillSequence.Contains("E") || skillSequence.Contains("e"))
                        {
                            dm.FindPic(95, 55, 935, 585, "bothp.bmp", "000000", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x + 10, (int)y + 30);
                                Thread.Sleep(20);
                                dm.KeyPress(69);
                                Thread.Sleep(20);
                                dm.LeftClick();
                            }
                        }
                    }
                    else
                    {
                        if (skillSequence.Contains("R") || skillSequence.Contains("r"))
                        {
                            dm.FindPic(95, 60, 935, 570, "bothp.bmp", "000000", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x + 10, (int)y + 30);
                                Thread.Sleep(20);
                                dm.KeyPress(82);
                                Thread.Sleep(20);
                                dm.LeftClick();
                                Thread.Sleep(20);
                                dm.KeyPress(88);
                                Thread.Sleep(20);
                                dm.KeyPress(67);
                                Thread.Sleep(20);
                                dm.KeyPress(86);
                                Thread.Sleep(20);
                                dm.KeyPress(66);
                                Thread.Sleep(20);
                                dm.KeyPress(27);
                            }
                        }
                        if (skillSequence.Contains("Q") || skillSequence.Contains("q"))
                        {
                            dm.FindPic(95, 60, 935, 570, "bothp.bmp", "000000", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x + 10, (int)y + 30);
                                Thread.Sleep(20);
                                dm.KeyPress(81);
                                Thread.Sleep(20);
                                dm.LeftClick();
                            }
                        }
                        if (skillSequence.Contains("W") || skillSequence.Contains("w"))
                        {
                            dm.FindPic(95, 60, 935, 570, "bothp.bmp", "000000", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x + 10, (int)y + 30);
                                Thread.Sleep(20);
                                dm.KeyPress(87);
                                Thread.Sleep(20);
                                dm.LeftClick();
                            }
                        }
                        if (skillSequence.Contains("E") || skillSequence.Contains("e"))
                        {
                            dm.FindPic(95, 60, 935, 585, "bothp.bmp", "000000", 0.9, 0, out x, out y);
                            if ((int)x > 0)
                            {
                                dm.MoveTo((int)x + 10, (int)y + 30);
                                Thread.Sleep(20);
                                dm.KeyPress(69);
                                Thread.Sleep(20);
                                dm.LeftClick();
                            }
                        }
                    }
                }
                if (1 == gather)
                {
                    if (1 == camp)
                    {
                        dm.MoveTo(20,755) ;
                        Thread.Sleep(20);
                        dm.RightClick() ;
                        Thread.Sleep(20);
                        dm.MoveTo(20,755) ;
                        Thread.Sleep(20);
                        dm.RightClick() ;
                        Thread.Sleep(20);
                        dm.MoveTo(20,755) ;
                        Thread.Sleep(20);
                        dm.RightClick() ;
                        Thread.Sleep(45000);
                    }
                    else
                    {
                        dm.MoveTo(193,590) ;
                        Thread.Sleep(50) ;
                        dm.RightClick() ;
                        Thread.Sleep(50) ;
                        dm.MoveTo(193,590) ;
                        Thread.Sleep(50) ;
                        dm.RightClick() ;
                        Thread.Sleep(50) ;
                        dm.MoveTo(193,590) ;
                        Thread.Sleep(50) ;
                        dm.RightClick() ;
                        Thread.Sleep(45000) ;
                    }
                    gather = 0 ;
                }
                if (15 == back)
                {
                    escape  = 1 ;
                    back    = 1 ;
                }
                if (30 == adjust)
                {
                    dm.KeyUp(16) ;
                    Thread.Sleep(50) ;
                    dm.KeyPress(27);
                    Thread.Sleep(50) ;
                    dm.KeyPress(83) ;
                }
                back++ ;
                adjust++ ;
            }
        }

        #endregion
        #region 喊话
        private void thread_megaphone(CDmSoft dm, int hwnd)
        {
            object x, y;
            while (true)
            {
                dm.FindPic(440, 160, 590, 195, "trade.bmp", "000000", 0.9, 0, out x, out y);
                if ((int)x > 0)
                {
                    dm.FindPic(570, 290, 640, 340, "refusetrade.bmp", "000000", 0.9, 0, out x, out y);
                    if ((int)x > 0)
                    {
                        dm.MoveTo((int)x + 2, (int)y + 2);
                        Thread.Sleep(50);
                        dm.LeftDown();
                        Thread.Sleep(50);
                        dm.LeftUp();
                    }
                }
                dm.FindPic(945, 460, 995, 550, "add.bmp", "000000", 0.9, 0, out x, out y);  //差30
                Thread.Sleep(200);
                dm.MoveTo((int)x, (int)y - 30);
                Thread.Sleep(200);
                dm.LeftClick();
                Thread.Sleep(200);
                dm.KeyPress(13);
                Thread.Sleep(200);
                dm.SendString2(hwnd, megaphoneText);
                Thread.Sleep(200);
                dm.KeyPress(13);

                //喊话延迟
                Thread.Sleep(megaphoneDelay * 1000);

            }


        }
        #endregion

        #region 线程验证
        public void thread_MulOpendet()
        {
            preventMulOpen prvMulOpen = new preventMulOpen();
            while (true)
            {
                Thread.Sleep(300000);
                prvMulOpen.guidWay();
                prvMulOpen.processWay();
                prvMulOpen.titleWay();
            }
        }
        #endregion
        #region 启动
        private void thread_startup()
        {
            //if (1 == autoLogin)
           //     button_stop_Click(null, null);
            hole_Dm.SetUAC(0);
            hole_Dm.DisablePowerSave();
            hole_Dm.DisableScreenSave();
            string hwndList;
            int index, dm_ret;
            APP application = new APP();
            Thread threadPrvOpen = new Thread(() => thread_MulOpendet());
            bindIndex = 0;
            dm_ret = 0;
            gameNeedUpdate = 0;
            //开启防破和游戏更新功能
            threadPrvOpen.Start();
            Thread_List.Add(threadPrvOpen);
            //自动登录功能
            if (1 == autoLogin)
                automaticLogin();
            threadGameCrash = new Thread(() => thread_gameCrash());
            threadGameCrash.Start();

            //先搜索存在的窗口
            hwndList = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
            if (hwndList.Length <= 0)
            {
                MessageBox.Show("没有找到DOTA2窗口,请开启DOTA2后再启动");
                return;
            }
            //开始遍历并且绑定
            try
            {
                //养成好习惯，清空一下数组
                hwndArrary.Clear();
                foreach (string hwnds in hwndList.Split(','))
                {
                    //将每个大漠对象实例化【新手很容易忽略这一句】
                    testDm[bindIndex] = new CDmSoft();
                    //设置大漠路径，这里大漠路径为大漠dll目录下的attachment文件夹，这文件夹里面存放图片和字库，在这里就是Debug目录，或者Release目录
                    testDm[bindIndex].SetPath(".\\game_script");
                    //设置0号字库
                    testDm[bindIndex].SetDict(0, "dm.txt");
                    //开始绑定，这里绑定的是后台
                    dm_ret = testDm[bindIndex].BindWindow(int.Parse(hwnds), "dx", "dx", "dx", 0);
                    if (1 == dm_ret)
                    {
                        Thread.Sleep(500);
//                         if (1 == downCpu)
//                         {
//                             testDm[bindIndex].DownCpu(downCpuValue);
//                         }
                        hwndArrary.Add(int.Parse(hwnds));//将句柄添加进句柄数组
                        testDm[bindIndex].LockInput(1);
                        //控制多开
                    }
                    else
                    {
                        MessageBox.Show("绑定失败:"+"请关闭游戏和脚本后右键程序找到管理员权限打开程序和游戏启动器(详情查看说明书)");
                        return;
                    }
                    bindIndex++;
                    if (bindIndex == threadNum) //先++在判断所以是10
                        break;
                }

            }
            catch (System.Exception ex)
            {

            }
            try
            {
                for (index = 0; index < hwndArrary.Count; index++)
                {
                    //执行带参数的线程，这里i为什么要赋值给index，不解释，大家照做，必须这样，特别是循环内启动线程一定要注意这点
                    Thread newThd = new Thread(() => thread_main(testDm[index], (int)hwndArrary[index]));
                    newThd.IsBackground = true;//设置为后台线程 有人说这就是设置线程模式为MTA模式，不知道是不是
                    //将这个线程添加进线程集合，方便停止
                    Thread_List.Add(newThd);
                    //启动
                    newThd.Start();
                    if (index == threadNum - 1)  //先判断在++所以要-1
                        break;
                    Thread.Sleep(200);
                }
            }
            catch (System.Exception ex)
            {

            }
            while (true)
            {
                Thread.Sleep(100000000);
            }
        }
        #endregion
        
        public void thread_gameUpdate()
        {
            APP application = new APP();
            string strsTemp;
            if (gameNeedUpdate > 15)
            {
                string hwndListTemp;
                hwndListTemp = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
                if (hwndListTemp.Length > 0)
                {
                    foreach (string hwnds in hwndListTemp.Split(','))
                    {
                        try
                        {
                            hole_Dm.SetWindowState(int.Parse(hwnds), 13);
                        }
                        catch (System.Exception ex)
                        {

                        }

                    }
                }
                hwndListTemp = hole_Dm.EnumWindow(0, "Dota 2启动器", tempStr, 1 + 2);
                {
                    foreach (string hwnds in hwndListTemp.Split(','))
                    {
                        try
                        {
                            hole_Dm.SetWindowState(int.Parse(hwnds), 13);
                        }
                        catch (System.Exception ex)
                        {

                        }
                    }
                }
                strsTemp = " -login account password ";
                try
                {
                    strsTemp = strsTemp.Replace("account", account[0].Split('-')[0]);
                    strsTemp = strsTemp.Replace("password", account[0].Split('-')[1]);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("账号-密码为空");
                }
                application.runapp(strsTemp);
                Thread.Sleep(1200000);
                hwndListTemp = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
                if (hwndListTemp.Length > 0)
                {
                    foreach (string hwnds in hwndListTemp.Split(','))
                    {
                        try
                        {
                            hole_Dm.SetWindowState(int.Parse(hwnds), 13);
                        }
                        catch (System.Exception ex)
                        {

                        }

                    }
                }
                hwndListTemp = hole_Dm.EnumWindow(0, "Dota 2启动器", tempStr, 1 + 2);
                {
                    foreach (string hwnds in hwndListTemp.Split(','))
                    {
                        try
                        {
                            hole_Dm.SetWindowState(int.Parse(hwnds), 13);
                        }
                        catch (System.Exception ex)
                        {

                        }
                    }
                }
                SendMessage(this.button_statr.Handle, WM_LBUTTONDOWN, 0, 0);
                SendMessage(this.button_statr.Handle, WM_LBUTTONUP, 0, 0);
            }
        }
        public void thread_gameCrash()
        {
            string hwndList;
            int i;
            i = 0;
            while(1 == mainIsRunning)
            {
                hwndList = hole_Dm.EnumWindow(0, "DOTA 2", "", 1 + 4 + 8);
                if (hwndList.Length <= 0)
                {
                    //关闭所有线程
                    foreach (Thread Thd in Thread_List)
                    {
                        try
                        {
                            Thd.Suspend();
                        }
                        catch (System.Exception ex)
                        {

                        }

                    }
                    for (i = 0; i < testDm.Length; i++)
                    {
                        try
                        {
                            testDm[i].UnBindWindow();
                            Thread.Sleep(500);
                        }
                        catch (System.Exception ex)
                        {

                        }
                    }
                    foreach (Thread Thd in Thread_List)
                    {
                        try
                        {
                            Thd.Abort();
                        }
                        catch (System.Exception ex)
                        {

                        }
                    }
                    //自启动
                    mainIsRunning = 0;
                    button_statr_Click(null,null) ;
                }
                Thread.Sleep(2 * 60 * 1000);
            }
            
        }
        private void checkBox_shutdown_CheckedChanged(object sender, EventArgs e)
        {
            if (true == checkBox_shutdown.Checked)
            {
                shutdown = 1;
            }
            else
            {
                shutdown = 0;
            }
            
        }
        private void checkBox_inning_CheckedChanged(object sender, EventArgs e)
        {
            if (true == checkBox_inning.Checked)
            {
                inningStopFun = 1;
            }
            else
            {
                inningStopFun = 0;
            }
        }

        private void textBox_inning_TextChanged(object sender, EventArgs e)
        {
            inningStopNum = int.Parse(textBox_inning.Text);
        }

        private void button_unbind_Click(object sender, EventArgs e)
        {
            SendMessage(this.button_stop.Handle, WM_LBUTTONDOWN, 0, 0);
            SendMessage(this.button_stop.Handle, WM_LBUTTONUP, 0, 0);
            a.Unbind();
        }

        private void textBox_skillSequence_TextChanged(object sender, EventArgs e)
        {
            skillSequence = textBox_skillSequence.Text;
        }

        private void checkBox_escapeMode_CheckedChanged(object sender, EventArgs e)
        {
            if(true == checkBox_escapeMode.Checked)
                escapeMode = 1 ;
            else
                escapeMode = 0;
           
        }
        private void calculateHeroCoordinate(int index,out int x,out int y)
        {
            int tempX,tempY;
            tempX = 0;
            tempY = 0;
            if (index + 1 < 8)                                                       //第一行
            {
                tempX = 40 + (((index + 1) % 7) * 40);
                tempY = 200;
                if ((index + 1) % 7 == 0)
                  tempX = 40 + 7 * 40;
            }
            else if (7 < index + 1 && index + 1 < 15)
            {
                tempX = 375 + (((index + 1) % 7) * 40);
                tempY = 200;
                if ((index + 1) % 7 == 0)
                    tempX = 375 + 7 * 40;
            }
            else if (14 < index + 1 && index + 1 < 22)
            {
                tempX = 710 + (((index + 1) % 7) * 40);
                tempY = 200;
                if ((index + 1) % 7 == 0)
                    tempX = 710 + 7 * 40;
            }
            else if (22 < index + 1 && index + 1 < 29)     //第二行
            {
                tempX = 40 + (((index + 1) % 7) * 40);
                tempY = 250;
                if ((index + 1) % 7 == 0)
                    tempX = 40 + 7 * 40;
            }
            else if (28 < index + 1 && index + 1 < 36)
            {
                tempX = 375 + (((index + 1) % 7) * 40);
                tempY = 250;
                if ((index + 1) % 7 == 0)
                    tempX = 375 + 7 * 40;
            }
            else if (35 < index + 1 && index + 1 < 43)
            {
                tempX = 710 + (((index + 1) % 7) * 40);
                tempY = 250;
                if ((index + 1) % 7 == 0)
                    tempX = 710 + 7 * 40;
            }
            else if (42 < index + 1 && index + 1 < 50)     //第三行
            {
                tempX = 40 + (((index + 1) % 7) * 40);
                tempY = 300;
                if ((index + 1) % 7 == 0)
                    tempX = 40 + 7 * 40;
            }
            else if (49 < index + 1 && index + 1 < 54)
            {
                tempX = 375 + (((index + 1) % 7) * 40);
                tempY = 300;
                if ((index + 1) % 7 == 0)
                    tempX = 375 + 7 * 40;
            }
            else if (53 < index + 1 && index + 1 < 58)
            {
                tempX = 710 + (((index + 1 + 3) % 7) * 40);   //+3补上少的三个人物
                tempY = 300;
                if ((index + 1) % 7 == 0)
                    tempX = 710 + 7 * 40;
            }
            else if (57 < index + 1 && index + 1 < 65)     //第四行
            {
                tempX = 40 + (((index + 1 + 6) % 7) * 40);
                tempY = 350;
                if ((index + 1) % 7 == 0)
                    tempX = 40 + 7 * 40;
            }
            else if (64 < index + 1 && index + 1 < 72)
            {
                tempX = 375 + (((index + 1 + 6) % 7) * 40);
                tempY = 350;
                if ((index + 1) % 7 == 0)
                    tempX = 375 + 7 * 40;
            }
            else if (71 < index + 1 && index + 1 < 79)
            {
                tempX = 710 + (((index + 1 + 6) % 7) * 40);
                tempY = 350;
                if ((index + 1) % 7 == 0)
                    tempX = 710 + 7 * 40;
            }
            else if (78 < index + 1 && index + 1 < 86)     //第五行
            {
                tempX = 40 + (((index + 1 + 6) % 7) * 40);
                tempY = 400;
                if ((index + 1) % 7 == 0)
                    tempX = 40 + 7 * 40;
            }
            else if (85 < index + 1 && index + 1 < 93)
            {
                tempX = 375 + (((index + 1 + 6) % 7) * 40);
                tempY = 400;
                if ((index + 1) % 7 == 0)
                    tempX = 375 + 7 * 40;
            }
            else if (92 < index + 1 && index + 1 < 100)
            {
                tempX = 710 + (((index + 1 + 6) % 7) * 40);
                tempY = 400;
                if ((index + 1) % 7 == 0)
                    tempX = 710 + 7 * 40;
            }
            else if (99 < index + 1 && index + 1 < 101)     //第六行
            {
                tempX = 40 + (((index + 1 + 6) % 7) * 40);
                tempY = 450;
                if ((index + 1) % 7 == 0)
                    tempX = 40 + 7 * 40;
            }
            else if (100 < index + 1 && index + 1 < 103)
            {
                tempX = 375 + (((index + 1 + 6 + 6) % 7) * 40);
                tempY = 450;
                if ((index + 1) % 7 == 0)
                    tempX = 375 + 7 * 40;
            }
            else if (102 < index + 1)
            {
                tempX = 710 + (((index + 1 + 6 + 6 + 5) % 7) * 40);
                tempY = 450;
                if ((index + 1) % 7 == 0)
                    tempX = 710 + 7 * 40;
            }
            x = tempX;
            y = tempY;
        }
//         private void checkBox_DownCpu_CheckedChanged(object sender, EventArgs e)
//         {
//             
//             if (true == checkBox_DownCpu.Checked)
//             {
//                 downCpu = 1;
//                 textBox_downCpuValue.Enabled = true;
//             }
//             else
//             {
//                 downCpu = 1;
//                 textBox_downCpuValue.Enabled = false;
//             }
//             
//         }
// 
//         private void textBox_downCpuValue_TextChanged(object sender, EventArgs e)
//         {
//             downCpuValue = int.Parse(textBox_downCpuValue.Text);
//         }
    }
}
        