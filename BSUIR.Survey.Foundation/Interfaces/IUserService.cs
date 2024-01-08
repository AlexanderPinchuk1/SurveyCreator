using BSUIR.Survey.Domain;
using BSUIR.Survey.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace BSUIR.Survey.Foundation
{
    public interface IUserService
    {
        public Task<IdentityResult> EditUserAsync(User user, string role);

        public Task<IdentityResult> DeleteUserAsync(User user);

        public Task<IdentityResult> AddUserAsync(User user, string password);

        public Task<IdentityResult> AddUserRole(User user, string role);

        public Task<PagedEntities<UserInfo>> GetUsersInfoForPageAsync(int pageIndex, int itemCountPerPage, string? searchKeyWord);

        public Task<User> FindUserByIdAsyncOrReturnNull(Guid id);

        public Task<string> GetUserRoleAsync(User user);

        public Task<bool> IsEmailUniqueAsync(Guid userId, string email);
    }
}
