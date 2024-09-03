using EDUHUNT_BE.Models;
using Microsoft.AspNetCore.Mvc;
using static EDUHUNT_BE.DTOs.ServiceResponses;

namespace EDUHUNT_BE.Interfaces
{
    public interface ICertificate
    {
        public Task<IEnumerable<Certificate>> GetCertificates();
        public Task<Certificate> GetCertificateById(Guid id);
        public Task<GeneralResponse> ApproveCertificate(Guid id, [FromBody] bool isApproved);
        public Task<GeneralResponse> DeleteCertificate(Guid id);
        public Task<GeneralResponse> PostCertificate(Certificate certificate);
    }
}
