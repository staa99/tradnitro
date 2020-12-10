using System;


namespace Tradnitro.Shared.Exceptions
{
    public class TradnitroException : ApplicationException
    {
        public TradnitroException(ErrorCode code, string message) : base(message)
        {
            Code = code;
        }


        public ErrorCode Code { get; }
    }
}