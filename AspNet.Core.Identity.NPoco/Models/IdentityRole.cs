namespace AspNet.Core.Identity.NPoco.Models {
    using System;

    public class IdentityRole {
        private IdentityRole() {
            Id = Guid.NewGuid().ToString();
        }

        public IdentityRole(string name) : this() {
            Name = name;
        }

        public IdentityRole(string name, string id) {
            Name = name;
            Id = id;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }
}
