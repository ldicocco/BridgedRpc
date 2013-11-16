using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfServer
{
	class Roots
	{
		private static readonly Lazy<Roots> _instance = new Lazy<Roots>(() => new Roots(), true);
		private Dictionary<string, string> _roots;
		public static Roots Instance
		{
			get
			{
				return _instance.Value;
			}
		}

		private Roots()
		{
			_roots = new Dictionary<string, string>();
			string _currentDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			string filePath = _currentDir + "/roots.txt";
			if (System.IO.File.Exists(filePath))
			{
				string[] lines = System.IO.File.ReadAllLines(filePath);
				foreach (var line in lines)
				{
					var fields = line.Split('=');
					if (fields.Length == 2)
					{
						_roots[fields[0]] = fields[1];
					}
				}
			}
		}

		public string this[string root]
		{
			get
			{
				return _roots[root];
			}
		}

		public bool ContainsRoot(string root)
		{
			return _roots.ContainsKey(root);
		}
	}
}
