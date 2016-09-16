using Arenaii.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Arenaii.Platform
{
	public class ConsoleBot : IDisposable
	{
		public Bot Bot { get; protected set; }

		private readonly Stopwatch Timer = new Stopwatch();

		/// <summary>Start the timer for the bot.</summary>
		public void Start() { Timer.Start(); }
		
		/// <summary>Stop the timer for the bot.</summary>
		public void Stop() { Timer.Stop(); }
		/// <summary>Get the elapsed milliseconds.</summary>
		public TimeSpan Elapsed { get { return Timer.Elapsed; } }

		/// <summary>Did the bot time out?</summary>
		public bool TimedOut { get; protected set; }
		public string Read(TimeSpan timeout)
		{
			Start();
			var tokenSource = new CancellationTokenSource();
			var token = tokenSource.Token;
			var task = Task.Factory.StartNew(() => process.StandardOutput.ReadLine(), token);
			if (!task.Wait((int)timeout.TotalMilliseconds, token))
			{
				process.Kill();
				TimedOut = true;
				return string.Empty;
			}
			Stop();
			return task.Result;
		}

		public void Write(string format)
		{
			process.StandardInput.WriteLine(format);
		}
		public void Write(string format, params object[] args)
		{
			process.StandardInput.WriteLine(format, args);
		}

		/// <summary>The process required to run the bot.</summary>
		protected Process process;

		protected static Process CreateProcess(FileInfo exe)
		{
			var p = new Process();
			p.StartInfo.WorkingDirectory = exe.Directory.FullName;
			p.StartInfo.FileName = exe.FullName;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.Start();
			return p;
		}

		public static ConsoleBot Create(Bot bot)
		{
			var console = new ConsoleBot()
			{
				Bot = bot,
				process = CreateProcess(bot.Location),
			};
			return console;
		}

		#region IDisposable

		/// <summary>Dispose the console platform.</summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Dispose the console platform.</summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!m_IsDisposed)
			{
				if (disposing && process != null)
				{
					process.Dispose();
				}
				m_IsDisposed = true;
			}
		}

		/// <summary>Destructor</summary>
		~ConsoleBot() { Dispose(false); }

		private bool m_IsDisposed = false;

		#endregion
	}
}
