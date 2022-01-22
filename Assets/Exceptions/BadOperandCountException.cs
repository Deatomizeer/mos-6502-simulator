using System;

public class BadOperandCountException : Exception
{
    public BadOperandCountException(string message) : base(message) { }

}
