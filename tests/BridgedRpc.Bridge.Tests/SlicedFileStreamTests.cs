using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Xunit;

using BridgedRpc.Bridge.SlicedFile;

namespace BridgedRpc.Bridge.Tests
{
	public class SlicedFileStreamTests
	{
		[Fact]
		public async void SlicedFileStream()
		{
/*			string str1 = Utilities.GenerateRandomString(200);
			Stream stream1 = new StringStream(str1);
			string str2 = Utilities.GenerateRandomString(300);
			Stream stream2 = new StringStream(str2);
			string str3 = Utilities.GenerateRandomString(3000);
			Stream stream3 = new StringStream(str3);
			string text = str1 + str2 + str3;
			SlicedFileStream sfs = new SlicedFileStream(3, "Tmp");
			var sd1 = await sfs.SetSliceAsync(0, stream1);
			var sd2 = await sfs.SetSliceAsync(1, stream2);
			var sd3 = await sfs.SetSliceAsync(2, stream3);
			using (var sr = new StreamReader(sfs))
			{
				var str = sr.ReadToEnd();
				Assert.Equal(str1 + str2 + str3, str);
			}*/
/*			int bufSize = 1024;
			byte[] buf = new byte[bufSize];
			int cnt = 0;
			int bytesRead = 0;
			do
			{
				cnt = await stream.ReadAsync(buf, 0, bufSize);
				Assert.Equal(new ASCIIEncoding().GetString(buf, 0, cnt), text.Substring(bytesRead, cnt));
				bytesRead += cnt;
			} while (cnt == bufSize);*/
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
