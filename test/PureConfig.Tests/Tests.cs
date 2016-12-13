using Xunit;
using System;
using System.IO;
using PureConfigNet;
using Newtonsoft.Json;

namespace Tests
{
	static class TestConfig
	{
		public const string TestFileDirectory = @"spec/tests/success/";
		public const string pureExt = @".pure";
		public const string jsonExt = @".json";
		public static string[] GetFileNames(string baseName)
		{
			return new string[]{
				TestFileDirectory + baseName + pureExt,
				TestFileDirectory + baseName + jsonExt
			};
		}
	}
	public class Tests
	{

		[Fact]
		public void TestString() 
		{
			string[] files = TestConfig.GetFileNames("strings");
			PureConfig p = new PureConfig(files[0]);
			using(StreamReader jsonFile = new StreamReader(File.OpenRead(files[1])))
			{
				Console.WriteLine(Environment.NewLine + files[0] + Environment.NewLine + "---");
				dynamic result = JsonConvert.DeserializeObject(jsonFile.ReadToEnd());
				foreach(var s in result)
				{
					Console.WriteLine(s.Name);
					Assert.Equal((string)s.Value, p.Get<string>(s.Name));
				}
			}
		}

		[Fact]
		public void TestBool() 
		{
			string[] files = TestConfig.GetFileNames("bools");
			PureConfig p = new PureConfig(files[0]);
			using(StreamReader jsonFile = new StreamReader(File.OpenRead(files[1])))
			{
				Console.WriteLine(Environment.NewLine + files[0] + Environment.NewLine + "---");
				dynamic result = JsonConvert.DeserializeObject(jsonFile.ReadToEnd());
				foreach(var s in result)
				{
					Console.WriteLine(s.Name);
					Assert.Equal((bool)s.Value, p.Get<bool>(s.Name));
				}
			}
		}

		[Fact]
		public void TestComments() 
		{
			string[] files = TestConfig.GetFileNames("comments");
			PureConfig p = new PureConfig(files[0]);
			using(StreamReader jsonFile = new StreamReader(File.OpenRead(files[1])))
			{
				Console.WriteLine(Environment.NewLine + files[0] + Environment.NewLine + "---");
				dynamic result = JsonConvert.DeserializeObject(jsonFile.ReadToEnd());
				foreach(var s in result)
				{
					//FIXME: Assert.Equal((bool)s.Value, p.Get<bool>(s.Name));
				}
			}
		}

		[Fact]
		public void TestDecimals() 
		{
			string[] files = TestConfig.GetFileNames("decimals");
			PureConfig p = new PureConfig(files[0]);
			using(StreamReader jsonFile = new StreamReader(File.OpenRead(files[1])))
			{
				Console.WriteLine(Environment.NewLine + files[0] + Environment.NewLine + "---");
				dynamic result = JsonConvert.DeserializeObject(jsonFile.ReadToEnd());
				foreach(var s in result)
				{
					Console.WriteLine(s.Name);
					Assert.Equal((double)s.Value, p.Get<double>(s.Name));
				}
			}
		}

		[Fact]
		public void TestIntegers() 
		{
			string[] files = TestConfig.GetFileNames("integers");
			PureConfig p = new PureConfig(files[0]);
			using(StreamReader jsonFile = new StreamReader(File.OpenRead(files[1])))
			{
				Console.WriteLine(Environment.NewLine + files[0] + Environment.NewLine + "---");
				dynamic result = JsonConvert.DeserializeObject(jsonFile.ReadToEnd());
				foreach(var s in result)
				{
					Console.WriteLine(s.Name);
					Assert.Equal((double)s.Value, p.Get<double>(s.Name));
				}
			}
		}

		[Fact]
		public void TestKeyValueFormat() 
		{
			string[] files = TestConfig.GetFileNames("key_value_format");
			PureConfig p = new PureConfig(files[0]);
			using(StreamReader jsonFile = new StreamReader(File.OpenRead(files[1])))
			{
				Console.WriteLine(Environment.NewLine + files[0] + Environment.NewLine + "---");
				dynamic result = JsonConvert.DeserializeObject(jsonFile.ReadToEnd());
				foreach(var s in result)
				{
					Console.WriteLine(s.Name);
					Assert.Equal((string)s.Value, p.Get<string>(s.Name));
				}
			}
		}
	}
}
