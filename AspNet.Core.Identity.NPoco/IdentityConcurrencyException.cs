namespace AspNet.Core.Identity.NPoco {
    using System;

    public class IdentityConcurrencyException : Exception {
        public IdentityConcurrencyException()
            : base("Could not update entity, it was changed by someone else prior to your save request") { }
    }
}
