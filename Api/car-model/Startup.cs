namespace Car_Model;

using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using Infraestrutura.Db;
using global::Dominio.Dtos;
using global::Dominio.Interface;
using global::Infraestrutura.Servicos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Car_Model.Dominio.ModelViews;
using global::Dominio.Entidades;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        Key = Configuration.GetSection("Jwt").ToString();

        if(string.IsNullOrEmpty(Key)){
            Key = "mySecretKeyForJwtToken123456789"; // At least 128 bits (16 bytes)
        }
    }

    public IConfiguration Configuration { get; }

    public string Key;

    #region Services
    public void ConfigureServices(IServiceCollection services)
    {

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
                ValidateAudience = false,
                ValidateIssuer = false
            };
        });

        // Add services before building the app
        services.AddScoped<IAdministradorServico, AdministradorServico>();
        services.AddScoped<IVeiculoServico, VeiculoServico>();
        services.AddAuthorization();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Insira o token desta maneira: Bearer {seu token}"
            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {

                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },

                    new string[] {}
                }
            });
        });

        services.AddDbContext<DbContexto>(options =>
        {
            options.UseMySql(
                Configuration.GetConnectionString("mysql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("mysql"))
            );
        });
    }
    #endregion

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            #region Home
            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
            #endregion

            #region Administradores

            string GerarTokenJwt(Administrador administrador)
            {
                if (string.IsNullOrEmpty(Key)) return string.Empty;
                
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Email, administrador.Email),
                    new Claim("Perfil", administrador.Perfil),
                    new Claim(ClaimTypes.Role, administrador.Perfil)
                };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);

            }
            ErroDeValidacao ValidacaoAdministrador(AdministradorDto administradorDto)
            {
                var mensagemValidacao = new ErroDeValidacao();

                if (string.IsNullOrEmpty(administradorDto.Email))
                {
                    mensagemValidacao.Mensagem.Add("O campo Email é obrigatório.");
                }
                if (string.IsNullOrEmpty(administradorDto.Senha))
                {
                    mensagemValidacao.Mensagem.Add("O campo Senha é obrigatório.");
                }
                if (administradorDto.Perfil == null)
                {
                    mensagemValidacao.Mensagem.Add("O campo Perfil é obrigatório.");
                }
                if (mensagemValidacao.Mensagem.Count > 0)
                {
                    Results.BadRequest(mensagemValidacao);
                }
                return mensagemValidacao;
                
            }
            endpoints.MapPost("/login", ([FromBody] LoginDto loginDto, IAdministradorServico administradorServico) =>
            {
                var administrador = administradorServico.Login(loginDto);
                // Dummy authentication logic
                if (administrador != null)
                {
                    string token = GerarTokenJwt(administrador);
                    return Results.Ok(new AdministradorLogin
                    {
                        Email = administrador.Email,
                        Perfil = administrador.Perfil,
                        Token = token
                    });
                }
                else
                {
                    return Results.Unauthorized();
                }
            }).AllowAnonymous().WithTags("Administradores");

            endpoints.MapPost("/Administradores", ([FromBody] AdministradorDto administradorDto, IAdministradorServico administradorServico) =>
            {
                var validacaoAdministrador = new ErroDeValidacao();

                ValidacaoAdministrador(administradorDto);


                var administrador = new Administrador
                {
                    Email = administradorDto.Email,
                    Senha = administradorDto.Senha,
                    Perfil = administradorDto.Perfil?.ToString() ?? ""
                };

                var administradorView = new AdministradorModelView
                {
                    Email = administrador.Email,
                    Id = administrador.Id,
                    Perfil = (Perfil)Enum.Parse(typeof(Perfil), administrador.Perfil!)
                };
                administradorServico.Incluir(administrador);
                return Results.Created($"/Administradores/{administrador.Id}", administradorView);
            }).RequireAuthorization().WithTags("Administradores");

            endpoints.MapGet("/Administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
            {
                var adm = new List<AdministradorModelView>();
                var administradores = administradorServico.Todos(pagina);

                foreach (var administrador in administradores)
                {
                    adm.Add(new AdministradorModelView
                    {
                        Email = administrador.Email,
                        Id = administrador.Id,
                        Perfil = (Perfil)Enum.Parse(typeof(Perfil), administrador.Perfil!)
                    });
                }
                return Results.Ok(administradores);
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "adm"}).WithTags("Administradores");

            endpoints.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
            {
                var administrador = administradorServico.BuscarPorId(id);

                if (administrador == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(new AdministradorModelView
                {
                    Email = administrador.Email,
                    Id = administrador.Id,
                    Perfil = (Perfil)Enum.Parse(typeof(Perfil), administrador.Perfil!)
                });
            }).RequireAuthorization(new AuthorizeAttribute {Roles = "adm"}).WithTags("Administradores");

            #endregion

            #region Veiculos

            ErroDeValidacao ValidacaoVeiculo(Car_Model.Dominio.Dtos.VeiculoDto veiculoDto)
            {
                var mensagemValidacao = new ErroDeValidacao();

                if (string.IsNullOrEmpty(veiculoDto.Nome))
                {
                    mensagemValidacao.Mensagem.Add("O campo Nome é obrigatório.");
                }
                if(string.IsNullOrEmpty(veiculoDto.Marca))
                {
                    mensagemValidacao.Mensagem.Add("O campo Marca é obrigatório.");
                }
                if (string.IsNullOrEmpty(veiculoDto.Ano.ToString()))
                {
                    mensagemValidacao.Mensagem.Add("O campo Ano é obrigatório.");
                }
                if (mensagemValidacao.Mensagem.Count > 0)
                {
                    Results.BadRequest(mensagemValidacao);
                }
                return mensagemValidacao;
                
            }
            endpoints.MapPost("/veiculos", ([FromBody] Car_Model.Dominio.Dtos.VeiculoDto veiculoDto, IVeiculoServico veiculoServico) =>
            {
                var veiculo = new Veiculo
                {
                    Nome = veiculoDto.Nome,
                    Marca = veiculoDto.Marca,
                    Ano = veiculoDto.Ano
                };

                ValidacaoVeiculo(veiculoDto);
                veiculoServico.Incluir(veiculo);
                return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "adm"})
            .RequireAuthorization(new AuthorizeAttribute {Roles = "Usuario"})
            .WithTags("Veiculos");

            endpoints.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
            {
                var veiculos = veiculoServico.Todos(pagina);
                return Results.Ok(veiculos);
            }).RequireAuthorization().WithTags("Veiculos");

            endpoints.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
            {
                var veiculo = veiculoServico.BuscarPorId(id);

                if (veiculo == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(veiculo);
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute {Roles = "adm"})
            .RequireAuthorization(new AuthorizeAttribute {Roles = "Usuario"}).WithTags("Veiculos");

            endpoints.MapPut("/veiculos/{id}", ([FromRoute] int id, Car_Model.Dominio.Dtos.VeiculoDto veiculoDto, IVeiculoServico veiculoServico) =>
            {
                var veiculoExistente = veiculoServico.BuscarPorId(id);
                if (veiculoExistente == null)
                {
                    return Results.NotFound();
                }

                veiculoExistente.Nome = veiculoDto.Nome;
                veiculoExistente.Marca = veiculoDto.Marca;
                veiculoExistente.Ano = veiculoDto.Ano;

                ValidacaoVeiculo(veiculoDto);

                veiculoServico.Atualizar(veiculoExistente);
                return Results.Ok(veiculoExistente);
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute {Roles = "adm"}).WithTags("Veiculos");

            endpoints.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
            {
                var veiculoExistente = veiculoServico.BuscarPorId(id);

                if (veiculoExistente == null)
                {
                    return Results.NotFound();
                }

                veiculoServico.Apagar(veiculoExistente);
                return Results.NoContent();
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute {Roles = "adm"}).WithTags("Veiculos")      ;
            #endregion
        });
    }
}