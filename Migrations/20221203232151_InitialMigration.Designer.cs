﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SubcriptionManagement.Models;

#nullable disable

namespace SubcriptionManagement.Migrations
{
    [DbContext(typeof(SubcriptionDbContext))]
    [Migration("20221203232151_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SubcriptionManagement.Models.Subscriber", b =>
                {
                    b.Property<Guid>("SubscriberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Age")
                        .HasColumnType("integer");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Gender")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Price")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("SubscriptionType")
                        .HasColumnType("integer");

                    b.HasKey("SubscriberId");

                    b.ToTable("Subscribers");
                });

            modelBuilder.Entity("SubcriptionManagement.Models.SubscriberChoice", b =>
                {
                    b.Property<Guid>("SubscriberChoiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Choice")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ChoiceCount")
                        .HasColumnType("integer");

                    b.Property<Guid>("SubscriberId")
                        .HasColumnType("uuid");

                    b.HasKey("SubscriberChoiceId");

                    b.HasIndex("SubscriberId");

                    b.ToTable("SubscriberChoices");
                });

            modelBuilder.Entity("SubcriptionManagement.Models.SubscriberChoice", b =>
                {
                    b.HasOne("SubcriptionManagement.Models.Subscriber", null)
                        .WithMany("SubscriberChoices")
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SubcriptionManagement.Models.Subscriber", b =>
                {
                    b.Navigation("SubscriberChoices");
                });
#pragma warning restore 612, 618
        }
    }
}
