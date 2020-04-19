﻿// <auto-generated />
using System;
using BurstChat.Shared.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BurstChat.Api.Migrations
{
    [DbContext(typeof(BurstChatContext))]
    partial class BurstChatContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("BurstChat.Shared.Schema.Alpha.AlphaInvitation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<Guid>("Code")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateExpired")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("AlphaInvitations");
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Chat.Link", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long?>("MessageId")
                        .HasColumnType("bigint");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.ToTable("Links");
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Chat.Message", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("ChannelId")
                        .HasColumnType("integer");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("DatePosted")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long?>("DirectMessagingId")
                        .HasColumnType("bigint");

                    b.Property<bool>("Edited")
                        .HasColumnType("boolean");

                    b.Property<long?>("PrivateGroupId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("DirectMessagingId");

                    b.HasIndex("PrivateGroupId");

                    b.HasIndex("UserId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Servers.Channel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("ServerId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Servers.Invitation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("Accepted")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateUpdated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Declined")
                        .HasColumnType("boolean");

                    b.Property<int>("ServerId")
                        .HasColumnType("integer");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.HasIndex("UserId");

                    b.ToTable("Invitations");
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Servers.Server", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Servers.Subscription", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ServerId")
                        .HasColumnType("integer");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.HasIndex("UserId");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Users.DirectMessaging", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("FirstParticipantUserId")
                        .HasColumnType("bigint");

                    b.Property<long>("SecondParticipantUserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("DirectMessaging");
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Users.OneTimePassword", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("OTP")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("OneTimePassword");
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Users.PrivateGroup", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PrivateGroups");
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Users.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("PrivateGroupId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("PrivateGroupId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Chat.Link", b =>
                {
                    b.HasOne("BurstChat.Shared.Schema.Chat.Message", null)
                        .WithMany("Links")
                        .HasForeignKey("MessageId");
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Chat.Message", b =>
                {
                    b.HasOne("BurstChat.Shared.Schema.Servers.Channel", null)
                        .WithMany("Messages")
                        .HasForeignKey("ChannelId");

                    b.HasOne("BurstChat.Shared.Schema.Users.DirectMessaging", null)
                        .WithMany("Messages")
                        .HasForeignKey("DirectMessagingId");

                    b.HasOne("BurstChat.Shared.Schema.Users.PrivateGroup", null)
                        .WithMany("Messages")
                        .HasForeignKey("PrivateGroupId");

                    b.HasOne("BurstChat.Shared.Schema.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Servers.Channel", b =>
                {
                    b.HasOne("BurstChat.Shared.Schema.Servers.Server", null)
                        .WithMany("Channels")
                        .HasForeignKey("ServerId");
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Servers.Invitation", b =>
                {
                    b.HasOne("BurstChat.Shared.Schema.Servers.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BurstChat.Shared.Schema.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Servers.Subscription", b =>
                {
                    b.HasOne("BurstChat.Shared.Schema.Servers.Server", null)
                        .WithMany("Subscriptions")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BurstChat.Shared.Schema.Users.User", null)
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Users.OneTimePassword", b =>
                {
                    b.HasOne("BurstChat.Shared.Schema.Users.User", null)
                        .WithMany("OneTimePasswords")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("BurstChat.Shared.Schema.Users.User", b =>
                {
                    b.HasOne("BurstChat.Shared.Schema.Users.PrivateGroup", null)
                        .WithMany("Users")
                        .HasForeignKey("PrivateGroupId");
                });
#pragma warning restore 612, 618
        }
    }
}
