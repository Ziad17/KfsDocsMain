﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication2.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DMS_dbEntities8 : DbContext
    {
        public DMS_dbEntities8()
            : base("name=DMS_dbEntities8")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Bookmark> Bookmarks { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeCredential> EmployeeCredentials { get; set; }
        public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<FileActionLog> FileActionLogs { get; set; }
        public virtual DbSet<FileContent> FileContents { get; set; }
        public virtual DbSet<FileLevel> FileLevels { get; set; }
        public virtual DbSet<FileMention> FileMentions { get; set; }
        public virtual DbSet<FilePermission> FilePermissions { get; set; }
        public virtual DbSet<FileType> FileTypes { get; set; }
        public virtual DbSet<FileVersion> FileVersions { get; set; }
        public virtual DbSet<Institution> Institutions { get; set; }
        public virtual DbSet<InstitutionActionLog> InstitutionActionLogs { get; set; }
        public virtual DbSet<InstitutionPermission> InstitutionPermissions { get; set; }
        public virtual DbSet<InstitutionType> InstitutionTypes { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<PersonActionLog> PersonActionLogs { get; set; }
        public virtual DbSet<PersonPermission> PersonPermissions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoleInstitutionPermission> RoleInstitutionPermissions { get; set; }
        public virtual DbSet<RolePersonPermission> RolePersonPermissions { get; set; }
        public virtual DbSet<FilesScope> FilesScopes { get; set; }
    }
}
