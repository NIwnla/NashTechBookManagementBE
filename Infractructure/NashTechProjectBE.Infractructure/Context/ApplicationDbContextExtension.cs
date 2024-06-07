using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NashTechProjectBE.Infractructure.Context;

public static class ApplicationDbContextExtension
{
    public static EntityTypeBuilder AddShadowProperties(this EntityTypeBuilder entity){
        entity.Property<DateTime>("CreatedTime");
        entity.Property<DateTime>("UpdatedTime");
        entity.Property<Guid?>("CreatedBy").IsRequired(false);
        entity.Property<Guid?>("UpdatedBy").IsRequired(false);
        return entity;
    }
}
