﻿// <auto-generated />
using System;
using HNG.StageTwoTask.BackendC_.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HNG.StageTwoTask.BackendC_.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240709000950_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("HNG.StageTwoTask.BackendC_.Models.Access.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("HNG.StageTwoTask.BackendC_.Models.Organisation.OrganisationUser", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("OrgId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "OrgId");

                    b.HasIndex("OrgId");

                    b.ToTable("OrganisationUsers");
                });

            modelBuilder.Entity("HNG.StageTwoTask.BackendC_.Models.Organisation.Organisations", b =>
                {
                    b.Property<string>("OrgId")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("OrgId");

                    b.ToTable("Organisations");
                });

            modelBuilder.Entity("HNG.StageTwoTask.BackendC_.Models.Organisation.OrganisationUser", b =>
                {
                    b.HasOne("HNG.StageTwoTask.BackendC_.Models.Organisation.Organisations", "Organisation")
                        .WithMany("OrganisationUser")
                        .HasForeignKey("OrgId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HNG.StageTwoTask.BackendC_.Models.Access.User", "User")
                        .WithMany("OrganisationUser")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organisation");

                    b.Navigation("User");
                });

            modelBuilder.Entity("HNG.StageTwoTask.BackendC_.Models.Access.User", b =>
                {
                    b.Navigation("OrganisationUser");
                });

            modelBuilder.Entity("HNG.StageTwoTask.BackendC_.Models.Organisation.Organisations", b =>
                {
                    b.Navigation("OrganisationUser");
                });
#pragma warning restore 612, 618
        }
    }
}
