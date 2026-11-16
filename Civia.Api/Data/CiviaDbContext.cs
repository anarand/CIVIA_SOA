using Civia.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Civia.Api.Data;

public class CiviaDbContext(DbContextOptions<CiviaDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<PreferenciaAcessibilidade> Preferencias => Set<PreferenciaAcessibilidade>();
    public DbSet<LocalRotina> LocaisRotina => Set<LocalRotina>();
    public DbSet<ContatoConfianca> ContatosConfianca => Set<ContatoConfianca>();
    public DbSet<SolicitacaoAjuda> Solicitacoes => Set<SolicitacaoAjuda>();
    public DbSet<AlertaUrbano> Alertas => Set<AlertaUrbano>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Todas as datas são gravadas em UTC; ao ler, o EF marca o valor como UTC
        // para que a API devolva o sufixo "Z" (ISO 8601).
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        // Enums são gravados como texto, legível direto no banco (ex.: "EM_ATENDIMENTO").
        configurationBuilder.Properties<Enum>().HaveConversion<string>().HaveMaxLength(30);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios");
            e.Property(u => u.Nome).HasMaxLength(100).IsRequired();
            e.Property(u => u.Email).HasMaxLength(150).IsRequired();
            e.HasIndex(u => u.Email).IsUnique();

            // Excluir o usuário remove, em cascata, todos os dados que pertencem a ele.
            e.HasOne(u => u.Preferencia).WithOne(p => p.Usuario)
                .HasForeignKey<PreferenciaAcessibilidade>(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasMany(u => u.Locais).WithOne(l => l.Usuario)
                .HasForeignKey(l => l.UsuarioId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(u => u.Contatos).WithOne(c => c.Usuario)
                .HasForeignKey(c => c.UsuarioId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(u => u.Solicitacoes).WithOne(s => s.Usuario)
                .HasForeignKey(s => s.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PreferenciaAcessibilidade>(e =>
        {
            e.ToTable("PreferenciasAcessibilidade");
            e.HasIndex(p => p.UsuarioId).IsUnique(); // garante a relação 1:1 no banco
        });

        modelBuilder.Entity<LocalRotina>(e =>
        {
            e.ToTable("LocaisRotina");
            e.Property(l => l.Nome).HasMaxLength(100).IsRequired();
            e.Property(l => l.Endereco).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<ContatoConfianca>(e =>
        {
            e.ToTable("ContatosConfianca");
            e.Property(c => c.Nome).HasMaxLength(100).IsRequired();
            e.Property(c => c.Telefone).HasMaxLength(16).IsRequired();
            e.Property(c => c.Relacao).HasMaxLength(50).IsRequired();

            // Um contato com histórico não pode ser apagado (o banco recusa e a API devolve 409).
            // Também evita dois caminhos de cascata até SolicitacoesAjuda, que o SQL Server não aceita.
            e.HasMany(c => c.Solicitacoes).WithOne(s => s.ContatoConfianca)
                .HasForeignKey(s => s.ContatoConfiancaId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<SolicitacaoAjuda>(e =>
        {
            e.ToTable("SolicitacoesAjuda");
            e.Property(s => s.Mensagem).HasMaxLength(500);
            e.HasIndex(s => new { s.UsuarioId, s.Status });
        });

        modelBuilder.Entity<AlertaUrbano>(e =>
        {
            e.ToTable("AlertasUrbanos");
            e.Property(a => a.Titulo).HasMaxLength(120).IsRequired();
            e.Property(a => a.Descricao).HasMaxLength(500).IsRequired();
            e.Property(a => a.Local).HasMaxLength(200).IsRequired();
            e.HasIndex(a => new { a.Tipo, a.Ativo });
        });
    }

    private sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
        v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
}
