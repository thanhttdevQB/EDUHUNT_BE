using EDUHUNT_BE.DTOs;
using static EDUHUNT_BE.DTOs.ServiceResponses;

namespace EDUHUNT_BE.Interfaces
{
    public interface IStudentType
    {
        Task<ICollection<StudentTypeDto>> GetStudentType();
        Task<StudentTypeDto> GetStudentTypeById(int studentTypeId);
        Task<GeneralResponse> CreateStudentType(StudentTypeDto studentType);
        Task<GeneralResponse> UpdateStudentType(StudentTypeDto studentType);
        Task<GeneralResponse> DeleteStudentType(int studentTypeId);
    }
}
