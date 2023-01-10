using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


public delegate void CallBack(Object data);
public delegate void CallBackEnd(Object data);
public delegate void CallBackError(Object data);
namespace WpfApp2.controller
{
    public class ChromeController
    {
        private string profilePath;
        private string chromePath;
        private bool isHide;
        private string user_agent;

        private CallBack callBack;
        private CallBackError callBackError;
        private CallBackEnd callBackEnd;

        private ChromeDriver chromeDriver;
        private ChromeDriverService chromeDriverService;
        private ChromeOptions chromeOptions;

        private static Random random = new Random();

        public ChromeController(string profilePath, string chromePath)
        {
            this.profilePath = profilePath;
            this.chromePath = chromePath;
            init(profilePath, chromePath);
        }

        public ChromeController()
        {
            
        }

        public string ProfilePath { get => profilePath; set => profilePath = value; }
        public string ChromePath { get => chromePath; set => chromePath = value; }
        public ChromeDriver ChromeDriver { get => chromeDriver; set => chromeDriver = value; }
        public ChromeDriverService ChromeDriverService { get => chromeDriverService; set => chromeDriverService = value; }
        public ChromeOptions ChromeOptions { get => chromeOptions; set => chromeOptions = value; }
        public bool IsHide { get => isHide; set => isHide = value; }
        public string User_agent { get => user_agent; set => user_agent = value; }

        public void init(string profilePath, string chromePath)
        {
            if (chromeDriverService == null)
            {
                chromeDriverService = ChromeDriverService.CreateDefaultService();
                chromeDriverService.HideCommandPromptWindow = true;
                chromeDriverService.SuppressInitialDiagnosticInformation = true;
            }
            if (chromeOptions == null)
            {
                chromeOptions = new ChromeOptions();
                chromeOptions.AcceptInsecureCertificates = true;
                int port = random.Next(3000, 30000);
                if (!String.IsNullOrEmpty(chromePath))
                    chromeOptions.BinaryLocation = chromePath;
                if (!String.IsNullOrEmpty(profilePath))
                {
                    int index = profilePath.LastIndexOf(@"\");
                    string profile2 = profilePath.Remove(0, index + 1);
                    chromeOptions.AddArgument($"--user-data-dir={profilePath}");
                    //chromeOptions.AddArgument($"--profile-directory={profile2}");
                }
                if (IsHide)
                {
                    chromeOptions.AddArguments(new string[]
                    {
                        "headless"
                    });
                }
                chromeOptions.AddArgument($"--remote-debugging-port={port}");
                if (!String.IsNullOrEmpty(user_agent))
                    chromeOptions.AddArgument($"--user-agent={user_agent}");
                chromeOptions.AddExcludedArgument("enable-automation");
                chromeOptions.AddArgument("--disable-notifications");
                chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
                chromeOptions.AddArgument("ignore-certificate-errors");
            }
            if (chromeDriver == null)
            {
                chromeDriver = new ChromeDriver(chromeDriverService, chromeOptions);
            }
        }
        public void init(ChromeDriverService chromeDriverService, ChromeOptions chromeOptions)
        {
            this.chromeDriverService = chromeDriverService;
            this.chromeOptions = chromeOptions;
            chromeDriver = new ChromeDriver(chromeDriverService, chromeOptions);
        }

        public String Comment(String id_post, String end_cursor)
        {
            String script = File.ReadAllText(@"source\comment.js");
            script = script.Replace("%<NEXT>%", end_cursor).Replace("%<POST>%", id_post);
            String output = (String) chromeDriver.ExecuteScript(script);
            
            return output;
        }

        public Thread getListComment(int timeDeleay,String id_post)
        {
            Thread thread = new Thread(() =>
            {
                bool isRun = true;
                String next = "";
                JArray endPointData = new JArray();
                while (isRun)
                {
                    JObject dataGet = new JObject(Comment(id_post, next));
                    if (!dataGet.ContainsKey("data") || !dataGet["data"].HasValues)
                    {
                        if (callBackError != null)
                            callBackError(dataGet);
                        isRun = false;
                        break;
                    }
                    endPointData.Add(dataGet);
                    if (callBack != null)
                        callBack(dataGet);
                    if (((bool)dataGet["data"]["node"]["display_comments"]["page_info"]["has_next_page"]))
                    {
                        next = dataGet["data"]["node"]["display_comments"]["page_info"]["end_cursor"].ToString();
                    }
                    else
                    {
                        callBackEnd(endPointData);
                        isRun = false;
                        break;
                    }
                    
                }
            });
            thread.IsBackground = true;
            thread.Start();
            return thread;
        }


    }
}
