using System;

public class BadJumpAddressException : Exception
{
    public BadJumpAddressException(string message) : base(message) { }

}
