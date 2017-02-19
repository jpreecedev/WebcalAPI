namespace Webcal.API.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data.Entity;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Security.Principal;
    using System.Threading;
    using Connect.Shared.Models;
    using Microsoft.AspNet.Identity;
    using Model;

    public static class Extensions
    {
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }

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
            return data.Skip((pageIndex - 1) * pageSize).Take(pageSize);
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

        public static MobileAppRequestViewModel<QCReport> MapToQcReport(this NameValueCollection formData)
        {
            return MapQcReport<QCReport>(formData);
        }

        public static MobileAppRequestViewModel<QCReport6Month> MapToQcReport6Month(this NameValueCollection formData)
        {
            return MapQcReport<QCReport6Month>(formData);
        }

        private static MobileAppRequestViewModel<T> MapQcReport<T>(NameValueCollection formData) where T : new()
        {
            var result = new MobileAppRequestViewModel<T>
            {
                Username = formData["message[username]"],
                Thumbprint = formData["message[thumbprint]"],
                Data = new T()
            };

            foreach (var originalKey in formData.AllKeys)
            {
                var actualKey = ProcessKey(originalKey);
                if (actualKey != null)
                {
                    var pi = typeof(T).GetProperty(actualKey, BindingFlags.Public | BindingFlags.Instance);
                    if (pi != null)
                    {
                        pi.SetValue(result.Data, ProcessFormDataItem(pi.PropertyType, formData[originalKey]), null);
                    }
                }
            }

            return result;
        }

        private static string ProcessKey(string key)
        {
            if (key.Contains("]["))
            {
                var updatedKey = key.Substring(key.LastIndexOf("[", StringComparison.Ordinal) + 1, key.Length - 1 - key.LastIndexOf("[", StringComparison.Ordinal) - 1);
                updatedKey = updatedKey.Substring(0, 1).ToUpper() + updatedKey.Substring(1);

                if (updatedKey == "QcManagerName")
                {
                    updatedKey = "QCManagerName";
                }

                return updatedKey;
            }
            return null;
        }

        private static object ProcessFormDataItem(Type type, string value)
        {
            if (type == typeof(DateTime))
            {
                return DateTime.Parse(value);
            }
            if (type == typeof(bool))
            {
                return bool.Parse(value);
            }
            if (type == typeof(bool?))
            {
                if (!string.IsNullOrEmpty(value))
                {
                    return bool.Parse(value);
                }
            }
            if (type == typeof(int?))
            {
                if (!string.IsNullOrEmpty(value))
                {
                    return int.Parse(value);
                }
            }
            return value;
        }
    }
}