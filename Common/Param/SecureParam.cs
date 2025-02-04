using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Param
{
    public class SecureParam : BaseParam
    {
        public string Data { get; set; } = null!;
        public string Key { get; set; } = null!;
    }
}
