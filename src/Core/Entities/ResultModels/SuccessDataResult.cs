using System;

namespace Core.Entities
{
    public class SuccessDataResult<T> 
    {
        public bool Success => true;
        public string Message { get; set; }
        public T Data { get; set; }
    }
}