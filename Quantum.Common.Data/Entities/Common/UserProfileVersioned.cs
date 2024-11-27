using System;
using System.Collections.Generic;
using System.Text;

namespace Quantum.Data.Entities.Common
{
    public class UserProfileVersioned : BaseUserEntity, IBaseVersionedEntity<UserProfile>
	{
		public virtual ICollection<UserProfile> Versions { get; set; }
	}
    
}
