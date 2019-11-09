using System;
using System.Collections.Generic;
using System.Text;

namespace Jaxosoft.CommandLineParser
{
    public class CommandLineArgumentInstance
    {
        internal CommandLineArgumentInstance()
        {
            Name = String.Empty;
        }

        public CommandLineArgumentInstance(string name)
        {
            Name = name;
        }

        public CommandLineArgumentDataType DataType { get; set; }
        public string Name { get; set; }
        public bool Exists { get; set; }
        public bool? BoolValue { get; set; }
        public List<string> Values { get; } = new List<string>();

        public object? ComputedValue
        {
            get
            {
                if (!Exists)
                    return null;
                if (BoolValue.HasValue)
                    return BoolValue.Value;
                if (Values.Count == 0)
                    return true;
                else if (Values.Count == 1)
                    return Values[0];
                else
                    return Values;
            }
        }

        public string? ComputedStringValue
        {
            get
            {
                if (!Exists)
                    return null;
                if (BoolValue.HasValue)
                    return BoolValue.Value ? Boolean.TrueString : Boolean.FalseString;
                if (Values.Count == 0)
                    return Boolean.TrueString;
                else if (Values.Count == 1)
                    return Values[0];
                else
                    return String.Join(',', Values);
            }
        }
    }
}
