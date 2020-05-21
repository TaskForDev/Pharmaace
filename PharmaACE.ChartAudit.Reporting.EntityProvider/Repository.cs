using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App;

namespace PharmaACE.ChartAudit.Reporting.EntityProvider
{
    public static class Repository
    {
        public static string ChartAuditModelConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
        }

    }

    public interface IUnitOfWork
    {
        void BeginTransaction();

        void Commit();

        void RollBack();

        ChartAuditModel DbContext { get; }


    }

    public class UnitOfWork: IUnitOfWork
    {

        private static IUnitOfWork instance;

        private DbContextTransaction masterTransaction;

        public static IUnitOfWork GetInstance()
        {
            return instance;
        }

        public ChartAuditModel DbContext
        {
            get;

            set;
        }

        public UnitOfWork()
        {
            instance = this;
            if (DbContext == null)
                DbContext = new ChartAuditModel(Repository.ChartAuditModelConnectionString);
        }
        private void Init()
        {
            instance = this;
            if (DbContext == null)
                DbContext = new ChartAuditModel(Repository.ChartAuditModelConnectionString);

        }

        public void BeginTransaction()
        {
            Init();
            if (DbContext != null && DbContext.Database.CurrentTransaction == null)
                masterTransaction = DbContext.Database.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                // commit transaction if there is one active                

                if (masterTransaction != null && DbContext.Database.CurrentTransaction != null)
                    masterTransaction.Commit();
            }
            catch
            {
                // rollback if there was an exception                

                if (masterTransaction != null && DbContext.Database.CurrentTransaction != null)
                    masterTransaction.Rollback();

                throw;
            }
            finally
            {
                Dispose();
            }
        }

        public void RollBack()
        {
            try
            {
                
                if (masterTransaction != null && DbContext.Database.CurrentTransaction != null)
                    masterTransaction.Rollback();
            }
            finally
            {
                Dispose();
            }
        }

        private void Dispose()
        {
            if (DbContext != null)
            {
                DbContext.Dispose();
                DbContext = null;
            }
        }
    }
}
