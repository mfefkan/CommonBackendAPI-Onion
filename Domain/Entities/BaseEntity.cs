using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; } // Her tablonun birincil anahtarı

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma zamanı

        public DateTime? UpdatedAt { get; set; } // Güncellenme zamanı

        public bool IsActive { get; set; } = true; // Aktif/Pasif durumu
    }
}
