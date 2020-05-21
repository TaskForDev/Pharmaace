using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using PharmaACE.ChartAudit.Models;
using static PharmaACE.ChartAudit.Models.UserInfoModel;

namespace PharmaACE.ChartAudit.IRepository
{
    public interface IReportService
    {
        int AddNewReport(List<ReportInformation> reportInformation,int permission);
        
        List<ReportDetail> GetReportList(int permission);

        ReportDetail GetSpecificReport(int reportId,int permission);
        
        int UpdateReport(ReportDetail reportDetails,int permission);

        int DeleteReport(int reportId,int userId,int permission);

        List<ReportCategoryModel> GetCategoryList(int permission);

        int AddNewCategory(ReportCategoryModel reportCategoryModel,int permission);

        int AddNewSubCategory(ReportCategoryModel reportCategoryModel,int permission);

        ReportCategoryModel GetSpecificCategorywithSub(int categoryId,int permission);

        List<ReportSubCategoryModel> GetSubCategories(int permission);

        int UpdateReportCategory(ReportCategoryModel reportCategoryModel, int permission);

        int UpdateReportSubCategory(ReportCategoryModel reportCategoryModel,int permission);

        int DeleteSubCategory(int subCategoryId,int permission);

        int DeleteCategory(int categoryId,int permission);
      
        List<ReportDetail> GetReports(int userId,int permission);

        List<Categories> GetCategories(int permission);

        bool ReportView(int reportId, int userId,int permission);

        ReportAndSlideFavorite GetFavoriteReportsSlides(int userId,int permission);

        bool FavoriteReport(int reportId, int userId,int permission);

        ReportInfo GetReportSlides(int reportId, int userId,int permission);

        List<string> GetSlideImage(int ID);

        bool FavoriteSlide(int slideId, int userId,int permission);

        bool CommentLike(int commentId, int userId,int permission);

        void SendComment(CommentDetails cmtDl, int permission);

        bool ViewComment(int commentId, int userId, int permission);

        List<CommentDetails> GetFavoriteComments(int userId, int permission);

        List<CommentDetails> GetComments(int userId, int permission);

        List<CommentDetails> GetSubComments(int commentId, int permission);

        int DeleteComment(int commentId, int permission);

        List<ReportDetail> FilterReport(ReportFilter item, int permission);

        List<CommentDetails> FilterComment(CommentsFilter item, int permission);

        bool SendMailWithSlide(MailWithSlide mail, string path, int permission);
        bool SendMailWithChart(MailWithSlide mail, string path, int permission);
        List<CommentDetails> GetNotification(int userId, int permission);

        List<string> GetReportImage(List<int> slideIds, int permission);

        List<ReportDetail> GetLatestReports(int userId, int permission);
    }
}