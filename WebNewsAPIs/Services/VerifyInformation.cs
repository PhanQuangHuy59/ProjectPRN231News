using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WebNewsAPIs.Services
{
    public class VerifyInformation
    {
        public static string ToSlug(string title)
        {
            // 1. Convert to lowercase
            title = title.ToLower();

            // 2. Remove accents and diacritics (e.g., é -> e, ü -> u)
            title = RemoveAccents(title);

            // 3. Replace non-alphanumeric characters with a space
            title = Regex.Replace(title, @"[^\w\s]", " ");

            // 4. Replace multiple spaces with a single space
            title = Regex.Replace(title, @"\s+", " ");

            // 5. Replace spaces with a dash
            title = title.Replace(" ", "-");

            // 6. Remove trailing dashes
            title = title.Trim('-');

            // 7. Limit the slug to a maximum length (optional)
            // title = title.Substring(0, Math.Min(title.Length, 200));

            return title;
        }

        static string RemoveAccents(string text)
        {
            var listChar = text.Normalize(NormalizationForm.FormD)
                  .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                  .ToList();
            return string.Join("", listChar);
        }
        public string IsValidPassword(string password, string username)
        {
            var passwordPolicy = new
            {
                RequiredLength = 12,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonAlphanumeric = true,
                DisallowUsername = true,
                DisallowCommonWords = true
            };



            if (string.IsNullOrWhiteSpace(password))
            {
                return "Mạt khẩu của bạn không được để trống dễ bị phát hiện.";
            }

            if (password.Length < passwordPolicy.RequiredLength)
            {
                return "Chiều dài mật khảu quá ngắn tối thiểu " + passwordPolicy.RequiredLength + " kí tự.";
            }

            if (passwordPolicy.RequireDigit && !HasDigit(password))
            {
                return "Mật khẩu không chứa số.";
            }

            if (passwordPolicy.RequireLowercase && !HasLowercase(password))
            {
                return "Mật khẩu không chứa chữ viết thường.";
            }

            if (passwordPolicy.RequireUppercase && !HasUppercase(password))
            {
                return "Mật khẩu không chứa chữ viết Hoa.";
            }

            if (passwordPolicy.RequireNonAlphanumeric && !HasNonAlphanumeric(password))
            {
                return "Mật khẩu của bạn khong chứa kí tự đặc biệt";
            }

            if (passwordPolicy.DisallowUsername && password.ToLower().Contains(username.ToLower()))
            {
                return "Mật khẩu của bạn chứa user name.";
            }

            if (passwordPolicy.DisallowCommonWords && IsCommonWord(password))
            {
                return "Mật khẩu của bạn có nhiều từ phổ biến!";
            }

            return "Ok";
        }

        private bool HasDigit(string password)
        {
            return password.Any(c => char.IsDigit(c));
        }

        private bool HasLowercase(string password)
        {
            return password.Any(c => char.IsLower(c));
        }

        private bool HasUppercase(string password)
        {
            return password.Any(c => char.IsUpper(c));
        }

        private bool HasNonAlphanumeric(string password)
        {
            return password.Any(c => !char.IsLetterOrDigit(c));
        }

        private bool IsCommonWord(string password)
        {
            var commonWords = new[]
            {
            "password",
            "iloveyou",
            "88888888888",
            "letmein",
            "monkey",
            "qwerty",
            "123456",
            "abcdefg",
            "football",
            "baseball"
        };

            return commonWords.Any(word => password.ToLower().Contains(word));
        }
    }

}
