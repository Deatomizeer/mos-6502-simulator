using System;

public class BadOperandTypeException : Exception
{
    public BadOperandTypeException(string message) : base(message) { }

}
