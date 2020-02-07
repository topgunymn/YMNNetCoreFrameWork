using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using NLog.Web;
using YMNNetCoreFrameWork.Core.Authoratication;
using YMNNetCoreFrameWork.EntityFrameworkCore;
using YMNNetCoreFrameWork.Host.Auths;
using YMNNetCoreFrameWork.Host.Middles;

namespace YMNNetCoreFrameWork.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddDbContext<YMNContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("YMNContextString")));
            var key = Configuration["Authentication:JwtBearer:SecurityKey"];
            var Issuer = Configuration["Authentication:JwtBearer:Issuer"];
            var Audience = Configuration["Authentication:JwtBearer:Audience"];


            services.AddIdentity<YMNUser, Role>()
  .AddEntityFrameworkStores<YMNContext>() ;
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            // //添加jwt验证：
            // .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            // {
            //添加授权策略
            services.AddAuthorization(options=> {
                options.AddPolicy("YMNPolicy", policy => policy.Requirements.Add(new YMNPolicy()));
            })              
           .AddAuthentication(options =>
            {
                     //identity.application
                     var a = options.DefaultAuthenticateScheme;
                var b = options.DefaultChallengeScheme;
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            }).AddJwtBearer("JwtBearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,//是否验证失效时间
                    ClockSkew = TimeSpan.FromSeconds(30),

                    ValidateAudience = true,//是否验证Audience
                                            //ValidAudience = Const.GetValidudience(),//Audience
                                            //这里采用动态验证的方式，在重新登陆时，刷新token，旧token就强制失效了
                    AudienceValidator = (m, n, z) =>
                  {
                      return m != null && m.FirstOrDefault().Equals(Audience);
                  },
                    ValidateIssuer = true,//是否验证Issuer
                    ValidIssuer = Issuer,//Issuer，这两项和前面签发jwt的设置一致

                    ValidateIssuerSigningKey = true,//是否验证SecurityKey
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))//拿到SecurityKey
                };
                //options.Events = new JwtBearerEvents
                //{
                //    OnAuthenticationFailed = context =>
                //    {
                //        //Token expired
                //        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                //        {
                //            context.Response.Headers.Add("Token-Expired", "true");
                //        }
                //        return Task.CompletedTask;
                //    }
                //};
            });
            //services.AddAuthentication(options => {
            //    options.DefaultAuthenticateScheme = "JwtBearer";
            //    options.DefaultChallengeScheme = "JwtBearer";
            //}).AddJwtBearer("JwtBearer", options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateLifetime = true,//是否验证失效时间
            //        ClockSkew = TimeSpan.FromSeconds(30),

            //        ValidateAudience = true,//是否验证Audience
            //                                //ValidAudience = Const.GetValidudience(),//Audience
            //                                //这里采用动态验证的方式，在重新登陆时，刷新token，旧token就强制失效了
            //        AudienceValidator = (m, n, z) =>
            //        {
            //            return m != null && m.FirstOrDefault().Equals(Const.ValidAudience);
            //        },
            //        ValidateIssuer = true,//是否验证Issuer
            //        ValidIssuer = Const.Domain,//Issuer，这两项和前面签发jwt的设置一致

            //        ValidateIssuerSigningKey = true,//是否验证SecurityKey
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Const.SecurityKey))//拿到SecurityKey
            //    };
            //    //options.Events = new JwtBearerEvents
            //    //{
            //    //    OnAuthenticationFailed = context =>
            //    //    {
            //    //        //Token expired
            //    //        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            //    //        {
            //    //            context.Response.Headers.Add("Token-Expired", "true");
            //    //        }
            //    //        return Task.CompletedTask;
            //    //    }
            //    //};
            //});
         
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v0.1.0",//版本号
                    Title = "尹大师框架说明,QQ:1390788386",//文档标题
                    Description = "框架说明文档",//文档描述
                    Contact = new OpenApiContact { Name = "道法自然", Email = "1390788386@qq.com"}//联系人
                });
                // Assign scope requirements to operations based on AuthorizeAttribute
                //options.OperationFilter<SecurityRequirementsOperationFilter>();
                //设置swagger的xml文档

                //c.DocInclusionPredicate((docName, description) => true);

                //// Define the BearerAuth scheme that's in use
                //c.AddSecurityDefinition("bearerAuth", new ApiKeyScheme()
                //{
                //    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                //    Name = "Authorization",
                //    In = "header",
                //    Type = "apiKey"
                //});
                //// Assign scope requirements to operations based on AuthorizeAttribute
                //c.OperationFilter<SecurityRequirementsOperationFilter>();

               // c.DocInclusionPredicate((docName, description) => true);
                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Description = "Authorization format : Bearer {token}",
                //    Name = "Authorization",
                //    In = "header",
                //    Type = "apiKey"
                //});//api界面新增authorize按钮
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "西方输入Token，使用Bearer开头",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                   {
                        new OpenApiSecurityScheme
                        {
                       Reference = new OpenApiReference {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                     }
                     },
            new string[] { }
                  }
                  });


                string filepath = $"{AppContext.BaseDirectory}YMNNetCoreFrameWork.Host.xml";
                c.IncludeXmlComments(filepath);
            });

            //注入授权Handler
            services.AddScoped<IAuthorizationHandler, PolicyHandler>();
            services.AddSession();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,ILogger<Startup> logger)
        {
            //异常处理
            app.UseMyExceptionHandler(loggerFactory);
            app.UseAuthentication();
            logger.LogError("启动程序");
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            // Enable middleware to serve generated Swagger as a JSON endpoint.

            //添加NLog
            loggerFactory.AddNLog();
            //读取Nlog配置文件
            env.ConfigureNLog("NLog.config");
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseSession();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }


        /* This method is needed to authorize SignalR javascript client.
 * SignalR can not send authorization header. So, we are getting it from query string as an encrypted text. */
        private static Task QueryStringTokenResolver(MessageReceivedContext context)
        {
            if (!context.HttpContext.Request.Path.HasValue ||
                !context.HttpContext.Request.Path.Value.StartsWith("/signalr"))
            {
                // We are just looking for signalr clients
                return Task.CompletedTask;
            }

            var qsAuthToken = context.HttpContext.Request.Query["enc_auth_token"].FirstOrDefault();
            if (qsAuthToken == null)
            {
                // Cookie value does not matches to querystring value
                return Task.CompletedTask;
            }

            // Set auth token from cookie
            // context.Token = SimpleStringCipher.Instance.Decrypt(qsAuthToken, AppConsts.DefaultPassPhrase);
            return Task.CompletedTask;
        }
    }
}
