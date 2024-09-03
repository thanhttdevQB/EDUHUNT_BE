using EDUHUNT_BE.Data;
using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDUHUNT_BE.Repositories
{
    public class CertificateRepository : ICertificate
    {
        private readonly AppDbContext _context;

        public CertificateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponses.GeneralResponse> ApproveCertificate(Guid id, [FromBody] bool isApproved)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null)
            {
                return new ServiceResponses.GeneralResponse(false,"Not found");
            }

            if (isApproved)
            {
                certificate.IsApproved = true;
                _context.Entry(certificate).State = EntityState.Modified;
                var profile = await _context.Profile.FirstOrDefaultAsync(p => p.UserId.ToString() == certificate.UserId);
                if (profile != null)
                {
                    profile.IsAllow = true;
                    _context.Entry(profile).State = EntityState.Modified;
                }
            }
            else
            {
                _context.Certificates.Remove(certificate);
            }
            await _context.SaveChangesAsync();

            return new ServiceResponses.GeneralResponse(true, "Approve Successfully");
        }

        public async Task<ServiceResponses.GeneralResponse> DeleteCertificate(Guid id)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null)
            {
                return new ServiceResponses.GeneralResponse(false,"Not Found");
            }

            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();

            return new ServiceResponses.GeneralResponse(true, "Delete Successfully");
        }

        public async Task<Certificate> GetCertificateById(Guid id)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            return certificate;
        }

        public async Task<IEnumerable<Certificate>> GetCertificates()
        {
            return await _context.Certificates.ToListAsync();
        }

        public async Task<ServiceResponses.GeneralResponse> PostCertificate(Certificate certificate)
        {
            certificate.Id = Guid.NewGuid();
            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();

            return new ServiceResponses.GeneralResponse(true,"Post Successfully");
        }
    }
}
