using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace GithubAccelerator
{
	class Accelerator
	{
		private static string[] Sites = new string[] {
				"github.com",
				"github.global.ssl.fastly.net",
				"assets-cdn.github.com",
				"documentcloud.github.com",
				"gist.github.com",
				"help.github.com",
				"nodeload.github.com",
				"raw.github.com",
				"status.github.com",
				"training.github.com",
				"ithubusercontent.com",
				"avatars1.githubusercontent.com",
				"codeload.github.com" };

		public void Accelerate()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append($"\n\n#Github Accelerator, Author: ChenWeihao, Update on {DateTime.UtcNow}\n");

			Console.WriteLine("Two ways to increase speed, input 1 to use way 1 and 2 for way 2: ");
			string choice = Console.ReadLine();

			switch(choice)
			{
				case "1":
					foreach (string site in Sites)
					{
						string IP = GetIP_1(site);
						Console.WriteLine(string.Format("{0,-40}{1,0}", site, IP));
						sb.Append(IP + " " + site + "\n");
					}
					break;

				case "2":
					foreach (string site in Sites)
					{
						string IP = GetIP_2(site);
						Console.WriteLine(string.Format("{0,-40}{1,0}", site, IP));
						sb.Append(IP + " " + site + "\n");
					}
					break;
				default:
					Console.WriteLine("Invalid Choice");
					return;
					break;
			}

			
			sb.Append("#End of the accelerated hosts list\n\n");
			string hostsLocation = @"C:\Windows\System32\drivers\etc\hosts";

			if (File.Exists(hostsLocation))
			{
				string strContent = File.ReadAllText(hostsLocation);
				strContent = Regex.Replace(strContent, @"#Github Accelerator, Author-ChenWeihao[\s\S]*#End of the accelerated hosts list", "")+sb.ToString();
				File.WriteAllText(hostsLocation, strContent);
				Console.WriteLine("Successfully write hosts info to hosts file");
			}

			Process p = new Process();
			p.StartInfo.FileName = "cmd.exe";
			p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
			p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
			p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
			p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
			p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
			p.Start();      //启动程序

			//向cmd窗口写入命令
			p.StandardInput.WriteLine("ipconfig /flushdns"+ "&exit");
			

			//获取cmd窗口的输出信息
			string output = p.StandardOutput.ReadToEnd();
			Console.WriteLine(output);
			p.WaitForExit();//等待程序执行完退出进程
			p.Close();
		}
		
		//get ip from www.ip.cn
		private string GetIP_1(string url)
		{
			try
			{
				string api = "https://www.ip.cn/api/index?";
				string requestURL = $"{api}ip={url}&type=1";

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestURL);
				request.Method = "GET";
				request.ContentType = "text/html;charset=UTF-8";
				request.UserAgent = "Mozilla / 5.0(Windows NT 10.0; Win64; x64; rv: 80.0) Gecko / 20100101 Firefox / 80.0";
				request.Timeout = 10 * 1000; //10 seconds
				request.ContentType = "	text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
				request.Host = "www.ip.cn";
				request.KeepAlive = true;
				request.Proxy = null;

				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				Stream myResponseStream = response.GetResponseStream();
				StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
				string retString = myStreamReader.ReadToEnd();
				myStreamReader.Close();
				myResponseStream.Close();

				JObject jo = (JObject)JsonConvert.DeserializeObject(retString);
				return jo["ip"].ToString();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return "";
			}
		}

		//get ip from haoip.cn
		private string GetIP_2(string url)
		{
			try
			{
				string api = "https://haoip.cn/ip/";
				string requestURL = api+url;

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestURL);
				request.Method = "GET";
				request.ContentType = "text/html;charset=UTF-8";
				request.UserAgent = "Mozilla / 5.0(Windows NT 10.0; Win64; x64; rv: 80.0) Gecko / 20100101 Firefox / 80.0";
				request.Timeout = 10 * 1000; //10 seconds
				request.ContentType = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
				request.Host = "haoip.cn";
				request.KeepAlive = true;
				request.Proxy = null;

				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				Stream myResponseStream = response.GetResponseStream();
				StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
				string html = myStreamReader.ReadToEnd();
				myStreamReader.Close();
				myResponseStream.Close();

				HtmlDocument doc = new HtmlDocument();
				doc.LoadHtml(html);

				string xpath1 = "//code[@class=\"text-primary bg-info\"]";
				var nodes = doc.DocumentNode.SelectNodes(xpath1);
				return nodes[1].InnerHtml;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return "";
			}
		}
	}
}
