using EDUHUNT_BE.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace EDUHUNT_BE.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
        {

        }
        public virtual DbSet<ScholarshipInfo> ScholarshipInfos { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<QA> QAs { get; set; }
        public virtual DbSet<Profile> Profile { get; set; }
        public virtual DbSet<CV> CVs { get; set; }
        public virtual DbSet<RoadMap> RoadMaps { get; set; }
        public virtual DbSet<Certificate> Certificates { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<CodeVerify> CodeVerifies { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<StudentType> StudentTypes { get; set; }
        public virtual DbSet<Survey> Surveys { get; set; }
        public virtual DbSet<SurveyAnswer> SurveyAnswers { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ScholarshipCategory> ScholarshipCategories { get; set; }
        public virtual DbSet<UserScholarship> UserScholarships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CodeVerify>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.UserId).IsRequired(false);
                entity.Property(e => e.Code).IsRequired();
                entity.Property(e => e.ExpirationTime).IsRequired();
                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.ContentURL).IsRequired();
                entity.Property(e => e.IsApproved).IsRequired();
                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StudentID).IsRequired();
                entity.Property(e => e.ScholarshipID).IsRequired();
                entity.Property(e => e.StudentCV).IsRequired(false);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.MeetingURL).IsRequired(false);
                entity.Property(e => e.ScholarshipProviderAvailableStartDate).IsRequired(false);
                entity.Property(e => e.ScholarshipProviderAvailableEndDate).IsRequired(false);
                entity.Property(e => e.ApplicationReason).IsRequired(false);
            });
            modelBuilder.Entity<RoadMap>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.ContentURL).IsRequired(false);
                entity.Property(e => e.IsApproved).IsRequired();
                entity.Property(e => e.Title).IsRequired(false);
                entity.Property(e => e.Content).IsRequired(false);
                entity.Property(e => e.Location).IsRequired(false);
                entity.Property(e => e.School).IsRequired(false);
                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScholarshipInfo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Budget).HasMaxLength(255).IsRequired(false);
                entity.Property(e => e.Title).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Location).HasMaxLength(255).IsRequired(false);
                entity.Property(e => e.SchoolName).HasMaxLength(255).IsRequired(false);
                entity.Property(e => e.IsInSite);
                entity.Property(e => e.Description).HasColumnType("text").IsRequired(false);
                entity.Property(e => e.Url).HasMaxLength(255).IsRequired(false);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.ImageUrl).IsRequired(false);
                entity.Property(e => e.Level).HasMaxLength(50).IsRequired();
                entity.Property(e => e.IsApproved).IsRequired();
                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(e => e.AuthorId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Sender).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Receiver).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.SentAt).IsRequired();
            });

            modelBuilder.Entity<QA>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AskerId).IsRequired();
                entity.Property(e => e.AnswerId).IsRequired();
                entity.Property(e => e.Subject).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Question).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Answer).HasMaxLength(255).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.ContentURL).IsRequired(false);
                entity.Property(e => e.FirstName).IsRequired(false);
                entity.Property(e => e.LastName).IsRequired(false);
                entity.Property(e => e.UserName).IsRequired(false);
                entity.Property(e => e.ContactNumber).IsRequired(false);
                entity.Property(e => e.Address).IsRequired(false);
                entity.Property(e => e.Description).IsRequired(false);
                entity.Property(e => e.UrlAvatar).IsRequired(false);
                entity.Property(e => e.IsAllow).IsRequired();
                entity.Property(e => e.IsVIP);
            });

            modelBuilder.Entity<CV>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.UrlCV).IsRequired();
            });

            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(e => e.AnswerId).HasName("PK__Answer__D4825024E9F7E377");

                entity.ToTable("Answer");

                entity.Property(e => e.AnswerId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("AnswerID");
                entity.Property(e => e.AnswerText).HasMaxLength(255);
                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("FK__Answer__Question__2F10007B")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.QuestionId).HasName("PK__Question__0DC06F8C4764F5B1");

                entity.ToTable("Question");

                entity.Property(e => e.QuestionId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("QuestionID");
                entity.Property(e => e.Content).HasMaxLength(255);
            });

            modelBuilder.Entity<StudentType>(entity =>
            {
                entity.HasKey(e => e.StudentTypeId).HasName("PK__StudentT__42041D9A39928EDD");

                entity.ToTable("StudentType");

                entity.Property(e => e.StudentTypeId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("StudentTypeID");
                entity.Property(e => e.Money)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.Study)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.TypeName)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Survey>(entity =>
            {
                entity.HasKey(e => e.SurveyId).HasName("PK__Survey__A5481F9D0CCB73F0");

                entity.ToTable("Survey");

                entity.HasIndex(e => e.UserId, "UQ__Survey__1788CCAD0C472ADB").IsUnique();

                entity.Property(e => e.SurveyId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("SurveyID");
                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("Create_at");
                entity.Property(e => e.Description).HasColumnType("text");
                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne<ApplicationUser>()
                  .WithOne()
                  .HasForeignKey<Survey>(d => d.UserId)
                  .HasConstraintName("FK__Survey__UserID__2A4B4B5E")
                  .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SurveyAnswer>(entity =>
            {
                entity.HasKey(e => e.SurveyAnswerId).HasName("PK__SurveyAn__E5C3DB53F4C84D85");

                entity.ToTable("SurveyAnswer");

                entity.Property(e => e.SurveyAnswerId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("SurveyAnswerID");
                entity.Property(e => e.AnswerId).HasColumnName("AnswerID");
                entity.Property(e => e.SurveyId).HasColumnName("SurveyID");

                entity.HasOne(d => d.Answer).WithMany(p => p.SurveyAnswers)
                    .HasForeignKey(d => d.AnswerId)
                    .HasConstraintName("FK__SurveyAns__Answe__32E0915F")
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Survey).WithMany(p => p.SurveyAnswers)
                    .HasForeignKey(d => d.SurveyId)
                    .HasConstraintName("FK__SurveyAns__Surve__31EC6D26")
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            });

            modelBuilder.Entity<ScholarshipCategory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ScholarshipId).IsRequired();
                entity.Property(e => e.CategoryId).IsRequired();

                entity.HasOne(d => d.ScholarshipInfo)
                      .WithMany(p => p.ScholarshipCategories)
                      .HasForeignKey(d => d.ScholarshipId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Category)
                      .WithMany(p => p.ScholarshipCategories)
                      .HasForeignKey(d => d.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<UserScholarship>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ScholarshipId).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
            });
        }
    }
}
