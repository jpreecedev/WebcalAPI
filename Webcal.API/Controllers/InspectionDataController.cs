namespace Webcal.API.Controllers
{
    using System.Linq;
    using System.Web.Http;
    using Connect.Shared.Models;
    using Core;
    using Model;

    [RoutePrefix("api/inspectiondata")]
    public class InspectionDataController : BaseApiController
    {
        [HttpGet]
        [Route("{vehicleRegistration}")]
        public IHttpActionResult Get(string vehicleRegistration)
        {
            if (string.IsNullOrEmpty(vehicleRegistration))
            {
                return BadRequest();
            }

            using (var context = new ConnectContext())
            {
                vehicleRegistration = vehicleRegistration.ToUpper().Trim().Replace(" ", "");

                var allDocuments = context.GetAllDocuments(ConnectUser)
                    .OfType<TachographDocument>()
                    .OrderByDescending(c => c.CalibrationTime.GetValueOrDefault())
                    .Where(c => c.RegistrationNumber == vehicleRegistration)
                    .ToList();

                var result = new InspectionDataViewModel();
                result.Parse(allDocuments.FirstOrDefault());

                if (allDocuments.Count > 0)
                {
                    var documents = allDocuments.Where(c => !string.IsNullOrEmpty(c.InspectionInfo)).Skip(1)
                        .Select(c => new HistoryViewModel(c.CalibrationTime.GetValueOrDefault(), c.InspectionInfo));

                    foreach (var item in documents)
                    {
                        result.History.Add(item);
                    }
                }

                return Ok(result);
            }
        }
    }
}