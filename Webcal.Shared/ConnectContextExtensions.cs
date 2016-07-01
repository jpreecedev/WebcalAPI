namespace Webcal.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.SqlTypes;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Connect.Shared;
    using Connect.Shared.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Model;

    public static class ConnectContextExtensions
    {
        public static async Task<DirectUploadDocument> GetDirectUploadDocumentAsync(this ConnectContext context, ConnectUser connectUser, int documentId)
        {
            var applicationUserManager = GetApplicationUserManager();
            var userRoles = applicationUserManager.GetRoles(connectUser.Id);

            if (userRoles.Any(role => string.Equals(ConnectRoles.Admin, role)))
            {
                return await (from document in context.Set<DirectUploadDocument>()
                    where document.Deleted == null && document.Id == documentId
                    select document).FirstOrDefaultAsync();
            }

            return await (from document in context.Set<DirectUploadDocument>()
                where document.Deleted == null && document.UserId == connectUser.Id && document.Id == documentId
                select document).FirstOrDefaultAsync();
        }

        public static async Task<IEnumerable<DirectUploadDocument>> GetDirectUploadDocumentsAsync(this ConnectContext context, ConnectUser connectUser)
        {
            var applicationUserManager = GetApplicationUserManager();

            IQueryable<DirectUploadDocument> documents;
            var userRoles = applicationUserManager.GetRoles(connectUser.Id);

            if (userRoles.Any(role => string.Equals(ConnectRoles.Admin, role)))
            {
                documents = from document in context.Set<DirectUploadDocument>()
                            where document.Deleted == null
                            select document;
            }
            else
            {
                documents = from document in context.Set<DirectUploadDocument>()
                            where document.Deleted == null && document.UserId == connectUser.Id
                            select document;
            }

            return await documents.ToListAsync();
        }

        public static IEnumerable<CalibrationsDueViewModel> CalibrationsDue(this ConnectContext context, ConnectUser connectUser, int userId, DateTime from, DateTime to)
        {
            return context.GetAllDocuments(connectUser, from, to).Select(c => new CalibrationsDueViewModel(c))
                .Where(c => (c.UserId == userId || userId == -1) && c.Created.Date >= from.Date && c.Expiration.Date <= to.Date);
        }

        public static IEnumerable<RecentCalibrationsViewModel> RecentCalibrations(this ConnectContext context, ConnectUser connectUser, int userId, DateTime from)
        {
            return context.GetAllDocuments(connectUser).Select(c => new RecentCalibrationsViewModel(c))
                    .Where(c => (c.UserId == userId || userId == -1) && c.Created.Date >= from.Date);
        }

        public static IEnumerable<T> GetReports<T>(this ConnectContext context, ConnectUser connectUser) where T : BaseReport
        {
            if (connectUser == null)
            {
                throw new ArgumentException("ConnectUser is invalid");
            }

            IEnumerable<T> result = new List<T>();

            result = result.Concat(DoGetReports<T>(context, connectUser));

            return result.OrderByDescending(c => c.Created.Date);
        }

        public static async Task<BaseModel> GetDocumentAsync(this ConnectContext context, DocumentType documentType, int id)
        {
            IQueryable<BaseModel> set = null;

            switch (documentType)
            {
                case DocumentType.Tachograph:
                    set = context.Set<TachographDocument>();
                    break;
                case DocumentType.Undownloadability:
                    set = context.Set<UndownloadabilityDocument>();
                    break;
                case DocumentType.LetterForDecommissioning:
                    set = context.Set<LetterForDecommissioningDocument>();
                    break;
                case DocumentType.QCReport:
                    set = context.Set<QCReport>();
                    break;
                case DocumentType.QCReport6Month:
                    set = context.Set<QCReport6Month>();
                    break;
            }

            if (set != null)
            {
                return await set.FirstOrDefaultAsync(document => document.Id == id);
            }

            return null;
        }

        public static IEnumerable<Document> GetAllDocuments(this ConnectContext context, ConnectUser connectUser, DateTime from, DateTime to)
        {
            if (connectUser == null)
            {
                throw new ArgumentException("ConnectUser is invalid");
            }

            IEnumerable<Document> result = new List<Document>();

            result = result.Concat(GetDocuments<TachographDocument>(context, connectUser, from, to));
            result = result.Concat(GetDocuments<UndownloadabilityDocument>(context, connectUser, from, to));
            result = result.Concat(GetDocuments<LetterForDecommissioningDocument>(context, connectUser, from, to));

            return result.OrderByDescending(c => c.InspectionDate.GetValueOrDefault());
        }

        public static IEnumerable<Document> GetAllDocuments(this ConnectContext context, ConnectUser connectUser)
        {
            if (connectUser == null)
            {
                throw new ArgumentException("ConnectUser is invalid");
            }

            IEnumerable<Document> result = new List<Document>();

            result = result.Concat(GetDocuments<TachographDocument>(context, connectUser));
            result = result.Concat(GetDocuments<UndownloadabilityDocument>(context, connectUser));
            result = result.Concat(GetDocuments<LetterForDecommissioningDocument>(context, connectUser));

            return result.OrderByDescending(c => c.InspectionDate.GetValueOrDefault());
        }

        private static IEnumerable<T> GetDocuments<T>(ConnectContext context, ConnectUser connectUser) where T : Document
        {
            return GetDocuments<T>(context, connectUser, (DateTime?)SqlDateTime.MinValue, (DateTime?)SqlDateTime.MaxValue);
        }

        private static IEnumerable<T> GetDocuments<T>(ConnectContext context, ConnectUser connectUser, DateTime? from, DateTime? to) where T : Document
        {
            var result = new List<T>();

            var documentCount = context.Set<T>().Count();
            if (documentCount > 0)
            {
                var applicationUserManager = GetApplicationUserManager();

                List<T> documents = null;
                var userRoles = applicationUserManager.GetRoles(connectUser.Id);

                if (userRoles.Any(role => string.Equals(ConnectRoles.Admin, role)))
                {
                    documents = (from document in context.Set<T>().SqlQuery(FastQueryHelper.GetSqlQueryFor<T>(true))
                                 where document.Deleted == null
                                 select document).ToList();
                }
                else if (userRoles.Any(role => string.Equals(ConnectRoles.TachographCentre, role)))
                {
                    documents = (from document in context.Set<T>().SqlQuery(FastQueryHelper.GetSqlQueryFor<T>(true))
                                 where document.Deleted == null && document.UserId == connectUser.Id
                                 select document).ToList();
                }
                else if (userRoles.Any(role => string.Equals(ConnectRoles.TachographCentre, role)) && connectUser.CustomerContact != null)
                {
                    documents = (from linkedVehicle in context.LinkedVehicles.Include(x => x.CustomerContact).Where(v => v.CustomerContact.Id == connectUser.CustomerContact.Id).DefaultIfEmpty()
                                 from document in context.Set<T>().Where(d => d.RegistrationNumber == linkedVehicle.VehicleRegistrationNumber).DefaultIfEmpty()
                                 select document).ToList();
                }

                if (from != null && to != null && documents != null)
                {
                    foreach (var document in documents.Where(d => d != null))
                    {
                        var inspectionDate = document.InspectionDate.GetValueOrDefault();
                        if (inspectionDate != default(DateTime))
                        {
                            var nextDueDate = inspectionDate.AddYears(2);
                            if (nextDueDate >= from.GetValueOrDefault() && nextDueDate <= to.GetValueOrDefault())
                            {
                                result.Add(document);
                            }
                        }
                    }
                }

                var users = context.Users.ToList().Where(u => documents.Any(d => d.UserId == u.Id)).ToList();

                documents.ForEach(document =>
                {
                    var user = users.FirstOrDefault(u => u.Id == document.UserId);
                    if (user != null)
                    {
                        document.CompanyName = user.CompanyKey;
                    }
                });
            }

            return result;
        }

        private static ApplicationUserManager GetApplicationUserManager()
        {
            return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        private static IEnumerable<T> DoGetReports<T>(ConnectContext context, ConnectUser connectUser) where T : BaseReport
        {
            return GetCentreReports<T>(context, connectUser);
        }

        private static IEnumerable<T> GetCentreReports<T>(DbContext context, ConnectUser connectUser) where T : BaseReport
        {
            var result = new List<T>();

            var documentCount = context.Set<T>().Count();
            if (documentCount > 0)
            {
                var applicationUserManager = GetApplicationUserManager();

                IQueryable<T> documents = null;
                var userRoles = applicationUserManager.GetRoles(connectUser.Id);

                if (userRoles.Any(role => string.Equals(ConnectRoles.Admin, role)))
                {
                    documents = from document in context.Set<T>()
                                where document.Deleted == null
                                select document;
                }
                else if (userRoles.Any(role => string.Equals(ConnectRoles.TachographCentre, role)))
                {
                    documents = from document in context.Set<T>()
                                where document.Deleted == null && (document.ConnectUserId.HasValue ? document.ConnectUserId.Value : 0) == connectUser.Id
                                select document;
                }

                if (documents != null)
                {
                    result.AddRange(documents);
                }
            }

            return result;
        }
    }
}