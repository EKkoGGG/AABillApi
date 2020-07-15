using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AABillApi.Models;
using AABillApi.Services;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace AABillApi
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                                  });
            });

            #region MongoDb

            services.Configure<BillsDatabaseSettings>(
               Configuration.GetSection(nameof(BillsDatabaseSettings)));

            services.AddSingleton<IBillsDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<BillsDatabaseSettings>>().Value);

            services.AddSingleton<BillService>();

            #endregion

            services.AddControllers();

            #region JWT

            services.Configure<TokenManagement>(Configuration.GetSection(nameof(TokenManagement)));
            services.AddSingleton<ITokenManagement>(sp =>
               sp.GetRequiredService<IOptions<TokenManagement>>().Value);
            var token = Configuration.GetSection("tokenManagement").Get<TokenManagement>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
                    ValidIssuer = token.Issuer,
                    ValidAudience = token.Audience,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy =>
                    policy.Requirements.Add(new AccountRequirement()
                    ));
                // options.AddPolicy("User", policy => policy.RequireClaim("User"));
            });
            services.AddSingleton<IAuthorizationHandler, CheckTokenRoomIdService>();
            services.AddScoped<IAuthenticateService, TokenAuthenticationService>();

            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
