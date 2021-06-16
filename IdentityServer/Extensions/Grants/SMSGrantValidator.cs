using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer.Constants.Grants;
using IdentityServer.Entities;
using IdentityServer.Helpers;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Extensions.Grants
{
    public class SmsGrantValidator : IExtensionGrantValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PhoneNumberTokenProvider<ApplicationUser> _phoneNumberTokenProvider;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public string GrantType => CustomGrantType.SMS;
        public SmsGrantValidator(
            PhoneNumberTokenProvider<ApplicationUser> phoneNumberTokenProvider,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _phoneNumberTokenProvider = phoneNumberTokenProvider;
            _signInManager = signInManager;
            _userManager = userManager;
        }


        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            try
            {
                // 参数获取
                var phoneNumber = context.Request.Raw["phone_number"];
                var token = context.Request.Raw["token"];
                // var channel = context.Request.Raw["channel"];

                var requireRegister = false;
                var user = await _userManager.Users.SingleOrDefaultAsync(x =>
                    x.PhoneNumber == _userManager.NormalizeName(phoneNumber));

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = phoneNumber,
                        PhoneNumber = phoneNumber,
                        SecurityStamp = new Secret("secret").Value + phoneNumber.Sha256()
                    };
                    requireRegister = true;
                }


                // 验证
                // var validationResult = TotpHelper.Validate(phoneNumber, code);
                var validationResult =
                    await _phoneNumberTokenProvider.ValidateAsync(
                        "sms",
                        token,
                        _userManager,
                        user);

                System.Console.WriteLine($"验证结果：{validationResult}");

                if (!validationResult)
                {
                    context.Result = new GrantValidationResult(
                        TokenRequestErrors.InvalidGrant,
                        "Invalidate failed.");
                    return;
                }

                // 注册
                if (requireRegister)
                {
                    user.PhoneNumberConfirmed = true;
                    var creationResult = await _userManager.CreateAsync(user);
                    if (creationResult != IdentityResult.Success)
                    {
                        context.Result = new GrantValidationResult(
                            TokenRequestErrors.InvalidGrant,
                            "Failed to create a new User.");
                        return;
                    }
                }


                //授权通过返回
                context.Result = new GrantValidationResult(
                    user.Id,
                    OidcConstants.AuthenticationMethods.ConfirmationBySms);
            }
            catch (Exception ex)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            }
        }

        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<List<Claim>> ValidateUserAsync(string phoneNumber, string code)
        {
            var validation = TotpHelper.Validate(phoneNumber, code);

            if (validation)
            {
                // 注册或登录，返回 ID
                System.Console.WriteLine("验证通过！");
            }
            else
            {
                System.Console.WriteLine("验证失败！");
            }

            return new List<Claim>()
            {
                new Claim(ClaimTypes.MobilePhone, phoneNumber),
            };
        }
    }
}
