using Lowtel.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EF.AspNetCore.Models
{
    public class LotelContext : DbContext
    {
        public LotelContext(DbContextOptions<LotelContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ----------- START define primary keys of the entityes -----------

            modelBuilder.Entity<Hotel>()
                .HasKey(hotel => new { hotel.Id });

            modelBuilder.Entity<Room>()
                .HasKey(room => new { room.Id, room.HotelId });

            modelBuilder.Entity<RoomType>()
                .HasKey(roomType => new { roomType.Id });

            modelBuilder.Entity<Client>()
                .HasKey(client => new { client.Id });

            modelBuilder.Entity<Reservation>()
                .HasKey(reservation => new { reservation.ClientId, reservation.HotelId, reservation.RoomId });

            // ----------- END define primary keys of the entityes -----------

            modelBuilder.Entity<Room>()
                .HasOne(room => room.Hotel)
                .WithMany()
                .HasForeignKey(room => room.HotelId);

            modelBuilder.Entity<Room>()
                .HasOne(room => room.RoomType)
                .WithMany()
                .HasForeignKey(room => room.RoomTypeId);

            modelBuilder.Entity<Reservation>()
                .HasOne(reservation => reservation.Hotel)
                .WithMany()
                .HasForeignKey(reservation => reservation.HotelId);

            modelBuilder.Entity<Reservation>()
                .HasOne(reservation => reservation.Room)
                .WithMany()
                .HasForeignKey(reservation => new { reservation.RoomId, reservation.HotelId });

            modelBuilder.Entity<Reservation>()
                .HasOne(reservation => reservation.Client)
                .WithMany()
                .HasForeignKey(reservation => reservation.ClientId);
        }

        public DbSet<Hotel> Hotel { get; set; }
        public DbSet<RoomType> RoomType { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<Client> Client { get; set; }        
        public DbSet<Reservation> Reservation { get; set; }
    }
}