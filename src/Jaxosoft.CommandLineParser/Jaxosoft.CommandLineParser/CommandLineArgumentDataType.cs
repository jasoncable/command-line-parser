using System;
using System.Collections.Generic;
using System.Text;

namespace Jaxosoft.CommandLineParser
{
    public enum CommandLineArgumentDataType
    {
        Unspecified = 0,
        Boolean = 1,
        String = 2,
        Date = 3,
        Uri = 4,
        Path = 5
    }
}
