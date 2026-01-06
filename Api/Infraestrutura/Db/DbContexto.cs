using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Dominio.Entidades;
using Pomelo.EntityFrameworkCore.MySql;

namespace Infraestrutura.Db;

public class DbContexto : DbContext
{
    private readonly IConfiguration _configurationAppSettings;

    public DbContexto(IConfiguration configurationAppSettings)
    {
        _configurationAppSettings = configurationAppSettings;
    }
    public DbSet<Administrador> Administradores { get; set; } = default!;
    
    public DbSet<Veiculo> Veiculos { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder){

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Veiculo).Assembly);

        modelBuilder.Entity("Dominio.Entidades.Veiculo", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));
            b.Property<string>("Nome").IsRequired().HasMaxLength(255).HasColumnType("varchar(255)");
            b.Property<string>("Marca").IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            b.Property<int>("Ano").IsRequired().HasColumnType("int");
            b.HasKey("Id");
            b.ToTable("Veiculos");
        });

        modelBuilder.Entity<Administrador>().HasData(
            new Administrador{
                Id = 1,
                Email = "admin@car.com",
                Senha = "12345",
                Perfil = "adm"
            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured){
            
        var connectionString = _configurationAppSettings.GetConnectionString("mysql")?.ToString();

        if(!string.IsNullOrEmpty(connectionString)){
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        }
     }
    }

}

