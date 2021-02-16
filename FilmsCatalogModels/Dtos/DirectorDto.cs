using System;

namespace FilmsCatalogModels.Dtos
{
    public class DirectorDto
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? LastModifiedUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}