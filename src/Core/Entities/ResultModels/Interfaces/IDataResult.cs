using System;

namespace Core.Entities
{
    interface IDataResult<T> : IResult
    {
        public T Data { get; set; }
    }

}