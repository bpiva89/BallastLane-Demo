using FluentNHibernate.Mapping;
using BallastLane.ProductManagement.Domain.Entities;

namespace BallastLane.ProductManagement.Infrastructure.Mappings
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("Users");
            Not.LazyLoad();

            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            Map(x => x.Username).Not.Nullable().Length(100).Column("Username");
            Map(x => x.PasswordHash).Not.Nullable().Length(500).Column("PasswordHash");
            Map(x => x.Email).Not.Nullable().Length(200).Column("Email");
            Map(x => x.CreatedAt).Not.Nullable().Column("CreatedAt");
        }
    }
}
