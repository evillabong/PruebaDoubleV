using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Result
{
    public class GetLoginResult : BaseResult
    {
        public string Token { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTimeOffset TokenExpire { get; set; }

    }
}
