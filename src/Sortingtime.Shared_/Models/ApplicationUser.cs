using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Sortingtime.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<long>
    {
        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<IdentityUserRole<long>> Roles { get; } = new List<IdentityUserRole<long>>();

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim<long>> Claims { get; } = new List<IdentityUserClaim<long>>();

        /// <summary>
        /// Navigation property for this users login accounts.
        /// </summary>
        public virtual ICollection<IdentityUserLogin<long>> Logins { get; } = new List<IdentityUserLogin<long>>();

        [Required(ErrorMessage = "Full Name is Required.")]
        [Display(Name = "Full Name")]
        [StringLength(200)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is Required.")]
        [Display(Name = "Email")]
        [StringLength(200)]
        public override string UserName { get; set; }

        [StringLength(200)]
        public override string Email { get; set; }

        [StringLength(100)]
        public override string PhoneNumber { get; set; }

        public virtual ICollection<TtaskItem> TaskItems { get; set; }
        public virtual ICollection<MaterialItem> MaterialItems { get; set; }

        public virtual ICollection<Report> Reports { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }

    }
}