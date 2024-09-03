using EDUHUNT_BE.Data;
using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.Models;
using Microsoft.EntityFrameworkCore;
using static EDUHUNT_BE.DTOs.ServiceResponses;


namespace EDUHUNT_BE.Repositories
{
    public class StudentTypeRepository : IStudentType
    {
        private readonly AppDbContext _context;
        public StudentTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GeneralResponse> CreateStudentType(StudentTypeDto studentTypeDto)
        {
            var studentType = new StudentType
            {
                StudentTypeId = studentTypeDto.StudentTypeId,
                TypeName = studentTypeDto.TypeName,
                Money = studentTypeDto.Money,
                Study = studentTypeDto.Study
            };
            await _context.StudentTypes.AddAsync(studentType);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Student type created successfully.");
        }

        public async Task<GeneralResponse> DeleteStudentType(int studentTypeId)
        {
            var studentType = _context.StudentTypes.Where(q => q.StudentTypeId == studentTypeId).FirstOrDefault();
            if (studentType == null)
            {
                return new GeneralResponse(false, "StudentType not found");
            }
            _context.StudentTypes.Remove(studentType);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "StudentType deleted successfully.");
        }

        public async Task<ICollection<StudentTypeDto>> GetStudentType()
        {
            return await _context.StudentTypes
                .Select(st => new StudentTypeDto
                {
                    StudentTypeId = st.StudentTypeId,
                    TypeName = st.TypeName,
                    Money = st.Money.ToString(),
                    Study = st.Study.ToString()
                })
                .ToListAsync();
        }

        public async Task<StudentTypeDto> GetStudentTypeById(int studentTypeId)
        {
            var studentType = await _context.StudentTypes.FindAsync(studentTypeId);
            if (studentType == null) {
                return null!;
            }
            var studentTypeDto = new StudentTypeDto
            {
                Money = studentType.Money.ToString(),
                Study = studentType.Study.ToString(),
                StudentTypeId = studentType.StudentTypeId,
                TypeName = studentType.TypeName
            };
            return studentTypeDto;

        }

        public async Task<GeneralResponse> UpdateStudentType(StudentTypeDto studentType)
        {
            var studentTypeToUpdate = _context.StudentTypes.Where(q => q.StudentTypeId == studentType.StudentTypeId).FirstOrDefault();
            if (studentTypeToUpdate == null)
            {
                return new GeneralResponse(false, "StudentType not found");
            }

            studentTypeToUpdate.Study = studentType.Study;
            studentTypeToUpdate.TypeName = studentType.TypeName;
            studentTypeToUpdate.Money = studentTypeToUpdate.Money;
            _context.StudentTypes.Update(studentTypeToUpdate);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "StudentType updated successfully.");
        }
    }
}