using System;

public class EmptyStackException : Exception
{
    public EmptyStackException(string message) : base(message) { }

}
