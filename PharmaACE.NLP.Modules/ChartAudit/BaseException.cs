using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.ChartAudit.NLIDB.ChartAudit
{
    public abstract class BaseException : Exception
    {
        public abstract BaseErrorCode ErrorCode { get; }

        protected BaseException() : base()
        {
        }
        protected BaseException(string message) : base(message)
        {
        }
        protected BaseException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class NoDataFoundException : BaseException
    {
        public override BaseErrorCode ErrorCode { get { return BaseErrorCode.NoDataFoundException; } }

        public NoDataFoundException() : base()
        {
        }
        public NoDataFoundException(string message) : base(message)
        {
        }
        public NoDataFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class MailNotSentException : BaseException
    {
        public override BaseErrorCode ErrorCode { get { return BaseErrorCode.MailNotSentException; } }

        public MailNotSentException() : base()
        {
        }
        public MailNotSentException(string message) : base(message)
        {
        }
        public MailNotSentException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class InvalidParameterException : BaseException
    {
        public override BaseErrorCode ErrorCode { get { return BaseErrorCode.InvalidParameterException; } }

        public InvalidParameterException() : base()
        {
        }
        public InvalidParameterException(string message) : base(message)
        {
        }
        public InvalidParameterException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class PermissionDeniedException : BaseException
    {
        public override BaseErrorCode ErrorCode { get { return BaseErrorCode.PermissionDeniedException; } }

        public PermissionDeniedException() : base()
        {
        }
        public PermissionDeniedException(string message) : base(message)
        {
        }
        public PermissionDeniedException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public enum BaseErrorCode
    {
        NoDataFoundException = 1,
        MailNotSentException = 2,
        InvalidParameterException = 3,
        PermissionDeniedException = 16
    }
}
