using System;

namespace Core.Entities; 
public class ErrorResult
{
    public bool Success => false;
    public string Message { get; set; } = null!;
}