using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using INK_API._Repositories.Interface;
using INK_API._Repositories.Repositories;
using INK_API._Services.Interface;
using INK_API._Services.Services;
using INK_API.Data;
using INK_API.Helpers;
using INK_API.Helpers.AutoMapper;
using INK_API.SignalrHub;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
namespace INK_API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private string factory;
        private string area;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            factory = Configuration.GetSection("Appsettings:Factory").Value;
            area = Configuration.GetSection("Appsettings:Area").Value;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddSignalR();
            services.AddLogging();
            services.AddCors();
            var connetionString = Configuration.GetConnectionString($"{factory}_{area}_DefaultConnection");
            services.AddDbContext<DataContext>(options => options.UseSqlServer(connetionString));
            services.AddDbContext<IoTContext>(options => options.UseMySQL(Configuration.GetConnectionString("IoTConnection")));
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            //Auto Mapper
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<IMapper>(sp =>
            {
                return new Mapper(AutoMapperConfig.RegisterMappings());
            });
            services.AddSingleton(AutoMapperConfig.RegisterMappings());

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                        .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ink Room", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,

                            },
                            new List<string>()
                        }
                    });

            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //Repository
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IGluesRepository, GluesRepository>();
            services.AddScoped<IUserDetailRepository, UserDetailRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IProcessRepository, ProcessRepository>();
            services.AddScoped<IBuildingRepository, BuildingRepository>();
            services.AddScoped<IBuildingUserRepository, BuildingUserRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IPartRepository, PartRepository>();
            services.AddScoped<IBuildingGlueRepository, BuildingGlueRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IRawDataRepository, RawDataRepository>();
            services.AddScoped<IInkRepository, InkRepository>();
            services.AddScoped<IChemicalRepository, ChemicalRepository>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IInkObjectRepository, InkObjectRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleUserRepository, RoleUserRepository>();
            services.AddScoped<ISchedulePartRepository, SchedulePartRepository>();
            services.AddScoped<IPartInkChemicalRepository, PartChemicalRepository>();
            services.AddScoped<IScheduleUpdateRepository, ScheduleUpdateRepository>();
            services.AddScoped<ITreatmentWayRepository, TreatmentWayRepository>();
            services.AddScoped<IStockRepository, StockRepository>();
            services.AddScoped<IWorkPlanRepository, WorkPlanRepository>();
            services.AddScoped<IWorkPlanMasterRepository, WorkPlanMasterRepository>();
            services.AddScoped<IPoGlueRepository, PoGlueRepository>();


            //Services
            services.AddScoped<IGluesService, GluesService>();
            services.AddScoped<ITreatmentWayService, TreatmentWayService>();
            services.AddScoped<IUserDetailService, UserDetailService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IBuildingService, BuildingService>();
            services.AddScoped<IBuildingUserService, BuildingUserService>();
            services.AddScoped<IProcessService, ProcessService>();
            services.AddScoped<IPartService, PartService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IInkService, InkService>();
            services.AddScoped<IChemicalService, ChemicalService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IInkObjectService, InkObjectService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRoleUserService, RoleUserService>();
            services.AddScoped<ISchedulePartService, SchedulePartService>();
            services.AddScoped<IWorkPlanService, WorkPlanService>();
            services.AddScoped<IWorkPlanMasterService, WorkPlanMasterService>();
            services.AddScoped<IPoGlueService, PoGlueService>();
            services.AddScoped<ISettingWorkPlanService, SettingWorkPlanService>();
            // services.AddScoped<IGlueService, GlueService>();

            //extension
            services.AddScoped<IMailExtension, MailExtension>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env , DataContext context)
        {
            //context.Database.Migrate();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ink Room ");
            });
            app.UseCors(x => 
            x.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin());
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapHub<ECHub>("/ec-hub");

            });
        }
    }
}
