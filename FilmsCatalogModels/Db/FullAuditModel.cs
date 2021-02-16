using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FilmsCatalogModels.Interfaces;

namespace FilmsCatalogModels.Db
{
    public class FullAuditModel : IIdentityModel, IAuditedModel, IActivatableModel, ISoftDeletable
    {
        public int Id { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? LastModifiedUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}
