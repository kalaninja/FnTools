using System;

namespace FnTools.Exceptions
{
    public class NoSuchElementException : InvalidOperationException
    {
        public NoSuchElementException() : base(ExceptionMessages.NoSuchElement)
        {
        }

        public NoSuchElementException(string? message) : base(message)
        {
        }
    }
}