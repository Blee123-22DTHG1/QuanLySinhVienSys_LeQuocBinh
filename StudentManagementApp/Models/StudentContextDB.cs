using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace StudentManagementApp.Models
{
    public partial class StudentContextDB : DbContext
    {
        public StudentContextDB()
            : base("name=StudentContextDB")
        {
        }

        public virtual DbSet<Faculty> Faculties { get; set; }
        public virtual DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Faculty>()
                .HasMany(e => e.Students)
                .WithOptional(e => e.Faculty)
                .HasForeignKey(e => e.FacultyID);

            modelBuilder.Entity<Faculty>()
                .HasMany(e => e.Students1)
                .WithOptional(e => e.Faculty1)
                .HasForeignKey(e => e.FacultyID);
        }
    }
}
