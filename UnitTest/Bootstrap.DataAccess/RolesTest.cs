﻿using Xunit;

namespace Bootstrap.DataAccess
{
    public class RolesTest : IClassFixture<BootstrapAdminStartup>
    {
        [Fact]
        public void SaveRolesByUserId_Ok()
        {
            var role = new Role();
            Assert.True(role.SaveByUserId("1", new string[] { "1", "2" }));
        }

        [Fact]
        public void RetrieveRolesByUserId_Ok()
        {
            var role = new Role();
            Assert.NotEmpty(role.RetrievesByUserId("1"));
        }

        [Fact]
        public void DeleteRole_Ok()
        {
            var role = new Role()
            {
                Description = "Role_Desc",
                RoleName = "UnitTest"
            };
            role.Save(role);
            Assert.True(role.Delete(new string[] { role.Id.ToString() }));
        }

        [Fact]
        public void SaveRole_Ok()
        {
            var role = new Role()
            {
                Description = "Role_Desc",
                RoleName = "UnitTest"
            };
            Assert.True(role.Save(role));
        }

        [Fact]
        public void RetrieveRolesByMenuId_Ok()
        {
            var menu = new Menu();
            menu.SaveMenusByRoleId("1", new string[] { "1" });

            var role = new Role();
            var rs = role.RetrievesByMenuId("1");
            Assert.Contains(rs, r => r.Checked == "checked");
        }

        [Fact]
        public void SavaRolesByMenuId_Ok()
        {
            var role = new Role();
            Assert.True(role.SavaByMenuId("1", new string[] { "1" }));
        }

        [Fact]
        public void RetrieveRolesByGroupId_Ok()
        {
            var role = new Role();
            Assert.Contains(role.RetrievesByGroupId("1"), r => r.Checked == "checked");
        }

        [Fact]
        public void RetrieveRolesByUserName_Ok()
        {
            var role = new Role();
            Assert.NotEmpty(role.RetrieveRolesByUserName("Admin"));
        }

        [Fact]
        public void RetrieveRolesByUrl_Ok()
        {
            var role = new Role();
            Assert.NotEmpty(role.RetrieveRolesByUrl("~/Home/Index"));
        }
    }
}
