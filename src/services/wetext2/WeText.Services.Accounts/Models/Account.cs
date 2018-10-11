// ----------------------------------------------------------------------------
//       ___ ___  ___     ___
// |  | |__   |  |__  \_/  |
// |/\| |___  |  |___ / \  |
//
// Yet another WeText application for demonstration.
// MIT License
//
// Copyright (c) 2018 Sunny Chen (daxnet)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ----------------------------------------------------------------------------

using System;
using WeText.Common;

namespace WeText.Services.Accounts.Models
{
    public class Account : IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool AuthenticateWith(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            return Password.Equals(Utils.EncryptPassword(password, Name));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj == null)
            {
                return false;
            }

            var account = obj as Account;
            return account != null &&
                   Id.Equals(account.Id) &&
                   Name == account.Name &&
                   DisplayName == account.DisplayName &&
                   Email == account.Email &&
                   Password == account.Password;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, DisplayName, Email, Password);
        }
    }
}