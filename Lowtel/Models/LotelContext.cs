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
            modelBuilder.Entity<Reservation>()
                .HasKey(reservation => new { reservation.ClientId, reservation.HotelId, reservation.RoomId });

            modelBuilder.Entity<Room>()
                .HasKey(room => new { room.Id, room.HotelId });
        }

        public DbSet<Hotel> Hotel { get; set; }
        public DbSet<RoomType> RoomType { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<Client> Client { get; set; }        
        public DbSet<Lowtel.Models.Reservation> Reservation { get; set; }
    }
}