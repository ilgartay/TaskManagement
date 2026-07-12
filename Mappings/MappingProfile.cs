using AutoMapper;
using TaskManagement.API.Models;
using TaskManagement.API.DTOs;

namespace TaskManagement.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();


            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();


    CreateMap<TaskItem, TaskItemDto>()
        .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));
    CreateMap<CreateTaskDto, TaskItem>();
    CreateMap<UpdateTaskDto, TaskItem>();
        }
    }
}