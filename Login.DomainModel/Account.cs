using Login.DomainModel.Core;

namespace Login.DomainModel
{
    public class Account : Entity
    {
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public byte[] Cipher { get; set; }
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
    }
}
