using Common.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Result
{
    public class GetPersonsResult : BaseResult
    {
        public List<PersonBase> Persons { get;set;} = new List<PersonBase>();
    }
}
