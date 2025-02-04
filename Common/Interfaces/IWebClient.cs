using Common.Param;
using Common.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IWebClient
    {
        Task<TResult> GetAsync<TResult>(string endpoint) where TResult : BaseResult;
        Task<TResult> GetAsync<TResult, TMethod>(TMethod endpoint) where TResult : BaseResult where TMethod : Enum;
        Task<TResult> PostAsync<TResult, TMethod, TParam>(TMethod endpoint, TParam data) where TResult : BaseResult where TMethod : Enum where TParam : BaseParam;
    }
}
