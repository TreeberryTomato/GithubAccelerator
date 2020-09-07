using System.Security.Principal;
using System;


namespace GithubAccelerator
{
	class Program
	{
		static void Main(string[] args)
		{
			using (WindowsIdentity wi = WindowsIdentity.GetCurrent()) // 用户
			{
				WindowsPrincipal wp = new WindowsPrincipal(wi);  // 用户组
				if (!wp.IsInRole(WindowsBuiltInRole.Administrator)) // 用户是否属于管理员
				{
					Console.WriteLine("请使用管理员登录打开此程序\nPlease run as Administrator");
					Console.WriteLine("\nPress any key to continue");
					Console.ReadKey();
					return;

				}
			}
			Accelerator accelerator = new Accelerator();
			accelerator.Accelerate();
			Console.WriteLine("\nPress any key to continue");
			Console.ReadKey();
		}
	}
}
