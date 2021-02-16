using System;

namespace FilmsCatalogModels.Interfaces
{
    public interface IActivatableModel
    {
        public bool IsActive { get; set; }
    }
}
