using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;
using NLog;
using PharmaACE.ChartAudit.IRepository;
using PharmaACE.ChartAudit.Models;
using PharmaACE.ChartAudit.Repository;
using PharmaACE.ChartAudit.Utility;
using PharmaACE.NLP.ChartAudit.NLIDB.ChartAudit;
using PharmaACE.NLP.QuestionAnswerService.Controllers;
using PharmaACE.NLP.QuestionAnswerService.Filters;
using PharmaACE.Utility;

namespace PharmaACE.ChartAudit.Reports.Controllers
{
    [Authorize]
    [TokenExtractFilter]
   // [EnableCors(origins: "*", headers: "*", methods: "*")]        //to do:-Enable for specific port
    public class ReportController : BaseController
    {
        IReportService reportService;
       
        public ReportController(IReportService reportService)
        {
            this.reportService = reportService;
          
        }

        static Logger logger = LogManager.GetCurrentClassLogger();

        [Route("api/Report/GetCategoryList")]
        public IHttpActionResult GetCategoryList()
        {
            List<ReportCategoryModel> reportCategoryModel = null;
            ActionStatus status = new ActionStatus();
            try
            {
                reportCategoryModel = reportService.GetCategoryList(permission);
                if (!reportCategoryModel.AnyOrNotNull())
                {
                    throw new NoDataFoundException();
                }
            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/GetCategoryList: {0} \r\n {1}", ex.ToString(), ex.StackTrace);

            }
            if (status.Number != -1)
                return Ok (new { reportCategoryModel = reportCategoryModel, Status = status } );
            else
                return InternalServerError();
        }
        
        [Route("api/Report/AddNewCategory")]
        [HttpPost]
        public IHttpActionResult AddNewCategory(ReportCategoryModel reportCategoryModel)
        {
            int result = 0;
            ActionStatus status = new ActionStatus(); 
            try
            {
                result = reportService.AddNewCategory(reportCategoryModel, permission);                
            }
            catch(ReportServiceException ex)
            {
                status.Number = (int)ex.ErrorCodeService;
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }

            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/AddNewCategory: {0} \r\n {1}", ex.ToString(), ex.StackTrace);

            }
            if (status.Number!= -1)
                return Ok (new { result = result, Status = status } );
            else
                return InternalServerError();
        }

        [HttpPost]
        [Route("api/Report/AddNewSubCategory")]
        public IHttpActionResult AddNewSubCategory(ReportCategoryModel reportCategoryModel)
        {
            int result = 0;
            ActionStatus status = new ActionStatus();
            try
            {
                result = reportService.AddNewSubCategory(reportCategoryModel,permission);
            }

            catch(ReportServiceException ex)
            {
                status.Number = (int)ex.ErrorCodeService;
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/AddNewCategory: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok(new {  result = result, Status = status });
            else
                return InternalServerError();
        }


        [Route("api/Report/GetSpecificCategory")]
        [HttpGet]
        public IHttpActionResult GetSpecificCategory(int categoryId)
        {
          
            ReportCategoryModel reportCategoryModel = null;
            ActionStatus status = new ActionStatus();
            try
            {
                reportCategoryModel = reportService.GetSpecificCategorywithSub(categoryId,permission);

                if(reportCategoryModel==null)
                {
                    throw new NoDataFoundException();
                }
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/GetSpecificCategory: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok (new {  reportCategoryModel = reportCategoryModel, Status = status } );
            else
                return InternalServerError();
        }


        [Route("api/Report/GetSubCategory")]
        [HttpGet]
        public IHttpActionResult GetSubCategory()
        {

            List<ReportSubCategoryModel> reportSubCategoryModel = null;
            ActionStatus status = new ActionStatus();
            try
            {
                reportSubCategoryModel = reportService.GetSubCategories(permission);

                if (reportSubCategoryModel == null)
                {
                    throw new NoDataFoundException();
                }
            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/GetSpecificCategory: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok (new { reportSubCategoryModel = reportSubCategoryModel, Status = status } );
            else
                return InternalServerError();
        }

        [HttpPut]
        [Route("api/Report/UpdateReportCategory")]
        public IHttpActionResult UpdateReportCategory(ReportCategoryModel reportCategoryModel)
        {
            int result = 0;
            ActionStatus status = new ActionStatus();
            try
            {
                result = reportService.UpdateReportCategory(reportCategoryModel,permission);
                
            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/UpdateReportCategory: {0} \r\n {1}", ex.ToString(), ex.StackTrace);

            }
            if (status.Number != -1)
                return Ok (new {  result = result, Status = status } );
            else
                return InternalServerError();
        }

        [HttpPut]
        [Route("api/Report/UpdateReportSubCategory")]
        public IHttpActionResult UpdateReportSubCategory(ReportCategoryModel reportCategoryModel)
        {
            int result = 0;
            ActionStatus status = new ActionStatus();
            try
            {
                result = reportService.UpdateReportSubCategory(reportCategoryModel,permission);
            }
            catch(ReportServiceException ex)
            {
                status.Number = (int)ex.ErrorCodeService;
                logger.Info("ReportSubCategory not updated.");
            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
                status.Message = "ReportSubCategory not updated";
                logger.Info("ReportSubCategory not updated.");

            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/UpdateReportSubCategory: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok (new {  result = result, Status = status } );
            else
                return InternalServerError();
        }


        [Route("api/Report/DeleteSubCategory")]
        [HttpDelete]
        public IHttpActionResult DeleteSubCategory(int subcategoryId)
        {
            ActionStatus status = new ActionStatus();
            int result = 0;
            try
            {
                result = reportService.DeleteSubCategory(subcategoryId,permission);
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {

                status.Number = -1;
                logger.Error("Exception in Report/DeleteSubCategory: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok (new { result = result, Status = status } );
            else
                return InternalServerError();
        }


        [Route("api/Report/DeleteCategory")]
        [HttpDelete]
        public IHttpActionResult DeleteCategory(int categoryId)
        {
            ActionStatus status = new ActionStatus();
            int result = 0;
            try
            {
                result = reportService.DeleteCategory(categoryId,permission);
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/DeleteCategory: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok (new { result = result, Status = status } );
            else
                return InternalServerError();
        }


        [HttpPost]
        [Route("api/Report/AddNewReport")]
        public IHttpActionResult AddNewReport()
        {
            string msg = String.Empty;
            int result = 0;
            ActionStatus status = new ActionStatus();
            try
            {
                HttpRequest httpRequest = HttpContext.Current.Request;

                string reportDetailsDyn = httpRequest.Unvalidated["ReportDetail"];
                List<ReportDetail> reportDetails = new JavaScriptSerializer { }.Deserialize<List<ReportDetail>>(reportDetailsDyn);
                Validator.ValidateReportDetail(reportDetails);                                                 //to do =>FluentValidation of POST api call having multiple/formData. link=>http://blog.marcinbudny.com/2014/02/sending-binary-data-along-with-rest-api.html
                if (reportDetails.Count != httpRequest.Files.Count)
                {
                    logger.Info("Count mismatch in Report Details and File Count");
                    throw new CountMismatchException();
                }
                else
                {
                    List<ReportInformation> reportInformation = new List<ReportInformation>();

                    for (int i = 0; i < httpRequest.Files.Count; i++)

                        if (Util.IsPPT(httpRequest.Files[i].ContentType))
                        {
                            string filename = httpRequest.Files[i].FileName;


                            if (Regex.IsMatch(filename, @"^[a-zA-Z0-9\s.\?\,\;\:\&\!\-\\_]+$"))   //Filename Regex
                            {
                                if (httpRequest.Files[i].ContentLength <= Util.MaxFileSize)
                                {
                                    using (var binaryReader = new BinaryReader(httpRequest.Files[i].InputStream))
                                    {
                                        using (var filestream = new MemoryStream(binaryReader.ReadBytes(httpRequest.Files[i].ContentLength)))
                                        {
                                            reportInformation.Add(new ReportInformation
                                            {

                                                ReportDetail = reportDetails[i],
                                                FileStream = filestream   //to do==>close memorystream
                                            });

                                            if (i == httpRequest.Files.Count - 1)
                                            {
                                                result = reportService.AddNewReport(reportInformation, permission);
                                            }

                                        }

                                    }
                                }

                                else
                                {
                                    logger.Info("File:{0} excceeds maximum threshold of 10Mb", httpRequest.Files[i].FileName);
                                    throw new InvalidFileSizeException();
                                }

                            }
                            else
                            {
                                logger.Info("Invalid File Name of :{0} ", httpRequest.Files[i].FileName);
                                throw new FileNameNotValidException();
                            }
                        }
                        else
                        {
                            logger.Info("Invalid File format of :{0}", httpRequest.Files[i].FileName);
                            throw new FormatNotSupportedException();
                        }

                }

            }
            catch(ReportServiceException ex)
            {
                status.Number = (int)ex.ErrorCodeService;
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/addNewReport: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok(new {  result = result, Status = status } );
            else
                return InternalServerError();
        }

       
        [Route("api/Report/GetAllReports")]
        public IHttpActionResult GetAllReports()
        {
            List<ReportDetail> reportDetails = null;
            ActionStatus status = new ActionStatus();
            try
            {
                reportDetails = reportService.GetReportList(permission);
                if (!reportDetails.AnyOrNotNull())
                {
                    throw new NoDataFoundException();
                } 
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/GetAllReports: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok(new {  reportDetails = reportDetails, Status = status } );
            else
                return InternalServerError();
        }


        [Route("api/Report/GetSpecificReport")]
        [HttpGet]
        public IHttpActionResult GetSpecificReport(int reportId)
        {
            ReportDetail reportDetail = null;
            ActionStatus status = new ActionStatus();
            try
            {
                reportDetail = reportService.GetSpecificReport(reportId,permission);
                if(reportDetail==null)
                {
                    throw new NoDataFoundException();
                }
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/GetSpecificReport: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok(new { reportDetail = reportDetail, Status = status } );
            else
                return InternalServerError();
        }

        [HttpPut]
        [Route("api/Report/UpdateReport")]
        public IHttpActionResult UpdateReport(ReportDetail reportDetail)
        {
            int result = 0;
            ActionStatus status = new ActionStatus(); 
            try
            {
                result = reportService.UpdateReport(reportDetail,permission);
              
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/UpdateReport: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }

            if (status.Number != -1)
                return Ok(new { result = result, Status = status } );
            else
                return InternalServerError();
        }


        [Route("api/Report/DeleteReport")]
        [HttpDelete]
        public IHttpActionResult DeleteReport(int reportId)
        {
            int result = 0;
            ActionStatus status = new ActionStatus();
            try
            {

                result = reportService.DeleteReport(reportId,userId,permission);
            }
            catch (BaseException ex)
            {
                status.Number =(int) ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/DeleteReport: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok (new { result = result, Status = status } );
            else
                return InternalServerError();
        }


        [Route("api/Report/GetReports")]

        public IHttpActionResult GetReports()
        {
            logger.Info("Inside Report/GetReports");
            ActionStatus status = new ActionStatus();
            List<ReportDetail> reportDetails = new List<ReportDetail>();
            try
            {
                logger.Info(" ReportList received Successfully");
                reportDetails = reportService.GetReports(userId,permission);
                if (!reportDetails.AnyOrNotNull())
                {
                    throw new NoDataFoundException();
                }
            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
                status.Message = "ReportList not found";
                logger.Info("ReportList not found.");
               
            }
            catch(Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/GetReport: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number!=-1)
                return Ok (new {Reports = reportDetails, Status = status });
            else
                return InternalServerError();
        }

        [HttpGet]
        [Route("api/Report/GetLatestReports")]
        public IHttpActionResult GetLatestReports()
        {
            logger.Info("Inside Report/GetLatestReports");
            ActionStatus status = new ActionStatus();
            //CategoryReportDetails reportDetails = new CategoryReportDetails();
            List<ReportDetail> reportDetails = new List<ReportDetail>();
            try
            {
                logger.Info(" ReportList received Successfully");
                reportDetails = reportService.GetLatestReports(userId, permission);
                if (!reportDetails.AnyOrNotNull())
                {
                    throw new NoDataFoundException();
                }
            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
                status.Message = "ReportList not found";
                logger.Info("ReportList not found.");

            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/GetReport: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok(new { Reports = reportDetails, Status = status });
            else
                return InternalServerError();
        }



        [Route("api/Report/GetCategories")]
        public IHttpActionResult GetCategories()
        {
            logger.Info("Inside Report/GetCategories");
            List<Categories> categories = new List<Categories>();
            ActionStatus status = new ActionStatus();
            categories = reportService.GetCategories(permission);
            try
            {
                if (!categories.AnyOrNotNull())
                {
                    throw new NoDataFoundException();
                }
            }
            catch(BaseException ex)
            {
                status.Number =(int)ex.ErrorCode;
                status.Message = "Categories not found";
                logger.Info("Categories not found.");
            }
            catch (Exception ex)
            {
                status.Number = -1;
            }
            if (status.Number!=-1)
                return Ok(new {  Categories = categories, Status = status } );
            else
                return InternalServerError();
        }


        [Route("api/Report/GetReportSlides/{reportId}")]
        [HttpGet]
        public IHttpActionResult GetReportSlides(int reportId)   //change favorite:userreportmapping
        {
            logger.Info("Inside Report/GetReportSlides");
            ActionStatus status = new ActionStatus();
            ReportInfo ReportInfoWithSlide = null;
            ReportInfoWithSlide = reportService.GetReportSlides(reportId,userId,permission);
            try
            {
                if (ReportInfoWithSlide == null)
                {
                    throw new NoDataFoundException();
                }

            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
                status.Message = "ReportSlides not found";
                logger.Info("ReportSlides not found.");

            }
            catch (Exception ex)
            {
                status.Number = -1;
            }
            if (status.Number != -1)
                return Ok (new { ReportSlides = ReportInfoWithSlide, Status = status });
            else
                return InternalServerError();

        }

        [HttpGet]
        [Route("api/Report/GetFavoriteReportsSlides")]       
        public IHttpActionResult GetFavoriteReportsSlides()
        {
            logger.Info("Inside Report/GetFavoriteReportsSlides");
            ActionStatus status = new ActionStatus();
            var reportSlideFavoriteResult = new ReportAndSlideFavorite();
            ReportAndSlideFavorite reportAndSlideFavorite = new ReportAndSlideFavorite();
            try
            {
                reportAndSlideFavorite = reportService.GetFavoriteReportsSlides(userId,permission);
                if (reportAndSlideFavorite != null && reportAndSlideFavorite.Reports == null && reportAndSlideFavorite.Slides == null)
                {
                   
                    throw new NoDataFoundException();
                }

            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
                status.Message = "FavoriteReportSlides not found.";
                logger.Info("FavoriteReportSlides not found.");
            }
            catch (Exception ex)
            {
               status.Number = -1;
            }           
           if (status.Number!= -1)
                return Ok(new { ReportAndSlideFavorite = reportAndSlideFavorite, Status= status } );
            else
                return InternalServerError();
        }
        [HttpGet]
        [Route("api/Report/FavoriteReport/{reportId}")]
        public IHttpActionResult FavoriteReport(int reportId)    
        {
            logger.Info("Inside Report/FavoriteReport");
            ActionStatus status = new ActionStatus();
            bool like = true;
            try
            {
                like = reportService.FavoriteReport(reportId,userId,permission);
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/FavoriteReport: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number!= -1)
                return Ok(new { IsLiked = like ,Status=status } );
            else
                return InternalServerError();
        }
        [HttpGet]
        [Route("api/Report/FavoriteSlide/{slideId}")]
        public IHttpActionResult FavoriteSlide(int slideId)
        {
            logger.Info("Inside Report/FavoriteSlides");
            ActionStatus status = new ActionStatus();
            bool like = true;
           
            try
            {
                like = reportService.FavoriteSlide(slideId,userId,permission); 
                
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/FavoriteSlide: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number !=-1)
                return Ok(new {IsLiked = like,Status=status });
            else
                return InternalServerError();
        }
        [HttpGet]
        [Route("api/Report/ViewReport/{reportId}")]
        public IHttpActionResult ViewReport(int reportId)
        {
            logger.Info("Inside Report/ViewReport");
            ActionStatus status = new ActionStatus();

            bool like = true;
            try
            {
                like = reportService.ReportView(reportId,userId,permission);
              
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/ViewReport: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number!=-1)
                return Ok(new { Result = like ,Status=status} );
            else
                return InternalServerError();
        }

        [HttpGet]
        [Route("api/Report/ViewComment/{commentId}")]
        
        public IHttpActionResult ViewComment(int commentId)
        {
            logger.Info("Inside Report/ViewComment");
            ActionStatus status = new ActionStatus();
            bool like = false;
            try
            {
                like = reportService.ViewComment(commentId,userId,permission);
               
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/ViewComment: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number!=-1)
                return Ok(new {  Result = like,Status=status} );
            else
                return InternalServerError();
        }

        [HttpGet]
        [Route("api/Report/CommentLike/{commentId}")]
     
        public IHttpActionResult CommentLike(int commentId)
        {
            logger.Info("Inside Report/CommentLike");
            ActionStatus status = new ActionStatus();
            bool like = false;
            try
            {
                like = reportService.CommentLike(commentId,userId,permission);               
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/CommentLike: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number!= -1)
                return Ok(new {  IsLiked = like,Status=status });
            else
                return InternalServerError();
        }
        [Route("api/Report/SendComment")]
        [HttpPost]
        public IHttpActionResult SendComment(CommentDetails cmtDl)
        {
            logger.Info("Inside Report/SendComment");
            ActionStatus status = new ActionStatus();
            //int result = 0;
            try
            {
              reportService.SendComment(cmtDl,permission);
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                status.Message = "Comment not Sent";
                logger.Info("Comment not Sent.");
                logger.Error("Exception in Report/SendComment: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
               
            }
            if (status.Number!= -1)
                return Ok(new { Status = status });
            else
                return InternalServerError();
        }

        [HttpGet]
        [Route("api/Report/GetFavoriteComments")]
       
        public IHttpActionResult GetFavoriteComments()
        {
            logger.Info("Inside Report/GetFavoriteComments");
            ActionStatus status = new ActionStatus();
            List<CommentDetails> commentDetails = new List<CommentDetails>();
            try
            {
                commentDetails = reportService.GetFavoriteComments(userId,permission);
                if (!commentDetails.AnyOrNotNull())
                {
                    throw new NoDataFoundException();
                }
            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
                status.Message = "Favorite CommentsList not found";
                logger.Info("Favorite CommentsList not found.");
            }

            catch (Exception ex)
            {
                logger.Error("Exception in Report/GetFavoriteComments: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok(new { Comments = commentDetails, Status = status });
            else
                return InternalServerError();

        }

        [HttpGet]
        [Route("api/Report/GetComments")]

        public IHttpActionResult GetComments()
        {
            logger.Info("Inside Report/GetComments");
            ActionStatus status = new ActionStatus();
            List<CommentDetails> CommentsArr = new List<CommentDetails>();
            try
            {
                CommentsArr = reportService.GetComments(userId,permission);
                if (!CommentsArr.AnyOrNotNull())
                {
                    throw  new NoDataFoundException();
                }
            }
           catch (BaseException ex)
            {
                 status.Number = (int)ex.ErrorCode;
                status.Message = "CommentsList not found";
                logger.Info("CommentsList not found.");
                   
            }
            
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/GetComments: {0}", ex.ToString());  //remove stacktrace
            }
            if (status.Number != -1)
                return Ok(new {  Comments = CommentsArr, Status = status });
            else
                return InternalServerError();
        }

        [HttpGet]
        [Route("api/Report/GetSlideImage/{ID}")]

        public IHttpActionResult GetSlideImage(int ID)
        {
            List<string> reportbase64 = null;
            ActionStatus status = new ActionStatus();
            reportbase64 = reportService.GetSlideImage(ID);
            try
            {
                if (!reportbase64.AnyOrNotNull())
                {
                    throw new NoDataFoundException();
                }

            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
                status.Message = "SlideImage not found";
                logger.Info("SlideImage not found.");

            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/GetSlideImage: {0}", ex.ToString());
            }
            if (status.Number != -1)
                return Ok(new { Image = reportbase64, Status = status });
            else
                return InternalServerError();

        }

        [HttpGet]
        [Route("api/Report/GetSubComments/{commentId}")]

        public IHttpActionResult GetSubComments(int commentId)
        {
            logger.Info("Inside Report/GetSubComments/{commentId}");
            ActionStatus status = new ActionStatus();
            List<CommentDetails> subCommentInfo = new List<CommentDetails>();
            try
            {
                subCommentInfo = reportService.GetSubComments(commentId,permission);
                if (!subCommentInfo.AnyOrNotNull())
                {
                    throw new NoDataFoundException();
                }
            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
                status.Message = "SubComments List not found";
                logger.Info("SubComments List not found.");
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/GetSubComments: {0} \r\n {1}", ex.ToString(), ex.StackTrace);

            }
            if (status.Number != -1)
                return Ok(new { SubComments = subCommentInfo , Status = status });
            else
                return InternalServerError();
        }

        [HttpDelete]
        [Route("api/Report/DeleteComment/{commentId}")]                     //creator of comment
        public IHttpActionResult DeleteComment(int commentId)           //list of ids,creator ,
        {
            logger.Info("Inside Report/DeleteComment");
            ActionStatus status = new ActionStatus();
            int result = 0;
            try
            {
                result=reportService.DeleteComment(commentId,permission);
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
                status.Message = "Delete comment failed";
                logger.Info("Delete comment failed");
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/DeleteComment: {0} \r\n {1}", ex.ToString(), ex.StackTrace);

            }
            if (status.Number != -1)
                return Ok(new {Result = result, Status = status });
            else
                return InternalServerError();
            }
        

        [Route("api/Report/FilterReport")]
        [HttpPost]
        public IHttpActionResult FilterReport(ReportFilter item)
        {
            logger.Info("Inside Report/FilterReport");
            ActionStatus status = new ActionStatus();
            List<ReportDetail> filterreport = new List<ReportDetail>();
            try
            {
                filterreport = reportService.FilterReport(item,permission);
                if (!filterreport.AnyOrNotNull())
                {
                    throw new NoDataFoundException();
                }
            }
            catch (BaseException ex)
            {
                status.Number =(int)ex.ErrorCode;
                status.Message = "No FilterReports Found";
                logger.Info("No FilterReports Found.");
            }
             catch(Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/Filterreport: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok(new {  Reports = filterreport, Status = status });
            else
                return InternalServerError();
        }

        [Route("api/Report/FilterComment")]
        [HttpPost]
        public IHttpActionResult FilterComment(CommentsFilter item)
        {
            logger.Info("Inside Report/Filtercomment");
            ActionStatus status = new ActionStatus();

            List<CommentDetails> commentDetails= new List<CommentDetails>();
            try
            {
               commentDetails = reportService.FilterComment(item,permission);
                if (!commentDetails.AnyOrNotNull())
                {
                    throw new NoDataFoundException();
                }
            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
                status.Message = "No filtercomments found";
                logger.Info("No filterComments Found.");
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/Filtercomment: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok(new {  Comments = commentDetails, Status = status });
            else
                return InternalServerError();
        }

        [Route("api/Report/SendMailWithSlide")]
        [HttpPost]
        public IHttpActionResult SendMailWithSlide(MailWithSlide mail)
        {
            logger.Info("Inside Report/SendMailWithSlide");
            string path = HostingEnvironment.MapPath("~/Content/Images");
            ActionStatus status = new ActionStatus();
            var result = false;
            try
            {
                result = reportService.SendMailWithSlide(mail,path,permission);
                if (result == true)
                {
                    logger.Info("Mail sent Successfully to :" + mail.ReceiverIds);
                }
                else
                {
                    throw new MailNotSentException();
                }

            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
                status.Message = "Mail not sent.";
                logger.Info("Mail not Sent.");
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/SendMailWithSlide: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok(new {IsSent=result, Status = status });
            else
                return InternalServerError();
        }

        [Route("api/Report/SendMailWithChart")]
        [HttpPost]
        public IHttpActionResult SendMailWithChart(MailWithSlide mail)
        {
            logger.Info("Inside Report/SendMailWithChart");
            string path = HostingEnvironment.MapPath("~/Content/Images");
            ActionStatus status = new ActionStatus();
            var result = false;
            try
            {
                result = reportService.SendMailWithChart(mail,path,permission);
                if (result == true)
                {
                    logger.Info("Mail sent Successfully to :" + mail.ReceiverIds);
                }
                else
                {
                    throw new MailNotSentException();
                }
            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
                status.Message = "Failed to sent Mail";
                logger.Info("Fail to sent mail to" + mail.ReceiverIds);
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/SendMailWithChart: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok(new { IsSent = result,Status=status});
            else
                return InternalServerError();
        }


        [HttpGet]
        [Route("api/Report/GetNotification")]
        public IHttpActionResult GetNotification()
        {
            logger.Info("Inside Report/GetNotification");
            ActionStatus status = new ActionStatus();
            List<CommentDetails> commentDetails = new List<CommentDetails>();
            try
            {
                commentDetails = reportService.GetNotification(userId,permission);
                if (!commentDetails.AnyOrNotNull())
                {
                    throw new NoDataFoundException();
                }
            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
                status.Message = "No notifications received";
                logger.Info("Failed to send Notification to" + userId);
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/GetNotification: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok(new {  Comments = commentDetails, Status = status } );
            else
                return InternalServerError();
        }


        [HttpGet]
        [Route("api/Report/GetReportImage")]
        public IHttpActionResult GetReportImage(List<int> slideIds)
        {
            List<string> reportbase64 = null;
            ActionStatus status = new ActionStatus();
            reportbase64 = reportService.GetReportImage(slideIds, permission);
            try
            {

                if (reportbase64.AnyOrNotNull())
                {
                    throw new NoDataFoundException();
                }

            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
                status.Message = "ReportSlide not found";
                logger.Info("ReportSlides not found.");

            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Report/GetReportImage: {0} \r\n {1}", ex.ToString(), ex.StackTrace);

            }
            if (status.Number != -1)
                return Ok(new {  reportbase64 = reportbase64, Status = status } );
            else
                return InternalServerError();

        }
    }      
}