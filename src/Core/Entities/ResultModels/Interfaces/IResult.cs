using System;

namespace Core.Entities
{
    interface IResult
    {
        public bool Success { get; }
        public string Message { get; set; }
    }
}