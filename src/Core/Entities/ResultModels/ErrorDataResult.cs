using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Core.Entities
{
    public class ErrorDataResult<T> : IDataResult<T>
    {
        public bool Success => false;
        public string Message { get; set; }
        public T Data { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
    public static class ModelStateExtensions
    {
        public static IEnumerable<string> GetErrors(this ModelStateDictionary modelState)       // for server-side error responses
        {
            return modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
        }
    }
}