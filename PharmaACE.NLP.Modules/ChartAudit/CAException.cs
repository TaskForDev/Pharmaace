using System;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    public abstract class ChartAuditExceptionBase : Exception
    {
        public abstract ChartAuditErrorCode ErrorCode { get; }

        protected ChartAuditExceptionBase() : base()
        {
        }
        protected ChartAuditExceptionBase(string message) : base(message)
        {
        }
        protected ChartAuditExceptionBase(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class ChartAuditDimensionUnidentifiedException : ChartAuditExceptionBase
    {
        public override ChartAuditErrorCode ErrorCode { get { return ChartAuditErrorCode.DimensionUnidentified; } }

        public ChartAuditDimensionUnidentifiedException() : base()
        {
        }
        public ChartAuditDimensionUnidentifiedException(string message) : base(message)
        {
        }
        public ChartAuditDimensionUnidentifiedException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class ChartAuditMultiTumorMultiRegimenTimeSeriesException : ChartAuditExceptionBase
    {
        public override ChartAuditErrorCode ErrorCode { get { return ChartAuditErrorCode.MultipleTumorTimeSeries; } }

        public ChartAuditMultiTumorMultiRegimenTimeSeriesException() : base()
        {
        }
        public ChartAuditMultiTumorMultiRegimenTimeSeriesException(string message) : base(message)
        {
        }
        public ChartAuditMultiTumorMultiRegimenTimeSeriesException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class ChartAuditMultipleMeasureException : ChartAuditExceptionBase
    {
        public override ChartAuditErrorCode ErrorCode { get { return ChartAuditErrorCode.DimensionUnidentified; } }

        public ChartAuditMultipleMeasureException() : base()
        {
        }
        public ChartAuditMultipleMeasureException(string message) : base(message)
        {
        }
        public ChartAuditMultipleMeasureException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class InvalidRefineEntityException : ChartAuditExceptionBase
    {
        public override ChartAuditErrorCode ErrorCode { get { return ChartAuditErrorCode.InvalidRefineEntity; } }

        public InvalidRefineEntityException() : base()
        {
        }
        public InvalidRefineEntityException(string message) : base(message)
        {
        }
        public InvalidRefineEntityException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class ChartAuditNoDataException : ChartAuditExceptionBase
    {
        public override ChartAuditErrorCode ErrorCode { get { return ChartAuditErrorCode.NoDataFound; } }

        public ChartAuditNoDataException() : base()
        {
        }
        public ChartAuditNoDataException(string message) : base(message)
        {
        }
        public ChartAuditNoDataException(string message, Exception inner) : base(message, inner)
        {
        }
    }
    


    public enum ChartAuditErrorCode
    {
        None,
        DimensionUnidentified,
        MultipleMeasure,
        MultipleTumorTimeSeries,
        InvalidRefineEntity,
        NoDataFound
    }
}
