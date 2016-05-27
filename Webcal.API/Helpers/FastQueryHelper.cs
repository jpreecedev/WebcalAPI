namespace Webcal.API.Helpers
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;

    public static class FastQueryHelper
    {
        public static string GetSqlQueryFor<T>()
        {
            var properties = typeof(T).GetProperties().Where(c => !Attribute.IsDefined(c, typeof(NotMappedAttribute)))
                .Where(c => c.Name != "SerializedData")
                .Where(c => c.GetSetMethod() != null);

            var builder = new StringBuilder();
            builder.Append("SELECT ");

            foreach (var propertyInfo in properties)
            {
                builder.Append(propertyInfo.Name + ", ");
            }

            builder.Append("NULL AS SerializedData");

            var result = builder.ToString();
            result = result + " FROM dbo.[" + typeof(T).Name + "s]";

            return result;
        }
    }
}