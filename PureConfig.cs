using System;
using System.IO;


namespace PureConfig
{
    public struct Quantity
	{
		
		public string unit;
		public double val;
		public double multiplier;

		public override string ToString()
		{
			return string.Format("{0}{1}(x{2})", val, unit, multiplier);
		}
	}

    public partial class Parser
    {
        public string FilePath{ get; private set; }

        public Parser(string configFile)
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
					throw new Exception("bad data");
			}
			else if(typeof(T) == typeof(int))
			{
				if(data.type != ConfigType.Decimal)
					throw new Exception("Requested int but type is ");
			}
			else if(typeof(T) == typeof(string))
			{
            if(data.type != ConfigType.String)
					throw new Exception("Requested int but type is ");
			}
			else if(typeof(T) == typeof(Quantity))
			{
				if(data.type != ConfigType.Quantity)
					throw new Exception("Requested int but type is ");
			}
			else if(typeof(T) == typeof(bool))
			{
				if(data.type != ConfigType.Bool)
					throw new Exception("Requested int but type is ");
			}
			else
			{
				throw new Exception("Requested data type is not supported");
			}
			return (T)Convert.ChangeType(data.data, typeof(T));
		}


    }

}