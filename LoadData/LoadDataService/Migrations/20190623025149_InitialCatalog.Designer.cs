﻿// <auto-generated />
using System;
using LoadDataService.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LoadDataService.Migrations
{
    [DbContext(typeof(QueryContext))]
    [Migration("20190623025149_InitialCatalog")]
    partial class InitialCatalog
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("LoadDataService.Models.Entities.Session", b =>
                {
                    b.Property<int>("SessionId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClientId");

                    b.Property<int>("CurrentPage");

                    b.Property<DateTime>("LastModification");

                    b.Property<string>("PreviousPages");

                    b.Property<int>("RowSize");

                    b.Property<string>("StringQueryPMI");

                    b.HasKey("SessionId");

                    b.ToTable("Session");
                });
#pragma warning restore 612, 618
        }
    }
}
