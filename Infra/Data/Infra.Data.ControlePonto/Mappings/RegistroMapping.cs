using Domain.ControlePonto.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.ControlePonto.Mappings;

public class RegistroMapping : IEntityTypeConfiguration<Registro>
{
    public void Configure(EntityTypeBuilder<Registro> builder)
    {

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.DiaHora)
            .HasColumnName("dia_hora");
        
        builder.HasIndex(x => x.DiaHora)
            .IsDescending(false);

        builder.ToTable("registro_ponto", schema:"controle_ponto");
    }
}