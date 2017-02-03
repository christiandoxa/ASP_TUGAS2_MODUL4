using System;
using System.Linq;
using ASP_TUGAS2_MODUL1.Models.DB;
using ASP_TUGAS2_MODUL1.Models.ViewModel;

namespace ASP_TUGAS2_MODUL1.Models.EntityManager
{
    public class UserManager
    {
        public void AddUserAccount(UserSignUpView user)
        {
            using (DemoDBEntities db = new DemoDBEntities())
            {
                SYSUSER SU = new SYSUSER();
                SU.LoginName = user.LoginName;
                SU.PasswordEncryptedText = user.Password;
                SU.RowCreatedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                SU.RowModifiedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                SU.RowCreatedDateTime = DateTime.Now;
                SU.RowModifiedDateTime = DateTime.Now;

                db.SYSUSER.Add(SU);
                db.SaveChanges();

                SYSUserProfile SUP = new SYSUserProfile();
                SUP.SYSUserID = SU.SYSUserID;
                SUP.FirstName = user.FirstName;
                SUP.LastName = user.LastName;
                SUP.Gender = user.Gender;
                SUP.RowCreatedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                SUP.RowModifiedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                SUP.RowCreatedDateTime = DateTime.Now;
                SUP.RowModifiedDateTime = DateTime.Now;

                db.SYSUserProfile.Add(SUP);
                db.SaveChanges();

                if (user.LOOKUPRoleID > 0)
                {
                    SYSUserRole SUR = new SYSUserRole();
                    SUR.LOOKUPRoleID = user.LOOKUPRoleID;
                    SUR.SYSUserID = user.SYSUserID;
                    SUR.IsActive = true;
                    SUR.RowCreatedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                    SUR.RowModifiedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                    SUR.RowCreatedDateTime = DateTime.Now;
                    SUR.RowModifiedDateTime = DateTime.Now;

                    db.SYSUserRole.Add(SUR);
                    db.SaveChanges();
                }
            }
        }

        public bool IsLoginNameExist(string loginName)
        {
            using (DemoDBEntities db = new DemoDBEntities())
            {
                return db.SYSUSER.Any(o => o.LoginName.Equals(loginName));
            }
        }

        public string GetUserPassword(string loginName)
        {
            using (DemoDBEntities db = new DemoDBEntities())
            {
                var user = db.SYSUSER.Where(o => o.LoginName.ToLower().Equals(loginName));
                if (user.Any())
                    return user.FirstOrDefault().PasswordEncryptedText;
                else
                    return string.Empty;
            }
        }

        public bool IsUserInRole(string loginName, string roleName)
        {
            using (DemoDBEntities db = new DemoDBEntities())
            {
                SYSUSER SU = db.SYSUSER.FirstOrDefault(o => o.LoginName.ToLower().Equals(loginName));
                if (SU != null)
                {
                    var roles = from q in db.SYSUserRole
                        join r in db.LOOKUPRole on q.LOOKUPRoleID equals r.LOOKUPRoleID
                        where r.RoleName.Equals(roleName) && q.SYSUserID.Equals(SU.SYSUserID)
                        select r.RoleName;

                    if (roles != null)
                    {
                        return roles.Any();
                    }
                }
                return false;
            }
        }
    }
}