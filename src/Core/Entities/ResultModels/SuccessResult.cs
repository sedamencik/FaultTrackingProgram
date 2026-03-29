using System;

namespace Core.Entities
{
    public class SuccessResult : IResult
    {
        public bool Success => true;
        public string Message { get; set; }
    }
}