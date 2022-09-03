﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NotesApp.Models;

#nullable disable

namespace NotesApp.Migrations
{
    [DbContext(typeof(NotesContext))]
    [Migration("20220903031723_Config_MinimizeOnClose")]
    partial class Config_MinimizeOnClose
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.8");

            modelBuilder.Entity("NotesApp.Models.Configuration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AlwaysOnTop")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CurrentNotePosition")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("MinimizeOnClose")
                        .HasColumnType("INTEGER");

                    b.Property<double>("WindowHeight")
                        .HasColumnType("REAL");

                    b.Property<double>("WindowLeft")
                        .HasColumnType("REAL");

                    b.Property<double>("WindowTop")
                        .HasColumnType("REAL");

                    b.Property<double>("WindowWidth")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Config");
                });

            modelBuilder.Entity("NotesApp.Models.Note", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Notes");
                });
#pragma warning restore 612, 618
        }
    }
}
