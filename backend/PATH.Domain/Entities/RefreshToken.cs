using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Token { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public string? DeviceInfo { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}
