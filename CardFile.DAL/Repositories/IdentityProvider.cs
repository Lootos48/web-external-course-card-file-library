﻿using CardFile.DAL.EF;
using CardFile.DAL.Entities;
using CardFile.DAL.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CardFile.DAL.Repositories
{
    public class IdentityProvider : IIdentityProvider
    {
        readonly UserManager<IdentityUser> _userManager;
        readonly RoleManager<IdentityRole> _roleManager;

        public IdentityProvider()
        {
            var roleStore = new RoleStore<IdentityRole>();
            _roleManager = new RoleManager<IdentityRole>(roleStore);

            var userStore = new UserStore<IdentityUser>();
            _userManager = new UserManager<IdentityUser>(userStore);

            _userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 3
            };
        }

        public async Task<bool> CreateUser(UserAuthInfo user)
        {
            var identityUser = new IdentityUser(user.Username);
            var result = await _userManager.CreateAsync(identityUser, user.Password);

            return result.Succeeded;
        }

        public async Task<bool> CreateRole(string role)
        {
            var identityRole = new IdentityRole(role);
            var result = await _roleManager.CreateAsync(identityRole);
            return result.Succeeded;

        }

        public async Task<bool> GiveRoleToUser(string role, string username)
        {
            var userId = _userManager.Users.First<IdentityUser>(u => u.UserName == username).Id;
            var result = await _userManager.AddToRoleAsync(userId, role);
            return result.Succeeded;
        }
        public async Task<bool> RemoveUserFromRole(string username, string role)
        {
            var userId = _userManager.Users.First<IdentityUser>(u => u.UserName == username).Id;
            var result = await _userManager.RemoveFromRoleAsync(userId, role);
            return result.Succeeded;
        }

        public async Task<ClaimsIdentity> GetUserClaim(UserAuthInfo user)
        {
            var identityUser = await _userManager.FindAsync(user.Username, user.Password);
            if (identityUser == null)
            {
                throw new ValidationException("User with that login info isn`t exist");
            }
            var userIdentity = _userManager.CreateIdentity(identityUser, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }

    }
}