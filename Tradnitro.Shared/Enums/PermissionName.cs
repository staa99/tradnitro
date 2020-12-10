using System;
using System.ComponentModel;


namespace Tradnitro.Shared.Enums
{
    [Flags]
    public enum PermissionName
    {
        Null = 0,

        #region Role Management Module

        [Description("Manage Roles")]
        ManageRoles = 1,

        #endregion

        #region Record Management Module

        [Description("View Records")]
        ViewRecords = 1 << 1,

        [Description("Edit Records")]
        EditRecords = 1 << 2,

        #endregion

        #region Business Management Module

        [Description("Manage Business Details")]
        ManageFieldConfiguration = 1 << 3,

        [Description("Manage MOU Template")]
        ManageMouTemplate = 1 << 4,

        #endregion

        #region User Management Module

        [Description("Change User Password")]
        ChangeUserPassword = 1 << 5,

        #endregion

        #region Company Management Module

        [Description("View Companies")]
        ViewCompanies = 1 << 6,

        [Description("Manage Companies")]
        ManageCompanies = 1 << 7,

        #endregion
    }
}