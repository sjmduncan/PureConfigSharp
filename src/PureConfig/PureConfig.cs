using System;
using System.IO;


namespace PureConfigNet
{
	public struct Quantity
	{
		
		public string unit;
		public double val;

		public override string ToString()
		{
			return string.Format("{0}{1}", val, unit);
		}
	}

	public partial class PureConfig
	{
		public string FilePath{ get; private set; }

		public PureConfig(string configFile)
		{
			if(!File.Exists(configFile))
				throw new Exception("File '" + configFile + "' does not exist");

			FilePath = configFile;

			using(StreamReader file = new StreamReader(File.OpenRead(configFile)))
			{
				ParseFile(file);
			}
		}

		public T Get<T>(string id)
		{
			ConfigData data = TryGetVal(id);
			if(typeof(T) == typeof(double) || typeof(T) == typeof(float))
			{
				if(data.type != ConfigType.Decimal)
					throw new Exception("Requested " + typeof(T) + " but key is of type " + data.type);
			}
			else if(typeof(T) == typeof(int))
			{
				if(data.type != ConfigType.Decimal)
					throw new Exception("Requested " + typeof(T) + " but key is of type " + data.type);
			}
			else if(typeof(T) == typeof(string))
			{
			if(data.type != ConfigType.String)
					throw new Exception("Requested " + typeof(T) + " but key is of type " + data.type);
			}
			else if(typeof(T) == typeof(Quantity))
			{
				if(data.type != ConfigType.Quantity)
					throw new Exception("Requested " + typeof(T) + " but key is of type " + data.type);
			}
			else if(typeof(T) == typeof(bool))
			{
				if(data.type != ConfigType.Bool)
					throw new Exception("Requested " + typeof(T) + " but key is of type " + data.type);
			}
			else
			{
				throw new Exception("Requested data type (" + typeof(T) + ") is not supported");
			}
			return (T)Convert.ChangeType(data.data, typeof(T));
		}


	}

}