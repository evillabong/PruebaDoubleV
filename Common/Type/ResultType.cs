using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Type
{
    public enum ResultType
    {
        Success = 0,
        Error = 1,
        SessionFail= 98,
        InternalError = 99,
        ContextFail = 100,
        UnknowError = 101
    }
}
