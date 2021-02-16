using System;

namespace FilmsCatalogModels.Interfaces
{
    public interface IAuditedModel
    {
        public Guid? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? LastModifiedUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
