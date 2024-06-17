using System;
using System.Collections.Generic;

namespace MC_GymMasterWebAPI.Model;

public partial class Member
{
    public int MemberId { get; set; }

    public string UserId { get; set; } = null!;
    public string Password { get; set; } = null;

    public char Sex { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime? BirthDate { get; set; }

    public string? Address { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ExpirationDate { get; set; }

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<MemberConnection> MemberConnections { get; set; } = new List<MemberConnection>();

    public virtual ICollection<ShareBoard> ShareBoards { get; set; } = new List<ShareBoard>();

    public virtual ICollection<WorkoutSet> WorkoutSets { get; set; } = new List<WorkoutSet>();
}
