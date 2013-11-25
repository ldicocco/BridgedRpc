using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.IO;

using BridgedRpc.Bridge.SlicedFile;

namespace BasicWebBridge.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{

			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}

	internal class StringStream : MemoryStream
	{
		public StringStream(string text)
			: base()
		{
			StreamWriter sw = new StreamWriter(this);
			sw.Write(text);
			sw.Flush();
			Position = 0;
		}

		public StringStream(string text, params object[] args)
			: base()
		{
			StreamWriter sw = new StreamWriter(this);
			sw.Write(text, args);
			sw.Flush();
			Position = 0;
		}
	}

	internal static class Utilities
	{
		static Random randomGenerator = new Random();
		internal static string GenerateRandomString(int length)
		{
			byte[] randomBytes = new byte[randomGenerator.Next(length)];
			randomGenerator.NextBytes(randomBytes);
			return Convert.ToBase64String(randomBytes);
		}
	}

}