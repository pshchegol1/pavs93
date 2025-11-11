using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VC3EYE.Entities;

namespace VC3EYE.Data;

public partial class Vc3eyeContext : DbContext
{
    public Vc3eyeContext()
    {
    }

    public Vc3eyeContext(DbContextOptions<Vc3eyeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ErrorLog> ErrorLogs { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceDownHistory> ServiceDownHistories { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_CI_AS");

        modelBuilder.Entity<ErrorLog>(entity =>
        {
            entity.HasKey(e => e.ErrorLogId).HasName("PK_ErrorLogs_ErrorLogID");

            entity.Property(e => e.ErrorLogId).HasColumnName("ErrorLogID");
            entity.Property(e => e.DateAdded).HasColumnType("datetime");
            entity.Property(e => e.ErrorName).HasMaxLength(150);
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK_Logs_LogID");

            entity.HasIndex(e => e.UserId, "IX_Log_UserID");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.DateAdded).HasColumnType("datetime");
            entity.Property(e => e.LogDescription).HasMaxLength(500);
            entity.Property(e => e.LogName).HasMaxLength(150);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Logs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Logs_UserID");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK_Roles_RoleID");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Description).HasMaxLength(100);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK_Services_ServiceID");

            entity.HasIndex(e => e.UserId, "IX_UserID");

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.DateAdded).HasColumnType("datetime");
            entity.Property(e => e.GeneralInformation).HasMaxLength(200);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.IsIcmpRunning)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.LastDateModified).HasColumnType("datetime");
            entity.Property(e => e.LastTimeChecked).HasColumnType("datetime");
            entity.Property(e => e.Location).HasMaxLength(50);
            entity.Property(e => e.LookupTerm).HasMaxLength(50);
            entity.Property(e => e.NotificationMessage).HasMaxLength(100);
            entity.Property(e => e.NotifyByMsteams)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasColumnName("NotifyByMSTeams");
            entity.Property(e => e.NumOfBackup).HasDefaultValueSql("((0))");
            entity.Property(e => e.NumOfDownTimes).HasDefaultValueSql("((0))");
            entity.Property(e => e.RssfeedLink)
                .HasMaxLength(100)
                .HasColumnName("RSSFeedLink");
            entity.Property(e => e.ServiceName).HasMaxLength(100);
            entity.Property(e => e.Url)
                .HasMaxLength(100)
                .HasColumnName("URL");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Services)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Services_UserID");
        });

        modelBuilder.Entity<ServiceDownHistory>(entity =>
        {
            entity.HasKey(e => e.ServiceDownHistorryId).HasName("PK_ServiceDownHistory_ServiceDownHistorryID");

            entity.ToTable("ServiceDownHistory");

            entity.HasIndex(e => e.ServiceId, "IX_ServiceID");

            entity.Property(e => e.ServiceDownHistorryId).HasColumnName("ServiceDownHistorryID");
            entity.Property(e => e.BackUpDateTime).HasColumnType("datetime");
            entity.Property(e => e.DateAdded).HasColumnType("datetime");
            entity.Property(e => e.Htmlresponse)
                .HasMaxLength(300)
                .HasColumnName("HTMLResponse");
            entity.Property(e => e.IsRssfeedFailed).HasColumnName("IsRSSFeedFailed");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceDownHistories)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServiceDownHistory_ServiceID");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.SettingId).HasName("PK_Settings_SettingID");

            entity.Property(e => e.SettingId).HasColumnName("SettingID");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MtClientId)
                .HasMaxLength(100)
                .HasColumnName("MT_ClientID");
            entity.Property(e => e.MtSecretKey)
                .HasMaxLength(500)
                .HasColumnName("MT_Secret_Key");
            entity.Property(e => e.SmtpPassword)
                .HasMaxLength(100)
                .HasColumnName("SMTP_Password");
            entity.Property(e => e.SmtpPort).HasColumnName("SMTP_Port");
            entity.Property(e => e.SmtpUserName)
                .HasMaxLength(100)
                .HasColumnName("SMTP_UserName");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_Users_UserID");

            entity.HasIndex(e => e.RoleId, "IX_RoleID");

            entity.HasIndex(e => e.SettingId, "IX_SettingID");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(60);
            entity.Property(e => e.City).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(40);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PostalCode).HasMaxLength(10);
            entity.Property(e => e.ProvinceState)
                .HasMaxLength(30)
                .HasColumnName("Province_State");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.SettingId).HasColumnName("SettingID");
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_RoleID");

            entity.HasOne(d => d.Setting).WithMany(p => p.Users)
                .HasForeignKey(d => d.SettingId)
                .HasConstraintName("FK_Users_SettingID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
