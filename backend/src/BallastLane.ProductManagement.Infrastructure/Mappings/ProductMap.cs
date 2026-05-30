using FluentNHibernate.Mapping;
using BallastLane.ProductManagement.Domain.Entities;

namespace BallastLane.ProductManagement.Infrastructure.Mappings
{
    public class ProductMap : ClassMap<Product>
    {
        public ProductMap()
        {
            Table("Products");
            Not.LazyLoad();

            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            Map(x => x.Name).Not.Nullable().Length(200).Column("Name");
            Map(x => x.Description).Not.Nullable().Length(1000).Column("Description");
            Map(x => x.Price).Not.Nullable().Column("Price");
            Map(x => x.Stock).Not.Nullable().Column("Stock");
            Map(x => x.CreatedAt).Not.Nullable().Column("CreatedAt");
            Map(x => x.UpdatedAt).Nullable().Column("UpdatedAt");
        }
    }
}
