using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace PharmaACE.NLP.ChartAudit.NLIDB.ChartAudit
    {
        public abstract class ReportServiceException : BaseException
        {

            public override BaseErrorCode ErrorCode { get; }

            public abstract ReportServiceErrorCode ErrorCodeService { get; }

            protected ReportServiceException() : base()
            {
            }
            protected ReportServiceException(string message) : base(message)
            {
            }
            protected ReportServiceException(string message, Exception inner) : base(message, inner)
            {
            }
        }


        public class CountMismatchException : ReportServiceException
        {
            public override ReportServiceErrorCode ErrorCodeService { get { return ReportServiceErrorCode.CountMismatchException; } }

            public CountMismatchException() : base()
            {
            }
            public CountMismatchException(string message) : base(message)
            {
            }
            public CountMismatchException(string message, Exception inner) : base(message, inner)
            {
            }
        }

        public class FormatNotSupportedException : ReportServiceException
        {
            public override ReportServiceErrorCode ErrorCodeService { get { return ReportServiceErrorCode.FormatNotSupportedException; } }

            public FormatNotSupportedException() : base()
            {
            }
            public FormatNotSupportedException(string message) : base(message)
            {
            }
            public FormatNotSupportedException(string message, Exception inner) : base(message, inner)
            {
            }
        }
        public class FileNameNotValidException : ReportServiceException
        {
            public override ReportServiceErrorCode ErrorCodeService { get { return ReportServiceErrorCode.FileNameNotValidException; } }

            public FileNameNotValidException() : base()
            {
            }
            public FileNameNotValidException(string message) : base(message)
            {
            }
            public FileNameNotValidException(string message, Exception inner) : base(message, inner)
            {
            }
        }

        public class CategoryAlreadyPresentException : ReportServiceException
        {
            public override ReportServiceErrorCode ErrorCodeService { get { return ReportServiceErrorCode.CategoryAlreadyPresentException; } }

            public CategoryAlreadyPresentException() : base()
            {
            }
            public CategoryAlreadyPresentException(string message) : base(message)
            {
            }
            public CategoryAlreadyPresentException(string message, Exception inner) : base(message, inner)
            {
            }
        }

        public class SubCategoryAlreadyPresentException : ReportServiceException
        {
            public override ReportServiceErrorCode ErrorCodeService { get { return ReportServiceErrorCode.SubCategoryAlreadyPresentException; } }

            public SubCategoryAlreadyPresentException() : base()
            {
            }
            public SubCategoryAlreadyPresentException(string message) : base(message)
            {
            }
            public SubCategoryAlreadyPresentException(string message, Exception inner) : base(message, inner)
            {
            }
        }

        public class InvalidFileSizeException : ReportServiceException
        {
            public override ReportServiceErrorCode ErrorCodeService { get { return ReportServiceErrorCode.InvalidFileSizeException; } }

            public InvalidFileSizeException() : base()
            {
            }
            public InvalidFileSizeException(string message) : base(message)
            {
            }
            public InvalidFileSizeException(string message, Exception inner) : base(message, inner)
            {
            }
        }

        public class AsposeBlockException : ReportServiceException
        {
            public override ReportServiceErrorCode ErrorCodeService { get { return ReportServiceErrorCode.AsposeBlockException; } }

            public AsposeBlockException() : base()
            {
            }
            public AsposeBlockException(string message) : base(message)
            {
            }
            public AsposeBlockException(string message, Exception inner) : base(message, inner)
            {
            }
        }

        public enum ReportServiceErrorCode
        {
            CountMismatchException = 4,
            FormatNotSupportedException= 5,
            FileNameNotValidException = 6,
            CategoryAlreadyPresentException=11,
            SubCategoryAlreadyPresentException=12,
            InvalidFileSizeException=13,
            AsposeBlockException=14,
        }
    
}






