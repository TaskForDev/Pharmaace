using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PharmaACE.ChartAudit.IRepository;
using PharmaACE.ChartAudit.Models;
using PharmaACE.ChartAudit.Reporting.EntityProvider;
using PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App;
using System.Drawing;
using Aspose.Slides;
using PharmaACE.Utility;
using NLog;
using System.Drawing.Imaging;
using System.Net.Mail;
using PharmaACE.ChartAudit.Utility;
using System.Net.Mime;
using System.Text;
using PharmaACE.NLP.ChartAudit.NLIDB.ChartAudit;


namespace PharmaACE.ChartAudit.Repository
{
  public class ReportService :IReportService
    {
        public IUnitOfWork unitOfWork { get; set; }
        
        public ReportService()
        {


        }

        public ReportService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

        }


        public static Logger logger = LogManager.GetCurrentClassLogger();
        public int AddNewReport(List<ReportInformation> reportInformation,int permission)
        {
            logger.Info("Inside ReportService/AddNewReport");
            int result = 0;
           
            try
            {
                if (permission > 0 && permission < 3)
                {
                    foreach (var reports in reportInformation)
                    {
                        

                            Report report = new Report
                            {
                                SubcategoryID = reports.ReportDetail.SubcategoryId,
                                Description = reports.ReportDetail.Description,
                                CreationDate = DateTime.UtcNow,
                                Author = reports.ReportDetail.AuthorId,
                                IsPublic = reports.ReportDetail.IsPublic
                            };

                            if (reports.ReportDetail.GroupIds.AnyOrNotNull())
                            {

                                foreach (int groupId in reports.ReportDetail.GroupIds)
                                {
                                    ReportGroup reportGroup = new ReportGroup
                                    {
                                        Report = report,

                                        GroupID = groupId
                                    };
                                    unitOfWork.DbContext.ReportGroup.Add(reportGroup);
                                }
                            }
                            try
                            {
                                using (Presentation presentation = new Presentation(reports.FileStream))
                                {

                                    //presentation.SlideSize.SetSize(SlideSizeType., SlideSizeScaleType.EnsureFit);

                                    for (int k = 0; k < presentation.Slides.Count; k++)
                                    {
                                        var slide = presentation.Slides[k];
                                        Size size = new Size();
                                        size.Width = reports.ReportDetail.Width;
                                        size.Height = reports.ReportDetail.Height;
                                        var bitmap = slide.GetThumbnail(size);
                                        MemoryStream memoryStream = new MemoryStream();
                                        bitmap.Save(memoryStream, ImageFormat.Jpeg);
                                        var streamToStore = memoryStream.ToArray();

                                        ReportSlide reportSlide = new ReportSlide
                                        {
                                            Report = report,
                                            Slide = streamToStore,
                                            Order = k + 1
                                        };
                                        unitOfWork.DbContext.ReportSlide.Add(reportSlide);
                                    }
                                }


                                unitOfWork.DbContext.SaveChanges();
                                result = 1;
                            }
                            catch (Exception ex)
                            {
                                logger.Info("Exception in Aspose Block");
                                throw ex;
                            }


                    }
                    return result;
                }  
                else
                {
                    throw new PermissionDeniedException();
                }

            }

            catch(PermissionDeniedException ex)
            {
                throw ex;
            }
       
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public List<ReportDetail> GetReportList(int permission)
        {
            logger.Info("Inside ReportService/GetReportList");
            
            List<ReportDetail> reportDetails = null;
            try
            {
                if (permission > 0 && permission < 3)
                {
                    logger.Info("Getting All ReportList");
                    reportDetails = unitOfWork.DbContext.Report
                     .Select(rd => new ReportDetail
                     {
                         ReportId = rd.ID,

                         SubCategoryName = rd.ReportSubCategory.Name,

                         SubcategoryId = rd.SubcategoryID,

                         Description = rd.Description,

                         IsPublic = rd.IsPublic,

                         AuthorId = rd.Author,

                         GroupIds = rd.ReportGroup.Select(rg => rg.GroupID).ToList(),

                         Author = rd.UserDetail.Name,

                         CreationDate = rd.CreationDate
                     }
                    ).OrderByDescending(r => r.CreationDate).ToList();

                    return reportDetails;
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(PermissionDeniedException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public ReportDetail GetSpecificReport(int reportId, int permission)
        {
            ReportDetail reportDetail = null;
            try
            {
                  
                    if(permission > 0 && permission < 3)
                    {
                        reportDetail = unitOfWork.DbContext.Report
                           .Where(r => r.ID == reportId)
                               .Select(rd => new ReportDetail
                               {
                                   ReportId = reportId,
                                   CategoryId = rd.ReportSubCategory.ReportCategoryID,
                                   SubcategoryId = rd.SubcategoryID,
                                   Description = rd.Description,
                                   IsPublic = rd.IsPublic,
                                   GroupInfos = rd.ReportGroup
                                            .Select(rg => new GroupInfo
                                            {
                                                GroupId = rg.GroupID,
                                                GroupName = rg.Group.Name
                                            })
                                            .ToList(),
                                   AuthorId = rd.Author

                               }
                               ).FirstOrDefault();

                        return reportDetail;
                    }
                    else
                    {
                        throw new PermissionDeniedException();
                    }
                                
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int UpdateReport(ReportDetail reportDetail, int permission)
        {
            int result = 0;

            try
            {
                if(permission > 0 && permission < 3)
                {
                    
                        var reportDetails = unitOfWork.DbContext.Report.Where(r => r.ID == reportDetail.ReportId).FirstOrDefault();
                        if (reportDetails != null)
                        {
                            reportDetails.Author = reportDetail.AuthorId;
                            reportDetails.Description = reportDetail.Description;
                            reportDetails.SubcategoryID = reportDetail.SubcategoryId;
                            reportDetails.IsPublic = reportDetail.IsPublic;

                            List<int> newGroupList = reportDetail.GroupIds;

                            List<int> oldGroupList = unitOfWork.DbContext.ReportGroup
                                .Where(rg => rg.ReportID == reportDetail.ReportId)
                                .Select(rg => rg.GroupID)
                                .ToList();
                            List<int> firstNotSecond = newGroupList.Except(oldGroupList).ToList();
                            List<int> secondNotFirst = oldGroupList.Except(newGroupList).ToList();

                            foreach (var groupId in firstNotSecond)
                            {
                                ReportGroup ReportGp = new ReportGroup()
                                {
                                    ReportID = reportDetail.ReportId,
                                    GroupID = groupId

                                };
                                unitOfWork.DbContext.ReportGroup.Add(ReportGp);
                            }

                            var oldGroupIds = unitOfWork.DbContext.ReportGroup.Where(rg => secondNotFirst.Contains(rg.GroupID) && rg.ReportID == reportDetails.ID).ToList();
                            unitOfWork.DbContext.ReportGroup.RemoveRange(oldGroupIds);


                            unitOfWork.DbContext.SaveChanges();

                            result = 1;
                        }
                        else
                        {
                            throw new NoDataFoundException();
                        }
                    
                    return result;
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            
            catch (Exception ex)
            {
                throw ex;
            }


        }


        public int DeleteReport(int reportId,int userId, int permission)
        {
            int result = 0;
            try
            {
                if(permission > 0 && permission <3)
                {
                    
                        var report = unitOfWork.DbContext.Report
                                .Where(re => re.ID == reportId)
                                .Select(re => new
                                {
                                    reportRecord = re,
                                    reportViewRecordList = re.ReportView,
                                    reportslideRecordList = re.ReportSlide,
                                    slideFavouriteRecordList = unitOfWork.DbContext.SlideFavorite
                                                                .Where(sf => sf.ReportSlide.ReportID == re.ID),

                                    reportGroupRecordList = re.ReportGroup,
                                    ReportFavouriteRecordList = re.ReportFavorite,
                                }).FirstOrDefault();

                        if(report == null)
                        {
                            throw new NoDataFoundException();
                        }

                        unitOfWork.DbContext.ReportView.RemoveRange(report.reportViewRecordList);
                        unitOfWork.DbContext.SlideFavorite.RemoveRange(report.slideFavouriteRecordList);
                        unitOfWork.DbContext.ReportSlide.RemoveRange(report.reportslideRecordList);
                        unitOfWork.DbContext.ReportGroup.RemoveRange(report.reportGroupRecordList);
                        unitOfWork.DbContext.ReportFavorite.RemoveRange(report.ReportFavouriteRecordList);
                        unitOfWork.DbContext.Report.Remove(report.reportRecord);

                        unitOfWork.DbContext.SaveChanges();
                        result = 1;

                        return result;
                   
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<ReportCategoryModel> GetCategoryList(int permission)
        {
            List<ReportCategoryModel> ReportCategoryModels = null;
            try
            {
                if(permission > 0 && permission < 3)
                {
                    ReportCategoryModels = unitOfWork.DbContext.ReportCategory
                     .Select(rc => new ReportCategoryModel
                     {
                         CategoryId = rc.ID,
                         CategoryName = rc.Name,
                         CreationDate = rc.CreationDate,
                         SubCategoryCount = rc.ReportSubCategory.Count()
                     }
                    ).OrderByDescending(rs => rs.CreationDate).ToList();

                    return ReportCategoryModels;
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int AddNewCategory(ReportCategoryModel reportCategoryModel, int permission)
        {
            int result = 0;
            try
            {
                if (permission > 0 && permission < 3)
                {

                        var category = unitOfWork.DbContext.ReportCategory
                                         .Any(rc => String.Compare(rc.Name, reportCategoryModel.CategoryName, true ) == 0
                                         );
                        if (!category)
                        {
                            ReportCategory Report = new ReportCategory
                            {
                                Name = reportCategoryModel.CategoryName,
                                CreationDate = DateTime.UtcNow
                            };
                            unitOfWork.DbContext.ReportCategory.Add(Report);
                            unitOfWork.DbContext.SaveChanges();
                            result = 1;
                        }
                        else
                        {
                            throw new CategoryAlreadyPresentException();
                        }

                    return result;
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(CategoryAlreadyPresentException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int AddNewSubCategory(ReportCategoryModel reportCategoryModel, int permission)
        {
            int result = 0;
            try
            {
                if(permission > 0 && permission <3)
                {
                   

                       // string subcategoryName = reportCategoryModel.ReportSubCategoryModel.Select(rsm => rsm.SubCategoryName).FirstOrDefault();

                        var subCategory = unitOfWork.DbContext.ReportSubCategory
                                         .Any(rsc => String.Compare(rsc.Name, reportCategoryModel.CategoryName, true) == 0
                                         );

                        if (!subCategory)
                        {
                            ReportSubCategory category = new ReportSubCategory
                            {
                                ReportCategoryID = reportCategoryModel.CategoryId,
                                //Name = reportCategoryModel.ReportSubCategoryModel.Select(rsm => rsm.SubCategoryName).FirstOrDefault(),
                                Name=reportCategoryModel.CategoryName,
                                CreationDate = DateTime.UtcNow
                            };
                            unitOfWork.DbContext.ReportSubCategory.Add(category);
                            unitOfWork.DbContext.SaveChanges();
                            result = 1;
                        }
                        else
                        {
                            throw new SubCategoryAlreadyPresentException();
                        }
                    
                    return result;
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(ReportServiceException ex)
            {
                throw ex;
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }

        public ReportCategoryModel GetSpecificCategorywithSub(int categoryId, int permission)
        {

            try
            {
                if(permission > 0 && permission < 3)
                {
                    ReportCategoryModel reportCategoryModel = null;
                    if (categoryId > 0)
                    {

                        reportCategoryModel = unitOfWork.DbContext.ReportCategory.Where(rc => rc.ID == categoryId)
                             .Select(rc => new ReportCategoryModel
                             {
                                 CategoryId = rc.ID,
                                 CategoryName = rc.Name,
                                 ReportSubCategoryModel = rc.ReportSubCategory
                                 .Select(rs => new ReportSubCategoryModel
                                 {
                                     SubCategoryId = rs.ID,
                                     SubCategoryName = rs.Name,
                                     CreationDate = rs.CreationDate
                                 }).OrderByDescending(rs => rs.CreationDate).ToList()
                             }).FirstOrDefault();
                        return reportCategoryModel;
                    }
                    else
                    {
                        return reportCategoryModel;
                    }
                }
               else
                {
                    logger.Info("Permission has been denied ");
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ReportSubCategoryModel> GetSubCategories(int permission)
        {
            try
            {
                if(permission > 0 && permission < 3)
                {
                    List<ReportSubCategoryModel> reportSubCategoryModel = null;

                    reportSubCategoryModel = unitOfWork.DbContext.Report
                        .Select(rsc => new ReportSubCategoryModel
                        {
                            SubCategoryId = rsc.SubcategoryID
                        }).Distinct().ToList();

                    return reportSubCategoryModel;
                }
                else
                {
                    logger.Info("Permission has been denied ");
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }


        public List<Categories> GetReportCategories(int permission)
        {
            try
            {
                if(permission > 0 && permission <3)
                {
                    List<Categories> categories = new List<Categories>();

                    categories = unitOfWork.DbContext.ReportCategory
                                        .Select(rc => new Categories
                                        {
                                            CategoryID = rc.ID,
                                            CategoryName = rc.Name,
                                            SubCategories = rc.ReportSubCategory.Select(src =>
                                                    new SubCategories
                                                    {
                                                        SubCategoryId = src.ID,
                                                        SubCategoryName = src.Name
                                                    }
                                                ).ToList()

                                        }).ToList();

                    return categories;
                }
                else
                {
                    logger.Info("Permission has been denied ");
                    throw new PermissionDeniedException();
                }
            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateReportCategory(ReportCategoryModel reportCategoryModel,int permission)
        {
            int result = 0;
            try
            {
                                   
                        var categoryDetail = unitOfWork.DbContext.ReportCategory.Where(rc => rc.ID == reportCategoryModel.CategoryId).FirstOrDefault();
                        if (categoryDetail != null)
                        {
                            categoryDetail.Name = reportCategoryModel.CategoryName;
                            unitOfWork.DbContext.SaveChanges();
                            result = 1;
                        }
                        else
                        {
                            throw new NoDataFoundException();
                        }
                
                return result;
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateReportSubCategory(ReportCategoryModel reportCategoryModel,int permission)
        {
            int result = 0;
            try
            {
                if (permission > 0 && permission < 3)
                {
                        var subCategoryPresent = unitOfWork.DbContext.ReportSubCategory
                                         .Any(rsc => String.Compare(rsc.Name, reportCategoryModel.CategoryName, true) == 0
                                         );
                        if (!subCategoryPresent)
                        {
                            var subCategoryDetail = unitOfWork.DbContext.ReportSubCategory.Where(rsc => rsc.ID == reportCategoryModel.CategoryId).FirstOrDefault();
                            if (subCategoryDetail != null)
                            {
                                subCategoryDetail.Name = reportCategoryModel.CategoryName;
                                unitOfWork.DbContext.SaveChanges();
                                result = 1;
                            }
                            else
                            {
                                throw new NoDataFoundException();
                            }
                        }
                        else
                        {
                            throw new SubCategoryAlreadyPresentException();
                        }
                    
                    return result;
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch (ReportServiceException ex)
            {
                throw ex;
            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public int DeleteSubCategory(int subCategoryId, int permission)
        {
            int result;
            try
            {
                if(permission > 0 && permission < 3)
                {
                   

                        var subCategory = unitOfWork.DbContext.ReportSubCategory.Where(rsc => rsc.ID == subCategoryId).FirstOrDefault();
                        if (subCategory != null)
                        {
                            unitOfWork.DbContext.ReportSubCategory.Remove(subCategory);
                            unitOfWork.DbContext.SaveChanges();
                            result = 1;
                        }
                        else
                        {
                            throw new NoDataFoundException();
                        }

                   
                    return result;
                }
                else
                {
                    throw new PermissionDeniedException();
                }               
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public int DeleteCategory(int categoryId, int permission)
        {
            int result = 0;
            try
            {
                if(permission > 0 && permission < 3)
                {
                    
                        var subCategoryList = unitOfWork.DbContext.ReportSubCategory.Where(rsc => rsc.ReportCategoryID == categoryId).ToList();
                        unitOfWork.DbContext.ReportSubCategory.RemoveRange(subCategoryList);
                        var category = unitOfWork.DbContext.ReportCategory.Where(rc => rc.ID == categoryId).FirstOrDefault();
                        if (category != null)
                        {
                            unitOfWork.DbContext.ReportCategory.Remove(category);
                            unitOfWork.DbContext.SaveChanges();
                            result = 1;
                        }
                        else
                        {
                            throw new NoDataFoundException();
                        }
                    
                    
                    return result;
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<ReportDetail> GetReports(int userid,int permission)
        {
            logger.Info("Inside ReportService/GetReports");
            logger.Info("userid:{0}", userid);
            List<ReportDetail> reportDetails = new List<ReportDetail>();

            try
            {
                if(permission > 0 && permission < 11)
                {
                    if (userid > 0)
                    {
                        DateTime currentDate = DateTime.Now;
                        int currentMonth = currentDate.Month;
                        var latestReportDateFromDB = unitOfWork.DbContext.Report
                                                    .OrderByDescending(re => re.CreationDate).FirstOrDefault();
                        if (latestReportDateFromDB != null)
                        {
                            DateTime latestRecordDate = latestReportDateFromDB.CreationDate;
                            var latestRecordMonth = latestRecordDate.Month;
                            var month = Math.Abs((currentDate.Month - latestRecordDate.Month) + 12 * (currentDate.Year - latestRecordDate.Year));
                            logger.Info("month:{0}", month);
                            if (month <= 12)
                            {
                                logger.Info("Getting ReportDetail List.");
                                reportDetails = unitOfWork.DbContext.Report
                                                          .Where(re => re.CreationDate.Month == latestRecordMonth
                                                                        && (re.IsPublic == true ||
                                                                        (re.IsPublic == false && (re.ReportGroup.Any(rg => rg.Group.UserGroup.Any(ug => ug.UserID == userid)))
                                                                        )))
                                                         .Select(re => new ReportDetail()
                                                         {
                                                             ReportId = re.ID,
                                                             SubcategoryId = re.SubcategoryID,
                                                             Description = re.Description,

                                                             IsPublic = re.IsPublic,
                                                             CreationDate = re.CreationDate,
                                                             tempImage = re.ReportSlide.Where(rs => rs.Order == 2)
                                                                        .Select(rs => rs.Slide).FirstOrDefault(),
                                                             IsViewed = re.ReportView
                                                                           .Where(rv => rv.UserID == userid)
                                                                           .Select(rv => rv.ViewCount)
                                                                           .Any(),
                                                             IsLiked = re.ReportFavorite
                                                                         .Where(rf => rf.UserID == userid)
                                                                         .Any()
                                                         }).OrderByDescending(re => re.CreationDate).ToList();
                            }
                            if (reportDetails != null)
                            {
                                logger.Info("Successfully got report list,Total report count:{0}", reportDetails.Count);
                            }
                        }
                    }
                    if (reportDetails.AnyOrNotNull())
                    {
                        foreach (var report in reportDetails)
                        {
                            if (report != null)
                            {
                                logger.Info("Converting image byte array to base 64 string for ReportId:{0}", report.ReportId);
                                report.Image = Convert.ToBase64String(report.tempImage);
                                report.tempImage = new byte[0];
                                logger.Info("Successfully converted byte array to string");

                            }
                        }
                    }
                    return reportDetails;
                }
                else
                {

                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ReportDetail> GetLatestReports(int userId, int permission)
        {
            logger.Info("Inside ReportService/GetReports");
            logger.Info("userid:{0}", userId);
            //CategoryReportDetails reportDetails = new CategoryReportDetails();
            List<ReportDetail> reportDetails = new List<ReportDetail>();
            try
            {
                if (permission > 0 && permission < 11)
                {
                    if (userId > 0)
                    {
                      { 
                        
                            List<Report> latestReports = unitOfWork.DbContext.Report
                                                          .Where(re =>(re.IsPublic == true ||
                                                                        (re.IsPublic == false && (re.ReportGroup.Any(rg => rg.Group.UserGroup.Any(ug => ug.UserID == userId)))
                                                                        ))).OrderByDescending(p => p.CreationDate).ToList();
                                              
                                logger.Info("Getting ReportDetail List.");

                            reportDetails = latestReports.GroupBy(p => p.SubcategoryID)
                                .Select(re => re.FirstOrDefault())
                                                        .Select(re => new ReportDetail()
                                                        {
                                                            ReportId = re.ID,
                                                            SubcategoryId = re.SubcategoryID,
                                                            Description = re.Description,

                                                            IsPublic = re.IsPublic,
                                                            CreationDate = re.CreationDate,
                                                            tempImage = re.ReportSlide.Where(rs => rs.Order == 2)
                                                                       .Select(rs => rs.Slide).FirstOrDefault(),
                                                            IsViewed = re.ReportView
                                                                          .Where(rv => rv.UserID == userId)
                                                                          .Select(rv => rv.ViewCount)
                                                                          .Any(),
                                                            IsLiked = re.ReportFavorite
                                                                        .Where(rf => rf.UserID == userId)
                                                                        .Any()
                                                        }).ToList();
                        }
                    }
                }
                
                if (reportDetails.AnyOrNotNull())
                {
                    foreach (var report in reportDetails)
                    {
                        if (report != null)
                        {
                            logger.Info("Converting image byte array to base 64 string for ReportId:{0}", report.ReportId);
                            report.Image = Convert.ToBase64String(report.tempImage);
                            report.tempImage = new byte[0];
                            logger.Info("Successfully converted byte array to string");

                        }
                    }
                }

                return reportDetails;
            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Categories> GetCategories(int permission)
        {
            logger.Info("Inside ReportService/GetCategories");

            List<Categories> categories = new List<Categories>();
            try
            {
                if(permission > 0 && permission < 11)
                {
                    logger.Info("Getting Categories List.");
                    categories = unitOfWork.DbContext.ReportCategory
                                       .Select(rc => new Categories
                                       {
                                           CategoryID = rc.ID,
                                           CategoryName = rc.Name,
                                           CreationDate = rc.CreationDate,
                                           SubCategories = rc.ReportSubCategory.Select(src =>
                                                   new SubCategories
                                                   {
                                                       SubCategoryId = src.ID,
                                                       SubCategoryName = src.Name,
                                                       CreationDate = src.CreationDate,
                                                   }
                                               ).OrderByDescending(src => src.CreationDate).ToList()

                                       }).OrderByDescending(rc => rc.CreationDate).ToList();
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return categories;
            
        }

        public List<string> GetSlideImage(int ID)
        {
            List<string> res = new List<string>();
              byte[] byteImage = unitOfWork.DbContext.ReportSlide
                                  .Where(rs => rs.ID == ID).Select(rs => rs.Slide).FirstOrDefault();
                string base64Str = null;
                try
                {
                    if (byteImage != null)
                    {
                        base64Str = Convert.ToBase64String(byteImage);
                        res.Add(base64Str);
                    }
                    else
                    {
                       throw new NoDataFoundException();
                    }
                }
                catch(BaseException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    logger.Error("Could not cast image to string : " + ex.ToString());
                }

                return res;
            
        }

        public ReportInfo GetReportSlides(int reportId,int userId, int permission)
        {
            logger.Info("Inside ReportService/GetReportSlides");
            logger.Info("userid:{0} reportId {1}", userId, reportId);
            ReportInfo reportInfoWithSlide = null;

            try
            {
                if(permission > 0 && permission < 11)
                {
                   
                        Report reportRecord = unitOfWork.DbContext.Report                             //change query into single
                                      .Where(re => re.ID == reportId)
                                      .FirstOrDefault();

                        if (reportRecord != null)

                        {
                            if (reportRecord.IsPublic || unitOfWork.DbContext.ReportGroup.Select(re => re.Group.UserGroup.Where(ug => ug.UserID == userId)).Any())
                            {
                                reportInfoWithSlide = new ReportInfo
                                {

                                    Slides = reportRecord.ReportSlide
                                                    .Select(rs => new ReportSlideDetail()
                                                    {
                                                        ReportId = rs.ID,
                                                        SlideId = rs.ID,
                                                        //Image = Convert.ToBase64String(rs.Slide),
                                                        IsLiked = rs.SlideFavorite
                                                                     .Where(sf => sf.UserID == userId).Any()
                                                    }).ToList(),
                                    Users = reportRecord.IsPublic == true ? unitOfWork.DbContext.UserDetail
                                                .Select(ud => new LoginDetail()
                                                {
                                                    UserId = ud.ID,
                                                    UserName = ud.Name
                                                }).ToList() : unitOfWork.DbContext.ReportGroup
                                                  .Select(rg => rg.Group.UserGroup
                                                  .Select(ug => new LoginDetail()
                                                  {
                                                      UserId = ug.UserDetail.ID,
                                                      UserName = ug.UserDetail.Name
                                                  }).ToList()
                                              ).FirstOrDefault(),
                                    Author = new AuthorInfo
                                    {
                                        AuthorId = reportRecord.UserDetail.ID,
                                        AuthorName = reportRecord.UserDetail.Name
                                    }
                                };
                            }
                        }
                        return reportInfoWithSlide;
                    

                }              
                else
                {
                    throw new PermissionDeniedException();
                }
            }       
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReportAndSlideFavorite GetFavoriteReportsSlides(int userId, int permission)
        {
            logger.Info("Inside ReportService/GetFavoriteReportsSlides");
            logger.Info("userid :{0}", userId);
            ReportAndSlideFavorite reportAndSlideFavorite = new ReportAndSlideFavorite();
            List<ReportDetail> reports = new List<ReportDetail>();
            List<ReportSlideDetail> slides = new List<ReportSlideDetail>();
            try
            {
                if (permission > 0 && permission < 11)
                {
                    
                        logger.Info("Getting FavoriteReport List");
                        var favouriteReports = unitOfWork.DbContext.ReportFavorite
                                                          .Where(r => r.UserDetail.ID == userId)
                                                          .Select(r => new ReportDetail
                                                          {
                                                              ReportId = r.ReportID,
                                                              Description = r.Report.Description,
                                                              IsPublic = r.Report.IsPublic,
                                                              CreationDate = r.CreationDate,
                                                              tempImage = r.Report.ReportSlide.Where(rs => rs.Order == 2)
                                                              .Select(rs => rs.Slide).FirstOrDefault(),
                                                              IsLiked = true
                                                          }).ToList();
                        logger.Info("FavoriteReportList Received Successfully.");

                        if (favouriteReports.AnyOrNotNull())
                        {
                            foreach (var favReport in favouriteReports)
                            {
                                ReportDetail reportDetail = new ReportDetail();
                                if (favReport != null)
                                {
                                    logger.Info("Converting Image byte array to Image base64");
                                    favReport.Image = Convert.ToBase64String(favReport.tempImage);
                                }
                            }
                        }

                        logger.Info("Getting FavoriteSlideList.");
                        var favouriteSlides = unitOfWork.DbContext.SlideFavorite
                                 .Where(sf => sf.UserID == userId)
                                 .Select(sf => new ReportSlideDetail
                                 {
                                     SlideId = sf.SlideID,
                                     IsLiked = true,
                                     UserId = sf.UserID,
                                     TempImage = sf.ReportSlide.Slide,
                                     IsPublic = sf.ReportSlide.Report.IsPublic,
                                     Author = new AuthorInfo
                                     {
                                         AuthorId = sf.ReportSlide.Report.Author,
                                         AuthorName = sf.ReportSlide.Report.UserDetail.Name
                                     }
                                 }).ToList();
                        logger.Info("FavoriteSlide List received successfully.");
                        if (favouriteSlides.AnyOrNotNull())
                        {
                            foreach (var favouriteSlide in favouriteSlides)
                            {
                                logger.Info("Converting Image bytearray to Image base64.");
                                favouriteSlide.Image = Convert.ToBase64String(favouriteSlide.TempImage);
                                logger.Info("Image Base64 received Successfully.");
                                favouriteSlide.Users = favouriteSlide.IsPublic == true ?
                                    unitOfWork.DbContext.UserDetail.Select(ud => new LoginDetail()
                                    {
                                        UserId = ud.ID,
                                        UserName = ud.Name
                                    }).ToList() : unitOfWork.DbContext.UserDetail
                                      .Where(ud => ud.ID == favouriteSlide.UserId)
                                      .FirstOrDefault()
                                    .UserGroup.Select(ug => new LoginDetail
                                    {
                                        UserId = ug.UserDetail.ID,
                                        UserName = ug.UserDetail.Name
                                    }).Distinct().ToList();

                            }
                            reportAndSlideFavorite.Reports = favouriteReports;
                            reportAndSlideFavorite.Slides = favouriteSlides;
                        }
                        return reportAndSlideFavorite;
                    
                }                   
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool FavoriteReport(int reportId, int userId, int permission)
        {
            logger.Info("Inside ReportService/FavoriteReport");
            logger.Info("reportId :{0} userid:{1}", reportId, userId);
            try
            {
                if (permission > 0 && permission < 11)
                {
                   
                        bool like = false;

                        ReportFavorite favorite = unitOfWork.DbContext.ReportFavorite
                                      .Where(rf => rf.ReportID == reportId && rf.UserDetail.ID == userId)
                                      .FirstOrDefault();
                        if (favorite != null)
                        {
                            logger.Info("Removing report from favorite for current user.");
                            unitOfWork.DbContext.ReportFavorite.Remove(favorite);
                            like = false;
                        }
                        else
                        {
                            logger.Info("Adding report to favorite for current user.");

                            ReportFavorite rf = new ReportFavorite()
                            {
                                ReportID = reportId,
                                UserID = userId,
                                CreationDate = DateTime.UtcNow
                            };
                            unitOfWork.DbContext.ReportFavorite.Add(rf);
                            like = true;

                        }
                        unitOfWork.DbContext.SaveChanges();
                        logger.Info("Successfully updated to reportFavorite.");
                        return like;
                    
                   
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool FavoriteSlide(int slideId, int userId, int permission)
        {
            logger.Info("Inside UserService/FavoriteSlide");
            logger.Info("slideId:{0},userId:{1}", slideId, userId);
            try
            {
                if (permission > 0 && permission < 11)
                {
                   
                        bool like = false;

                        SlideFavorite favorite = unitOfWork.DbContext.SlideFavorite.Where(sf => sf.SlideID == slideId && sf.UserDetail.ID == userId).FirstOrDefault();
                        if (favorite != null)
                        {
                            unitOfWork.DbContext.SlideFavorite.Remove(favorite);
                            like = false;
                        }
                        else
                        {
                            SlideFavorite sf = new SlideFavorite()
                            {
                                SlideID = slideId,
                                UserID = userId,
                                CreationDate = DateTime.UtcNow
                            };

                            unitOfWork.DbContext.SlideFavorite.Add(sf);

                            like = true;
                        }
                        unitOfWork.DbContext.SaveChanges();
                        logger.Info("Successfully added FavoriteSlide of a user to database");
                        return like;                   
                   
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ReportView(int reportId, int userId, int permission)
        {
            logger.Info("Inside UserService/ReportView");
            logger.Info("reportId:{0} userId:{1}", reportId, userId);
            try
            {
                if (permission > 0 && permission < 11)
                {
                   
                        bool result = false;

                        logger.Info("Getting ReportView for a particular user.");
                        var reportView = unitOfWork.DbContext.ReportView.Where(rv => rv.ReportID == reportId && rv.UserID == userId).FirstOrDefault();//reportview

                        if (reportView != null)
                        {
                            reportView.ViewCount = reportView.ViewCount + 1;
                            result = true;
                        }
                        else
                        {
                            ReportView rv = new ReportView
                            {
                                UserID = userId,
                                ReportID = reportId,
                                ViewCount = 1,
                                LastViewedDate = DateTime.UtcNow
                            };
                            unitOfWork.DbContext.ReportView.Add(rv);
                            result = true;
                        }
                        logger.Info("Trying to save ViewReportDetails in database");
                        unitOfWork.DbContext.SaveChanges();
                        logger.Info("ViewReportDetails saved successfully to database.");
                        return result;
                    
                   
                }
                else
                {
                    logger.Info("Unauthorized request");
                    throw new PermissionDeniedException();
                }
            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<ReportDetail> FilterReport(ReportFilter filter, int permission)
        {
            logger.Info("Inside UserService/FilterReport");
            logger.Info("UserId:{0} SortBy :{1} ViewBy:{2} categories: {3}", filter.UserId, filter.SortBy, filter.ViewBy, filter.Categories);
            List<ReportDetail> reportDetails = new List<ReportDetail>();
            try
            {
                if (permission > 0 && permission < 11)
                {
                    switch ((SortBy)filter.SortBy)
                    {
                        case SortBy.Recency:
                            reportDetails = FetchSortedReports(filter.UserId, filter.SortBy, filter.ViewBy, filter.Categories, permission).OrderByDescending(o => o.CreationDate).ToList();
                            break;
                        case SortBy.MostViewed:
                            reportDetails = FetchSortedReports(filter.UserId, filter.SortBy, filter.ViewBy, filter.Categories, permission).OrderByDescending(o => o.GlobalReportViewCount).ToList();
                            break;
                        case SortBy.MostFavorite:
                            reportDetails = FetchSortedReports(filter.UserId, filter.SortBy, filter.ViewBy, filter.Categories, permission).OrderByDescending(o => o.ReportLikeCount).ToList();
                            break;
                        default:
                            break;
                    }
                    if (reportDetails.AnyOrNotNull())
                    {
                        foreach (var report in reportDetails)
                        {
                            if (report != null)
                            {
                                logger.Info("Converting Image bytearray to image Base64");
                                report.Image = Convert.ToBase64String(report.tempImage);
                                logger.Info("Image Base64 Received Successfully.");
                                report.tempImage = new byte[0];
                            }
                        }
                    }
                    return reportDetails;
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<ReportDetail> FetchSortedReports(int userId, int sortBy, int viewBy, List<int> categories, int permission)
        {
            logger.Info("Inside UserService/FetchSortedReports");
            try
            {
                if (permission > 0 && permission < 11)
                {
                    
                        DateTime currentDate = DateTime.UtcNow;
                        int currentMonth = currentDate.Month;
                        var latestReportDateFromDB = unitOfWork.DbContext.Report
                                                    .OrderByDescending(re => re.CreationDate).FirstOrDefault();
                        DateTime latestRecordDate = latestReportDateFromDB.CreationDate;
                        var latestRecordMonth = latestRecordDate.Month;
                        var month = Math.Abs((currentDate.Month - latestRecordDate.Month) + 12 * (currentDate.Year - latestRecordDate.Year));

                        if (month <= 12)
                        {
                            //checking if Report is in group of which user is a member 
                            logger.Info("Getting Filter ReportDetailList");
                            var reportList = unitOfWork.DbContext.Report
                                                      .Where(re => (re.CreationDate.Month == latestRecordMonth && re.IsPublic == true)
                                                      || (re.CreationDate.Month == latestRecordMonth && re.Author == userId) || (re.CreationDate.Month == latestRecordMonth
                                                      && re.ReportGroup.Where(rg => rg.Group.UserGroup.Where(ug => ug.UserID == userId).Any()).Any())
                                                      )
                                                      .Select(re => new ReportDetail()
                                                      {
                                                          ReportId = re.ID,
                                                          SubcategoryId = re.SubcategoryID,
                                                          CategoryId = re.ReportSubCategory.ReportCategory.ID,
                                                          Description = re.Description,
                                                          CreationDate = re.CreationDate,
                                                          IsPublic = re.IsPublic,
                                                          tempImage = re.ReportSlide.Where(rs => rs.Order == 2)
                                                          .Select(rs => rs.Slide).FirstOrDefault(),

                                                          IsViewed = re.ReportView.Where(rv => rv.UserID == userId).Any(),
                                                          IsLiked = re.ReportFavorite
                                                                   .Where(rf => rf.UserID == userId)
                                                                   .Any(),
                                                          GlobalReportViewCount = re.ReportView
                                                                                .GroupBy(rv => rv.ReportID)
                                                                                .Select(rv => rv.Sum(v => v.ViewCount)
                                                                                ).FirstOrDefault(),
                                                          ReportLikeCount = re.ReportFavorite.Count(),

                                                      }).AsQueryable();

                            if (viewBy == (int)ViewBy.Read)
                            {
                                reportList = reportList.Where(rl => rl.IsViewed);

                            }
                            else if (viewBy == (int)ViewBy.UnRead)
                            {
                                reportList = reportList.Where(rl => rl.IsViewed == false);

                            }
                            if (categories.AnyOrNotNull())
                            {
                                reportList = reportList.Where(rl => categories.Contains(rl.SubcategoryId));
                            }
                            return reportList;
                        }
                        return null;
                    
                   
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ViewComment(int commentId, int userId, int permission)
        {
            logger.Info("Inside UserService/ViewComment");
            logger.Info("commentId :{0} userId :{1}", commentId, userId);
            try
            {
                if (permission > 0 && permission < 11)
                {
                    
                        bool result = false;

                        logger.Info("Getting ViewedCommment for a particular user.");
                        var commentViewed = unitOfWork.DbContext.CommentView.Where(cv => cv.CommentId == commentId && cv.UserId == userId).FirstOrDefault();

                        if (commentViewed != null)
                        {
                            commentViewed.ViewCount = commentViewed.ViewCount + 1;
                            result = true;
                        }
                        else
                        {
                            CommentView commentView = new CommentView
                            {
                                UserId = userId,
                                CommentId = commentId,
                                ViewCount = 1
                            };
                            unitOfWork.DbContext.CommentView.Add(commentView);
                            result = true;
                        }

                        unitOfWork.DbContext.SaveChanges();
                        logger.Info("ViewedCommentdetail added successfully to database.");
                        return result;
                    
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool CommentLike(int commentId, int userId, int permission)
        {
            logger.Info("Inside UserService/CommentLike");
            logger.Info("commentId:{0} userId:{1}", commentId, userId);
            try
            {
                if (permission > 0 && permission < 11)
                {
                    
                        bool like = false;

                        CommentLike commentlike = unitOfWork.DbContext.CommentLike.Where(cl => cl.CommentID == commentId && cl.UserDetail.ID == userId).FirstOrDefault();
                        if (commentlike != null)
                        {
                            unitOfWork.DbContext.CommentLike.Remove(commentlike);

                        }
                        else
                        {
                            CommentLike cl = new CommentLike()
                            {
                                CommentID = commentId,
                                UserID = userId,
                                CreationDate = DateTime.UtcNow
                            };
                            logger.Info("Trying to add CommentLike detail to database");
                            unitOfWork.DbContext.CommentLike.Add(cl);
                            like = true;
                        }
                        unitOfWork.DbContext.SaveChanges();
                        return like;
                   
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void SendComment(CommentDetails cmtDl, int permission)
        {
            logger.Info("Inside UserService/SendComment");
            logger.Info("UserId:{0} SlideId:{1} CommentDescription:{2} ParentId:{3} Image:{4}", cmtDl.UserId, cmtDl.SlideId, cmtDl.CommentDescription, cmtDl.ParentId, cmtDl.Image);
            try
            {
                if(permission > 0 && permission < 11)
                {
                   
                        bool isPublic = false;
                        if (!cmtDl.TaggedUsers.AnyOrNotNull())
                        {
                            isPublic = true;
                        }

                        Reporting.EntityProvider.IOS_App.Comment comments = new Reporting.EntityProvider.IOS_App.Comment()
                        {
                            UserID = cmtDl.UserId,
                            ReportSlideID = cmtDl.SlideId,
                            Description = cmtDl.CommentDescription,
                            ParentCommentID = cmtDl.ParentId,
                            CreationDate = DateTime.UtcNow,
                            IsPublic = isPublic,
                            UpdatedDate = DateTime.UtcNow
                        };
                        unitOfWork.DbContext.Comment.Add(comments);

                        foreach (var userId in cmtDl.TaggedUsers)
                        {
                            if (userId != 0)
                            {
                                CommentTagUser commentTagUser = new CommentTagUser()
                                {
                                    Comment = comments,
                                    UserID = userId
                                };
                                unitOfWork.DbContext.CommentTagUser.Add(commentTagUser);
                            };
                        }
                        if (cmtDl.ParentId != -1)
                        {
                            var subComment = unitOfWork.DbContext.Comment.Where(cm => cm.ParentCommentID == cmtDl.ParentId).FirstOrDefault();
                            if (subComment != null)
                            {
                                comments.UpdatedDate = DateTime.UtcNow;
                            };
                        }

                        if (!string.IsNullOrEmpty(cmtDl.Image))
                        {
                            comments.Image = cmtDl.Image;
                            logger.Info("Trying to save image to database");
                        }
                        unitOfWork.DbContext.SaveChanges();

                    
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<CommentDetails> GetComments(int userId, int permission)
        {
            logger.Info("Inside UserService/GetComments");
            try
            {
                if (permission > 0 && permission < 11)
                {
                   
                        List<CommentDetails> commentsArr = new List<CommentDetails>();

                        var currentDate = DateTime.UtcNow;
                        DateTime dateBeforeThreeMonth = currentDate.AddMonths(-3);

                        commentsArr = unitOfWork.DbContext.Comment
                            .Where(cv => (cv.IsPublic == true && cv.ParentCommentID == -1)
                            || (cv.ParentCommentID == -1 && cv.CommentTagUser.Where(ctu => ctu.UserID == userId).Any()
                            && cv.UpdatedDate >= dateBeforeThreeMonth))
                                    .Select(c => new CommentDetails
                                    {
                                        CommentId = c.ID,
                                        ParentId = c.ParentCommentID,
                                        UserId = c.UserID,
                                        CommentAuthor = c.UserDetail.Name,
                                        CommentDescription = c.Description,
                                        CreationDate = c.CreationDate,
                                        Image = c.Image,
                                        SlideId = c.ReportSlideID,
                                        IsLiked = c.CommentLike.Any(),
                                        CommentLikes = c.CommentLike.Count(),
                                        Views = c.CommentView.Select(cv => cv.ViewCount).FirstOrDefault(),
                                        Replies = unitOfWork.DbContext.Comment.Where(cm => cm.ParentCommentID == c.ID).Count()
                                    }).OrderByDescending(c => c.CreationDate).ToList();

                        return commentsArr;
                    
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CommentDetails> GetFavoriteComments(int userId, int permission)
        {
            logger.Info("Inside UserService/GetFavoriteComments");
            try
            {
                if (permission > 0 && permission < 11)
                {
                    
                        var currentDate = DateTime.UtcNow;
                        DateTime dateBeforeThreeMonth = currentDate.AddMonths(-3);
                        List<CommentDetails> commentDetails = unitOfWork.DbContext.Comment
                            .Where(c => c.CommentLike.Where(cl => cl.UserID == userId).Any() && c.UpdatedDate >= dateBeforeThreeMonth)
                            .Select(c => new CommentDetails
                            {
                                CommentId = c.ID,
                                UserId = c.UserID,
                                SlideId = c.ReportSlideID,
                                CommentDescription = c.Description,
                                ParentId = c.ParentCommentID,
                                Image = c.Image,
                                IsLiked = c.CommentLike.Any(),
                                CreationDate = c.CreationDate,
                                TaggedUsers = c.CommentTagUser.Select(ctu => ctu.UserID).ToList(),
                                CommentLikes = c.CommentLike.Count(),
                                CommentAuthor = c.UserDetail.Name,
                                Views = c.CommentView
                                   .Where(cv => cv.UserId == userId)
                                   .Select(cv => cv.ViewCount).FirstOrDefault(),
                                Replies = unitOfWork.DbContext.Comment.Where(cm => cm.ParentCommentID == c.ID).Count()

                            }).OrderByDescending(c => c.CreationDate).ToList();

                        return commentDetails;
                    
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<CommentDetails> GetSubComments(int commentId, int permission)
        {
            logger.Info("Inside UserService/GetSubComments");
            try
            {
                if (permission > 0 && permission < 11)
                {
                   
                        List<CommentDetails> subCommentsArr = new List<CommentDetails>();


                        //get sub Comments for given CommentId
                        subCommentsArr = unitOfWork.DbContext.Comment
                                       .Where(c => c.ParentCommentID == commentId)
                                         .Select(c => new CommentDetails
                                         {
                                             CommentId = c.ID,
                                             UserId = c.UserID,
                                             CommentDescription = c.Description,
                                             CreationDate = c.CreationDate,
                                             CommentAuthor = c.UserDetail.Name,
                                         }).OrderByDescending(c => c.CreationDate).ToList();

                        return subCommentsArr;
                    
                    
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public int DeleteComment(int commentId, int permission)
        {
            int result = 0;
            try
            {
                if (permission > 0 && permission < 11)
                {
                    
                        var deleteComment = unitOfWork.DbContext.Comment
                       .Where(c => c.ID == commentId)
                       .Select(c => new
                       {
                           commentTagUserlist = c.CommentTagUser,
                           commentLikeList = c.CommentLike,
                           commentViewList = c.CommentView,

                            // commentList = masterContext.Comment.Where(cm => cm.ID == c.ID || cm.ParentCommentID == c.ID).Select(cm=>cm).ToList()

                        }).FirstOrDefault();
                        var commentList = unitOfWork.DbContext.Comment.Where(c => c.ID == commentId).FirstOrDefault();
                        var subcommentList = unitOfWork.DbContext.Comment.Where(cm => cm.ParentCommentID == commentId).ToList();

                        if (deleteComment != null)
                        {

                            if (deleteComment.commentTagUserlist.AnyOrNotNull())
                            {
                                unitOfWork.DbContext.CommentTagUser.RemoveRange(deleteComment.commentTagUserlist);

                            }

                            if (deleteComment.commentLikeList.AnyOrNotNull())
                            {
                                unitOfWork.DbContext.CommentLike.RemoveRange(deleteComment.commentLikeList);

                            }
                            if (deleteComment.commentViewList.AnyOrNotNull())
                            {
                                unitOfWork.DbContext.CommentView.RemoveRange(deleteComment.commentViewList);

                            }

                            unitOfWork.DbContext.Comment.RemoveRange(subcommentList);
                            unitOfWork.DbContext.Comment.Remove(commentList);
                            unitOfWork.DbContext.SaveChanges();
                            result = 1;
                        }
                        return result;
                    
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<CommentDetails> FilterComment(CommentsFilter item, int permission)
        {
            List<CommentDetails> commentDetails = new List<CommentDetails>();
            try
            {
                if (permission > 0 && permission < 11)
                {
                    switch ((SortBy)item.SortBy)
                    {

                        case SortBy.Recency:
                            commentDetails = FetchSortedComments(item.UserId, item.SortBy, item.ThreadBy, item.PrivacyBy, item.Categories, permission).OrderByDescending(o => o.CreationDate).ToList();
                            break;
                        case SortBy.MostViewed:
                            commentDetails = FetchSortedComments(item.UserId, item.SortBy, item.ThreadBy, item.PrivacyBy, item.Categories, permission).OrderByDescending(o => o.GlobalCommentViewCount).ToList();
                            break;
                        case SortBy.MostFavorite:
                            commentDetails = FetchSortedComments(item.UserId, item.SortBy, item.ThreadBy, item.PrivacyBy, item.Categories, permission).OrderByDescending(o => o.CommentLikes).ToList();
                            break;
                        default:
                            break;
                    }
                    return commentDetails;
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CommentDetails> FetchSortedComments(int userId, int sortBy, int threadBy, int privacyBy, List<int> categories, int permission)

        {
            try
            {
               
                    List<CommentDetails> commentDetails = unitOfWork.DbContext.Comment
                        .Where(cv => (cv.IsPublic == true && cv.ParentCommentID == -1)
                        || (cv.ParentCommentID == -1 && cv.CommentTagUser.Where(ctu => ctu.UserID == userId).Any()
                                                     ))
                                                   .Select(c => new CommentDetails
                                                   {
                                                       CommentId = c.ID,
                                                       ParentId = c.ParentCommentID,
                                                       UserId = c.UserID,
                                                       IsPublic = c.IsPublic,
                                                       CommentAuthor = c.UserDetail.Name,
                                                       Subcategory = unitOfWork.DbContext.ReportSlide
                                                              .Where(rs => rs.ID == c.ReportSlideID)
                                                              .Select(rs => rs.Report.SubcategoryID).FirstOrDefault(),
                                                       CommentDescription = c.Description,
                                                       CreationDate = c.CreationDate,
                                                       TaggedUsers = c.CommentTagUser.Select(ctu => ctu.UserID).ToList(),
                                                       Image = c.Image,
                                                       SlideId = c.ReportSlideID,
                                                       CommentLikes = c.CommentLike.Count(),

                                                       Views = c.CommentView.Count(),
                                                       IsLiked = c.CommentLike
                                                              .Where(cl => cl.UserID == userId)
                                                              .Any(),
                                                       GlobalCommentViewCount = c.CommentView
                                                                      .GroupBy(cv => cv.CommentId)
                                                                        .Select(cv => cv.Sum(v => v.ViewCount)
                                                                        ).FirstOrDefault(),

                                                       Replies = unitOfWork.DbContext.Comment.Where(cm => cm.ParentCommentID == c.ID).Count()
                                                   }).ToList();


                    if (threadBy == (int)ThreadBy.FromMe)
                    {
                        commentDetails = commentDetails.Where(cd => cd.UserId == userId).ToList();

                    }
                    else if (threadBy == (int)ThreadBy.ToMe)
                    {
                        commentDetails = commentDetails.Where(cd => cd.UserId != userId).ToList();
                    }

                    if (privacyBy == (int)PrivacyBy.Public)
                    {
                        commentDetails = commentDetails.Where(cd => cd.CommentTagUser == null || cd.CommentTagUser.Count > 1).ToList();
                    }
                    else if (privacyBy == (int)PrivacyBy.Private)
                    {
                        commentDetails = commentDetails.Where(cd => cd.CommentTagUser != null && cd.CommentTagUser.Count == 1).ToList();
                    }

                    if (categories.AnyOrNotNull())
                    {
                        commentDetails = commentDetails.Where(rl => categories.Contains(rl.Subcategory)).ToList();
                    }
                    return commentDetails;

            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public bool SendMailWithSlide(MailWithSlide mail, string path, int permission)
        {
            bool result = false;

            List<LoginDetail> logindetail = new List<LoginDetail>();
            try
            {
                if (permission > 0 && permission < 11)
                {
                    
                        var userdetail = unitOfWork.DbContext.UserDetail.Where(ud => ud.ID == mail.UserId)
                                          .Select(ud => new { username = ud.Name, useremail = ud.Email }).FirstOrDefault();
                        var reportSlideImage = unitOfWork.DbContext.ReportSlide.Where(rs => rs.ID == mail.SlideId).Select(rs => rs.Slide).FirstOrDefault();
                        if (mail.ReceiverIds != null)
                        {
                            var userEmailIdList = unitOfWork.DbContext.UserDetail.Where(ud => mail.ReceiverIds.Contains(ud.ID))
                                                 .Select(ud => new { username = ud.Name, useremail = ud.Email }).ToList();

                            LinkedResource logo = new LinkedResource(path + "\\logo.png");
                            logo.ContentId = Guid.NewGuid().ToString();
                            if (reportSlideImage != null)
                            {
                                using (MemoryStream m = new MemoryStream(reportSlideImage))
                                {
                                    LinkedResource slideimage = new LinkedResource(m);
                                    slideimage.ContentId = Guid.NewGuid().ToString();

                                    for (int i = 0; i < userEmailIdList.Count; i++)
                                    {
                                        string htmlBody = GetHtmlBodyForSlide(userEmailIdList[i].username, mail.Message, slideimage.ContentId);
                                        AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                                        alternateView.LinkedResources.Add(logo);
                                        alternateView.LinkedResources.Add(slideimage);

                                        result = SendMail.SendEmail(mail.Subject, htmlBody, userEmailIdList[i].useremail, new List<AlternateView> { alternateView });
                                    }
                                }
                            }
                        }
                        return result;
                    
                }
                else
                {
                    throw new PermissionDeniedException();
                }

            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool SendMailWithChart(MailWithSlide mail, string path, int permission)
        {
            bool result = false;

            List<LoginDetail> logindetail = new List<LoginDetail>();
            try
            {
                if (permission > 0 && permission < 11)
                {
                    
                        var userdetails = unitOfWork.DbContext.UserDetail.Where(ud => ud.ID == mail.UserId)
                                            .Select(ud => new { username = ud.Name, useremail = ud.Email }).FirstOrDefault();

                        var userEmailIdList = unitOfWork.DbContext.UserDetail.Where(ud => mail.ReceiverIds.Contains(ud.ID))
                                         .Select(ud => new { username = ud.Name, useremail = ud.Email }).ToList();


                        LinkedResource logo = new LinkedResource(path + "\\logo.png");
                        logo.ContentId = Guid.NewGuid().ToString();
                        var imageData = Convert.FromBase64String(mail.Image);
                        LinkedResource chartimage = new LinkedResource(new MemoryStream(imageData), "image/jpeg");
                        chartimage.ContentId = Guid.NewGuid().ToString();
                        chartimage.TransferEncoding = TransferEncoding.Base64;

                        for (int i = 0; i < userEmailIdList.Count; i++)
                        {
                            string htmlBody = GetHtmlBodyForSlide(userEmailIdList[i].username, mail.Message, chartimage.ContentId);
                            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                            alternateView.LinkedResources.Add(logo);
                            alternateView.LinkedResources.Add(chartimage);
                            SendMail.SendEmail(mail.Subject, htmlBody, userEmailIdList[i].useremail, new List<AlternateView> { alternateView });
                        }
                        result = true;
                        return result;
                    
                }
                else
                {
                    throw new PermissionDeniedException();
                }

            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GetHtmlBodyForSlide(string username, string mailMessage, string slideContentId)
        {
            StringBuilder sbEmailBody = new StringBuilder();
            sbEmailBody.Append("<table style =\"color:#535353;font-family:arial,helvetica,sans-serif;font-size:12px;width:100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
            sbEmailBody.Append("<tbody>");
            sbEmailBody.Append("<tr>");
            sbEmailBody.Append("<td style=\"padding:30px 0 100px 0;text-align:center\" align=\"center\" bgcolor=\"#f0f0f0\">");
            sbEmailBody.Append("<table  style=\"margin:0 auto;width:600px\" align=\"center\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
            sbEmailBody.Append("<tbody>");
            sbEmailBody.Append("<tr>");
            sbEmailBody.Append("<td style=\"padding: 20px 0 20px 0; border - bottom:1px dotted #b8b8b8\" bgcolor=\"#ffffff\">");
            sbEmailBody.Append("<table style=\"width:100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
            sbEmailBody.Append("<tbody>");
            sbEmailBody.Append("<tr>");
            sbEmailBody.Append("<td style=\"width:420px;text-align:left;padding-left:20px\" align=\"left\" width=\"420\" bgcolor=\"#ffffff\">");
            sbEmailBody.Append("<a target=\"_blank\">");
            //sbEmailBody.Append("<img alt=\"\" src ='cid:" + logoContentId + "' border=\"0\">");
            sbEmailBody.Append("</a>");
            sbEmailBody.Append("</td>");
            sbEmailBody.Append("<td style=\"width:180px;text-align:right;color:#003049;font-size:12px;font-family:Cambria,\"Hoefler Text\",\"Liberation Serif\",Times,\"Times New Roman\",serif;padding-right:20px\" align=\"right\" width=\"180\" valign=\"top\">");
            sbEmailBody.Append("</td>");
            sbEmailBody.Append("</tr>");
            sbEmailBody.Append("</tbody>");
            sbEmailBody.Append("</table>");
            sbEmailBody.Append("</td>");
            sbEmailBody.Append("</tr>");
            sbEmailBody.Append("</tbody>");
            sbEmailBody.Append("</table>");
            sbEmailBody.Append("<table style=\"margin:0 auto;width:600px\" align=\"center\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
            sbEmailBody.Append("<tbody>");
            sbEmailBody.Append("<tr>");
            sbEmailBody.Append("<td style=\"padding:30px\" valign=\"top\" bgcolor=\"#ffffff\">");
            sbEmailBody.Append("<table style=\"width:100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
            sbEmailBody.Append("<tbody>");
            sbEmailBody.Append("<tr>");
            sbEmailBody.Append("<td style=\"padding:0;text-align:left;color:#3c3c3b;font-size:12px;font-family:arial,helvetica,sans-serif;line-height:16px\">");
            sbEmailBody.Append("<p> Hi " + username + ",</p>");
            sbEmailBody.Append("<p>");
            sbEmailBody.Append("<b style =\"color:#2a7e80;font-size:16px\">" + mailMessage + "</b></p>");
            sbEmailBody.Append("<img alt=\"\" src ='cid:" + slideContentId + "'  width=\"500\" height=\"300\" class=\"CToWUd\" border=\"0\" style =\"display:block;width:100%\">");
            sbEmailBody.Append("<p>If any of the above is incorrect, please contact us on");
            sbEmailBody.Append("<a href=\"mail: tech.support @pharmaace.com\" value=\"tech.support @pharmaace.com\" target=\"_blank\">");
            sbEmailBody.Append("tech.support@pharmaace.com");
            sbEmailBody.Append("</a>");
            sbEmailBody.Append("</p>");
            sbEmailBody.Append("<table style=\"border-collapse:collapse;min-width:100%;width:100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
            sbEmailBody.Append("<tbody>");
            sbEmailBody.Append("<tr>");
            sbEmailBody.Append("<td style=\"padding:0\" align=\"center\" valign=\"top\"></td>");
            sbEmailBody.Append("</tr>");
            sbEmailBody.Append("</tbody>");
            sbEmailBody.Append("</table>");
            sbEmailBody.Append("</td>");
            sbEmailBody.Append("</tr>");
            sbEmailBody.Append("</tbody>");
            sbEmailBody.Append("</table>");
            sbEmailBody.Append("</td>");
            sbEmailBody.Append("</tr>");
            sbEmailBody.Append("</tbody>");
            sbEmailBody.Append("</table>");
            sbEmailBody.Append("<table  style=\"margin:0 auto;width:600px\" align=\"center\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
            sbEmailBody.Append("<tbody>");
            sbEmailBody.Append("<tr>");
            sbEmailBody.Append("<td width=\"617\" bgcolor=\"#003049\">");
            sbEmailBody.Append("<table style=\"width:600px\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
            sbEmailBody.Append("<tbody>");
            sbEmailBody.Append("<tr>");
            sbEmailBody.Append("<td style=\"text-align:left;width:420px;margin:0 auto;font-family:arial,helvetica,sans-serif;font-size:12px;color:#ffffff;padding:20px\" align=\"center\" width=\"420\" valign=\"top\" bgcolor=\"#003049\">Visit our website");
            sbEmailBody.Append("<a href=\"http://support.pacehomepage.com\" style=\"color:#ffffff\" target=\"_blank\">");
            sbEmailBody.Append("</a>");
            sbEmailBody.Append("<br>");
            sbEmailBody.Append("</td>");
            sbEmailBody.Append("<td style=\"text-align:center;width:180px;margin:0 auto;font-family:arial,helvetica,sans-serif;font-size:11px;color:#ffffff\" align=\"center\" width=\"180\" bgcolor=\"#003049\">");
            sbEmailBody.Append("<table style=\"width:30%\" align=\"center\" border=\"0\">");
            sbEmailBody.Append("<tbody>");
            sbEmailBody.Append("</tbody>");
            sbEmailBody.Append("</table>");
            sbEmailBody.Append("</td>");
            sbEmailBody.Append("</tr>");
            sbEmailBody.Append("</tbody>");
            sbEmailBody.Append("</table>");
            sbEmailBody.Append("</td>");
            sbEmailBody.Append("</tr>");
            sbEmailBody.Append("</tbody>");
            sbEmailBody.Append("</table>");
            sbEmailBody.Append("</td>");
            sbEmailBody.Append("</tr>");
            sbEmailBody.Append("</tbody>");
            sbEmailBody.Append("</table>");

            return sbEmailBody.ToString();
        }


        public List<string> GetReportImage(List<int> slideIds, int permission)
        {
            List<string> res = new List<string>();

            string base64Str = null;
            try
            {
                if (permission > 0 && permission < 11)
                {
                    foreach (var Id in slideIds)
                    {
                        byte[] byteImage = unitOfWork.DbContext.ReportSlide
                               .Where(rs => rs.ID == Convert.ToInt32(Id)).Select(rs => rs.Slide).FirstOrDefault();

                        base64Str = Convert.ToBase64String(byteImage);
                        if (base64Str != null)
                        {
                            res.Add(base64Str);
                        }
                        else
                        {
                            throw new NoDataFoundException();
                        }
                    }
                    return res;
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                logger.Error("Could not cast image to string : " + ex.ToString());
                throw ex;
            }
        }

        public List<CommentDetails> GetNotification(int userId, int permission)
        {
            List<CommentDetails> commentDetails = new List<CommentDetails>();
            try
            {
                if (permission > 0 && permission < 11)
                {
                    
                        var parentCommentForNotification = unitOfWork.DbContext.Comment
                       .Where(c => (c.UserID != userId)
                       && (!c.CommentView.Any(cv => cv.UserId == userId)
                       && (c.ParentCommentID == -1)
                       && (c.CommentTagUser.Count == 0)))
                      .Select(c => new CommentDetails
                      {
                          CommentId = c.ID,
                          UserId = c.UserID,
                          CommentDescription = c.Description,
                          Image = c.Image,
                          CreationDate = c.CreationDate,
                          ParentId = c.ParentCommentID,
                          SlideId = c.ReportSlideID,
                          IsViewed = c.CommentView.Where(cv => cv.UserId == userId).Any(),
                          Message = c.UserDetail.Name + " added new Comment",

                      }).OrderByDescending(c => c.CreationDate).ToList();

                        commentDetails.AddRange(parentCommentForNotification);

                        var parentCommentForNotificationForTaggedUser = unitOfWork.DbContext.Comment
                           .Where(c => (c.UserID != userId)
                           && (!c.CommentView.Any(cv => cv.UserId == userId)
                           && (c.ParentCommentID == -1)
                           && (c.CommentTagUser.Count > 0)
                           && (c.CommentTagUser.Any(ctu => ctu.UserID == userId))))
                          .Select(c => new CommentDetails

                          {
                              CommentId = c.ID,
                              UserId = c.UserID,
                              CommentDescription = c.Description,
                              Image = c.Image,
                              CreationDate = c.CreationDate,
                              ParentId = c.ParentCommentID,
                              SlideId = c.ReportSlideID,
                              IsViewed = c.CommentView.Where(cv => cv.UserId == userId).Any(),
                              Message = c.UserDetail.Name + " tagged you in a Comment",

                          }).OrderByDescending(c => c.CreationDate).ToList();

                        commentDetails.AddRange(parentCommentForNotificationForTaggedUser);

                        //Sub Comment on Parent Comment Notify
                        var allCommentByCurrentUser = unitOfWork.DbContext.Comment
                                                   .Where(c => c.UserID == userId)
                                                   .ToList();
                        foreach (var item in allCommentByCurrentUser)
                        {
                            var replyFromOtherUsersForComment = unitOfWork.DbContext.Comment
                                .Where(c => c.ParentCommentID == item.ID && c.UserID != userId && (!c.CommentView.Where(cv => cv.UserId == userId).Any()))
                               .Select(c => new CommentDetails
                               {
                                   CommentId = c.ID,
                                   UserId = c.UserID,
                                   CommentDescription = c.Description,
                                   CreationDate = c.CreationDate,
                                   Image = c.Image,
                                   ParentId = c.ParentCommentID,
                                   SlideId = c.ReportSlideID,
                                   IsViewed = c.CommentView.Where(cv => cv.UserId == userId).Any(),
                                   Message = c.UserDetail.Name + " replied to your Comment",

                               }).OrderByDescending(c => c.CreationDate).ToList();
                            commentDetails.AddRange(replyFromOtherUsersForComment);
                        }

                        var parentCommentsInwhichloggedInUserTag = unitOfWork.DbContext.Comment
                                                                   .Where(c => c.UserID != userId
                                                                    && c.ParentCommentID == -1
                                                                    && c.CommentTagUser.Any(ctu => ctu.UserID == userId))
                                                                    .ToList();

                        foreach (var parentComment in parentCommentsInwhichloggedInUserTag)
                        {

                            var unViewdSubComment = unitOfWork.DbContext.Comment
                                                    .Where(c => c.ParentCommentID == parentComment.ID && c.UserID != userId && !c.CommentView.Any(cv => cv.UserId == userId))
                                                    .ToList();
                            foreach (var subComment in unViewdSubComment)
                            {
                                CommentDetails cd = new CommentDetails
                                {
                                    CommentId = subComment.ID,
                                    UserId = subComment.UserID,
                                    CommentDescription = subComment.Description,
                                    CreationDate = subComment.CreationDate,
                                    Image = subComment.Image,
                                    ParentId = subComment.ParentCommentID,
                                    SlideId = subComment.ReportSlideID,
                                    Message = subComment.UserDetail.Name + " replied on Comment in which you are mentioned."
                                };
                                commentDetails.Add(cd);
                            }
                        }

                        return commentDetails;

                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }


}
