using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PATH.Application.Exceptions;
using PATH.Domain.Entities;
using PATH.Domain.Models;
using PATH.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Tests.Organizations
{
    public class OrganizationServiceTests
    {


        [Fact]
        public async Task GetUserOrgMembership_UserIsAdmin_ReturnsCorrectPermissions()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();
            var user = new ApplicationUser
            {
                Email = $"test{Guid.NewGuid()}@example.com",
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hashedpassword",
                BirthDate = new DateOnly(1990, 1, 1)
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var organization = new Organization
            {
                Name = "Test Organization",
                CreatedBy = user,
            };

            await context.Organizations.AddAsync(organization);
            await context.SaveChangesAsync();

            var organizationMember = new OrganizationMember
            {
                User = user,
                Organization = organization,
                Role = OrganizationRole.Admin
            };


            await context.OrganizationMembers.AddAsync(organizationMember);
            await context.SaveChangesAsync();


            var organizationService = new OrganizationService(new UserService(context), context);
            var membership = await organizationService.GetUserOrgMembership(user.Id, organization.Id);
            Assert.NotNull(membership);
            Assert.Equal(OrganizationRole.Admin, membership.Role);
            Assert.True(membership.Permissions.CanAddMembers);
            Assert.True(membership.Permissions.CanAssignTasks);
            Assert.True(membership.Permissions.CanCreateProject);


        }
        [Fact]
        public async Task GetUserOrgMembership_UserIsManager_ReturnsCorrectPermissions()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();
            var user = new ApplicationUser
            {
                Email = $"test{Guid.NewGuid()}@example.com",
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hashedpassword",
                BirthDate = new DateOnly(1990, 1, 1)
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var organization = new Organization
            {
                Name = "Test Organization",
                CreatedBy = user,
            };

            await context.Organizations.AddAsync(organization);
            await context.SaveChangesAsync();

            var organizationMember = new OrganizationMember
            {
                User = user,
                Organization = organization,
                Role = OrganizationRole.Manager
            };


            await context.OrganizationMembers.AddAsync(organizationMember);
            await context.SaveChangesAsync();


            var organizationService = new OrganizationService(new UserService(context), context);
            var membership = await organizationService.GetUserOrgMembership(user.Id, organization.Id);
            Assert.NotNull(membership);
            Assert.Equal(OrganizationRole.Manager, membership.Role);
            Assert.False(membership.Permissions.CanAddMembers);
            Assert.True(membership.Permissions.CanAssignTasks);
            Assert.True(membership.Permissions.CanCreateProject);


        }

        [Fact]

        public async Task GetUserOrgMembership_UserIsMember_ReturnsCorrectPermissions()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();
            var user = new ApplicationUser
            {
                Email = $"test{Guid.NewGuid()}@example.com",
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hashedpassword",
                BirthDate = new DateOnly(1990, 1, 1)
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var organization = new Organization
            {
                Name = "Test Organization",
                CreatedBy = user,
            };

            await context.Organizations.AddAsync(organization);
            await context.SaveChangesAsync();

            var organizationMember = new OrganizationMember
            {
                User = user,
                Organization = organization,
                Role = OrganizationRole.Member
            };


            await context.OrganizationMembers.AddAsync(organizationMember);
            await context.SaveChangesAsync();

            var organizationService = new OrganizationService(new UserService(context), context);
            var membership = await organizationService.GetUserOrgMembership(user.Id, organization.Id);
            Assert.NotNull(membership);
            Assert.Equal(OrganizationRole.Member, membership.Role);
            Assert.False(membership.Permissions.CanAddMembers);
            Assert.False(membership.Permissions.CanAssignTasks);
            Assert.False(membership.Permissions.CanCreateProject);

        }

        [Fact]
        public async Task GetUserOrgMembership_UserNotInOrganization_ThrowsException()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();
            var user = new ApplicationUser
            {
                Email = $"test{Guid.NewGuid()}@example.com",
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hashedpassword",
                BirthDate = new DateOnly(1990, 1, 1)
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();


            var unauthorizedUser = new ApplicationUser
            {
                Email = $"unauthorized{Guid.NewGuid()}@example.com",
                FirstName = "Unauthorized",
                LastName = "User",
                PasswordHash = "hashedpassword",
                BirthDate = new DateOnly(1990, 1, 1)
            };

            var organization = new Organization
            {
                Name = "Test Organization",
                CreatedBy = user,
            };
            await context.Organizations.AddAsync(organization);
            await context.SaveChangesAsync();

            var organizationService = new OrganizationService(new UserService(context), context);

            await Assert.ThrowsAsync<AppException>(() => organizationService.GetOrganizationById(unauthorizedUser.Id, organization.Id));
        }


    }
}