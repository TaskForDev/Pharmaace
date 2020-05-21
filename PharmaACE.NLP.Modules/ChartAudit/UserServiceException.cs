using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.ChartAudit.NLIDB.ChartAudit
{
    public abstract class UserServiceException : BaseException
    {
        public override BaseErrorCode ErrorCode { get; }

        public abstract UserServiceErrorCode ErrorCodeService { get; }

        protected UserServiceException() : base()
        {
        }
        protected UserServiceException(string message) : base(message)
        {
        }
        protected UserServiceException(string message, Exception inner) : base(message, inner)
        {
        }
    }
    public class UserNotValidException : UserServiceException
    {
        public override UserServiceErrorCode ErrorCodeService { get { return UserServiceErrorCode.UserNotValidException; } }

        public UserNotValidException() : base()
        {
        }
        public UserNotValidException(string message) : base(message)
        {
        }
        public UserNotValidException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class PasswordNotValidException : UserServiceException
    {
        public override UserServiceErrorCode ErrorCodeService { get { return UserServiceErrorCode.PasswordNotValidException; } }

        public PasswordNotValidException() : base()
        {
        }
        public PasswordNotValidException(string message) : base(message)
        {
        }
        public PasswordNotValidException(string message, Exception inner) : base(message, inner)
        {
        }
    }



    public class UserAlreadyPresentException : UserServiceException
    {
        public override UserServiceErrorCode ErrorCodeService { get { return UserServiceErrorCode.UserAlreadyPresentException; } }

        public UserAlreadyPresentException() : base()
        {
        }
        public UserAlreadyPresentException(string message) : base(message)
        {
        }
        public UserAlreadyPresentException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class GroupAlreadyPresentException : UserServiceException
    {
        public override UserServiceErrorCode ErrorCodeService { get { return UserServiceErrorCode.GroupAlreadyPresentException; } }

        public GroupAlreadyPresentException() : base()
        {
        }
        public GroupAlreadyPresentException(string message) : base(message)
        {
        }
        public GroupAlreadyPresentException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class NotificationNotSentException : UserServiceException
    {
        public override UserServiceErrorCode ErrorCodeService { get { return UserServiceErrorCode.NotificationNotSentException; } }

        public NotificationNotSentException() : base()
        {
        }
        public NotificationNotSentException(string message) : base(message)
        {
        }
        public NotificationNotSentException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public enum UserServiceErrorCode
    {
        UserNotValidException = 7,
        PasswordNotValidException = 8,
        UserAlreadyPresentException = 9,
        GroupAlreadyPresentException = 10,
        NotificationNotSentException=15
    }
}




