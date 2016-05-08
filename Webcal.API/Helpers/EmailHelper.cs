namespace Webcal.API.Helpers
{
    using System.Collections.Generic;
    using System.Text;
    using Webcal.Model;

    public static class EmailHelper
    {
        public static string GetBasicTemplate(string header, string body)
        {
            return "<!DOCTYPE html><html lang=\"en\"><style>body{color: #555555; font-weight: 200; font-family: 'Trebuchet MS', Helvetica, sans-serif; background-color: #f5f5f5;}h2{font-size: 30px; font-weight: 300; margin-top: 30px; margin-bottom: 30px;}table>thead>tr>th{vertical-align: bottom; border-bottom: 2px solid #dddddd; line-height: 1.42857143; text-align: left; padding: 8px; font-size: 16px; font-weight: bold;}.email-template{max-width: 1200px; margin: 30px; padding: 30px;}table{width: 100%; border: none;}</style><head></head><body> <div class=\"email-template\"> <div> <img alt=\"Webcal Calibration Software\" src=\"http://test.webcalconnect.com/img/webcal.png\" style=\"max-width:200px; margin-left: auto; margin-right: auto; display: block; text-align: center;\"></div><div><h2>" + header + "</h2>" + body + "</div></div></body></html>";
        }

        public static string GetCalibrationDataTable(IEnumerable<RecentCalibrationsViewModel> calibrations)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("<table><thead><tr>");
            stringBuilder.Append("<th>&nbsp;</th>" +
                                 "<th>Company Name</th>" +
                                 "<th>Doc Type</th>" +
                                 "<th>Expiration</th>" +
                                 "<th>Registration</th>" +
                                 "<th>Technician</th>" +
                                 "<th>Customer</th>");
            stringBuilder.Append("</tr></thead><tbody>");

            foreach (var document in calibrations)
            {
                stringBuilder.Append("<tr>");
                stringBuilder.Append($"<td><img src=\"{IconHelper.GetIcon(document.DocumentTypeEnum)}\" alt=\"{document.DocumentType}\" /></td>");
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
            stringBuilder.Append("<th>&nbsp;</th>" +
                                 "<th>Date</th>" +
                                 "<th>Expiration</th>" +
                                 "<th>Registration</th>" +
                                 "<th>Technician</th>" +
                                 "<th>Customer</th>" +
                                 "<th>Vehicle Manufacturer</th>");
            stringBuilder.Append("</tr></thead><tbody>");

            foreach (var document in calibrations)
            {
                stringBuilder.Append("<tr>");
                stringBuilder.Append($"<td><img src=\"{IconHelper.GetIcon(document.DocumentTypeEnum)}\" alt=\"{document.DocumentType}\" /></td>");
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