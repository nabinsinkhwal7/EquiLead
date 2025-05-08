using System;
using System.Collections.Generic;
using EquidCMS.Dto;
using EquidCMS.Models.KeyLessModels;
using Microsoft.EntityFrameworkCore;

namespace EquidCMS.Models;

public partial class EquiDbContext : DbContext
{
    public EquiDbContext()
    {
    }

    public EquiDbContext(DbContextOptions<EquiDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Applicant> Applicants { get; set; }

    public virtual DbSet<ApplicantCareerPreference> ApplicantCareerPreferences { get; set; }

    public virtual DbSet<ApplicantCertificationTraning> ApplicantCertificationTranings { get; set; }

    public virtual DbSet<ApplicantEducation> ApplicantEducations { get; set; }

    public virtual DbSet<ApplicantLanguage> ApplicantLanguages { get; set; }

    public virtual DbSet<ApplicantProfile> ApplicantProfiles { get; set; }

    public virtual DbSet<ApplicantSkill> ApplicantSkills { get; set; }

    public virtual DbSet<ApplicantVolunteerExperience> ApplicantVolunteerExperiences { get; set; }

    public virtual DbSet<ApplicantWorkExperience> ApplicantWorkExperiences { get; set; }

    public virtual DbSet<JobClickLog> JobClickLogs { get; set; }
    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<MstEventpricing> MstEventpricings { get; set; }

    public virtual DbSet<MstEventtype> MstEventtypes { get; set; }

    public virtual DbSet<MstLookup> MstLookups { get; set; }

    public virtual DbSet<MstMenu> MstMenus { get; set; }

    public virtual DbSet<MstRole> MstRoles { get; set; }

    public virtual DbSet<MstRsdocumenttype> MstRsdocumenttypes { get; set; }

    public virtual DbSet<MstTheme> MstThemes { get; set; }

    public virtual DbSet<MstUser> MstUsers { get; set; }

    public virtual DbSet<NewsletterSubscription> NewsletterSubscriptions { get; set; }

    public virtual DbSet<ReferralCode> ReferralCodes { get; set; }

    public virtual DbSet<ResourceClickLog> ResourceClickLogs { get; set; }

    public virtual DbSet<RoleMenu> RoleMenus { get; set; }

    public virtual DbSet<TblSocialLinkdin> TblSocialLinkdins { get; set; }

    public virtual DbSet<Tblaboutu> Tblaboutus { get; set; }

    public virtual DbSet<Tblcompany> Tblcompanies { get; set; }

    public virtual DbSet<Tblevent> Tblevents { get; set; }

    public virtual DbSet<Tbleventbenefit> Tbleventbenefits { get; set; }

    public virtual DbSet<Tbleventparticipant> Tbleventparticipants { get; set; }

    public DbSet<ResourceExport> ResourceExports { get; set; }
    public virtual DbSet<Tblevidence> Tblevidences { get; set; }

    public virtual DbSet<Tblinfographic> Tblinfographics { get; set; }

    public virtual DbSet<Tbljob> Tbljobs { get; set; }

    public virtual DbSet<Tbllandingpage> Tbllandingpages { get; set; }

    public virtual DbSet<Tblourteam> Tblourteams { get; set; }

    public virtual DbSet<Tblresource> Tblresources { get; set; }

    public virtual DbSet<Tblsuccesstest> Tblsuccesstests { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = config.GetConnectionString("EquiDB");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<ResourceExport>().HasNoKey();
        modelBuilder.Entity<Applicant>(entity =>
        {
            entity.HasKey(e => e.ApplicantId).HasName("applicant_pkey");

            entity.ToTable("applicant");

            entity.Property(e => e.ApplicantId)
                .HasDefaultValueSql("nextval('applicants_applicant_id_seq'::regclass)")
                .HasColumnName("applicant_id");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(10)
                .HasColumnName("country_code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.IsMigrated)
                .HasDefaultValue(false)
                .HasColumnName("is_migrated");
            entity.Property(e => e.LinkedinProfile)
                .HasMaxLength(255)
                .HasColumnName("linkedin_profile");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.Password)
                .HasDefaultValueSql("'password'::character varying")
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(100)
                .HasColumnName("phone_number");
            entity.Property(e => e.ReferenceCode)
                .HasMaxLength(10)
                .HasColumnName("reference_code");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ApplicantCareerPreference>(entity =>
        {
            entity.HasKey(e => e.PreferenceId).HasName("career_preferences_pkey");

            entity.ToTable("applicant_career_preferences");

            entity.HasIndex(e => e.ApplicantId, "applicant_career_preferences_applicant_id_key").IsUnique();

            entity.Property(e => e.PreferenceId)
                .HasDefaultValueSql("nextval('career_preferences_preference_id_seq'::regclass)")
                .HasColumnName("preference_id");
            entity.Property(e => e.ApplicantId).HasColumnName("applicant_id");
            entity.Property(e => e.EmploymentTypePreference)
                .HasMaxLength(100)
                .HasColumnName("employment_type_preference");
            entity.Property(e => e.IndustriesOfInterest)
                .HasMaxLength(255)
                .HasColumnName("industries_of_interest");
            entity.Property(e => e.LeadershipAspirations)
                .HasMaxLength(255)
                .HasColumnName("leadership_aspirations");
            entity.Property(e => e.PreferredJobLocation)
                .HasColumnType("character varying")
                .HasColumnName("preferred_job_location");
            entity.Property(e => e.PreferredJobRole)
                .HasMaxLength(255)
                .HasColumnName("preferred_job_role");
            entity.Property(e => e.WillingToRelocate)
                .HasColumnType("character varying")
                .HasColumnName("willing_to_relocate");

            entity.HasOne(d => d.Applicant).WithOne(p => p.ApplicantCareerPreference)
                .HasForeignKey<ApplicantCareerPreference>(d => d.ApplicantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("career_preferences_applicant_id_fkey");
        });

        modelBuilder.Entity<ApplicantCertificationTraning>(entity =>
        {
            entity.HasKey(e => e.CertificationTrainingId).HasName("certifications_pkey");

            entity.ToTable("applicant_certification_traning");

            entity.Property(e => e.CertificationTrainingId)
                .HasDefaultValueSql("nextval('certifications_certification_id_seq'::regclass)")
                .HasColumnName("certification_training_id");
            entity.Property(e => e.ApplicantId).HasColumnName("applicant_id");
            entity.Property(e => e.IssuingOrganization)
                .HasMaxLength(255)
                .HasColumnName("issuing_organization");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.YearEarned).HasColumnName("year_earned");

            entity.HasOne(d => d.Applicant).WithMany(p => p.ApplicantCertificationTranings)
                .HasForeignKey(d => d.ApplicantId)
                .HasConstraintName("certifications_applicant_id_fkey");
        });

        modelBuilder.Entity<ApplicantEducation>(entity =>
        {
            entity.HasKey(e => e.EducationId).HasName("education_pkey");

            entity.ToTable("applicant_education");

            entity.Property(e => e.EducationId)
                .HasDefaultValueSql("nextval('education_education_id_seq'::regclass)")
                .HasColumnName("education_id");
            entity.Property(e => e.ApplicantId).HasColumnName("applicant_id");
            entity.Property(e => e.Degree)
                .HasMaxLength(255)
                .HasColumnName("degree");
            entity.Property(e => e.FieldOfStudy)
                .HasMaxLength(255)
                .HasColumnName("field_of_study");
            entity.Property(e => e.InstitutionName)
                .HasMaxLength(255)
                .HasColumnName("institution_name");
            entity.Property(e => e.YearOfCompletion).HasColumnName("year_of_completion");

            entity.HasOne(d => d.Applicant).WithMany(p => p.ApplicantEducations)
                .HasForeignKey(d => d.ApplicantId)
                .HasConstraintName("education_applicant_id_fkey");
        });

        modelBuilder.Entity<ApplicantLanguage>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("languages_pkey");

            entity.ToTable("applicant_languages");

            entity.Property(e => e.LanguageId)
                .HasDefaultValueSql("nextval('languages_language_id_seq'::regclass)")
                .HasColumnName("language_id");
            entity.Property(e => e.ApplicantId).HasColumnName("applicant_id");
            entity.Property(e => e.Language)
                .HasMaxLength(100)
                .HasColumnName("language");
            entity.Property(e => e.ProficiencyLevel).HasColumnName("proficiency_level");

            entity.HasOne(d => d.Applicant).WithMany(p => p.ApplicantLanguages)
                .HasForeignKey(d => d.ApplicantId)
                .HasConstraintName("languages_applicant_id_fkey");
        });

        modelBuilder.Entity<ApplicantProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("profiles_pkey");

            entity.ToTable("applicant_profiles");

            entity.HasIndex(e => e.ApplicantId, "profiles_applicant_id_key").IsUnique();

            entity.Property(e => e.ProfileId)
                .HasDefaultValueSql("nextval('profiles_profile_id_seq'::regclass)")
                .HasColumnName("profile_id");
            entity.Property(e => e.ApplicantId).HasColumnName("applicant_id");
            entity.Property(e => e.CommunityAdvocacyExperience).HasColumnName("community_advocacy_experience");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DesiredSalaryRange)
                .HasMaxLength(50)
                .HasColumnName("desired_salary_range");
            entity.Property(e => e.Headline)
                .HasMaxLength(255)
                .HasColumnName("headline");
            entity.Property(e => e.InternationalExperience).HasColumnName("international_experience");
            entity.Property(e => e.JobSearchStatus).HasColumnName("job_search_status");
            entity.Property(e => e.PersonalWebsite)
                .HasMaxLength(255)
                .HasColumnName("personal_website");
            entity.Property(e => e.Pronouns)
                .HasMaxLength(50)
                .HasColumnName("pronouns");
            entity.Property(e => e.ShortBio).HasColumnName("short_bio");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Applicant).WithOne(p => p.ApplicantProfile)
                .HasForeignKey<ApplicantProfile>(d => d.ApplicantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("profiles_applicant_id_fkey");
        });

        modelBuilder.Entity<ApplicantSkill>(entity =>
        {
            entity.HasKey(e => e.SkillId).HasName("skills_pkey");

            entity.ToTable("applicant_skills");

            entity.Property(e => e.SkillId)
                .HasDefaultValueSql("nextval('skills_skill_id_seq'::regclass)")
                .HasColumnName("skill_id");
            entity.Property(e => e.ApplicantId).HasColumnName("applicant_id");
            entity.Property(e => e.SkillName)
                .HasMaxLength(255)
                .HasColumnName("skill_name");
            entity.Property(e => e.SkillType).HasColumnName("skill_type");

            entity.HasOne(d => d.Applicant).WithMany(p => p.ApplicantSkills)
                .HasForeignKey(d => d.ApplicantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("skills_applicant_id_fkey");
        });

        modelBuilder.Entity<ApplicantVolunteerExperience>(entity =>
        {
            entity.HasKey(e => e.VolunteerId).HasName("volunteer_experience_pkey");

            entity.ToTable("applicant_volunteer_experience");

            entity.Property(e => e.VolunteerId)
                .HasDefaultValueSql("nextval('volunteer_experience_volunteer_id_seq'::regclass)")
                .HasColumnName("volunteer_id");
            entity.Property(e => e.ApplicantId).HasColumnName("applicant_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.OrganizationName)
                .HasMaxLength(255)
                .HasColumnName("organization_name");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasColumnName("role");
            entity.Property(e => e.StartDate).HasColumnName("start_date");

            entity.HasOne(d => d.Applicant).WithMany(p => p.ApplicantVolunteerExperiences)
                .HasForeignKey(d => d.ApplicantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("volunteer_experience_applicant_id_fkey");
        });

        modelBuilder.Entity<ApplicantWorkExperience>(entity =>
        {
            entity.HasKey(e => e.WorkExperienceId).HasName("work_experience_pkey");

            entity.ToTable("applicant_work_experience");

            entity.Property(e => e.WorkExperienceId)
                .HasDefaultValueSql("nextval('work_experience_work_experience_id_seq'::regclass)")
                .HasColumnName("work_experience_id");
            entity.Property(e => e.ApplicantId).HasColumnName("applicant_id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .HasColumnName("company_name");
            entity.Property(e => e.EmploymentType)
                .HasMaxLength(50)
                .HasColumnName("employment_type");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Industry)
                .HasMaxLength(255)
                .HasColumnName("industry");
            entity.Property(e => e.JobTitle)
                .HasMaxLength(255)
                .HasColumnName("job_title");
            entity.Property(e => e.KeyResponsibilities).HasColumnName("key_responsibilities");
            entity.Property(e => e.LeadershipImpact).HasColumnName("leadership_impact");
            entity.Property(e => e.StartDate).HasColumnName("start_date");

            entity.HasOne(d => d.Applicant).WithMany(p => p.ApplicantWorkExperiences)
                .HasForeignKey(d => d.ApplicantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("work_experience_applicant_id_fkey");
        });

        modelBuilder.Entity<JobClickLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("JobClickLogs_pkey");

            entity.Property(e => e.ClickedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.UserId);
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LangId).HasName("language_pkey");

            entity.ToTable("language");

            entity.Property(e => e.LangId).HasColumnName("lang_id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_on");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Lang)
                .HasMaxLength(255)
                .HasColumnName("lang");
            entity.Property(e => e.Sequence).HasColumnName("sequence");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UpdatedOn)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_on");
        });

        modelBuilder.Entity<MstEventpricing>(entity =>
        {
            entity.HasKey(e => e.Eventpricingtypeid).HasName("mst_eventpricing_pkey");

            entity.ToTable("mst_eventpricing");

            entity.Property(e => e.Eventpricingtypeid).HasColumnName("eventpricingtypeid");
            entity.Property(e => e.Eventpricingtype)
                .HasMaxLength(255)
                .HasColumnName("eventpricingtype");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
        });

        modelBuilder.Entity<MstEventtype>(entity =>
        {
            entity.HasKey(e => e.Eventtypeid).HasName("mst_eventtype_pkey");

            entity.ToTable("mst_eventtype");

            entity.Property(e => e.Eventtypeid).HasColumnName("eventtypeid");
            entity.Property(e => e.Eventtype)
                .HasMaxLength(255)
                .HasColumnName("eventtype");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
        });

        modelBuilder.Entity<MstLookup>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("mst_lookup");

            entity.Property(e => e.Active)
                .HasDefaultValue(true)
                .HasColumnName("active");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.Hintdetails)
                .HasMaxLength(50)
                .HasColumnName("hintdetails");
            entity.Property(e => e.Languageid)
                .HasDefaultValue(1)
                .HasColumnName("languageid");
            entity.Property(e => e.Lookupcode).HasColumnName("lookupcode");
            entity.Property(e => e.Lookupflag).HasColumnName("lookupflag");
            entity.Property(e => e.Seqno).HasColumnName("seqno");
        });

        modelBuilder.Entity<MstMenu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("mst_menu_pkey");

            entity.ToTable("mst_menu");

            entity.Property(e => e.MenuId).HasColumnName("menu_id");
            entity.Property(e => e.Action)
                .HasMaxLength(255)
                .HasColumnName("action");
            entity.Property(e => e.Controller)
                .HasMaxLength(255)
                .HasColumnName("controller");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_on");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Menu)
                .HasMaxLength(255)
                .HasColumnName("menu");
            entity.Property(e => e.MenuParentId).HasColumnName("menu_parent_id");
            entity.Property(e => e.MenuSequence).HasColumnName("menu_sequence");
            entity.Property(e => e.MenuType).HasColumnName("menu_type");
            entity.Property(e => e.StyleClass)
                .HasMaxLength(255)
                .HasColumnName("style_class");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UpdatedOn)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_on");
        });

        modelBuilder.Entity<MstRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("mst_role_pkey");

            entity.ToTable("mst_role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_on");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.LangId).HasColumnName("lang_id");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasColumnName("role");
            entity.Property(e => e.RoleType).HasColumnName("role_type");
            entity.Property(e => e.Seq).HasColumnName("seq");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UpdatedOn)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_on");

            entity.HasOne(d => d.Lang).WithMany(p => p.MstRoles)
                .HasForeignKey(d => d.LangId)
                .HasConstraintName("fk_language");
        });

        modelBuilder.Entity<MstRsdocumenttype>(entity =>
        {
            entity.HasKey(e => e.Rsdocumenttypeid).HasName("mst_rsdocumenttype_pkey");

            entity.ToTable("mst_rsdocumenttype");

            entity.Property(e => e.Rsdocumenttypeid).HasColumnName("rsdocumenttypeid");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Rsdocumenttype)
                .HasMaxLength(255)
                .HasColumnName("rsdocumenttype");
        });

        modelBuilder.Entity<MstTheme>(entity =>
        {
            entity.HasKey(e => e.ThemeId).HasName("mst_theme_pkey");

            entity.ToTable("mst_theme");

            entity.Property(e => e.ThemeId).HasColumnName("theme_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Theme)
                .HasMaxLength(255)
                .HasColumnName("theme");
        });

        modelBuilder.Entity<MstUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("mst_user_pkey");

            entity.ToTable("mst_user");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.AuthToken)
                .HasMaxLength(255)
                .HasColumnName("auth_token");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_on");
            entity.Property(e => e.EmailId)
                .HasMaxLength(255)
                .HasColumnName("email_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(0)
                .HasColumnName("is_deleted");
            entity.Property(e => e.LastLogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login");
            entity.Property(e => e.Mobile)
                .HasMaxLength(15)
                .HasColumnName("mobile");
            entity.Property(e => e.NoofLogin).HasColumnName("noof_login");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Pincode).HasColumnName("pincode");
            entity.Property(e => e.PwdLinkExpTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("pwd_link_exp_time");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UpdatedOn)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_on");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .HasColumnName("user_name");
            entity.Property(e => e.Vcode)
                .HasMaxLength(255)
                .IsFixedLength();

            entity.HasOne(d => d.Role).WithMany(p => p.MstUsers)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("fk_role");
        });

        modelBuilder.Entity<NewsletterSubscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("newsletter_subscriptions_pkey");

            entity.ToTable("newsletter_subscriptions");

            entity.HasIndex(e => e.Email, "newsletter_subscriptions_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Subscribed)
                .HasDefaultValue(true)
                .HasColumnName("subscribed");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ReferralCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("referral_codes_pkey");

            entity.ToTable("referral_codes");

            entity.HasIndex(e => e.Code, "referral_codes_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApplicantId).HasColumnName("applicant_id");
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .HasColumnName("code");
            entity.Property(e => e.Count)
                .HasDefaultValue(0)
                .HasColumnName("count");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Applicant).WithMany(p => p.ReferralCodes)
                .HasForeignKey(d => d.ApplicantId)
                .HasConstraintName("fk_referral_applicant");
        });

        modelBuilder.Entity<ResourceClickLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ResourceClickLogs_pkey");

            entity.Property(e => e.ClickedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.UserId);
        });

        modelBuilder.Entity<RoleMenu>(entity =>
        {
            entity.HasKey(e => e.RoleMenuId).HasName("role_menu_pkey");

            entity.ToTable("role_menu");

            entity.Property(e => e.RoleMenuId).HasColumnName("role_menu_id");
            entity.Property(e => e.AddNew).HasColumnName("add_new");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_on");
            entity.Property(e => e.Display).HasColumnName("display");
            entity.Property(e => e.Edit).HasColumnName("edit");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.MenuId).HasColumnName("menu_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UpdatedOn)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_on");

            entity.HasOne(d => d.Menu).WithMany(p => p.RoleMenus)
                .HasForeignKey(d => d.MenuId)
                .HasConstraintName("fk_menu");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleMenus)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("fk_role");
        });

        modelBuilder.Entity<TblSocialLinkdin>(entity =>
        {
            entity.HasKey(e => e.ScLinkdinId).HasName("tblSocialLinkdin_pkey");

            entity.ToTable("tblSocialLinkdin");

            entity.Property(e => e.ScLinkdinId).HasDefaultValueSql("nextval('tblsociallinkdin_sclinkdinid_seq'::regclass)");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<Tblaboutu>(entity =>
        {
            entity.HasKey(e => e.Tblaboutusid).HasName("tblaboutus_pkey");

            entity.ToTable("tblaboutus");

            entity.Property(e => e.Tblaboutusid).HasColumnName("tblaboutusid");
            entity.Property(e => e.Bannerimglink).HasColumnName("bannerimglink");
            entity.Property(e => e.Bannermainheading).HasColumnName("bannermainheading");
            entity.Property(e => e.Bannersubheading).HasColumnName("bannersubheading");
            entity.Property(e => e.Secdesc).HasColumnName("secdesc");
            entity.Property(e => e.Sechdtxt).HasColumnName("sechdtxt");
            entity.Property(e => e.Thirddesc).HasColumnName("thirddesc");
            entity.Property(e => e.Thirdhdtxt).HasColumnName("thirdhdtxt");
            entity.Property(e => e.Thirdsubonedesc).HasColumnName("thirdsubonedesc");
            entity.Property(e => e.Thirdsubonehdtxt).HasColumnName("thirdsubonehdtxt");
            entity.Property(e => e.Thirdsubtwodesc).HasColumnName("thirdsubtwodesc");
            entity.Property(e => e.Thirdsubtwohdtxt).HasColumnName("thirdsubtwohdtxt");
        });

        modelBuilder.Entity<Tblcompany>(entity =>
        {
            entity.HasKey(e => e.Companyid).HasName("tblcompany_pkey");

            entity.ToTable("tblcompany");

            entity.Property(e => e.Companyid).HasColumnName("companyid");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Logo).HasColumnName("logo");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Overview).HasColumnName("overview");
            entity.Property(e => e.Sociallink).HasColumnName("sociallink");
            entity.Property(e => e.Website).HasColumnName("website");
        });

        modelBuilder.Entity<Tblevent>(entity =>
        {
            entity.HasKey(e => e.Eventid).HasName("tblevent_pkey");

            entity.ToTable("tblevent");

            entity.Property(e => e.Eventid).HasColumnName("eventid");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createdon)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdon");
            entity.Property(e => e.Descriptionofevent).HasColumnName("descriptionofevent");
            entity.Property(e => e.Enddateofevent).HasColumnName("enddateofevent");
            entity.Property(e => e.EventPricingTypeId).HasColumnName("EventPricingTypeID");
            entity.Property(e => e.EventTypeId).HasColumnName("EventTypeID");
            entity.Property(e => e.Evidenceid).HasColumnName("evidenceid");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Isvalidate).HasColumnName("isvalidate");
            entity.Property(e => e.Numberofattended).HasColumnName("numberofattended");
            entity.Property(e => e.Startdateofevent).HasColumnName("startdateofevent");
            entity.Property(e => e.Themeid).HasColumnName("themeid");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");
            entity.Property(e => e.Updatedon)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedon");

            entity.HasOne(d => d.EventPricingType).WithMany(p => p.Tblevents)
                .HasForeignKey(d => d.EventPricingTypeId)
                .HasConstraintName("fk_EventPricing");

            entity.HasOne(d => d.EventType).WithMany(p => p.Tblevents)
                .HasForeignKey(d => d.EventTypeId)
                .HasConstraintName("fk_Eventtype");

            entity.HasOne(d => d.Event).WithOne(p => p.InverseEvent)
                .HasForeignKey<Tblevent>(d => d.Eventid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_evidence");

            entity.HasOne(d => d.Theme).WithMany(p => p.Tblevents)
                .HasForeignKey(d => d.Themeid)
                .HasConstraintName("fk_theme");
        });

        modelBuilder.Entity<Tbleventbenefit>(entity =>
        {
            entity.HasKey(e => e.Eventbenefitid).HasName("tbleventbenefit_pkey");

            entity.ToTable("tbleventbenefit");

            entity.Property(e => e.Eventbenefitid)
                .HasDefaultValueSql("nextval('tbleventbenefit_eventbenefitid_seq1'::regclass)")
                .HasColumnName("eventbenefitid");
            entity.Property(e => e.Eventbenefit).HasColumnName("eventbenefit");
            entity.Property(e => e.Eventid).HasColumnName("eventid");

            entity.HasOne(d => d.Event).WithMany(p => p.Tbleventbenefits)
                .HasForeignKey(d => d.Eventid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_eventbenefit");
        });

        modelBuilder.Entity<Tbleventparticipant>(entity =>
        {
            entity.HasKey(e => e.Participantid).HasName("tbleventparticipant_pkey");

            entity.ToTable("tbleventparticipant");

            entity.Property(e => e.Participantid).HasColumnName("participantid");
            entity.Property(e => e.Linkedin).HasColumnName("linkedin");
            entity.Property(e => e.Mobileno).HasColumnName("mobileno");

            entity.HasOne(d => d.Event).WithMany(p => p.Tbleventparticipants)
                .HasForeignKey(d => d.Eventid)
                .HasConstraintName("fkeventid");
        });

        modelBuilder.Entity<Tblevidence>(entity =>
        {
            entity.HasKey(e => e.Evidenceid).HasName("tblevidence_pkey");

            entity.ToTable("tblevidence");

            entity.Property(e => e.Evidenceid).HasColumnName("evidenceid");
            entity.Property(e => e.Eventid).HasColumnName("eventid");
            entity.Property(e => e.Evidencelink).HasColumnName("evidencelink");
            entity.Property(e => e.Evidencepath).HasColumnName("evidencepath");

            entity.HasOne(d => d.Event).WithMany(p => p.Tblevidences)
                .HasForeignKey(d => d.Eventid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_event");
        });

        modelBuilder.Entity<Tblinfographic>(entity =>
        {
            entity.HasKey(e => e.Infogid).HasName("tblinfographic_pkey");

            entity.ToTable("tblinfographic");

            entity.Property(e => e.Infogid).HasColumnName("infogid");
            entity.Property(e => e.Createdon).HasColumnName("createdon");
            entity.Property(e => e.Infodesc).HasColumnName("infodesc");
            entity.Property(e => e.Infoheading).HasColumnName("infoheading");
            entity.Property(e => e.Infoimage).HasColumnName("infoimage");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
        });

        modelBuilder.Entity<Tbljob>(entity =>
        {
            entity.HasKey(e => e.Jobid).HasName("joblistings_pkey");

            entity.ToTable("tbljob");

            entity.Property(e => e.Jobid)
                .HasDefaultValueSql("nextval('joblistings_jobid_seq'::regclass)")
                .HasColumnName("jobid");
            entity.Property(e => e.Applicationdeadline).HasColumnName("applicationdeadline");
            entity.Property(e => e.City).HasColumnName("city");
            entity.Property(e => e.Companyid).HasColumnName("companyid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Dateposted).HasColumnName("dateposted");
            entity.Property(e => e.Deiandwomenfriendlypolicies).HasColumnName("deiandwomenfriendlypolicies");
            entity.Property(e => e.Education).HasColumnName("education");
            entity.Property(e => e.Employeetype).HasColumnName("employeetype");
            entity.Property(e => e.Experience).HasColumnName("experience");
            entity.Property(e => e.Flexibleworkoption).HasColumnName("flexibleworkoption");
            entity.Property(e => e.Functionalcategory).HasColumnName("functionalcategory");
            entity.Property(e => e.Healthcareandwellness).HasColumnName("healthcareandwellness");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Jobpostinglink).HasColumnName("jobpostinglink");
            entity.Property(e => e.Jobtitle)
                .HasMaxLength(255)
                .HasColumnName("jobtitle");
            entity.Property(e => e.Keyresponsibility).HasColumnName("keyresponsibility");
            entity.Property(e => e.Learninganddeveloment).HasColumnName("learninganddeveloment");
            entity.Property(e => e.Leavepolicies).HasColumnName("leavepolicies");
            entity.Property(e => e.Maxsalary).HasColumnName("maxsalary");
            entity.Property(e => e.Minsalary).HasColumnName("minsalary");
            entity.Property(e => e.Roleoverview).HasColumnName("roleoverview");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Workmode).HasColumnName("workmode");
            entity.Property(e => e.Yearexperience).HasColumnName("yearexperience");

            entity.HasOne(d => d.Company).WithMany(p => p.Tbljobs)
                .HasForeignKey(d => d.Companyid)
                .HasConstraintName("fk_company");
        });

        modelBuilder.Entity<Tbllandingpage>(entity =>
        {
            entity.HasKey(e => e.Landingpageid).HasName("tbllandingpage_pkey");

            entity.ToTable("tbllandingpage");

            entity.Property(e => e.Landingpageid).HasColumnName("landingpageid");
            entity.Property(e => e.Bannerbtnlftlink).HasColumnName("bannerbtnlftlink");
            entity.Property(e => e.Bannerbtnlfttext).HasColumnName("bannerbtnlfttext");
            entity.Property(e => e.Bannerbtnrhslink).HasColumnName("bannerbtnrhslink");
            entity.Property(e => e.Bannerbtnrhstext).HasColumnName("bannerbtnrhstext");
            entity.Property(e => e.Bannerimglink).HasColumnName("bannerimglink");
            entity.Property(e => e.Bannermainheading).HasColumnName("bannermainheading");
            entity.Property(e => e.Bannersubheading).HasColumnName("bannersubheading");
            entity.Property(e => e.Sec3mnbtnlnk).HasColumnName("sec3mnbtnlnk");
            entity.Property(e => e.Sec3mnbtntext).HasColumnName("sec3mnbtntext");
            entity.Property(e => e.Sec3mnhd).HasColumnName("sec3mnhd");
            entity.Property(e => e.Sec4mnbg).HasColumnName("sec4mnbg");
            entity.Property(e => e.Sec4mnbtntxt).HasColumnName("sec4mnbtntxt");
            entity.Property(e => e.Sec4mndsc).HasColumnName("sec4mndsc");
            entity.Property(e => e.Sec4mnhd).HasColumnName("sec4mnhd");
            entity.Property(e => e.Sec4mnlnk).HasColumnName("sec4mnlnk");
            entity.Property(e => e.Sec5mnbtnlnk).HasColumnName("sec5mnbtnlnk");
            entity.Property(e => e.Sec5mnbtntext).HasColumnName("sec5mnbtntext");
            entity.Property(e => e.Sec5mnhd).HasColumnName("sec5mnhd");
            entity.Property(e => e.Sec6bg).HasColumnName("sec6bg");
            entity.Property(e => e.Sec6btnlnk).HasColumnName("sec6btnlnk");
            entity.Property(e => e.Sec6btntxt).HasColumnName("sec6btntxt");
            entity.Property(e => e.Sec6dsc).HasColumnName("sec6dsc");
            entity.Property(e => e.Sec6hd).HasColumnName("sec6hd");
            entity.Property(e => e.Sec7btnlnk).HasColumnName("sec7btnlnk");
            entity.Property(e => e.Sec7btntxt).HasColumnName("sec7btntxt");
            entity.Property(e => e.Sec7hd).HasColumnName("sec7hd");
            entity.Property(e => e.Sec8bg).HasColumnName("sec8bg");
            entity.Property(e => e.Sec8btnlnk).HasColumnName("sec8btnlnk");
            entity.Property(e => e.Sec8btntxt).HasColumnName("sec8btntxt");
            entity.Property(e => e.Sec8dsc).HasColumnName("sec8dsc");
            entity.Property(e => e.Sec8hd).HasColumnName("sec8hd");
            entity.Property(e => e.Secdesc).HasColumnName("secdesc");
            entity.Property(e => e.Sechdtxt).HasColumnName("sechdtxt");
            entity.Property(e => e.Secspn).HasColumnName("secspn");
            entity.Property(e => e.Sectile1anclnk).HasColumnName("sectile1anclnk");
            entity.Property(e => e.Sectile1anctxt).HasColumnName("sectile1anctxt");
            entity.Property(e => e.Sectile1hd).HasColumnName("sectile1hd");
            entity.Property(e => e.Sectile1img).HasColumnName("sectile1img");
            entity.Property(e => e.Sectile2anclnk).HasColumnName("sectile2anclnk");
            entity.Property(e => e.Sectile2anctxt).HasColumnName("sectile2anctxt");
            entity.Property(e => e.Sectile2hd).HasColumnName("sectile2hd");
            entity.Property(e => e.Sectile2img).HasColumnName("sectile2img");
            entity.Property(e => e.Sectile3anclnk).HasColumnName("sectile3anclnk");
            entity.Property(e => e.Sectile3anctxt).HasColumnName("sectile3anctxt");
            entity.Property(e => e.Sectile3hd).HasColumnName("sectile3hd");
            entity.Property(e => e.Sectile3img).HasColumnName("sectile3img");
        });

        modelBuilder.Entity<Tblourteam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tblourteam_pk");

            entity.ToTable("tblourteam");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Designation).HasColumnName("designation");
            entity.Property(e => e.Linkedin).HasColumnName("linkedin");
            entity.Property(e => e.Organization).HasColumnName("organization");
            entity.Property(e => e.Photo).HasColumnName("photo");
        });

        modelBuilder.Entity<Tblresource>(entity =>
        {
            entity.HasKey(e => e.Resourceid).HasName("tblresource_pkey");

            entity.ToTable("tblresource");

            entity.Property(e => e.Resourceid).HasColumnName("resourceid");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createdon)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdon");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Ispublic).HasColumnName("ispublic");
            entity.Property(e => e.Isrelatedrs).HasColumnName("isrelatedrs");
            entity.Property(e => e.Isverified).HasColumnName("isverified");
            entity.Property(e => e.Rsage).HasColumnName("rsage");
            entity.Property(e => e.Rsdocument).HasColumnName("rsdocument");
            entity.Property(e => e.Rsdocumenttypeid).HasColumnName("rsdocumenttypeid");
            entity.Property(e => e.Rshead).HasColumnName("rshead");
            entity.Property(e => e.Rsimage).HasColumnName("rsimage");
            entity.Property(e => e.Rsshortdescription).HasColumnName("rsshortdescription");
            entity.Property(e => e.Rsversion).HasColumnName("rsversion");
            entity.Property(e => e.RsvideoLink).HasColumnName("RSVideoLink");
            entity.Property(e => e.ThemeId).HasColumnName("theme_id");
            entity.Property(e => e.Topic).HasColumnName("topic");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");
            entity.Property(e => e.Updatedon)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedon");

            entity.HasOne(d => d.Rsdocumenttype).WithMany(p => p.Tblresources)
                .HasForeignKey(d => d.Rsdocumenttypeid)
                .HasConstraintName("fk_rsdocumenttype_id");

            entity.HasOne(d => d.Theme).WithMany(p => p.Tblresources)
                .HasForeignKey(d => d.ThemeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_theme_id");
        });

        modelBuilder.Entity<Tblsuccesstest>(entity =>
        {
            entity.HasKey(e => e.Sstid).HasName("tblsuccesstest_pkey");

            entity.ToTable("tblsuccesstest");

            entity.Property(e => e.Sstid).HasColumnName("sstid");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Linkedin).HasColumnName("linkedin");
            entity.Property(e => e.Photo).HasColumnName("photo");
        });
        modelBuilder.HasSequence("certifications_certification_id_seq");
        modelBuilder.HasSequence("education_education_id_seq");
        modelBuilder.HasSequence("languages_language_id_seq");
        modelBuilder.HasSequence("mst_eventpricing_eventpricingtypeid_seq");
        modelBuilder.HasSequence("mst_eventtype_eventtypeid_seq");
        modelBuilder.HasSequence("mst_rsdocumenttype_rsdocumenttypeid_seq");
        modelBuilder.HasSequence("tbleventbenefit_eventbenefitid_seq");
        modelBuilder.HasSequence("tblresource_resourceid_seq");
        modelBuilder.HasSequence("tblsociallinkdin_sclinkdinid_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
