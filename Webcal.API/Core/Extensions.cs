namespace Webcal.API.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Security.Principal;
    using System.Threading;
    using Connect.Shared.Models;
    using Microsoft.AspNet.Identity;
    using Shared;

    public static class Extensions
    {
        public static ConnectUser GetConnectUser(this IPrincipal principal)
        {
            if (principal == null || principal.Identity == null)
            {
                return null;
            }

            var userId = principal.Identity.GetUserId<int>();
            if (userId < 1)
            {
                return null;
            }

            using (var context = new ConnectContext())
            {
                return context.Users.Include(c => c.CustomerContact)
                    .FirstOrDefault(c => c.Id == userId);
            }
        }

        public static IEnumerable<T> AsPagedData<T>(this IEnumerable<T> data, int pageIndex, int pageSize)
        {
            return data.Skip((pageIndex - 1)*pageSize).Take(pageSize);
        }

        public static DateTime StartOfLastMonth(this DateTime dateTime)
        {
            return DateTime.Parse($"1/{dateTime.Month}/{dateTime.Year}").AddMonths(-1);
        }

        public static DateTime StartOfMonth(this DateTime dateTime)
        {
            return DateTime.Parse($"1/{dateTime.Month}/{dateTime.Year}");
        }

        public static DateTime EndOfNextMonth(this DateTime dateTime)
        {
            return DateTime.Parse($"1/{DateTime.Now.Month}/{DateTime.Now.Year}").AddMonths(2).AddDays(-1);
        }

        public static string ToTitleCase(this string source)
        {
            var textInfo = Thread.CurrentThread.CurrentUICulture.TextInfo;
            return textInfo.ToTitleCase(source.ToLower());
        }

        public static byte[] Decompress(this byte[] data)
        {
            try
            {
                using (var stream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress))
                {
                    const int size = 4096;
                    var buffer = new byte[size];
                    using (var memory = new MemoryStream())
                    {
                        int count;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        } while (count > 0);
                        return memory.ToArray();
                    }
                }
            }
            catch
            {
                return data;
            }
        }
    }
}