using BRoles.Models;

namespace BRoles.UtilityService
{
    public interface Icheck
    {
        public String checkData(checkUser user, int apiNum);
        public void saveAttempt(String u, string concernedApi, string message);
    }
}
