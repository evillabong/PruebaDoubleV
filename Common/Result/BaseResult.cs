using Common.Type;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Result
{
    public class BaseResult
    {
        public ResultType ResultCode { get; set; } = 0;
        public string Message { get; set; } = string.Empty;
        public DateTimeOffset DateTimeOffset { get; set; } = DateTimeOffset.Now;
    }
}
