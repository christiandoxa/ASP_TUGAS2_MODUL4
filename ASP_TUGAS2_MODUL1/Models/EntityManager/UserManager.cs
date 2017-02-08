using System;
using System.Collections.Generic;
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

        public List<LOOKUPAvailableRole> GetAllRoles()
        {
            using (DemoDBEntities db = new DemoDBEntities())
            {
                var roles = db.LOOKUPRole.Select(o => new LOOKUPAvailableRole
                {
                    LOOKUPRoleID = o.LOOKUPRoleID,
                    RoleName = o.RoleName,
                    RoleDescription = o.RoleDescription
                }).ToList();
                return roles;
            }
        }

        public int GetUserID(string loginName)
        {
            using (DemoDBEntities db = new DemoDBEntities())
            {
                var user = db.SYSUSER.Where(o => o.LoginName.Equals(loginName));
                if (user.Any())
                    return user.FirstOrDefault().SYSUserID;
            }
            return 0;
        }

        public List<UserProfileView> GetAllUserProfiles()
        {
            List<UserProfileView> profiles = new List<UserProfileView>();
            using (DemoDBEntities db = new DemoDBEntities())
            {
                UserProfileView UPV;
                var users = db.SYSUSER.ToList();
                foreach (SYSUSER u in db.SYSUSER)
                {
                    UPV = new UserProfileView();
                    UPV.SYSUserID = u.SYSUserID;
                    UPV.LoginName = u.LoginName;
                    UPV.Password = u.PasswordEncryptedText;
                    var SUP = db.SYSUserProfile.Find(u.SYSUserID);
                    if (SUP != null)
                    {
                        UPV.FirstName = SUP.FirstName;
                        UPV.LastName = SUP.LastName;
                        UPV.Gender = SUP.Gender;
                    }
                    var SUR = db.SYSUserRole.Where(o => o.SYSUserID.Equals(u.SYSUserID));
                    if (SUR.Any())
                    {
                        var userRole = SUR.FirstOrDefault();
                        UPV.LOOKUPRoleID = userRole.LOOKUPRoleID;
                        UPV.RoleName = userRole.LOOKUPRole.RoleName;
                        UPV.IsRoleActive = userRole.IsActive;
                    }
                    profiles.Add(UPV);
                }
            }
            return profiles;
        }

        public UserDataView GetUserDataView(string loginName)
        {
            UserDataView UDV = new UserDataView();
            List<UserProfileView> profiles = GetAllUserProfiles();
            List<LOOKUPAvailableRole> roles = GetAllRoles();
            int? userAssignedRoleID = 0, userID = 0;
            string userGender = string.Empty;
            userID = GetUserID(loginName);
            using (DemoDBEntities db = new DemoDBEntities())
            {
                userAssignedRoleID = db.SYSUserRole.FirstOrDefault(o => o.SYSUserID ==
                                                                        userID)?.LOOKUPRoleID;
                userGender = db.SYSUserProfile.FirstOrDefault(o => o.SYSUserID ==
                                                                   userID)?.Gender;
            }
            List<Gender> genders = new List<Gender>();
            genders.Add(new Gender {Text = "Male", Value = "M"});
            genders.Add(new Gender {Text = "Female", Value = "F"});
            UDV.UserProfile = profiles;
            UDV.UserRoles = new UserRoles
            {
                SelectedRoleID = userAssignedRoleID,
                UserRoleList
                    = roles
            };
            UDV.UserGender = new UserGender
            {
                SelectedGender = userGender,
                Gender = genders
            };
            return UDV;
        }

        public void UpdateUserAccount(UserProfileView user)
        {
            using (DemoDBEntities db = new DemoDBEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        SYSUSER SU = db.SYSUSER.Find(user.SYSUserID);
                        SU.LoginName = user.LoginName;
                        SU.PasswordEncryptedText = user.Password;
                        SU.RowCreatedSYSUserID = user.SYSUserID;
                        SU.RowModifiedSYSUserID = user.SYSUserID;
                        SU.RowCreatedDateTime = DateTime.Now;
                        SU.RowModifiedDateTime = DateTime.Now;
                        db.SaveChanges();
                        var userProfile = db.SYSUserProfile.Where(o => o.SYSUserID ==
                                                                       user.SYSUserID);
                        if (userProfile.Any())
                        {
                            SYSUserProfile SUP = userProfile.FirstOrDefault();
                            SUP.SYSUserID = SU.SYSUserID;
                            SUP.FirstName = user.FirstName;
                            SUP.LastName = user.LastName;
                            SUP.Gender = user.Gender;
                            SUP.RowCreatedSYSUserID = user.SYSUserID;
                            SUP.RowModifiedSYSUserID = user.SYSUserID;
                            SUP.RowCreatedDateTime = DateTime.Now;
                            SUP.RowModifiedDateTime = DateTime.Now;
                            db.SaveChanges();
                        }
                        if (user.LOOKUPRoleID > 0)
                        {
                            var userRole = db.SYSUserRole.Where(o => o.SYSUserID ==
                                                                     user.SYSUserID);
                            SYSUserRole SUR = null;
                            if (userRole.Any())
                            {
                                SUR = userRole.FirstOrDefault();
                                SUR.LOOKUPRoleID = user.LOOKUPRoleID;
                                SUR.SYSUserID = user.SYSUserID;
                                SUR.IsActive = true;
                                SUR.RowCreatedSYSUserID = user.SYSUserID;
                                SUR.RowModifiedSYSUserID = user.SYSUserID;
                                SUR.RowCreatedDateTime = DateTime.Now;
                                SUR.RowModifiedDateTime = DateTime.Now;
                            }
                            else
                            {
                                SUR = new SYSUserRole();
                                SUR.LOOKUPRoleID = user.LOOKUPRoleID;
                                SUR.SYSUserID = user.SYSUserID;
                                SUR.IsActive = true;
                                SUR.RowCreatedSYSUserID = user.SYSUserID;
                                SUR.RowModifiedSYSUserID = user.SYSUserID;
                                SUR.RowCreatedDateTime = DateTime.Now;
                                SUR.RowModifiedDateTime = DateTime.Now;
                                db.SYSUserRole.Add(SUR);
                            }
                            db.SaveChanges();
                        }
                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }

        public void DeleteUser(int userID)
        {
            using (DemoDBEntities db = new DemoDBEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var SUR = db.SYSUserRole.Where(o => o.SYSUserID == userID);
                        if (SUR.Any())
                        {
                            db.SYSUserRole.Remove(SUR.FirstOrDefault());
                            db.SaveChanges();
                        }
                        var SUP = db.SYSUserProfile.Where(o => o.SYSUserID == userID);
                        if (SUP.Any())
                        {
                            db.SYSUserProfile.Remove(SUP.FirstOrDefault());
                            db.SaveChanges();
                        }
                        var SU = db.SYSUSER.Where(o => o.SYSUserID == userID);
                        if (SU.Any())
                        {
                            db.SYSUSER.Remove(SU.FirstOrDefault());
                            db.SaveChanges();
                        }
                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }
    }
}