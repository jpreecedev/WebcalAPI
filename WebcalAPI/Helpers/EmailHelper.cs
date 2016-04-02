namespace WebcalAPI.Helpers
{
    using System.Collections.Generic;
    using System.Text;
    using Models;

    public static class EmailBuilder
    {
        public static string GetCalibrationDataTable(ICollection<RecentCalibrationsViewModel> calibrations)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("<table><thead><tr>");
            stringBuilder.Append("<th>Company Name</th>" +
                                 "<th>Doc Type</th>" +
                                 "<th>Expiration</th>" +
                                 "<th>Registration</th>" +
                                 "<th>Technician</th>" +
                                 "<th>Customer</th>");
            stringBuilder.Append("</tr></thead><tbody>");

            foreach (var document in calibrations)
            {
                stringBuilder.Append("<tr>");
                stringBuilder.Append($"<td>{document.CompanyName}</td>");
                stringBuilder.Append($"<td>{document.DocumentType}</td>");
                stringBuilder.Append($"<td>{document.DocumentType}</td>");
                stringBuilder.Append($"<td>{document.Registration}</td>");
                stringBuilder.Append($"<td>{document.Technician}</td>");
                stringBuilder.Append($"<td>{document.Customer}</td>");
                stringBuilder.Append("</tr>");
            }

            stringBuilder.Append("</tbody></table>");
            return stringBuilder.ToString();
        }
    }
}