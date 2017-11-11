/*
 * 由SharpDevelop创建。
 * 用户： Bob (XuYong Hou) houxuyong@hotmail.com
 * 日期: 2017/11/11
 * 时间: 5:20
 * 
 * 
 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;


namespace demon
{
	class Program
	{
		private static int tomcat_pid = -1;
		private static string java = "";
		private static string argfile = "";
		private static string nfile = "";
		private static int interval = 10000;
		private static string catalina_home = "";
		private static string CATALINA_KEY= "CATALINA_HOME";
		
		public static void Main(string[] args)
		{
			Console.WriteLine("tomcat demon v0.9.1.20171111.1205");

			catalina_home = Environment.GetEnvironmentVariable(CATALINA_KEY);
			Console.WriteLine("debug: CATALINA_HONE={0}",catalina_home);
			
			if (args.Length != 4)
			{
				Console.WriteLine("Invalid arguments - please use \"demon.exe <java_path> <args_file> <notify_file> <interval>\"");
				Console.ReadKey(true);
				return;
			}
			java = args[0];
			argfile = args[1];
			nfile = args[2];
			interval = int.Parse(args[3]);
			
			Timer t = new Timer(TimerCallback, null, 0, interval);		
			Console.ReadKey(true);
		}
		
		private static void TimerCallback(Object o) {
		     
		     if (File.Exists(nfile))
		     {
		     	var ncmd = File.ReadAllText(nfile);
		     	if (ncmd == "start")
		     	{
		     		if (!File.Exists(argfile))
		     		{
		     			Console.WriteLine("[{0}] - tomcat args file does not exist", DateTime.Now);
		     			return;
		     		}
		     		var args = File.ReadAllText(argfile);
		     		args = args.Replace("%"+CATALINA_KEY+"%", catalina_home);
		     		//Console.WriteLine("debug: args={0}",args);
			
		     		LaunchCommandLineApp(java, args);
		     	} 
		     	else if (ncmd == "stop")
		     	{
		     		KillLineApp();
		     	}
		     }
		      
		}
		
		public static bool IsRunning(int pid)
		{
			Process[] processlist = Process.GetProcesses();
			foreach (Process theprocess in processlist) {
				if (pid == theprocess.Id)
					return true;
			}
			return false;
		}
		
		private static bool launchDumped = false;
		
		static void LaunchCommandLineApp(string file, string arg)
		{
			if (IsRunning(tomcat_pid)) {
				if (!launchDumped){
					Console.WriteLine("[{0}] - tomcat pid:{1} already launched", DateTime.Now, tomcat_pid);
					launchDumped=!launchDumped;
				}
				return;
			}
	        
			// Use ProcessStartInfo class
			var startInfo = new ProcessStartInfo();
			startInfo.CreateNoWindow = true;
			startInfo.UseShellExecute = false;
			startInfo.FileName = file;
			startInfo.WindowStyle = ProcessWindowStyle.Normal;
			startInfo.Arguments = arg;
	
			try {
				// Start the process with the info we specified.
				// Call WaitForExit and then the using statement will close.
				using (Process exeProcess = Process.Start(startInfo)) {
					tomcat_pid = exeProcess.Id;
					Console.WriteLine("[{0}] - tomcat pid:{1} is launching",  DateTime.Now,tomcat_pid);
					//XHO: WaitForExit will make the current thread wait until the process terminates
					//https://msdn.microsoft.com/en-us/library/fb4aw7b8(v=vs.110).aspx
					
					exeProcess.WaitForExit();					
				}
			} catch (Exception e) {
				// Log error.
				throw e;
			}
		}
		
		
		static void KillLineApp()
		{
			if (IsRunning(tomcat_pid)) {
				Console.WriteLine("[{0}] - tomcat pid:{1} to be killed...", DateTime.Now,tomcat_pid );
				try {
					Process p = Process.GetProcessById(tomcat_pid);
					p.Kill();
					p.WaitForExit(); // possibly with a timeout
					Console.WriteLine("[{0}] - tomcat pid:{1} killed", DateTime.Now,tomcat_pid);
					tomcat_pid = -1;	
					launchDumped = false;
				} catch (Win32Exception winException) {
					// process was terminating or can't be terminated - deal with it
				} catch (InvalidOperationException invalidException) {
					// process has already exited - might be able to let this one go
				}
			}
		}
	}
}