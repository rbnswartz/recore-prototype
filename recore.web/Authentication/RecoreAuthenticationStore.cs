using Microsoft.AspNetCore.Identity;
using recore.db;
using recore.db.Commands;
using recore.db.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace recore.web.Authentication
{
    public class RecoreAuthenticationStore : IUserStore<IdentityUser>
    {
        private readonly IDataService service;
        private readonly List<string> allFields = new List<string>
        {
            "accessfailedcount",
            "concurrencystamp",
            "email",
            "emailconfirmed",
            "lockoutenabled",
            "lockoutend",
            "passwordhash",
            "phonenumber",
            "phonenumberconfirmed",
            "secuirtystamp",
            "twofactorenabled",
            "username"
        };

        public RecoreAuthenticationStore(IDataService service)
        {
            this.service = service;
        }
        public Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            service.Execute(new CreateRecordCommand() {Target = ConvertIdentityUserToRecord(user)});
            return Task.FromResult(new IdentityResult());
        }

        public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            service.Execute(new DeleteRecordCommand() { Type = "recore_user", Id = new Guid(user.Id) });
            return Task.FromResult(new IdentityResult());
        }

        public void Dispose()
        {
        }

        public Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            BasicQuery query = new BasicQuery("recore_user");
            query.Columns = allFields;
            query.Filters.Add(new QueryFilter() { Column = "recore_userid", Operation = FilterOperation.Equal, Value = userId });
            var result = ((QueryRecordsResult)this.service.Execute(new QueryRecordsCommand() { Query = query })).Result;
            if (result.Count == 0)
            {
                throw new KeyNotFoundException("Unable to find user with Id");
            };
            return Task.FromResult(ConvertRecordToIdentityUser(result[0]));
        }

        public Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            BasicQuery query = new BasicQuery("recore_user");
            query.Columns = allFields;
            query.Filters.Add(new QueryFilter() { Column = "username", Operation = FilterOperation.Equal, Value = normalizedUserName });
            var result = ((QueryRecordsResult)this.service.Execute(new QueryRecordsCommand() { Query = query })).Result;
            if (result.Count == 0)
            {
                throw new KeyNotFoundException("Unable to find user with username");
            };
            return Task.FromResult(ConvertRecordToIdentityUser(result[0]));
        }

        public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            service.Execute(new UpdateRecordCommand() { Target = ConvertIdentityUserToRecord(user)});
            return Task.FromResult(new IdentityResult());
        }
        private IdentityUser ConvertRecordToIdentityUser(Record input)
        {
            IdentityUser output = new IdentityUser()
            {
                AccessFailedCount = (int)input["accessfailedcount"],
                ConcurrencyStamp = (string)input["concurrencystamp"],
                Email = (string)input["email"],
                EmailConfirmed = (bool)input["emailconfirmed"],
                Id = input.Id.ToString(),
                LockoutEnabled = (bool)input["lockoutenabled"],
                LockoutEnd = (DateTime)input["lockoutend"],
                PasswordHash = (string)input["passwordhash"],
                PhoneNumber = (string)input["phonenumber"],
                PhoneNumberConfirmed = (bool)input["phonenumberconfirmed"],
                SecurityStamp = (string)input["securitystamp"],
                TwoFactorEnabled = (bool)input["twofactorenabled"],
                UserName = (string)input["username"],
            };

            return output;
        }
        private Record ConvertIdentityUserToRecord(IdentityUser input)
        {
            Record output = new Record("recore_user")
            {
                Id = new Guid(input.Id),
                ["accessfailedcount"] = input.AccessFailedCount,
                ["concurrencystamp"] = input.ConcurrencyStamp,
                ["email"] = input.Email,
                ["emailconfirmed"] = input.EmailConfirmed,
                ["lockoutenabled"] = input.LockoutEnabled,
                ["lockoutend"] = input.LockoutEnd,
                ["passwordhash"] = input.PasswordHash,
                ["phonenumber"] = input.PhoneNumber,
                ["phonenumberconfirmed"] = input.PhoneNumberConfirmed,
                ["securitystamp"] = input.SecurityStamp,
                ["twofactorenabled"] = input.TwoFactorEnabled,
                ["username"] = input.UserName,
            };
            return output;
        }
    }
}
