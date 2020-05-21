namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ChartAuditModel : DbContext
    {
        public ChartAuditModel(string masterModelConnectionString)
            : base(masterModelConnectionString)
        {
        }

        public virtual DbSet<AdminDetail> AdminDetail { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<CommentLike> CommentLike { get; set; }
        public virtual DbSet<comments_users> Comments_users { get; set; }
        public virtual DbSet<CommentTagUser> CommentTagUser { get; set; }
        public virtual DbSet<CommentView> CommentView { get; set; }
        public virtual DbSet<Group> Group { get; set; }
        public virtual DbSet<PhraseScore> PhraseScore { get; set; }
        public virtual DbSet<PhraseTable> PhraseTable { get; set; }
        public virtual DbSet<Report> Report { get; set; }
        public virtual DbSet<report_new_copy> Report_new_copy { get; set; }
        public virtual DbSet<ReportCategory> ReportCategory { get; set; }
        public virtual DbSet<ReportFavorite> ReportFavorite { get; set; }
        public virtual DbSet<ReportGroup> ReportGroup { get; set; }
        public virtual DbSet<ReportSlide> ReportSlide { get; set; }
        public virtual DbSet<ReportSubCategory> ReportSubCategory { get; set; }
        public virtual DbSet<ReportView> ReportView { get; set; }
        public virtual DbSet<ShMeasure> ShMeasure { get; set; }
        public virtual DbSet<SlideFavorite> SlideFavorite { get; set; }
        public virtual DbSet<StopWords> StopWords { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<UserDetail> UserDetail { get; set; }
        public virtual DbSet<UserGroup> UserGroup { get; set; }
        public virtual DbSet<UserSearchDetails> UserSearchDetails { get; set; }
        public virtual DbSet<UserSearchThumbnail> UserSearchThumbnail { get; set; }
        public virtual DbSet<Views> Views { get; set; }
        public virtual DbSet<DeviceTokenMapping> DeviceTokenMapping { get; set; }
        public virtual DbSet<PasswordResetMapping> PasswordResetMapping { get; set; }

        public virtual DbSet<PermissionLevel> PermissionLevel { get; set; }

        public virtual DbSet<PermissionLevelMapping> PermissionLevelMappings { get; set; }
        public virtual DbSet<WordScore> WordScore { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminDetail>()
            .Property(e => e.Name)
            .IsUnicode(false);

            modelBuilder.Entity<AdminDetail>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<AdminDetail>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<AdminDetail>()
                .Property(e => e.AdminTelephone)
                .IsUnicode(false);

            modelBuilder.Entity<AdminDetail>()
                .Property(e => e.CompanyName)
                .IsUnicode(false);

            modelBuilder.Entity<AdminDetail>()
                .Property(e => e.Salt)
                .IsUnicode(false);

            modelBuilder.Entity<Comment>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Comment>()
                .Property(e => e.Image)
                .IsUnicode(false);

            modelBuilder.Entity<Comment>()
                .HasMany(e => e.CommentLike)
                .WithRequired(e => e.Comment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Comment>()
                .HasMany(e => e.CommentTagUser)
                .WithRequired(e => e.Comment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Comment>()
                .HasMany(e => e.CommentView)
                .WithRequired(e => e.Comment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Group>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Group>()
                .HasMany(e => e.ReportGroup)
                .WithRequired(e => e.Group)
                .HasForeignKey(e => e.GroupID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Group>()
                .HasMany(e => e.UserGroup)
                .WithRequired(e => e.Group)
                .HasForeignKey(e => e.GroupID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PhraseScore>()
                .Property(e => e.Score)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Report>()
                .Property(e => e.Description)
                .IsUnicode(false);
            
            modelBuilder.Entity<Report>()
                .HasMany(e => e.ReportFavorite)
                .WithRequired(e => e.Report)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Report>()
                .HasMany(e => e.ReportSlide)
                .WithRequired(e => e.Report)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Report>()
                .HasMany(e => e.ReportView)
                .WithRequired(e => e.Report)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Report>()
                .HasMany(e => e.ReportGroup)
                .WithRequired(e => e.Report)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<report_new_copy>()
                .Property(e => e.report_description)
                .IsUnicode(false);

            modelBuilder.Entity<report_new_copy>()
                .Property(e => e.report_image)
                .IsUnicode(false);

            modelBuilder.Entity<ReportCategory>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ReportCategory>()
                .HasMany(e => e.ReportSubCategory)
                .WithRequired(e => e.ReportCategory)
                .WillCascadeOnDelete(false);



            modelBuilder.Entity<ReportSlide>()
                .HasMany(e => e.SlideFavorite)
                .WithRequired(e => e.ReportSlide)
                .HasForeignKey(e => e.SlideID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ReportSubCategory>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ReportSubCategory>()
                .HasMany(e => e.Report)
                .WithRequired(e => e.ReportSubCategory)
                .HasForeignKey(e => e.SubcategoryID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ShMeasure>()
                .Property(e => e.Share)
                .HasPrecision(18, 8);

            modelBuilder.Entity<ShMeasure>()
                .Property(e => e.ShareR2M)
                .HasPrecision(18, 8);

            modelBuilder.Entity<ShMeasure>()
                .Property(e => e.ShareR3M)
                .HasPrecision(18, 8);

            modelBuilder.Entity<UserDetail>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<UserDetail>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<UserDetail>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<UserDetail>()
                .Property(e => e.UserTelephone)
                .IsUnicode(false);

            modelBuilder.Entity<UserDetail>()
                .Property(e => e.CompanyName)
                .IsUnicode(false);

            modelBuilder.Entity<UserDetail>()
                .Property(e => e.Salt)
                .IsUnicode(false);

            modelBuilder.Entity<UserDetail>()
                .HasMany(e => e.Comment)
                .WithRequired(e => e.UserDetail)
                .HasForeignKey(e => e.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserDetail>()
                .HasMany(e => e.CommentLike)
                .WithRequired(e => e.UserDetail)
                .HasForeignKey(e => e.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserDetail>()
                .HasMany(e => e.Comments_users)
                .WithOptional(e => e.UserDetail)
                .HasForeignKey(e => e.user_details_id);

            modelBuilder.Entity<UserDetail>()
                .HasMany(e => e.CommentTagUser)
                .WithRequired(e => e.UserDetail)
                .HasForeignKey(e => e.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserDetail>()
                .HasMany(e => e.CommentView)
                .WithRequired(e => e.UserDetail)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserDetail>()
                .HasMany(e => e.Report)
                .WithRequired(e => e.UserDetail)
                .HasForeignKey(e => e.Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserDetail>()
                .HasMany(e => e.Report1)
                .WithRequired(e => e.UserDetail1)
                .HasForeignKey(e => e.Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserDetail>()
                .HasMany(e => e.ReportFavorite)
                .WithRequired(e => e.UserDetail)
                .HasForeignKey(e => e.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserDetail>()
                .HasMany(e => e.ReportView)
                .WithRequired(e => e.UserDetail)
                .HasForeignKey(e => e.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserDetail>()
                .HasMany(e => e.SlideFavorite)
                .WithRequired(e => e.UserDetail)
                .HasForeignKey(e => e.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserDetail>()
                .HasMany(e => e.UserGroup)
                .WithRequired(e => e.UserDetail)
                .HasForeignKey(e => e.UserID)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<UserDetail>()
                .HasMany(e => e.DeviceTokenMapping)
                .WithRequired(e => e.UserDetail)
                .HasForeignKey(e => e.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserDetail>()
             .HasMany(e => e.PermissionLevelMapping)
             .WithRequired(e => e.UserDetail)
             .HasForeignKey(e => e.UserId)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<DeviceTokenMapping>()
                .Property(e => e.UserDeviceToken)
                .IsUnicode(false);

            modelBuilder.Entity<UserDetail>()
              .HasMany(e => e.PasswordResetMapping)
              .WithRequired(e => e.UserDetail)
              .HasForeignKey(e => e.UserId)
              .WillCascadeOnDelete(false);


            modelBuilder.Entity<UserDetail>()
                .HasMany(e => e.Views)
                .WithOptional(e => e.userDetail)
                .HasForeignKey(e => e.user_details_id);

            modelBuilder.Entity<WordScore>()
                .Property(e => e.Score)
                .HasPrecision(5, 2);

            modelBuilder.Entity<PermissionLevel>()
                .Property(e => e.Role)
                .IsUnicode(false);

            modelBuilder.Entity<PermissionLevel>()
                .HasMany(e => e.PermissionLevelMapping)
                .WithRequired(e => e.PermissionLevel)
                .HasForeignKey(e => e.PermissionId)
                .WillCascadeOnDelete(false);
        }
    }
}
