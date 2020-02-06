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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            //注入授权Handler
            services.AddSingleton<IAuthorizationHandler, PolicyHandler>();

            services.AddDbContext<YMNContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("YMNContextString")));
            services.AddSession();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //异常处理
            app.UseMyExceptionHandler(loggerFactory);
            app.UseAuthentication();

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
