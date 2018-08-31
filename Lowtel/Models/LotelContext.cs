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
        }

        public DbSet<Hotel> Hotel { get; set; }
        public DbSet<RoomType> RoomType { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<Client> Client { get; set; }        
        public DbSet<Lowtel.Models.Reservation> Reservation { get; set; }
    }
}