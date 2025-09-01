using Core.DTOs.Portfolio;

namespace Core.Interfaces
{
    public interface IPortfolioService
    {
        Task<List<PortfolioCategoryDTO>> GetAllProjectCategoriesAsync();
        //Task<Core.Models.ProjectCategory?> GetProjectCategoryByIdAsync(int id);
        Task<PortfolioCategoryDTO> CreateProjectCategoryAsync(CreatePortfolioCategoryDTO newCategory);
        //Task<bool> UpdateProjectCategoryAsync(int id, Core.DTOs.Portfolio.UpdateProjectCategoryDTO updatedCategory);
        Task<bool> DeleteProjectCategoryAsync(int id);

        Task<List<ProjectDTO>> GetAllProjectsAsync();
        Task<ProjectDTO> GetProjectByTitleAsync(string title);
        Task<List<ProjectDTO>> GetProjectsByCategoryIdAsync(int categoryId);
        Task<ProjectDTO> CreateProjectAsync(CreateProjectDTO newProject);

    }
}
