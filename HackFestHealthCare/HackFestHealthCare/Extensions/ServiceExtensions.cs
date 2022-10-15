using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackFestHealthCare.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder
                .WithOrigins("https://www.anshewo.com")
                .AllowAnyHeader()
                .WithMethods("POST", "GET", "PUT", "DELETE")
                .AllowCredentials()
                .WithHeaders("accept", "content-type")
                .Build());
            });


        public static void ConfigureIISIntegration(this IServiceCollection services) =>
       services.Configure<IISOptions>(options =>
       {

       });

        public static string GetErrors(this IdentityResult result)
        {
            var message = string.Join(",", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
            return message;
        }
        public static async Task<Page<T>> ToPagedListAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var count = await query.CountAsync();
            int offset = (pageNumber - 1) * pageSize;
            var items = await query.Skip(offset).Take(pageSize).ToArrayAsync();
            return new Page<T>(items, count, pageNumber, pageSize);
        }

        public static Page<T> ToPagedList<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var count = query.Count();
            int offset = (pageNumber - 1) * pageSize;
            var items = query.Skip(offset).Take(pageSize).ToArray();
            return new Page<T>(items, count, pageNumber, pageSize);
        }

        public static Page<TResult> Select<T, TResult>(this Page<T> page, Func<T, TResult> selector)
        {
            var mapped = page.Items.Select(selector).ToArray();
            return new Page<TResult>(mapped, page.TotalSize, page.PageNumber, page.PageSize);
        }
        public static Page<T> ToPageList<T>(this IEnumerable<T> query, int pageNumber, int pageSize)
        {
            var count = query.Count();
            int offset = (pageNumber - 1) * pageSize;
            var items = query.Skip(offset).Take(pageSize).ToArray();
            return new Page<T>(items, count, pageNumber, pageSize);
        }
        public static Page<T> NoPaginate<T>(this IEnumerable<T> query, int pageNumber, int pageSize)
        {
            var count = query.Count();
            var items = query;
            return new Page<T>(items.ToArray(), count, pageNumber, pageSize);
        }
        public static string GenerateRandom(this string value, int length)
        {
            Random random = new Random();
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string number = "0123456789";
            const string symbol = "~!@#$%^&*?-_";
            string final = "";


            final = new string(Enumerable.Repeat(upper + lower + number + symbol, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return final;
        }
        public static string GenerateRandom()
        {
            Random generator = new Random();
            String rand = generator.Next(0, 9999).ToString("D4");
            return rand;
        }
    }
}
