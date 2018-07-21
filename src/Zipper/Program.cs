using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;

namespace Zipper
{
	class Program
	{
		static void Main(string[] args)
		{


			if (File.Exists("c:\\users\\starcev.m\\Desktop\\result\\submission.zip"))
				File.Delete("c:\\users\\starcev.m\\Desktop\\result\\submission.zip");
			var pathToNbtDir = "C:\\Users\\starcev.m\\Desktop\\result";
			var outputPath = "c:\\users\\starcev.m\\Desktop\\result\\submission.zip";
			using (ZipFile zip = new ZipFile())
			{
				zip.AddDirectory(pathToNbtDir);
				zip.Save(outputPath);
			}

			Console.WriteLine("id  - 30b02e264245426487a38b8e99c8fee7");

			SHA256 mySHA256 = SHA256Managed.Create();

			using (var filestream = new FileStream(outputPath, FileMode.Open))
			{
				filestream.Position = 0;
				byte[] hashValue = mySHA256.ComputeHash(filestream);
				Console.WriteLine("sha - "+BitConverter.ToString(hashValue).Replace("-", String.Empty));
			} ;

			



			byte[] data = File.ReadAllBytes(outputPath);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://transfer.sh/submission.zip");
			request.PreAuthenticate = true;
			request.Method = "PUT";
			request.ContentType = "application / zip";
			request.ContentLength = data.Length;

			using (Stream stream = request.GetRequestStream())
			{
				stream.Write(data, 0, data.Length);
			}
			using (Stream stream = request.GetResponse().GetResponseStream())
			{
				var reader = new StreamReader(stream);
				Console.WriteLine("url - "+reader.ReadToEnd());
			}


			var response = (HttpWebResponse)request.GetResponse();
			response.Close();
		}
}

}