namespace WebcalAPI.Helpers
{
    using System.Collections.Generic;
    using System.Text;
    using Models;

    public static class EmailHelper
    {
        public static string GetCalibrationDataTable(IEnumerable<RecentCalibrationsViewModel> calibrations)
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
                stringBuilder.Append($"<td>{document.Expiration.ToString("MMM dd, yyyy")}</td>");
                stringBuilder.Append($"<td>{document.Registration}</td>");
                stringBuilder.Append($"<td>{document.Technician}</td>");
                stringBuilder.Append($"<td>{document.Customer}</td>");
                stringBuilder.Append("</tr>");
            }

            stringBuilder.Append("</tbody></table>");
            return stringBuilder.ToString();
        }

        public static string GetCalibrationDataTable(IEnumerable<CalibrationsDueViewModel> calibrations)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("<table><thead><tr>");
            stringBuilder.Append("<th>Date</th>" +
                                 "<th>Expiration</th>" +
                                 "<th>Registration</th>" +
                                 "<th>Technician</th>" +
                                 "<th>Customer</th>" +
                                 "<th>Vehicle Manufacturer</th>");
            stringBuilder.Append("</tr></thead><tbody>");

            foreach (var document in calibrations)
            {
                stringBuilder.Append("<tr>");
                stringBuilder.Append($"<td>{document.Date.ToString("MMM dd, yyyy")}</td>");
                stringBuilder.Append($"<td>{document.Expiration.ToString("MMM dd, yyyy")}</td>");
                stringBuilder.Append($"<td>{document.Registration}</td>");
                stringBuilder.Append($"<td>{document.Technician}</td>");
                stringBuilder.Append($"<td>{document.Customer}</td>");
                stringBuilder.Append($"<td>{document.VehicleManufacturer}</td>");
                stringBuilder.Append("</tr>");
            }

            stringBuilder.Append("</tbody></table>");
            return stringBuilder.ToString();
        }
    }
}